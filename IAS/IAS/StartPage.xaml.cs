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
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using ias.andonmanager;
using System.Threading;
using System.Windows.Threading;

namespace IAS
{
    /// <summary>
    /// Interaction logic for startPage.xaml
    /// </summary>
    public partial class StartPage : Window
    {
        AndonManager andonManager = null;
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;
        Queue<int> lineQ = null;
        Queue<int> departmentQ = null;
        DataTable departmentTable = null;
        lineCollection lines = null;
        ShiftCollection shifts = null;
        ContactCollection contacts = null;

        Dictionary<int, Issue> Issues = null;
        List<double> timeout = null;

        System.Timers.Timer commandTimer = null;
        String dataSeperator = ";";
        String timingsDataSeperator = ":";

        Queue<TimeSpan> hourlyUpdateTimings;
        Queue<TimeSpan> shiftUpdateTimings;

        DateTime? nextHourlyUpdateTiming;
        DateTime? nextShiftUpdateTiming;

        int prevTabIndex = -1;
        DataTable dt;
        public enum DeviceCommand
        {
            SET_PLANNED_QUANTITY = 1, SET_SCHEDULE = 2, SET_SHIFT_TIMINGS = 3,
            SET_BREAK_TIMINGS = 4, SET_RTC = 5, SET_QA_PERIOD = 6, SET_LINE_NAME = 7,
            SET_LINE_MODEL = 8, UPDATE_LINE_DATA = 9
        };

        public StartPage()
        {
            try
            {
                InitializeComponent();
                _dbConnectionString  = ConfigurationSettings.AppSettings["DBConStr"];

                DataAccess.conStr = _dbConnectionString;

                MainMenu _mainMenu = new MainMenu(_dbConnectionString);
                _mainFrame.Navigate(_mainMenu);
                dataAccess = new DataAccess();
                lines = dataAccess.getLines();
                lineQ = new Queue<int>();

                foreach (line l in lines)
                    lineQ.Enqueue(l.ID);

                departmentTable = dataAccess.getDepartmentInfo("");

                departmentQ = new Queue<int>();
                for (int i = 0; i < departmentTable.Rows.Count; i++)
                    departmentQ.Enqueue((int)departmentTable.Rows[i]["id"]);
                andonManager = new AndonManager(lineQ,departmentQ,AndonManager.MODE.MASTER);

                shifts = dataAccess.getShifts();

                contacts = dataAccess.getContacts();

                commandTimer = new System.Timers.Timer(30 * 1000);
                commandTimer.Elapsed += new System.Timers.ElapsedEventHandler(commandTimer_Elapsed);
                commandTimer.AutoReset = false;

                commandTimer.Start();


                hourlyUpdateTimings = new Queue<TimeSpan>();
                shiftUpdateTimings = new Queue<TimeSpan>();
                String hourlyUpdateTimingsString = ConfigurationSettings.AppSettings["HourlyUpdateTimings"];

                String[] hourlyUpdateTimingsList = hourlyUpdateTimingsString.Split(dataSeperator.ToCharArray());

                foreach(String s in hourlyUpdateTimingsList )
                {
                    String[] timings = s.Split(timingsDataSeperator.ToCharArray());

                    int hours = Convert.ToInt32(timings[0]);
                    int minutes= Convert.ToInt32(timings[1]);
                    int seconds = Convert.ToInt32(timings[2]);
                    hourlyUpdateTimings.Enqueue(new TimeSpan(hours, minutes, seconds));
                }

                TimeSpan[] temp = new TimeSpan[hourlyUpdateTimings.Count];
                hourlyUpdateTimings.CopyTo(temp, 0);
                /*
                foreach (TimeSpan ts in temp)
                {
                    TimeSpan ts1 = hourlyUpdateTimings.Dequeue();
                    hourlyUpdateTimings.Enqueue(ts1);
                    if (ts >= DateTime.Now.TimeOfDay)
                    {
                        nextHourlyUpdateTiming = ts;
                        break;
                    }
                    
                }
                */
                nextHourlyUpdateTiming = getNextHourlyUpdateTime();

                Issues = new Dictionary<int, Issue>();
                timeout = dataAccess.getEscalationTimeout();
                               
                
                andonManager.andonAlertEvent += new EventHandler<AndonAlertEventArgs>(andonManager_andonAlertEvent);

                andonManager.start();

                dataAccess.updateIssueMarquee();
            }




            catch( Exception e)
            {
                tbMsg.Text+= e.Message;
            }
        }

