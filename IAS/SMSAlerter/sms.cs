using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.Collections;

using GsmComm.GsmCommunication;

using GsmComm.PduConverter;
using GsmComm.PduConverter.SmartMessaging;
using ias.devicedriver;
namespace gsm.sms
{
    class SMS
    {
        private GsmCommMain comm = null;

        private GSM_SMS gsmComm = null;

        // SMS config
        private string smscAddress;
        public string SMScAddress
        {
            get
            {
                if( comm == null)
                    return String.Empty;
                AddressData addrData = comm.GetSmscAddress();
                return addrData.Address;

            }

            set
            {
                if( comm == null )
                    smscAddress =String.Empty;
                else
                {
                    AddressData addrData = comm.GetSmscAddress();
                    comm.SetSmscAddress( new AddressData(value , addrData.TypeOfAddress));
                }
            }
        }

        // SMS options
        private bool unicode = false;
        public bool Unicode
        {
            get { return unicode; }
            set { unicode = value; }
        }

        private bool alert = false;
        public bool Alert
        {
            get { return alert; }
            set { alert = value; }
        }



        private bool statReport = false;
        public bool StatReport
        {
            get { return statReport; }
            set { statReport = value; }
        }

        private int gsmBaudRate = 115200;
        public int GSMBaudRate
        {
            get { return gsmBaudRate; }
            set
            {
                gsmBaudRate = value;
            }
        }

        private int gsmTimeOut = 500;
        public int GSMTimeOut
        {
            get { return gsmTimeOut; }
            set
            {
                gsmTimeOut = value;
            }
        }



        public delegate void Logger(String msg);
        Logger logger = null;

        public SMS(Logger logger)
        {
            gsmBaudRate = Convert.ToInt32(ConfigurationSettings.AppSettings["BaudRate"]);

            gsmTimeOut = Convert.ToInt32(ConfigurationSettings.AppSettings["TimeOut"]);
            this.logger = logger;
        }

        public bool initialize()
        {
            try
            {
                gsmComm = new GSM_SMS();



                logger("Finding GSM Communication Port...");

                gsmComm.initialize();
                gsmComm.setMessageMode(GSM_SMS.MessageMode.TEXT);


//                int? commPort = findGSMCommPort();


                if( gsmComm.ComPort == String.Empty)
            

                //if (commPort == null)
                {
                    logger("None Found");
                    return false;
                }


                //logger("Found on port number :" + commPort.Value.ToString());

                logger("Found on port :" + gsmComm.ComPort);

                //logger("Setting GSM Port ....");

//                comm = new GsmCommMain(commPort.Value, gsmBaudRate, gsmTimeOut);
//                comm.Open();
                

//                comm.MessageReceived += new MessageReceivedEventHandler(comm_MessageReceived);
////                comm.EnableMessageNotifications();

//                comm.MessageSendComplete += new GsmCommMain.MessageEventHandler(comm_MessageSendComplete);

//                smscAddress = comm.GetSmscAddress().Address;




                //logger("Successful");

                return true;
            }
            catch (Exception e)
            {
                comm = null;
                logger(" Failure ....");
                return false;
            }
        }

