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

using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Timers;

using GsmComm.GsmCommunication;



namespace gsm.sms
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SMSAlerter : Window
    {

        DataAccess dataAccess = null;

        SMS sms = null;

        System.Timers.Timer timer;
        double timerTick = 100;//default value

        bool initializationAttempted = false;
        bool waitingForStartup = false;

        String gsmPort = string.Empty;
        int msgCount = 0;
        public SMSAlerter()
        {
            InitializeComponent();
            

            dataAccess = new DataAccess();

            timerTick = Convert.ToDouble(ConfigurationSettings.AppSettings["TimerTick"]);

            gsmPort = ConfigurationSettings.AppSettings["GSM_PORT"];

            timer = new System.Timers.Timer(50); 
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = false;
            timer.Start();



        }

        void  timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (initializationAttempted == false)
            {
                //#region WaitForStartupNotificaton
                //String waitForStartup = ConfigurationSettings.AppSettings["WaitForStartup"];
                //String startupKey = ConfigurationSettings.AppSettings["StartupKey"];
                //String startupValue = ConfigurationSettings.AppSettings["StartupValue"];
                //double startupWaitPeriod = Convert.ToDouble(ConfigurationSettings.AppSettings["StartupWaitPeriod"]);
                
                //if (waitForStartup == "Yes")
                //{
                //    if (waitingForStartup == false)
                //    {
                //        addMsg("Waiting For Startup ");
                //        waitingForStartup = true;
                //    }
                //    String status = dataAccess.getConfigValue(startupKey);
                //    if (status != startupValue)
                //    {
                //        timer.Interval = startupWaitPeriod;
                //        timer.Start();
                //        return;

                //    }
                    
                //    addMsg("Received Startup");

                //}
                //#endregion

                if (gsmPort == String.Empty)
                {
                    addMsg("Attempting Initialzation....");
                }
                else
                {
                    addMsg("Attempting Initialization on Port: " + gsmPort);
                }

                sms = new SMS(addMsg);

                bool result = sms.initialize(gsmPort);

                if (result == false)
                {
                    addMsg("Initialization Failed !! ");
                }
                else
                {
                    addMsg("Initialization Successful");
                    
                    //timer.Interval = timerTick * 1000;
                    timer.Start();

                }
                initializationAttempted = true;
            }
            else
            {
                DataTable dt = dataAccess.getOpenSMSAlerts();

                if (dt.Rows.Count > 0)
                {
                    Dictionary<String, ArrayList> smsList = new Dictionary<string, ArrayList>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        String message = (String)dt.Rows[i]["message"];
                        String receiver = (String)dt.Rows[i]["receiver"];
                        if (smsList.ContainsKey(message))
                        {
                            smsList[message].Add(receiver);
                        }
                        else
                        {
                            ArrayList receiverList = new ArrayList();
                            receiverList.Add(receiver);
                            smsList.Add(message, receiverList);
                        }
                    }

                    foreach (KeyValuePair<String, ArrayList> smsMsg in smsList)
                    {
                        sms.send(smsMsg.Key, smsMsg.Value);
                    }


                }
                timer.Start(); 
            }
	        
        }




        private void updateMsg(String msg)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                           
                msgCount++;
                if (msgCount >= 20)
                {
                    tbMsg.Clear();
                    msgCount = 0;
                }
                tbMsg.Text = tbMsg.Text
                    + msg;

            }
            ));
        }

        private void addMsg(String msg)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                msgCount++;
                if (msgCount >= 20)
                {
                    tbMsg.Clear();
                    msgCount = 0;
                }
                tbMsg.Text = tbMsg.Text
                    + Environment.NewLine
                    + msg;
            }
            ));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dataAccess != null)
                dataAccess.close();
            if( sms != null)
                sms.cleanUp();

            if (timer.Enabled)
                timer.Stop();

        }

        
    }
}
