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
using System.ComponentModel;
using System.Timers;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.IO;
using ias.shared;


namespace ias.client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        DataAccess dataAccess = null;
        DataTable[] lineInfoTable = null;
        DataTable[] modelInfoTable = null;

        DataTable dt = null, linesInfoTable;

        ShiftCollection shifts = null;

        String[] lines = null;
        string[] lineIds = null;
        string[] seperator = { "," };
        String lineSelection = String.Empty;
        System.Timers.Timer timer = null;

        int lineViewIndex = -1;
        int timerElapsedCount = -1;
        bool marqueeLoaded = false;

        System.Timers.Timer appTimer = null;
        double timertick = 1000.0;
        double updateInterval = 0;

        DoubleAnimation marqueeAnimation = null;
        DoubleAnimation issueMarqueeAnimation = null;

        String dataSeperator = ";";

        public enum DeviceCommand { SET_PLANNED_QUANTITY = 1 , SET_SCHEDULE = 2, SET_SHIFT_TIMINGS = 3 , 
                                    SET_BREAK_TIMINGS = 4 , SET_RTC = 5, SET_QA_PERIOD = 6 , SET_LINE_NAME = 7,
                                    SET_LINE_MODEL = 8,UPDATE_LINE_DATA = 9
        };


        public enum TransactionStatus
        {
            NONE = 0, OPEN = 1, INPROCESS = 2,
            COMPLETE = 3, TIMEOUT = 4
        };

        


        LineStatusCollection[] lineStatus= null;
        String customerLogoPath = String.Empty;


        double issueMarqueeSpeed = 0.0;
        double messageMarqueeSpeed = 0.0;

        public Window1()
        {
            InitializeComponent();

            timertick = Convert.ToDouble(ConfigurationSettings.AppSettings["TIMERTICK"]);

            customerLogoPath = ConfigurationSettings.AppSettings["Customer_Logo"];

            string MonitorEnable = ConfigurationSettings.AppSettings["Monitor_Enabled"];
            if (MonitorEnable == "Yes")
            {
                LineMonitor.Visibility = Visibility.Visible;
            }
            string quantityEnable = ConfigurationSettings.AppSettings["Quantity_Enabled"];

            if (quantityEnable == "Yes")
            {
                LineStatsGrid.Columns[1].Visibility = Visibility.Visible;
                LineStatsGrid.Columns[2].Visibility = Visibility.Visible;
            }

            issueMarqueeSpeed = Convert.ToDouble(ConfigurationSettings.AppSettings["ISSUE_MARQUEE_SPEED"]);
            messageMarqueeSpeed = Convert.ToDouble(ConfigurationSettings.AppSettings["MESSAGE_MARQUEE_SPEED"]);


            try
            {
                
                //customerLogo.Source = new BitmapImage(new Uri(customerLogoPath,UriKind.RelativeOrAbsolute));
                dataAccess = new DataAccess();
                dpFrom.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }


            catch (SqlException s)
            {
                MessageBox.Show("Unable to Connect to DataBase ", "ERROR",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }



            lines = new String[3];

            lines[0] = ConfigurationSettings.AppSettings["Lines_List_1"];
            lines[1] = ConfigurationSettings.AppSettings["Lines_List_2"];
            lines[2] = ConfigurationSettings.AppSettings["Lines_List_3"];


            lineInfoTable = new DataTable[3];
            lineInfoTable[0] = dataAccess.getProductionLineInfo(lines[0]);
            lineInfoTable[1] = dataAccess.getProductionLineInfo(lines[1]);
            lineInfoTable[2] = dataAccess.getProductionLineInfo(lines[2]);

            linesInfoTable = dataAccess.getProductionLineInfo();

            cmbProductionLineSelector.DataContext = linesInfoTable;

            shifts = dataAccess.getShifts();

            foreach (Shift s in shifts)
            {
                s.Sessions = dataAccess.getSessions(s.ID);
            }




            lineStatus = new LineStatusCollection[3];


            for (int i = 0; i < 3; i++)
            {
                lineStatus[i] = new LineStatusCollection();
            }


            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < lineInfoTable[j].Rows.Count; i++)
                {
                    lineStatus[j].Add(new LineStatus((String)lineInfoTable[j].Rows[i]["description"],
                        (int)lineInfoTable[j].Rows[i]["id"]));

                }
            }


            marqueeAnimation = new DoubleAnimation();
            //marqueeAnimation.Completed += new EventHandler(marqueeAnimation_Completed);


            issueMarqueeAnimation = new DoubleAnimation();
            issueMarqueeAnimation.Completed += new EventHandler(issueMarqueeAnimation_Completed);

            appTimer = new System.Timers.Timer(10);
            appTimer.Elapsed += new System.Timers.ElapsedEventHandler(appTimer_Elapsed);
            appTimer.AutoReset = false;

            
            appTimer.Start();


            TBLine.SelectedIndex = 1;
            tbIssueMarquee_Loaded();

            cmbShiftSelector.DataContext = shifts;

        }

        void marqueeAnimation_Completed(object sender, EventArgs e)
        {
            tbMarquee_Loaded();
        }

        void issueMarqueeAnimation_Completed(object sender, EventArgs e)
        {
            tbIssueMarquee_Loaded();
        }


        void appTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            appTimer.Stop();

            appTimer.Interval = 500;
            timerElapsedCount++;
            if (timerElapsedCount >= (messageMarqueeSpeed*2/3))
            {
                timerElapsedCount = 0;
            }

            if (timerElapsedCount == 0)
            {
                lineViewIndex++;
                if (lineViewIndex >= 3)
                {
                    lineViewIndex = 0;
                    marqueeLoaded = false;
                }
                
                int targetShift = 0;
                int targetSession = 0;
                DeviceCommand cmd;
                String commandData = String.Empty;

                foreach (LineStatus l in lineStatus[lineViewIndex])
                {
                    l.ActualQuantity = string.Empty;
                    l.TargetQuantity = String.Empty;
                }
                TimeSpan ts = DateTime.Now.TimeOfDay;

                for (int i = 0; i < shifts.Count; i++)
                {
                    for (int j = 0; j < shifts[i].Sessions.Count; j++)
                    {
                        if (shifts[i].Sessions[j].IsWithin(ts))
                        {
                            if (j == 0)
                            {
                                targetShift = shifts[i].ID - 1;
                                targetSession = shifts[i].Sessions[shifts[i].Sessions.Count - 1].ID;
                            }
                            else
                            {
                                targetShift = shifts[i].ID;
                                targetSession = shifts[i].Sessions.getTargetSession(ts).ID - 1;
                            }
                            break;
                        }

                    }
                    if (targetShift != 0)
                        break;
                }
                
                foreach (LineStatus l in lineStatus[lineViewIndex])
                {
                    l.TargetQuantity = dataAccess.getTargetQuantity(targetShift,targetSession,l.id);
                    l.ActualQuantity = dataAccess.getActualQuantity(l.id);
                }
                


                if ((lineViewIndex == 0) && marqueeLoaded == false)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        tbMarquee_Loaded();
                        //tbIssueMarquee_Loaded();

                    }));
                    marqueeLoaded = true;
                }



                List<int> stationList = dataAccess.getBreakDownStatus(lines[lineViewIndex]);

                foreach (LineStatus ls in lineStatus[lineViewIndex])
                {
                    if (stationList.Contains(ls.id))
                    {
                        ls.Breakdown = 1;
                    }
                    else
                    {
                        ls.Breakdown = 0;
                    }
                }


                stationList = dataAccess.getQualityStatus(lines[lineViewIndex]);

                foreach (LineStatus ls in lineStatus[lineViewIndex])
                {
                    if (stationList.Contains(ls.id))
                    {
                        ls.Quality = 1;
                    }
                    else
                    {
                        ls.Quality = 0;
                    }
                }


                stationList = dataAccess.getMaterialShortageStatus(lines[lineViewIndex]);

                foreach (LineStatus ls in lineStatus[lineViewIndex])
                {
                    if (stationList.Contains(ls.id))
                    {
                        ls.MaterialShortage = 1;
                    }
                    else
                    {
                        ls.MaterialShortage = 0;
                    }
                }








            }




            foreach (LineStatus ls in lineStatus[lineViewIndex])
            {
                ls.updateStatus();
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
 new Action(() =>
 {
     LineStatsGrid.DataContext = null;
     LineStatsGrid.DataContext = lineStatus[lineViewIndex];

 }));

            appTimer.Start();
        }





        private void setupShiftTime(ref TimeSpan ts, String t)
        {
            if (t == String.Empty)
            {
                return;
            }
            else
            {
                try
                {
                    String[] timeparams = t.Split(':');
                    ts = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                        int.Parse(timeparams[2]));
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appTimer.Stop();
            dataAccess.close();
        }


        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

            int lineIndex = cmbProductionLineSelector.SelectedIndex;

            if (lineIndex == -1)
            {
                MessageBox.Show("Please Select Line  ", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            int lineId = (int)linesInfoTable.Rows[lineIndex]["id"];
            String commandData = String.Empty;
            DateTime date = DateTime.Now;
            if (NPcheckBox.IsChecked == true)
            {
                commandData =
                    ((Shift)cmbShiftSelector.SelectedItem).ID.ToString() + dataSeperator + "0";
                try
                {
                    dt = dataAccess.addCommand(lineId,
                                             (int)DeviceCommand.UPDATE_LINE_DATA, (int)TransactionStatus.OPEN,
                                             commandData, date.ToString());
                    MessageBox.Show("Command Sent to Server  ", "Info",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException sq)
                {
                    MessageBox.Show("Error Updating Line Info\n Contact Administrator  ", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }

            else
            {

                foreach (shiftConfig s in shiftConfigTable.Items)
                {
                    if (int.Parse(s.PlannedQuantity) <= 0)
                    {
                        continue;
                    }
                    commandData =
                           ((Shift)cmbShiftSelector.SelectedItem).ID.ToString() + dataSeperator +
                           s.ID.ToString() + dataSeperator + s.PlannedQuantity;





                    try
                    {

                        dt = dataAccess.addCommand(lineId,
                                             (int)DeviceCommand.SET_PLANNED_QUANTITY, (int)TransactionStatus.OPEN,
                                             commandData, date.ToString());
                    }
                    catch (SqlException sq)
                    {
                        MessageBox.Show("Error Setting Target Quantity\n Contact Administrator  ", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                MessageBox.Show("Target Quantity sent to Server  ", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
        

        private void cmbProductionLineSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            shiftConfigTable.Items.Clear();

            if (cmbProductionLineSelector.SelectedIndex == -1)
            {
                return;
            }

            TBLine.Visibility = Visibility.Visible;

            

            cmbShiftSelector.SelectedIndex = 0;
            cmbShiftSelector.IsEnabled = true;
           
        }

            
            

        

        public class shiftConfig : IEditableObject, INotifyPropertyChanged
        {
            int id;
            public int ID
            {
                get { return id; }
                set
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }

            String shift;
            public String Shift
            {
                get { return shift; }
                set
                {
                    shift = value;
                    OnPropertyChanged("Shift");
                }
            }
            int plannedQuantity;
            public String PlannedQuantity
            {
                get { return plannedQuantity.ToString(); }
                set
                {
                    try
                    {
                        plannedQuantity = int.Parse(value);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Invalid Input for Shift : " + Shift, "Error",
                                            MessageBoxButton.OK, MessageBoxImage.Error);

                    }

                    OnPropertyChanged("PlannedQuantity");
                }

            }


            public shiftConfig(int number)
            {
                switch (number)
                {
                    case 0: shift = "A";
                        break;
                    case 1: shift = "B";
                        break;
                    case 2: shift = "C";
                        break;
                    default: break;
                }

                plannedQuantity = 0;

            }

            public shiftConfig(String name ,int id,string targetQuatity)
            {
                Shift = name;
                ID = id;
                PlannedQuantity = targetQuatity;
            }


            #region IEditableObjectMembers
            void IEditableObject.BeginEdit()
            {
            }
            void IEditableObject.CancelEdit()
            {
            }
            void IEditableObject.EndEdit()
            {
            }
            #endregion

            #region INotifyPropetyChangedHandler
            public event PropertyChangedEventHandler PropertyChanged;
            // Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
            #endregion
        }




        private void tbMarquee_Loaded()
        {
            tbMarquee.Text = dataAccess.getMarquee();

            tbMarquee.UpdateLayout();
            cMarquee.Width = this.Width;

            double height = cMarquee.ActualHeight - tbMarquee.ActualHeight;
            tbMarquee.Margin = new Thickness(0, height / 2, 0, 0);
            marqueeAnimation.From = -tbMarquee.ActualWidth;
            marqueeAnimation.To = cMarquee.ActualWidth;

            double duration = (marqueeAnimation.To.Value - marqueeAnimation.From.Value) / 20;

            marqueeAnimation.Duration = new Duration(TimeSpan.FromSeconds(messageMarqueeSpeed));
            tbMarquee.BeginAnimation(Canvas.RightProperty, marqueeAnimation);




        }





        private void tbIssueMarquee_Loaded()
        {
            tbIssueMarquee.Text = dataAccess.getIssueMarquee();


            tbIssueMarquee.UpdateLayout();

            cIssueMarquee.Width = this.Width;

            double height = cIssueMarquee.ActualHeight - tbIssueMarquee.ActualHeight;
            tbIssueMarquee.Margin = new Thickness(0, height / 2, 0, 0);
            issueMarqueeAnimation.From = -tbIssueMarquee.ActualWidth;
            issueMarqueeAnimation.To = cIssueMarquee.ActualWidth;
            //issueMarqueeAnimation.RepeatBehavior = RepeatBehavior.Forever;

            double duration = (issueMarqueeAnimation.To.Value - issueMarqueeAnimation.From.Value) / issueMarqueeSpeed;

            

            issueMarqueeAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
            tbIssueMarquee.BeginAnimation(Canvas.RightProperty, issueMarqueeAnimation);
        }



        #region ReportsTab

        System.Data.DataTable ReportTable = null;
        AvailabilityReport avReport = null;

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            ReportTable = null;
            avReport = null;
            dgReportGrid.DataContext = null;
            dgReportGrid.Visibility = Visibility.Visible;
            if (cmbReportSelector.SelectedIndex == 0)
            {

                ReportTable = dataAccess.GetReportData(dpFrom.SelectedDate.Value, dpTo.SelectedDate.Value);

                dgReportGrid.DataContext = ReportTable;

                
            }
            else if (cmbReportSelector.SelectedIndex == 1)
            {
                avReport = new AvailabilityReport();
                int[] lineids = {1,2,3,4,5,6,7,8,9,10};
                foreach (Shift s in shifts)
                {
                    if (s.Name != "GEN_SHIFT")
                    {
                        String date = String.Empty;
                        foreach (Session se in s.Sessions)
                        {
                            DateTime f = new DateTime(dpFrom.SelectedDate.Value.Year,
                                dpFrom.SelectedDate.Value.Month,
                                dpFrom.SelectedDate.Value.Day,
                                TimeSpan.Parse(se.StartTime).Hours,
                                TimeSpan.Parse(se.StartTime).Minutes,
                                TimeSpan.Parse(se.StartTime).Seconds
                                );
                            
                                
                            DateTime t = new DateTime(dpTo.SelectedDate.Value.Year,
                                dpTo.SelectedDate.Value.Month,
                                dpTo.SelectedDate.Value.Day,
                                TimeSpan.Parse(se.EndTime).Hours,
                                TimeSpan.Parse(se.EndTime).Minutes,
                                TimeSpan.Parse(se.EndTime).Seconds
                                );
                            if (s.Name == "THIRD SHIFT" && se.Name != "FIRST HOUR")
                            {
                                f = f.AddDays(1);
                                t = t.AddDays(1);
                                date = t.ToShortDateString();
                            }
                            else
                                date = f.ToShortDateString();
                            foreach(int n in lineids)
                            {
                                
                                DataTable dt = dataAccess.GetHourlyReportData(n, f, t);
                                if (dt.Rows.Count == 0) continue;
                                Availability av = new Availability();
                                for (int j = 0; j < dt.Rows.Count; j++)
                                {

                                    IssueDetails issueDetail = new IssueDetails();
                                    issueDetail.Line = (int)dt.Rows[j]["LINE"];
                                    issueDetail.Station = (int)dt.Rows[j]["STATION"];
                                    issueDetail.Tolerance = (int)dt.Rows[j]["Tolerance"];
                                    issueDetail.Raised = (TimeSpan)dt.Rows[j]["Raised"];
                                    if (dt.Rows[j]["Resolved"] == DBNull.Value)
                                        issueDetail.Resolved = TimeSpan.Parse(se.EndTime);
                                    else
                                    {
                                        issueDetail.Resolved = (TimeSpan)dt.Rows[j]["Resolved"];
                                        if (issueDetail.Resolved > TimeSpan.Parse(se.EndTime))
                                        {
                                            issueDetail.Resolved = TimeSpan.Parse(se.EndTime);
                                        }
                                    }

                                    TimeSpan ToleranceLimit = new TimeSpan(0, issueDetail.Tolerance, 0);
                                    if (issueDetail.Resolved - issueDetail.Raised > ToleranceLimit)
                                    {
                                        av.Add(issueDetail);
                                    }
                                }
                                avReport.Add(new AvailabilityRecord((string)dt.Rows[0]["LINENAME"],
                                    date, f.ToShortTimeString() + "-" + t.ToShortTimeString(),
                                    av.getAvailability( f.TimeOfDay, t.TimeOfDay).ToString(),av.EffectiveDowntime.ToString()));



                            }
                        }
                    }
                }
                dgReportGrid.AutoGenerateColumns = true;
                dgReportGrid.DataContext = avReport;
            }



        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV (.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {

                try
                {
                    if (cmbReportSelector.SelectedIndex == 0)
                    {
                        String filename = dlg.FileName;
                        FileInfo report = new FileInfo(filename);
                        StreamWriter sw = report.CreateText();
                        sw.Write("DATE,LINE,STATION,ISSUE,DETAILS,RAISED,RESOLVED,DOWNTIME" + Environment.NewLine);

                        for (int i = 0; i < ReportTable.Rows.Count; i++)
                        {
                            String raisedTime = ReportTable.Rows[i]["RAISED"] == DBNull.Value ? ("")
                                                : ((TimeSpan)ReportTable.Rows[i]["RAISED"]).ToString();
                            String resolvedTime = ReportTable.Rows[i]["RESOLVED"] == DBNull.Value ? ("")
                                                : ((TimeSpan)ReportTable.Rows[i]["RESOLVED"]).ToString();

                            String downTime = ReportTable.Rows[i]["DOWNTIME"] == DBNull.Value ? ("")
                                                : ((TimeSpan)ReportTable.Rows[i]["DOWNTIME"]).ToString();

                            String reportEntry = (String)ReportTable.Rows[i]["DATE"] + ","
                                //+ (String)ReportTable.Rows[i]["CELL"] + ","
                                                + (String)ReportTable.Rows[i]["LINE"] + ","
                                              
                                                + (String)ReportTable.Rows[i]["ISSUE"] + ","
                                //+ (String)ReportTable.Rows[i]["MESSAGE"] + ","
                                                + raisedTime + ","
                                                + resolvedTime + ","
                                                + downTime;
                            sw.Write(reportEntry);
                            sw.Write(Environment.NewLine);
                        }
                        sw.Close();
                        MessageBox.Show("Report Generation Successful", "Report Generation Message",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if( cmbReportSelector.SelectedIndex == 1)
                    {
                        String filename = dlg.FileName;
                        FileInfo report = new FileInfo(filename);
                        StreamWriter sw = report.CreateText();
                        sw.Write("DATE,LINE,SESSION,AVAILABILITY(%)" + Environment.NewLine);

                        foreach( AvailabilityRecord avr in avReport )
                        {
                             String reportEntry = avr.ToString();
                            sw.Write(reportEntry);
                            sw.Write(Environment.NewLine);
                        }
                        sw.Close();
                        MessageBox.Show("Report Generation Successful", "Report Generation Message",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                

                catch (Exception exp)
                {
                    MessageBox.Show("Error Generating Report" + Environment.NewLine + exp.Message, "Report Generation Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        private void cmbShiftSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            shiftConfigTable.DataContext = null;
            shiftConfigTable.Items.Clear();

            if (cmbProductionLineSelector.SelectedIndex == -1)
            {
                
                cmbShiftSelector.SelectedIndex = -1;
                cmbShiftSelector.IsEnabled = false;
                return;
            }

            if (cmbShiftSelector.SelectedIndex == -1)
                return;

            
            foreach (Session s in ((Shift)cmbShiftSelector.SelectedItem).Sessions)
            {
                shiftConfigTable.Items.Add(new shiftConfig(s.Name,s.ID,
                    dataAccess.getTargetQuantity(((Shift)cmbShiftSelector.SelectedItem).ID,s.ID,(int)linesInfoTable.Rows[cmbProductionLineSelector.SelectedIndex]["id"])));
            }


        }

        private void NPcheckBox_Checked(object sender, RoutedEventArgs e)
        {

                shiftConfigTable.IsEnabled = false;

        }

        private void NPcheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            shiftConfigTable.IsEnabled = true;
        }





    }
}