        void commandTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            commandTimer.Stop();
            DataTable dt;

            DeviceCommand cmd;
            String commandData = String.Empty;
            dt = dataAccess.getCommands();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int lineId = (int)dt.Rows[i]["line_id"];
                cmd = (DeviceCommand)dt.Rows[i]["command"];
                commandData = (String)dt.Rows[i]["command_data"];
                processCommand(lineId, cmd, commandData);

                dataAccess.updateCommandStatus((int)dt.Rows[i]["id"],
                 DataAccess.TransactionStatus.COMPLETE);
            }

            if (nextHourlyUpdateTiming == null)
            {
                nextHourlyUpdateTiming = getNextHourlyUpdateTime();
            }

            else if (DateTime.Now >= nextHourlyUpdateTiming.Value)
            {
                generateHourlySummary();
                nextHourlyUpdateTiming = getNextHourlyUpdateTime();
                
            }
            commandTimer.Start();
            
        }

        DateTime? getNextHourlyUpdateTime()
        {
            DateTime? ts;
            
            List<Shift> shl = shifts.getShifts(TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss")));
            Shift s = new Shift();
            foreach (Shift sl in shl)
            {
                if (sl.Name != "GEN SHIFT")
                {
                    s = sl;
                    break;
                }
            }
            Session se = s.getSession(DateTime.Now.TimeOfDay);
            if (se == null)
                return null;
            TimeSpan temp = TimeSpan.Parse(se.EndTime);
            temp = temp.Add(new TimeSpan(0, 5, 0));
            if (DateTime.Now.TimeOfDay <= temp)
                ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, temp.Hours, temp.Minutes, temp.Seconds);
            else
            {
                DateTime n = DateTime.Now.AddDays(1);
                ts = new DateTime(n.Year, n.Month, n.Day, temp.Hours, temp.Minutes, temp.Seconds);
            }

            
            return ts;

        }

        DateTime? getNextShiftUpdateTime()
        {
            DateTime? ts;

            List<Shift> shl = shifts.getShifts(DateTime.Now.TimeOfDay);
            Shift s = new Shift();
            foreach (Shift sl in shl)
            {
                if (sl.Name != "GEN SHIFT")
                {
                    s = sl;
                    break;
                }
            }
            TimeSpan temp = TimeSpan.Parse(s.EndTime);
            temp = temp.Add(new TimeSpan(0, 5, 0));
            if (DateTime.Now.TimeOfDay <= temp)
                ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, temp.Hours, temp.Minutes, temp.Seconds);
            else
            {
                DateTime n = DateTime.Now.AddDays(1);
                ts = new DateTime(n.Year, n.Month, n.Day, temp.Hours, temp.Minutes, temp.Seconds);
            }


            return ts;


        }


        void generateHourlySummary()
        {
            
            TimeSpan temp = new TimeSpan(nextHourlyUpdateTiming.Value.Hour, nextHourlyUpdateTiming.Value.Minute - 6 , nextHourlyUpdateTiming.Value.Second );
            int id;
            Shift s = new Shift();
            List<Shift> shl = shifts.getShifts(temp);
            DataTable dt;
    
            Availability[] availability = new Availability[lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                availability[i] = new Availability();
            }


            foreach (Shift sl in shl)
            {
                if (sl.Name != "GEN SHIFT")
                {
                    s = sl;
                    break;
                }
            }

            Session se =  s.getSession(temp);

            

            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, stringtoTS(se.StartTime).Hours,
               stringtoTS(se.StartTime).Minutes, stringtoTS(se.StartTime).Seconds);

            DateTime to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, stringtoTS(se.EndTime).Hours,
                stringtoTS(se.EndTime).Minutes, stringtoTS(se.EndTime).Seconds);
            dt = dataAccess.GetReportData(from, to);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                IssueDetails issueDetail = new IssueDetails();
                issueDetail.Line = (int)dt.Rows[i]["LINE"];
                issueDetail.Station = (int)dt.Rows[i]["STATION"];
                issueDetail.Tolerance = (int)dt.Rows[i]["Tolerance"];
                issueDetail.Raised = (TimeSpan) dt.Rows[i]["Raised"];
                if( dt.Rows[i]["Resolved"] == DBNull.Value )
                    issueDetail.Resolved = stringtoTS(se.EndTime);
                else
                {   
                    issueDetail.Resolved = (TimeSpan)dt.Rows[i]["Resolved"];
                    if( issueDetail.Resolved > stringtoTS(se.EndTime) )
                    {
                        issueDetail.Resolved = stringtoTS(se.EndTime);
                    }
                }

                TimeSpan ToleranceLimit = new TimeSpan(0,issueDetail.Tolerance,0);
                if (issueDetail.Resolved - issueDetail.Raised > ToleranceLimit)
                {
                    availability[issueDetail.Line - 1].Add(issueDetail);
                }
            }


            ContactCollection cl = dataAccess.getHourlyUpdateContacts();

            String hourlySummary = "Hourly Summary MCB Finishing" + "\r\n";

            for (int i = 0; i < 6; i++)
            {
                hourlySummary+= "Line " + lines[i].ID.ToString()+":"+ getHourlyUpdateInfo(i, s.ID, se.ID)+","
                    +"Avl-"+ (availability[i].getAvailability(stringtoTS(se.StartTime), stringtoTS(se.EndTime))).ToString() + "%";
                hourlySummary += "\r\n";

                
            }
            foreach (Contact c in cl)
            {
                dataAccess.insertSmsTrigger(c.Number, hourlySummary, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),5);
            }

            hourlySummary = "Hourly Summary " + "\r\n";

            for (int i = 6, j = 1; i < 8; i++, j++)
            {
                hourlySummary += "RCCB Finishing Line " + j.ToString() + ":" + getHourlyUpdateInfo(i, s.ID, se.ID) + ","
                    + "Avl-" + (availability[i].getAvailability(stringtoTS(se.StartTime), stringtoTS(se.EndTime))).ToString() + "%";
                hourlySummary += "\r\n";


            }         
            
            for (int i = 9, j = 1; i < 11; i++, j++)
            {
                hourlySummary += "TIMNH Finishing Line " + j.ToString() + ":" + getHourlyUpdateInfo(i, s.ID, se.ID) + ","
                    + "Avl-" + (availability[i].getAvailability(stringtoTS(se.StartTime), stringtoTS(se.EndTime))).ToString() + "%";
                hourlySummary += "\r\n";


            }
            

            hourlySummary += "ADDITIM Line " + getHourlyUpdateInfo(8, s.ID, se.ID) + ","
                + "Avl-" + (availability[8].getAvailability(stringtoTS(se.StartTime), stringtoTS(se.EndTime))).ToString() + "%";
            hourlySummary += "\r\n";


            
            foreach (Contact c in cl)
            {
                dataAccess.insertSmsTrigger(c.Number, hourlySummary, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 5);
            }


           
        }

        public String getHourlyUpdateInfo(int line,int shift, int session)
        {
            String hourlyUpdateInfo = String.Empty;
                       
                hourlyUpdateInfo += dataAccess.getActualQuantity(lines[line].ID).ToString()
                    + "/" + dataAccess.getTargetQuantity(shift, session, lines[line].ID);
            return hourlyUpdateInfo;
        }

        void andonManager_andonAlertEvent(object sender, AndonAlertEventArgs e)
        {
            int recordId = -1;

            try
            {

                foreach (LogEntry lg in e.StationLog)
                {
                    if (lg.Department == 0)
                    {
                         dataAccess.updateActualQuantity(e.StationId, Convert.ToInt32( lg.Data));

                        tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                        {
                            tbMsg.Text +=lines.getLineName(e.StationId) + ":"+
                                "Production Quantity - " + lg.Data.ToString()+Environment.NewLine;
                        }));

                        continue;
                    }
                    String logMsg = String.Empty;

                    logMsg += lines.getLineName(e.StationId) + ":";
                    /*
                    String stationName = lines.getStationName(e.StationId, lg.Station);
                    if (stationName == string.Empty)
                        stationName = "Station #" + lg.Station.ToString();

                    logMsg += stationName + ":";
                     */
                    logMsg += departmentTable.Rows[lg.Department - 1]["description"] + "-";
                    //if (lg.Department == 3)
                    //{
                    //    logMsg += "PART #" + lg.Data;
                    //}
                    //else
                    //{

                    //    String className = lines.getClassName(e.StationId, lg.Station,
                    //                    lg.Department, Convert.ToInt32(lg.Data));

                    //    if (className == String.Empty)
                    //        lg.Data = "Class Code #" + Convert.ToInt32(lg.Data);
                    //    else lg.Data = className;

                    //    logMsg += "--" + lg.Data;
                    //}



                    recordId = dataAccess.findIssueRecord(e.StationId, lg.Station, lg.Department, lg.Data);
                    
                     


                    if (recordId == -1)     //new issue
                    {

                        recordId =
                            dataAccess.insertIssueRecord(e.StationId, lg.Station, lg.Department, lg.Data, logMsg);

                        Issue issue = new Issue(e.StationId,lg.Station,lg.Department,logMsg,timeout);
                        issue.issueEscalationEvent+=new EventHandler<issueEscalateEventArgs>(issue_issueEscalationEvent);

                        Issues.Add(recordId,issue);

                        issue.raise();

                        logMsg += "--Issue Raised at " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");

                        

                        
                    }
                    else
                    {
                        dataAccess.updateIssueStatus(recordId);

                        if (Issues.ContainsKey(recordId))
                        {
                            Issues[recordId].resolve();
                            Issues.Remove(recordId);
                        }

                        logMsg += "--Issue Resolved";
                    }
                    dataAccess.updateIssueMarquee();
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() => { tbMsg.Text += logMsg + Environment.NewLine; }));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK);
            }

        }

        void issue_issueEscalationEvent(object sender, issueEscalateEventArgs e)
        {
            Issue issue = (Issue)sender;

            
            List<Shift> sl = shifts.getShifts(DateTime.Now.TimeOfDay);

            List<int> slID = new List<int>();

            foreach( Shift s in sl)
                slID.Add(s.ID);




            List<Contact> receivers =  contacts.getContactList(issue.Line,slID ,
                                    issue.Department, e.escalationLevel);
            foreach (Contact c in receivers)
                dataAccess.insertSmsTrigger(c.Number, issue.Message + "-- Escalation Level" + e.escalationLevel.ToString(), 1, DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));


            if (e.escalationLevel > 0)
            {
                tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                        {
                            tbMsg.Text += issue.Message + "-- Escalation Level" + e.escalationLevel.ToString()
                                + Environment.NewLine;
                        }));
            }


        }


        private void processCommand(int lineId, DeviceCommand cmd, String cmdData)
        {
            

            String msg = String.Empty;
            String[] cmdParams = new String[5];
            int shift; Shift s;
            try
            {
                switch (cmd)
                {
                    case DeviceCommand.UPDATE_LINE_DATA:
                        updateMsg("Processing Command - " + DeviceCommand.UPDATE_LINE_DATA);
                        cmdParams = separateCommandData(cmdData);
                        shift = Convert.ToInt32(cmdParams[0]);
                        if (lines.getLineName(lineId) == String.Empty)
                        {
                            updateMsg("Line not supported");
                            return;
                        }
                        s = shifts.getShift(shift);
                        if ( s == null)
                        {
                            updateMsg( "Shift not supported");
                            return;
                        }
                        msg = "Setting NP for Line : {0},Shift : {1} ";
                        msg = String.Format(msg, lines.getLineName(lineId), s.Name);
                        dataAccess.updateNP(lineId, s.ID);
                        updateMsg(msg);
                        break;

                    case DeviceCommand.SET_PLANNED_QUANTITY:
                        updateMsg("Processing Command - "+ DeviceCommand.SET_PLANNED_QUANTITY);

                        cmdParams = separateCommandData(cmdData);
                        shift = Convert.ToInt32(cmdParams[0]);
                        int session = Convert.ToInt32(cmdParams[1]);
                        int plannedQty = int.Parse(cmdParams[2]);

                        if( lines.getLineName(lineId) == String.Empty)
                        {
                            updateMsg("Line not supported");
                            return;
                        }
                        s = shifts.getShift(shift);
                        if ( s == null)
                        {
                            updateMsg( "Shift not supported");
                            return;
                        }
                
                        Session se = s.Sessions.getSession(session);
                        if( se == null)
                        {
                            updateMsg("Session not supported");
                            return;
                        }
                        

                        msg = "Setting Target Qty for Line : {0},Shift : {1} Session:{2} Qty:{3}";



                        msg = String.Format(msg, lines.getLineName(lineId), s.Name,se.Name, plannedQty);

                        updateMsg(msg);

                        dataAccess.updateTargetQuantity(lineId, shift,session ,plannedQty);

                        break;

                    default:
                        break;
                }
            }
            catch (SqlException se)
            {

            }
        }

        private String[] separateCommandData(String cmdData)
        {

            return
                cmdData.Split(dataSeperator.ToCharArray());
        }



        void updateMsg(String msg)
        {
            tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
         new Action(() =>
         {
             tbMsg.Text += msg + Environment.NewLine;
         }));
        }


        TimeSpan stringtoTS(String time)
        {
            String sep = ":";
            String[] details = time.Split(sep.ToCharArray());
            int hours = Convert.ToInt32(details[0]);
            int minutes = Convert.ToInt32(details[1]);
            int seconds = Convert.ToInt32(details[2]);

            return new TimeSpan(hours, minutes, seconds);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            commandTimer.Stop();
            commandTimer.Close();
            commandTimer.Dispose();

            if (andonManager != null)
                andonManager.stop();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (dgOpenIssuesGrid.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Issue", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int lineNO;
            List<Byte> data = dataAccess.getIssueInfo((int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"], out  lineNO);
            //if (data != null)
            //    andonManager.addTransaction(lineNO,
            //        AndonCommand.CMD_CLEAR_ISSUE, data);


            dataAccess.updateIssueStatus((int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"]);


            if (Issues.ContainsKey((int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"]))
            {


                Issues[(int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"]].resolve();
                Issues.Remove((int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"]);
            }

            String logMsg = "--Issue Resolved";


            tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    new Action(() => { tbMsg.Text += logMsg + Environment.NewLine; }));


            dt.Rows.RemoveAt(dgOpenIssuesGrid.SelectedIndex);

            dgOpenIssuesGrid.DataContext = null;

            dgOpenIssuesGrid.DataContext = dt;
            dataAccess.updateIssueMarquee();

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prevTabIndex == tbMain.SelectedIndex)
                return;
            if (tbMain.SelectedIndex == 2)
            {
                dgOpenIssuesGrid.DataContext = null;
                dt = dataAccess.GetOpenIssues();
                dgOpenIssuesGrid.DataContext = dt;
            }
            prevTabIndex = tbMain.SelectedIndex;
        }
    }



    public class Availability
    {
        public int Line{get;set;}
        public List<IssueDetails> issues;

        public Availability()
        {
            issues = new List<IssueDetails>();
        }

        public void Add(IssueDetails issue)
        {
            if (issues.Count == 0)
            {
                issues.Add(issue);
                return;
            }
           foreach( IssueDetails id in issues)
           {

               if ((issue.Raised < id.Raised) && (issue.Resolved <= id.Resolved)) // overlap case 1
               {
                   issue.Resolved = id.Raised;
                   issues.Add(issue);
                   break;
               }
               else if ((issue.Raised >= id.Raised) && (issue.Resolved > id.Resolved)) // overlap case 2
               {
                   issue.Raised = id.Resolved;
                   issues.Add(issue);
                   break;
               }
               else if ((issue.Raised >= id.Raised) && (issue.Raised <= id.Resolved)) //overlap case 3
               {
                   continue;
               }
               else
               {
                   issues.Add(issue);
                   break ;
               }
           }
           
        }

        public int getAvailability(TimeSpan from , TimeSpan to)
        {
            int availability = 0;
            foreach (IssueDetails i in issues)
            {
                TimeSpan downtime = i.Resolved - i.Raised;
                availability += downtime.Hours * 60 * 60 + downtime.Minutes * 60 + downtime.Seconds;
            }

            int totalavailability = (to - from).Hours *60*60 + (to - from).Minutes*60 + (to - from).Seconds;

             
            int availabilityPercentage = ((totalavailability - availability) * 100/ totalavailability);

            if (availabilityPercentage < 0) availabilityPercentage = 0;

            return availabilityPercentage;
        }

    }



    public class IssueDetails
    {
        public int Line { get; set; }
        public int Station { get; set; }
        public int Tolerance { get; set; }

        public TimeSpan Raised { get; set; }
        public TimeSpan Resolved { get; set; }

        public IssueDetails()
        {
        }
    }
}
