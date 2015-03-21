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
    /// Interaction logic for ContactDetails.xaml
    /// </summary>
    public partial class ContactDetails : UserControl
    {
        DataAccess dataAccess = null;

        public ContactDetails()
        {
            InitializeComponent();
            dataAccess = new DataAccess();


        }

        private void LineSummaryCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ShiftSummaryCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }


    }
}
