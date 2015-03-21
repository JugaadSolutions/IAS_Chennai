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
using System.Collections.ObjectModel;

namespace IAS
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : PageFunction<settings>
    {
        settings settings = null;
        public Settings()
        {
            InitializeComponent();
        }
        public Settings(settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            settingsGrid.DataContext = this.settings;
            escalationDurationTable.DataContext = settings.EscalationSettings;
        }

        private void tbOldPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbOldPassword.Password.Length < 4)
            {
                tbnewPassword.IsEnabled = false;
                return;
            }
            if (tbOldPassword.Password != settings.Password)
            {
                MessageBox.Show("Incorrect Password", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                tbOldPassword.Clear();
                tbOldPassword.Focus();
                return;
            }

            tbnewPassword.IsEnabled = true;
        }

        private void tbnewPassword_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (tbnewPassword.IsEnabled == true && (tbnewPassword.Password.Length < 4))
            {
                MessageBox.Show("Password should be of length 4 ", "Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                tbnewPassword.Focus();


                return;
            }

            OnReturn(new ReturnEventArgs<settings>(settings));

        }



        





        
    }
    
    public class settings : INotifyPropertyChanged
    {
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

        string marquee = String.Empty;
        public string Marquee
        {
            get { return marquee; }
            set
            {
                marquee = value;
                OnPropertyChanged("Marquee");
            }
        }

        int marqueeSpeed;
        public String MarqueeSpeed
        {
            get { return marqueeSpeed.ToString(); }
            set
            {
                try
                {
                    marqueeSpeed = int.Parse(value);
                    OnPropertyChanged("MarqueeSpeed");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid Input - Issue Marquee Speed ", "Error", MessageBoxButton.OK
                        , MessageBoxImage.Error);
                }
            }
        }


        int issuemarqueeSpeed;
        public String IssuemarqueeSpeed
        {
            get { return issuemarqueeSpeed.ToString(); }
            set
            {
                try
                {
                    issuemarqueeSpeed = int.Parse(value);
                    OnPropertyChanged("IssuemarqueeSpeed");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid Input - Issue Marquee Speed ", "Error", MessageBoxButton.OK
                        , MessageBoxImage.Error);
                }
            }
        }

        string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public ObservableCollection<escalationInfo> EscalationSettings;

        public settings()
        {
        }

     }

    public class escalationInfo : INotifyPropertyChanged, IEditableObject
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

        String name;
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        int duration;
        public String Duration
        {
            get { return duration.ToString(); }
            set
            {
                try
                {
                    duration = int.Parse(value);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid Input for Escalation : " + Name, "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                }

                OnPropertyChanged("Duration");
            }

        }


        public escalationInfo(String name, int id, int duration)
        {
            Name = name;
            ID = id;
            Duration = duration.ToString();
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



        
}