        public bool initialize(String port)
        {
            if (gsmComm == null)
            {
                gsmComm = new GSM_SMS();
            }
            try
            {
                gsmComm.initializePort(port);
                if( gsmComm.setMessageMode(GSM_SMS.MessageMode.TEXT) == false)
                    return false;
            }
            catch (GSM_SMSException g)
            {
                MessageBox.Show(g.message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }


        void comm_MessageSendComplete(object sender, MessageEventArgs e)
        {
           
            
            
        }
        



        private int? findGSMCommPort()
        {
            String[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            int? portNumber = null;

            foreach (string portName in portNames)
            {
                String[] port = portName.Split(new string[] { "COM" }, StringSplitOptions.None);

                GsmCommMain comm = new GsmCommMain(Int32.Parse(port[1]), gsmBaudRate/*baudRate*/, gsmTimeOut/*timeout*/);
                try
                {
                    comm.Open();

                    if (comm.IsConnected())
                    {

                        AddressData addrData = comm.GetSmscAddress();
                        portNumber = Convert.ToInt32(port[1]);

                   }
                   comm.Close();
                   if (portNumber != null)
                       break;
                }
                catch (Exception e)
                {
                    if( comm.IsOpen())
                        comm.Close();
                }
            }

            
            return portNumber;
        }

        

        public void cleanUp()
        {
            if (comm == null)
                return;

            //comm.DisableMessageNotifications();     //disable message notifications
            // unregister all the event handlers.
            comm.MessageReceived -= new MessageReceivedEventHandler(comm_MessageReceived);

            comm.MessageSendComplete -= new GsmCommMain.MessageEventHandler(comm_MessageSendComplete);
            
            // Close connection to phone
            if ( comm.IsOpen())
                comm.Close();
            
            

            logger = null;
        }

        private void ShowException(Exception ex)
        {
            logger("Error: " + ex.Message + " (" + ex.GetType().ToString() + ")");
        }

        public  void send(String message , ArrayList recptList)
        {
            //#region GSMCommCode
            //// Check the message length to decide whether multiple pdus have to be created
            //// For UNICODE masimu of 70 characters can be sent in a single sms; 160 if not 
            //// unicode.

            //bool concatenate = false;
            //if (
            //    (unicode && message.Length > SmsPdu.MaxUnicodeTextLength)
            //    ||
            //    (!unicode && message.Length > SmsPdu.MaxTextLength)
            //    )
            //{
            //    concatenate = true;
            //}

            //try
            //{
            //    foreach (string recpt in recptList)
            //    {
            //        if (concatenate)
            //        {
            //            // Create the pdus.
            //            OutgoingSmsPdu[] pdus = CreateConcatMessage(message, recpt, unicode);

            //            // Send'em
            //            if (pdus != null)
            //            {
            //                string msg = String.Format( "\n Sending message to {0}" , recpt);
            //                logger(msg);
            //                SendMultiple(pdus);
            //            }
            //        }
            //        else
            //        {
            //            // Send an SMS message
            //            SmsSubmitPdu pdu = null;

            //            if (!alert && !unicode)
            //            {
            //                // The straightforward version
            //                pdu = new SmsSubmitPdu(message, recpt, smscAddress);
                            

            //            }
            //            else
            //            {
            //                // The extended version with dcs
            //                byte dcs;

            //                // Alert is a class 0 message.
            //                // For unicode, the Data code scheme is 16 bit
            //                if (unicode)
            //                {
            //                    if (alert)
            //                    {
            //                        dcs = DataCodingScheme.Class0_16Bit;
            //                    }
            //                    else
            //                    {
            //                        dcs = DataCodingScheme.NoClass_16Bit;
            //                    }
            //                }
            //                else
            //                {
            //                    if (alert)
            //                    {
            //                        dcs = DataCodingScheme.Class0_7Bit;
            //                    }
            //                    else
            //                    {
            //                        dcs = DataCodingScheme.NoClass_7Bit;
            //                    }
            //                }

            //                pdu = new SmsSubmitPdu(message, recpt, smscAddress, dcs);
            //            }

            //            // If a status report should be generated, set that here
            //            if (statReport)
            //                pdu.RequestStatusReport = true;

            //            // Send the message
            //            string msg = String.Format("\n Sending message to {0}", recpt);
            //            logger(msg);
            //            comm.SendMessage(pdu);

            //            logger("Message sent.");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ShowException(ex);
            //    return;
            //}
            //#endregion
            try
            {
                foreach (string recpt in recptList)
                {
                    logger("Sending Message to : " + recpt);
                    if (gsmComm.sendSMS(recpt, message) == true)
                        logger("Message Sent Successfully");
                    else
                        logger("Failed to Send Message");
                }
            }
            catch (Exception e)   
            {
                ShowException(e);
                return;
            }
        }

        private OutgoingSmsPdu[] CreateConcatMessage(string message, string number, bool unicode)
        {
            OutgoingSmsPdu[] pdus = null;
            try
            {
                if (!unicode)
                {
                    logger("Creating concatenated message.");
                    pdus = SmartMessageFactory.CreateConcatTextMessage(message, number);
                }
                else
                {
                    logger("Creating concatenated Unicode message.");
                    pdus = SmartMessageFactory.CreateConcatUnicodeTextMessage(message, number);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
                return null;
            }

            if (pdus.Length == 0)
            {
                logger("Error: No PDU parts have been created!");
                return null;
            }
            else
            {
                logger(pdus.Length.ToString() + " message part(s) created.");
            }

            return pdus;
        }

        private void SendMultiple(OutgoingSmsPdu[] pdus)
        {
            int num = pdus.Length;
            try
            {
                // Send the created messages
                int i = 0;
                foreach (OutgoingSmsPdu pdu in pdus)
                {
                    i++;
                    logger("Sending message " + i.ToString() + " of " + num.ToString() + "...");
                    comm.SendMessage(pdu);
                }
                logger("Done.");
            }
            catch (Exception ex)
            {
                ShowException(ex);
                logger("Message sending aborted because of an error.");
            }
        }

        private void comm_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                IMessageIndicationObject obj = e.IndicationObject;
                if (obj is MemoryLocation)
                {
                    MemoryLocation loc = (MemoryLocation)obj;
                    string logmsg = string.Format("New message received in storage \"{0}\", index {1}.", loc.Storage, loc.Index);

                    logger(logmsg);

                    DecodedShortMessage msg = comm.ReadMessage(loc.Index, loc.Storage);
                    ShowMessage(msg.Data);
                    return;
                }

                if (obj is ShortMessage)
                {
                    ShortMessage msg = (ShortMessage)obj;
                    SmsPdu pdu = comm.DecodeReceivedMessage(msg);
                    logger("New message received:");
                    ShowMessage(pdu);

                    return;
                }
                logger("Error: Unknown notification object!");
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }

        }

        private void ShowMessage(SmsPdu pdu)
        {
            if (pdu is SmsSubmitPdu)
            {
                // Stored (sent/unsent) message
                SmsSubmitPdu data = (SmsSubmitPdu)pdu;
                logger("SENT/UNSENT MESSAGE");
                logger("Recipient: " + data.DestinationAddress);
                logger("Message text: " + data.UserDataText);
                logger("-------------------------------------------------------------------");
                return;
            }
            if (pdu is SmsDeliverPdu)
            {
                // Received message
                SmsDeliverPdu data = (SmsDeliverPdu)pdu;
                logger("RECEIVED MESSAGE");
                logger("Sender: " + data.OriginatingAddress);
                logger("Sent: " + data.SCTimestamp.ToString());
                logger("Message text: " + data.UserDataText);
                logger("-------------------------------------------------------------------");
                return;
            }
            if (pdu is SmsStatusReportPdu)
            {
                // Status report
                SmsStatusReportPdu data = (SmsStatusReportPdu)pdu;
                logger("STATUS REPORT");
                logger("Recipient: " + data.RecipientAddress);
                logger("Status: " + data.Status.ToString());
                logger("Timestamp: " + data.DischargeTime.ToString());
                logger("Message ref: " + data.MessageReference.ToString());
                logger("-------------------------------------------------------------------");
                return;
            }
            logger("Unknown message type: " + pdu.GetType().ToString());
        }


        internal void readAllMessages()
        {
            logger("Read from <2>:\n1.SIM \n2.Phone ");
            string msgStorage;
            string usrInput = Console.ReadLine();
            if (usrInput.Length <= 0 || usrInput.Equals("2"))
                msgStorage = PhoneStorageType.Phone;
            else
                msgStorage = PhoneStorageType.Sim;

            try
            {
                // Read all SMS messages from the storage
                DecodedShortMessage[] messages = comm.ReadMessages(PhoneMessageStatus.All, msgStorage);
                foreach (DecodedShortMessage message in messages)
                {
                    logger(string.Format("Message status = {0}, Location = {1}/{2}",
                        StatusToString(message.Status), message.Storage, message.Index));
                    ShowMessage(message.Data);
                }
                logger(string.Format("{0,9} messages read.", messages.Length.ToString()));
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private string StatusToString(PhoneMessageStatus status)
        {
            // Map a message status to a string
            string ret;
            switch (status)
            {
                case PhoneMessageStatus.All:
                    ret = "All";
                    break;
                case PhoneMessageStatus.ReceivedRead:
                    ret = "Read";
                    break;
                case PhoneMessageStatus.ReceivedUnread:
                    ret = "Unread";
                    break;
                case PhoneMessageStatus.StoredSent:
                    ret = "Sent";
                    break;
                case PhoneMessageStatus.StoredUnsent:
                    ret = "Unsent";
                    break;
                default:
                    ret = "Unknown (" + status.ToString() + ")";
                    break;
            }
            return ret;
        }

    }
}