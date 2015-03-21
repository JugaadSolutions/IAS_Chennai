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

namespace IAS
{
    /// <summary>
    /// Interaction logic for optionsPage.xaml
    /// </summary>
    public partial class MainMenu : PageFunction<String>
    {
        static String _dbConnectionString = String.Empty;

        static ContactCollection contacts;
        DataAccess dataAccess = null;
        public MainMenu()
        {
            InitializeComponent();
        }

        public MainMenu(String dbConnectionString)
        {
            InitializeComponent();
            _dbConnectionString = dbConnectionString;

            dataAccess = new DataAccess();
            


        }


        #region LINE MANAGEMENT

        private void lineManageMentLink_Click(object sender, RoutedEventArgs e)
        {
            LineManagement lineManagement = new LineManagement(_dbConnectionString);
            addModifyDeleteControl lineControl = new addModifyDeleteControl();
            NavigationService.Navigate(lineManagement);


            
            lineManagement.Return += new ReturnEventHandler<string>(lineManagement_Return);
            

        }

        void lineManagement_Return(object sender, ReturnEventArgs<string> e)
        {
            
        }

        #endregion

        #region SHIFT MANAGEMENT

        private void shiftManagementLink_Click(object sender, RoutedEventArgs e)
        {
            ShiftManagement shiftManagement = new ShiftManagement();
            NavigationService.Navigate(shiftManagement);

            shiftManagement.Return += new ReturnEventHandler<string>(shiftManagement_Return);
            

        }

        void shiftManagement_Return(object sender, ReturnEventArgs<string> e)
        {
            
        }

        #endregion

        #region SUPPORT GROUP MANAGEMENT
        private void supportGroupManagementLink_Click(object sender, RoutedEventArgs e)
        {

            SupportGroupManagement supportGroupManagement = new SupportGroupManagement(contacts);
            addModifyDeleteControl lineControl = new addModifyDeleteControl();
            NavigationService.Navigate(supportGroupManagement);

            supportGroupManagement.Return += new ReturnEventHandler<ContactCollection>(supportGroupManagement_Return);

        }

        void supportGroupManagement_Return(object sender, ReturnEventArgs<ContactCollection> e)
        {
            
        }


        #endregion

        #region SETTINGS

        private void settingsLink_Click(object sender, RoutedEventArgs e)
        {
            
            settings settings = new settings();

            settings.Marquee = dataAccess.getMarquee();
            settings.MarqueeSpeed = dataAccess.getMarqueeSpeed();
            settings.IssuemarqueeSpeed = dataAccess.getIssuemarqueeSpeed();
            settings.EscalationSettings = dataAccess.getEscalationSettings();
            settings.Password = dataAccess.getPassword();

            Settings settingsPage = new Settings(settings);

            NavigationService.Navigate(settingsPage);

            settingsPage.Return += new ReturnEventHandler<settings>(settingsPage_Return);

        }

        void settingsPage_Return(object sender, ReturnEventArgs<settings> e)
        {
            dataAccess = new DataAccess();
            dataAccess.updateMarquee(e.Result.Marquee);
            dataAccess.updateMarqueeSpeed(e.Result.MarqueeSpeed);
            dataAccess.updateIssuemarqueeSpeed(e.Result.IssuemarqueeSpeed);
            dataAccess.updateEscalationSettings(e.Result.EscalationSettings);
            if (e.Result.Password != dataAccess.getPassword())
            {
                dataAccess.updatePassword(e.Result.Password);
            }
        }



        #endregion

        private void PageFunction_Loaded(object sender, RoutedEventArgs e)
        {
            dataAccess = new DataAccess(_dbConnectionString);
            contacts = dataAccess.getContacts();
        }

    }
}
