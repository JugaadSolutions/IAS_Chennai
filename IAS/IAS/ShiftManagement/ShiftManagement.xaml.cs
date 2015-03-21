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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using System.Collections.ObjectModel;

namespace IAS
{
    /// <summary>
    /// Interaction logic for ShiftManagement.xaml
    /// </summary>
    public partial class ShiftManagement : PageFunction<String>
    {
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;

        ShiftCollection Shifts = null;


        public ShiftManagement()
        {
            InitializeComponent();
            dataAccess = new DataAccess();

            Shifts = dataAccess.getShifts();
            shiftControl.DataContext = Shifts;
            ((Label)shiftControl.aMDGroupBox.Header).Content = "SHIFTS";
            ((Label)sessionControl.aMDGroupBox.Header).Content = "SESSIONS";

            
            
  

        }



        private void shiftControl_selectionChanged(object sender, AddModifyDeleteSelectionChangedEventArgs e)
        {
            if (shiftControl.dgItem.SelectedIndex == -1)
                return;
            sessionControl.DataContext = null;
            sessionControl.DataContext = Shifts[shiftControl.dgItem.SelectedIndex].Sessions;
        }

        private void sessionControl_selectionChanged(object sender, AddModifyDeleteSelectionChangedEventArgs e)
        {

        }

        private void shiftControl_addClicked(object sender, EventArgs e)
        {
            ShiftInfo ShiftInfo = new ShiftInfo(null);
            ShiftInfo.Return += new ReturnEventHandler<shiftInfo>(ShiftInfo_Return);
            

            NavigationService.Navigate(ShiftInfo);

            NavigationService a = ShiftInfo.NavigationService;

        }

        void ShiftInfo_Return(object sender, ReturnEventArgs<shiftInfo> e)
        {
            if (e.Result == null)
                return;

            Shift Shift = new Shift();

            Shift.Name = e.Result.Name;
            Shift.StartTime = e.Result.StartTime;
            Shift.EndTime = e.Result.EndTime;

            dataAccess.addShift(e.Result.Name,e.Result.StartTime,e.Result.EndTime);
            Shifts.Add(Shift);
            shiftControl.dgItem.SelectedIndex = -1;
            shiftControl.btnAdd.Focus();
            
        }

