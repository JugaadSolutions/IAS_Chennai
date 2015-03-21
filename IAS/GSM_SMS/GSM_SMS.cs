using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Configuration;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace ias.devicedriver
{
    public class GSM_SMS
    {
        SerialPortDriver spDriver = null;


        public enum MessageMode { PDU = 0, TEXT = 1 };


        String atResponse = String.Empty;

        String comPort = String.Empty;
        public String ComPort
        {
            get { return comPort; }
            set
            {
                comPort = value;
            }
        }


        System.Timers.Timer transactionTimer = null;
        int responseTimeout = 50; //response timeout in milliseconds

        AutoResetEvent transactionEvent = null;

        TraceSource _gsmSMSTrace = null;
        TextWriterTraceListener _gsmSMSTraceListener = null;

        String NONE = "";
        String CMD_MESSAGE_MODE = "+CMGF";
        String CMD_SEND_SMS = "+CMGS";
        String CMD_SMSC_ADDRESS = "+CSCA";
        String CMD_ECHO_OFF = "E0";

        int retries = 0;

        String cmdPrefix = "AT";
        String cmdSuffix = new String((char)13, 1);

        public GSM_SMS()
        {
            //spDriver = new SerialPortDriver();
            spDriver = new SerialPortDriver(9600, 8, StopBits.One, Parity.None, Handshake.None);



            responseTimeout = int.Parse(ConfigurationSettings.AppSettings["ResponseTimeout"]);

            transactionTimer = new System.Timers.Timer(responseTimeout);
            transactionTimer.Elapsed += new ElapsedEventHandler(transactionTimeout);
            transactionTimer.AutoReset = false;

            transactionEvent = new AutoResetEvent(false);

            _gsmSMSTrace = new TraceSource("GSM_SMSTrace");
            _gsmSMSTrace.Switch = new SourceSwitch("GSM_SMSTraceSwitch");


            String gsmSMSTraceFile = ConfigurationSettings.AppSettings["GSM_SMSTraceFile"];

            if (gsmSMSTraceFile != String.Empty && gsmSMSTraceFile!= null)
            {
                _gsmSMSTraceListener = new TextWriterTraceListener(gsmSMSTraceFile);
                _gsmSMSTrace.Listeners.Add(_gsmSMSTraceListener);
                _gsmSMSTrace.Switch.Level = SourceLevels.Information;
            }
            else
            {
                _gsmSMSTrace.Switch.Level = SourceLevels.Off;
            }
        }

        public void initialize()
        {
            String[] ports = SerialPort.GetPortNames();

            foreach (String port in ports)
            {
                try
                {
                    if (spDriver.open(port) == false)
                        continue;

                   // spDriver.ReadTimeout = 2000;
                    //spDriver.WriteTimeout = 2000;
                    if (transact(NONE, String.Empty) == false)
                    {
                        spDriver.Close();
                        continue;
                    }
                    if (transact(CMD_SMSC_ADDRESS, "?") == false)
                    {
                        spDriver.Close();
                        continue;
                    }

                    comPort = port;
                    break;
                }

                catch (Exception e)
                {
                    if (spDriver.IsOpen)
                        spDriver.Close();
                    continue;
                }
            }

        }

        public void initializePort(String port)
        {
            if (spDriver.open(port) == false)
                throw new GSM_SMSException("Unable to open Port:" + port);
               

            //spDriver.ReadTimeout = 2000;
            //spDriver.WriteTimeout = 2000;

            int i = 3;

            do
            {
                if (transact(String.Empty, string.Empty) == true)
                    break;
                //spDriver.WriteToPort(cmdPrefix + cmdSuffix);
            } while (--i > 0);

            i = 3;
            do
            {
                if (transact(CMD_ECHO_OFF, String.Empty) == true)
                    break;

                //spDriver.WriteToPort(cmdPrefix + CMD_ECHO_OFF+cmdSuffix);
            } while (--i > 0);

            if (transact(NONE, String.Empty) == false)
            {
                spDriver.Close();
                throw new GSM_SMSException("Unable to find GSM Modem on" + port);
            }


            if (transact(CMD_SMSC_ADDRESS, "?") == false)
            {
                spDriver.Close();
                throw new GSM_SMSException("Unable to find SMS centre address:" + port);
            }

            comPort = port;
        }


        public bool setMessageMode(MessageMode mode)
        {
            bool result = false;

            String cmdData = "=" + (int)mode;

            result = transact(CMD_MESSAGE_MODE, cmdData);


            return result;
        }


        public bool sendSMS(String no, String message)
        {
            bool result = false;

            String cmdData ="="+"\""+ no +"\"" + cmdSuffix ;

            result = transact(CMD_SEND_SMS, cmdData);

            if (result == true)
            {
                cmdData = message + new String((char)26, 1);
                result = transact(cmdData);
            }

            return result;
        }



        private bool parseATresponse(String atResponse, String atCommand)
        {
            bool result = false;

            #region TRACE_CODE

            String traceString = DateTime.Now.ToString();

            traceString += ":" + atResponse;

            traceString += Environment.NewLine;

            _gsmSMSTrace.TraceInformation(traceString);
            foreach (TraceListener l in _gsmSMSTrace.Listeners)
            {
                l.Flush();
            }

            #endregion

            switch( atCommand)
            {
                case "":
                    if( atResponse.Contains("OK"))
                        result = true;
                    break;
                case "+CSCA":
                    if( atResponse.Contains("OK"))
                        result = true;
                    break;

                case "+CMGF":
                    if(atResponse.Contains("OK"))
                        result = true;
                    break;

                case "+CMGS":
                    if (atResponse.Contains(">"))
                        return true;
                       
                    if( atResponse.Contains("OK"))
                        result = true;
                    break;

                default:
                    result = false;
                    break;
            }
            return result;
        }
                    



     

        private bool transact(String atCommand , String data)
        {
            bool result = false;

            String command = cmdPrefix + atCommand + data + cmdSuffix;

            #region TRACE_CODE

            String traceString = DateTime.Now.ToString();

            traceString += ":" + command;

            traceString += Environment.NewLine;

            _gsmSMSTrace.TraceInformation(traceString);
            foreach (TraceListener l in _gsmSMSTrace.Listeners)
            {
                l.Flush();
            }

            #endregion


            if (command != String.Empty)
            {
                try
                {
                    if (spDriver.IsOpen)
                    {
                        spDriver.DiscardInBuffer();
                        spDriver.WriteToPort(command);
                    }
                }
                catch (Exception e)
                {

                    throw new GSM_SMSException("Serial Port Write Error");
                }
                transactionTimer.Interval = 5000;
                transactionTimer.Start();           //start transaction timer 
                transactionEvent.WaitOne();         //wait for response

                if (atResponse == String.Empty)               //if no response
                {
                    result = false;                 //indicate failure
                }
                else
                {

                    result = parseATresponse(atResponse, atCommand);

                }
            }

            return result;
        }

        private bool transact(String data)
        {
            bool result = false;


            #region TRACE_CODE

            String traceString = DateTime.Now.ToString();

            traceString += ":" + data;

            traceString += Environment.NewLine;

            _gsmSMSTrace.TraceInformation(traceString);
            foreach (TraceListener l in _gsmSMSTrace.Listeners)
            {
                l.Flush();
            }

            #endregion


            try
            {
                if (spDriver.IsOpen)
                {
                    spDriver.DiscardInBuffer();
                    spDriver.WriteToPort(data);
                }
            }
            catch (Exception e)
            {

                throw new GSM_SMSException("Serial Port Write Error");
            }
            transactionTimer.Interval = 7000;
            transactionTimer.Start();           //start transaction timer 
            transactionEvent.WaitOne();         //wait for response

            if (atResponse == String.Empty)               //if no response
            {
                result = false;                 //indicate failure
            }
            else
            {

                result = parseATresponse(atResponse, CMD_SEND_SMS);

            }
        
            return result;
        }


        private void transactionTimeout(object sender, ElapsedEventArgs e)
        {
            transactionTimer.Stop();    //stop the timer

            try
            {
                atResponse = spDriver.ReadExisting();
                if (atResponse == String.Empty)
                {
                    if (retries < 2)
                    {
                        transactionTimer.Start();
                        retries++;
                    }
                    else
                    {
                        retries = 0;
                        transactionEvent.Set();
                    }
                }
                else
                {
                    
                    retries = 0;
                    transactionEvent.Set();
                }

            }
            catch (Exception ex)
            {
                #region TRACE_CODE

                String traceString = DateTime.Now.ToString();

                traceString += ":" + ex.Message;

                traceString += Environment.NewLine;
                traceString += ex.StackTrace;

                _gsmSMSTrace.TraceInformation(traceString);
                foreach (TraceListener l in _gsmSMSTrace.Listeners)
                {
                    l.Flush();
                }

                #endregion
                atResponse = String.Empty;
            }

            
        }

        
        
    }


    public class GSM_SMSException : Exception
    {
        public String message = String.Empty;
        public GSM_SMSException(String msg)
        {
            message = msg;
        }
    }    
}
