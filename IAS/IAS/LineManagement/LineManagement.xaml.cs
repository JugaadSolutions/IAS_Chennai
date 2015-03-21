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
    /// Interaction logic for LineManagement.xaml
    /// </summary>
    public partial class LineManagement : PageFunction<String>
    {
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;
        lineCollection lines = null;
        line changedLine = null;
        PageFunction<lineInfo> lineInfoPage = null;
        StationInfo stationInfoPage = null;
        PageFunction<classInfo> classInfoPage = null;
        int selectedIndex = -1;

        DataTable dt;

        public lineCollection Lines
        {
            get { return lines; }
            set
            {
                lines = value;
            }
        }

        #region CONSTRUCTORS

        public LineManagement()
        {
            InitializeComponent();
            if (dataAccess == null)
            dataAccess = new DataAccess();

            lines = dataAccess.getLines();

            lines.Header = "LINES";
            lineControl.DataContext = lines;
            ((Label)lineControl.aMDGroupBox.Header).Content = "LINES";
            ((Label)stationControl.aMDGroupBox.Header).Content = "STATIONS";
            ((Label)breakdownControl.aMDGroupBox.Header).Content = "BREAKDOWN";
            ((Label)qualityControl.aMDGroupBox.Header).Content = "QUALITY";

             dt= dataAccess.GetOpenIssues();
            dgOpenIssuesGrid.DataContext = dt;
            stationControl.allowModification();
        }

        public LineManagement(String dbConnectionString)
        {
            InitializeComponent();
            _dbConnectionString = dbConnectionString;

            if( dataAccess == null)
            dataAccess = new DataAccess(_dbConnectionString);

            lines = dataAccess.getLines();
            lines.Header = "LINES";
            lineControl.DataContext = lines;
            ((Label)lineControl.aMDGroupBox.Header).Content = "LINES";
            ((Label)stationControl.aMDGroupBox.Header).Content = "STATIONS";
            ((Label)breakdownControl.aMDGroupBox.Header).Content = "BREAKDOWN";
            ((Label)qualityControl.aMDGroupBox.Header).Content = "QUALITY";

            dt = dataAccess.GetOpenIssues();
            dgOpenIssuesGrid.DataContext = dt;
            stationControl.allowModification();
        }

        #endregion

        #region EVENT HANDLERS

        #region LINE CONTROL EVENT HANDLERS

        private void lineControl_selectionChanged(object sender, AddModifyDeleteSelectionChangedEventArgs e)
        {
            stationControl.DataContext = null;
            breakdownControl.DataContext = null;
            qualityControl.DataContext = null;
            if (lineControl.dgItem.SelectedIndex == -1) return;

            stationControl.DataContext = ((line)lineControl.dgItem.SelectedItem).Stations;

        }

        private void stationControl_selectionChanged(object sender, AddModifyDeleteSelectionChangedEventArgs e)
        {
            breakdownControl.DataContext = null;
            qualityControl.DataContext = null;
            if (lineControl.dgItem.SelectedIndex == -1)
                return;
            if (stationControl.dgItem.SelectedIndex == -1)
                return;
            breakdownControl.DataContext = 
                ((line)lineControl.dgItem.SelectedItem).Stations[stationControl.dgItem.SelectedIndex].BreakdownClass;

            qualityControl.DataContext =
                ((line)lineControl.dgItem.SelectedItem).Stations[stationControl.dgItem.SelectedIndex].QualityClass;
        }

        
        private void lineControl_addClicked(object sender, EventArgs e)
        {

            lineInfoPage = new LineInfo(null);

            NavigationService.Navigate(lineInfoPage);


            lineInfoPage.Return += new ReturnEventHandler<lineInfo>(nextPage_Return);
        }

        void nextPage_Return(object sender, ReturnEventArgs<lineInfo> e)
        {
            if (e.Result == null)
                return;

            line line = new line(e.Result.ID, e.Result.Name);

            lines.Add(line);

            dataAccess.addLine(e.Result.ID, e.Result.Name);
            lineControl.dgItem.SelectedIndex = -1;
            lineControl.btnAdd.Focus();

        }


        



        private void lineControl_deleteClicked(object sender, EventArgs e)
        {
            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            dataAccess.deleteLine(lines[lineControl.dgItem.SelectedIndex].ID);
            lines.Remove(lines[lineControl.dgItem.SelectedIndex]);
        }
        #endregion

        #region STATION CONTROL EVENT HANDLERS

        private void stationControl_addClicked(object sender, EventArgs e)
        {

            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            stationInfo stationInfo = new stationInfo();
            stationInfo.LineIndex = lineControl.dgItem.SelectedIndex;
            stationInfoPage = new StationInfo(stationInfo);

            NavigationService.Navigate(stationInfoPage);


            stationInfoPage.Return += new ReturnEventHandler<stationInfo>(stationInfoPage_Return);

        }

        void stationInfoPage_Return(object sender, ReturnEventArgs<stationInfo> e)
        {
            if (e.Result == null)
                return;
            if (e.Result.Modify == true)
            {
                dataAccess.updateStation(lines[e.Result.LineIndex].ID, e.Result.ID, e.Result.Name, e.Result.Tolerance);
                lines[e.Result.LineIndex].Stations[e.Result.ItemIndex].Name = e.Result.Name;
                lines[e.Result.LineIndex].Stations[e.Result.ItemIndex].Tolerance  = e.Result.Tolerance;

                lineControl.dgItem.SelectedIndex = e.Result.LineIndex;
            }
            else
            {
                Station station = new Station(lines[e.Result.LineIndex].ID, e.Result.ID, e.Result.Name, e.Result.Tolerance);


                lines[e.Result.LineIndex].Stations.Add(station);

                dataAccess.addStation(lines[e.Result.LineIndex].ID, station.ID, station.Name, station.Tolerance);
                lineControl.dgItem.SelectedIndex = e.Result.LineIndex;
            }
            stationControl.dgItem.SelectedIndex = -1;
            stationControl.btnAdd.Focus();

        }

        private void stationControl_deleteClicked(object sender, EventArgs e)
        {

            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            dataAccess.removeStation(lines[lineControl.dgItem.SelectedIndex].ID, 
                lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].ID);
            lines[lineControl.dgItem.SelectedIndex].Stations.Remove(lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex]);

        }

        private void stationControl_modifyClicked(object sender, EventArgs e)
        {
            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            stationInfo stationInfo = new stationInfo();
            stationInfo.LineIndex = lineControl.dgItem.SelectedIndex;
            stationInfo.ItemIndex = stationControl.dgItem.SelectedIndex;
            stationInfo.ID = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].ID;
            stationInfo.Name = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].Name;
            stationInfo.Tolerance = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].Tolerance;

            stationInfoPage = new StationInfo(stationInfo);
            stationInfoPage.allowModification();

            NavigationService.Navigate(stationInfoPage);


            stationInfoPage.Return += new ReturnEventHandler<stationInfo>(stationInfoPage_Return);
           
        }


        #endregion

        private void breakdownControl_addClicked(object sender, EventArgs e)
        {
            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            classInfo classInfo= new classInfo();
            classInfo.LineIndex = lineControl.dgItem.SelectedIndex;
            classInfo.StationIndex = stationControl.dgItem.SelectedIndex;
            classInfoPage = new ClassInfo(classInfo);

            NavigationService.Navigate(classInfoPage);


            classInfoPage.Return += new ReturnEventHandler<classInfo>(classInfoPage_Return);
        }

        void classInfoPage_Return(object sender, ReturnEventArgs<classInfo> e)
        {
            if (e.Result == null)
                return;

            Class Class = new Class(1, e.Result.ID, e.Result.Name);

            lines[e.Result.LineIndex].Stations[e.Result.StationIndex].BreakdownClass.Add(Class);

            dataAccess.addClass(lines[e.Result.LineIndex].ID, lines[e.Result.LineIndex].Stations
            [e.Result.StationIndex].ID,1,e.Result.ID,e.Result.Name);

            lineControl.dgItem.SelectedIndex = e.Result.LineIndex;
            stationControl.dgItem.SelectedIndex = e.Result.StationIndex;
            breakdownControl.dgItem.SelectedIndex = -1;
            breakdownControl.btnAdd.Focus();
        }

        private void breakdownControl_deleteClicked(object sender, EventArgs e)
        {
            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (breakdownControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Class", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            int line = lines[lineControl.dgItem.SelectedIndex].ID;
            int station = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].ID;
            int code = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].BreakdownClass[breakdownControl.dgItem.SelectedIndex].ID;


            dataAccess.removeClass(line , station,1,code);

            lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].BreakdownClass.Remove(lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].BreakdownClass[breakdownControl.dgItem.SelectedIndex]);

        }

        private void qualityControl_addClicked(object sender, EventArgs e)
        {

            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            classInfo classInfo = new classInfo();
            classInfo.LineIndex = lineControl.dgItem.SelectedIndex;
            classInfo.StationIndex = stationControl.dgItem.SelectedIndex;
            classInfoPage = new ClassInfo(classInfo);

            NavigationService.Navigate(classInfoPage);


            classInfoPage.Return += new ReturnEventHandler<classInfo>(classInfoPage_QReturn);

        }


        void classInfoPage_QReturn(object sender, ReturnEventArgs<classInfo> e)
        {
            if (e.Result == null)
                return;

            Class Class = new Class(2, e.Result.ID, e.Result.Name);

            lines[e.Result.LineIndex].Stations[e.Result.StationIndex].QualityClass.Add(Class);

            dataAccess.addClass(lines[e.Result.LineIndex].ID, lines[e.Result.LineIndex].Stations
            [e.Result.StationIndex].ID, 2, e.Result.ID, e.Result.Name);
            qualityControl.dgItem.SelectedIndex = -1;
        }



        private void qualityControl_deleteClicked(object sender, EventArgs e)
        {
            if (lineControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Line", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (stationControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Station", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            if (qualityControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Class", "Info", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }
            int line = lines[lineControl.dgItem.SelectedIndex].ID;
            int station = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].ID;
            int code = lines[lineControl.dgItem.SelectedIndex].Stations[stationControl.dgItem.SelectedIndex].QualityClass[qualityControl.dgItem.SelectedIndex].ID;


            dataAccess.removeClass(line, station, 2, code);


        }
      

        

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (dgOpenIssuesGrid.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Issue", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            dataAccess.updateIssueStatus((int)dt.Rows[dgOpenIssuesGrid.SelectedIndex]["SlNo"]);
            
            dt.Rows.RemoveAt(dgOpenIssuesGrid.SelectedIndex);

            dgOpenIssuesGrid.DataContext = null;

            dgOpenIssuesGrid.DataContext = dt;
            dataAccess.updateIssueMarquee();
           
        }

        private void btnCloseAll_Click(object sender, RoutedEventArgs e)
        {
            dataAccess.updateIssueStatus(dt);

            dt.Clear();
            dgOpenIssuesGrid.DataContext = null;
            dgOpenIssuesGrid.DataContext = dt;
            dataAccess.updateIssueMarquee();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbcLineControl.SelectedIndex == 1)
            {
                
            }
        }


    }


    #region DATACLASSES
    public class line : INotifyPropertyChanged
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

        string name = String.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

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



        StationCollection stations;

        public StationCollection Stations
        {
            get { return stations; }
            set
            {
                stations = value;
                OnPropertyChanged("Stations");
            }
        }


        DataAccess dataAccess;

        Dictionary<int, Issue> issue_record_map = null;

        public event EventHandler<EscalationEventArgs> escalationEvent;

        public line(int id, String name)
        {

            this.ID = id;
            this.Name = name;

            dataAccess = new DataAccess();


            

            issue_record_map = new Dictionary<int, Issue>();


            stations = dataAccess.getStations(id);

        }

      
    }

    public class lineInfo
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }

        public lineInfo()
        {
        }
    }

    public class lineCollection : ObservableCollection<line>
    {
        string header = String.Empty;
        public String Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Header"));
            }
        }

        public Dictionary<int, string> _dictionary = null;

        public lineCollection()
        {
            _dictionary = new Dictionary<int, string>();
        }

        public bool find(lineInfo lineInfo)
        {
            if( _dictionary.ContainsValue(lineInfo.Name)) return true;

            
            if( _dictionary.ContainsKey(lineInfo.ID)) return true;

            return false;
        }

        public void add(line line)
        {
            try
            {
                _dictionary.Add(line.ID, line.Name);
                Add(line);
            }
            catch (Exception s)
            {
                MessageBox.Show("Unable to Add Line", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public String getLineName(int id)
        {
            return _dictionary[id];
        }

        public String getStationName(int line, int station)
        {
            IEnumerator<line> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID == line)
                    return enumerator.Current.Stations.getStationName(station);
            }
            return String.Empty;
        }

        public String getClassName(int line, int station, int department, int Class)
        {
            String classDescription = String.Empty;
            IEnumerator<line> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID != line)
                    continue;
                IEnumerator<Station> senumerator = enumerator.Current.Stations.GetEnumerator();
                while (senumerator.MoveNext())
                {
                    if (senumerator.Current.ID == station)
                    {
                        if (department == 1)
                        {
                          classDescription =   senumerator.Current.BreakdownClass.getClassName(Class);
                        }
                        else
                        {
                          classDescription =  senumerator.Current.QualityClass.getClassName(Class);
                        }
                    }
                }
            }
            return classDescription;
        }









    }

    public partial class Station : INotifyPropertyChanged
    {
        int line;

        int id = 0;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        int tolerance = 0;
        public int Tolerance
        {
            get { return tolerance; }
            set
            {
                tolerance = value;
                OnPropertyChanged("Tolerance");
            }
        }

        bool modify = false;
        public bool Modify
        {
            get { return modify; }
            set
            {
                modify = value;
                OnPropertyChanged("Modify");
            }
        }

        ClassCollection breakdownClass;
        public ClassCollection BreakdownClass
        {
            get { return breakdownClass; }
            set
            {
                breakdownClass = value;
                OnPropertyChanged("BreakdownClass");
            }
        }

        ClassCollection qualityClass;

        public ClassCollection QualityClass
        {
            get { return qualityClass; }
            set
            {
                qualityClass = value;
                OnPropertyChanged("QualityClass");
            }
        }


        DataAccess dataAccess;


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


        public Station(int line, int ID, String name,int tolerance)
        {
            this.line = line;
            this.ID = ID;

            this.Name = name;
            this.Tolerance = tolerance;
            dataAccess = new DataAccess();

            breakdownClass = new ClassCollection();
            qualityClass = new ClassCollection();

            breakdownClass = dataAccess.getClass(line, ID, 1);
            qualityClass = dataAccess.getClass(line, ID, 2);


        }

    }

    public class StationCollection : ObservableCollection<Station>
    {
        public Dictionary<int, string> _dictionary = null;

        public StationCollection()
        {
            _dictionary = new Dictionary<int, string>();
        }
        public bool find(lineInfo lineInfo)
        {
            if (_dictionary.ContainsValue(lineInfo.Name)) return true;


            if (_dictionary.ContainsKey(lineInfo.ID)) return true;

            return false;
        }

        public string getStationName(int id)
        {
            IEnumerator<Station> se = this.GetEnumerator();
            while( se.MoveNext())
            {
                if( se.Current.ID == id)
                    return se.Current.Name;
            }
            return string.Empty;

        }


    }

    public class stationInfo
    {
        public int LineIndex { get; set; }
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }
        public int Tolerance { get; set; }
        public bool Modify { get; set; }
        public stationInfo()
        {
        }
    }
    public class Class : INotifyPropertyChanged
    {
        int department;

        int id = 0;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string name = String.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }


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

        public Class(int department, int ID, string Name)
        {
            this.department = department;
            this.ID = ID;
            this.Name = Name;

        }

    }

    public class ClassCollection : ObservableCollection<Class>
    {

        public string getClassName(int id)
        {
            IEnumerator<Class> se = this.GetEnumerator();
            while( se.MoveNext())
            {
                if( se.Current.ID == id)
                    return se.Current.Name;
            }
            return string.Empty;
        }


    }

    public class classInfo
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }
        public int LineIndex { get; set; }
        public int StationIndex { get; set; }

        public classInfo()
        {
        }
    }


    #endregion


    


}