        private void shiftControl_deleteClicked(object sender, EventArgs e)
        {

            if (shiftControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Shift", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            dataAccess.deleteShift(Shifts[shiftControl.dgItem.SelectedIndex].ID);
            Shifts.Remove(Shifts[shiftControl.dgItem.SelectedIndex]);

        }

        private void sessionControl_addClicked(object sender, EventArgs e)
        {
            if (shiftControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Shift", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            
            
            sessionInfo sessionInfo = new sessionInfo();
            sessionInfo.ShiftIndex = shiftControl.dgItem.SelectedIndex;

            SessionInfo SessionInfo = new SessionInfo(sessionInfo);

            SessionInfo.Return += new ReturnEventHandler<sessionInfo>(SessionInfo_Return);
            NavigationService.Navigate(SessionInfo);


            

        }

        void SessionInfo_Return(object sender, ReturnEventArgs<sessionInfo> e)
        {
            if (e.Result == null)
                return;

            Session Session = new Session(
                Shifts[e.Result.ShiftIndex].ID, Shifts[e.Result.ShiftIndex].Sessions.Count+1,
                e.Result.Name,e.Result.StartTime,e.Result.EndTime);

            Shifts[e.Result.ShiftIndex].Sessions.Add(Session);

            dataAccess.addSession(Shifts[e.Result.ShiftIndex].ID, Shifts[e.Result.ShiftIndex].Sessions.Count + 1,
                e.Result.Name, e.Result.StartTime, e.Result.EndTime);
            shiftControl.dgItem.SelectedIndex = e.Result.ShiftIndex;
            sessionControl.dgItem.SelectedIndex = -1;
            sessionControl.btnAdd.Focus();
            
        }

        private void sessionControl_deleteClicked(object sender, EventArgs e)
        {
            if (shiftControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Shift", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            if (sessionControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Session", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }


            dataAccess.deleteSession(Shifts[shiftControl.dgItem.SelectedIndex].ID,
                Shifts[shiftControl.dgItem.SelectedIndex].Sessions[sessionControl.dgItem.SelectedIndex].ID);
            Shifts[shiftControl.dgItem.SelectedIndex].Sessions.Remove(
                Shifts[shiftControl.dgItem.SelectedIndex].Sessions[sessionControl.dgItem.SelectedIndex]);

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }
    }


    public class Shift
    {
        public int ID { get; set; }
        public string Name { get; set; }
        TimeSpan startTime;
        public string StartTime
        {
            get { return startTime.ToString(); }
            set
            {
                if (value == String.Empty)
                {

                }
                else
                {
                    try
                    {
                        String[] timeparams = value.Split(':');
                        startTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                            int.Parse(timeparams[2]));
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        TimeSpan endTime;
        public string EndTime
        {
            get { return endTime.ToString(); }
            set
            {
                if (value == String.Empty)
                {

                }
                else
                {
                    try
                    {
                        String[] timeparams = value.Split(':');
                        endTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                            int.Parse(timeparams[2]));
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
        }

        public SessionCollection Sessions;

        DataAccess dataAccess;

        public Shift()
        {
        }

        public Shift(int id, string description, string startTime, string endTime)
        {
            ID = id;
            Name = description;
            StartTime = startTime;
            EndTime = endTime;
            Sessions = new SessionCollection();
            dataAccess = new DataAccess();
            Sessions = dataAccess.getSessions(ID);


        }


        public Shift(int id, string description, TimeSpan startTime, TimeSpan endTime)
        {
            ID = id;
            Name = description;
            this.startTime = startTime;
            this.endTime = endTime;
            Sessions = new SessionCollection();
            dataAccess = new DataAccess();
            Sessions = dataAccess.getSessions(ID);


        }


        public Session getSession(TimeSpan time)
        {
            foreach (Session s in Sessions)
            {
                if (s.IsWithin(time) == true)
                    return s;
            }
            return null;
        }

        public bool IsWithin(TimeSpan ts)
        {
            TimeSpan start = startTime;
            TimeSpan end = endTime;
            bool result = false;

            if (end < startTime)            //third shift 
            {
                if (ts <= startTime && ts < endTime)
                    result = true;
                else if (ts > startTime && ts > endTime)
                    result = true;
                
            }

            else
            {
                if (ts >= startTime && ts < endTime)
                    result = true;
                
            }
            return result;
        }
    }

    public class ShiftCollection : ObservableCollection<Shift>
    {
        public List<Shift> getShifts(TimeSpan time)
        {
            List<Shift> shiftList = new List<Shift>();
            IEnumerator<Shift> enumerator = this.GetEnumerator();
           
            while( enumerator.MoveNext())
            {
                if (enumerator.Current.IsWithin(time))
                {
                    shiftList.Add(enumerator.Current);
                }
                    
            }
            return shiftList ;
        }

        public Shift getShift(int id)
        {

            IEnumerator<Shift> enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID == id)
                    return enumerator.Current;
            }
            return null;
        }



    }

    public class shiftInfo
    {
        public string Name { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }


        public shiftInfo()
        {
        }
    }

    public class Session
    {
        public int Shift { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        TimeSpan startTime;
        public string StartTime 
        { 
            get{ return startTime.ToString();}
            set{ 
            if (value == String.Empty)
            {
                
            }
            else
            {
                try
                {
                    String[] timeparams = value.Split(':');
                    startTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                        int.Parse(timeparams[2]));
                }
                catch (Exception e)
                {
                    return;
                }
            }
            }
        }

        TimeSpan endTime;
        public string EndTime { 
            get{ return endTime.ToString();}
            set{ 
            if (value == String.Empty)
            {
                
            }
            else
            {
                try
                {
                    String[] timeparams = value.Split(':');
                    endTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                        int.Parse(timeparams[2]));
                }
                catch (Exception e)
                {
                    return;
                }
            }
            }
        }

        public Session()
        {
        }
        public Session(int shift,int id, string description, TimeSpan startTime, TimeSpan endTime)
        {
            Shift = shift; 
            ID = id;
            Name = description;
            this.startTime= startTime;
            this.endTime = endTime;
        }


        public Session(int shift, int id, string description, String startTime, String endTime)
        {
            Shift = shift;
            ID = id;
            Name = description;
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool IsWithin( TimeSpan ts )
        {
            TimeSpan start = startTime;
            TimeSpan end = endTime;
            bool result = false;

            if (end < startTime)            //third shift 
            {
                if (ts <= startTime && ts < endTime)
                    result = true;
                else if (ts > startTime && ts > endTime)
                    result = true;

            }

            else
            {
                if (ts >= startTime && ts < endTime)
                    result = true;

            }
            return result;
        }
    }

    public class SessionCollection : ObservableCollection<Session>
    {
        public Session getSession(int id)
        {
            IEnumerator<Session> enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID == id)
                    return enumerator.Current;
            }
            return null;
        }
    }

    public class sessionInfo
    {
        public int ShiftIndex { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public sessionInfo()
        {
        }
    }


}
