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
    /// Interaction logic for SessionInfo.xaml
    /// </summary>
    public partial class SessionInfo : PageFunction<sessionInfo>
    {
        sessionInfo _sessionInfo;
        public SessionInfo(sessionInfo sessionInfo)
        {
            InitializeComponent();
            _sessionInfo = sessionInfo;

            if (sessionInfo != null)
            {
                nameGrid.DataContext = _sessionInfo;
                startTimeGrid.DataContext = _sessionInfo;
                endTimeGrid.DataContext = _sessionInfo;

            }
            tbLineID.Focus();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_sessionInfo == null)
                    _sessionInfo = new sessionInfo();
                _sessionInfo.Name = tbLineID.Text;
                _sessionInfo.StartTime = tbStartTime.Text;
                _sessionInfo.EndTime = tbEndTime.Text;
                OnReturn(new ReturnEventArgs<sessionInfo>(_sessionInfo));
            }
            catch (Exception s)
            {
                OnReturn(new ReturnEventArgs<sessionInfo>(null));
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(new ReturnEventArgs<sessionInfo>(null));
        }
        private void textChangeEventHandler(object sender, TextChangedEventArgs e)
        {

            TextBox tb = (TextBox)sender;
            if ((tb.Text.Length == 2) || (tb.Text.Length == 5))
            {
                tb.Text += ":";
            }
            tb.CaretIndex = tb.Text.Length;
            e.Handled = true;
        }
    }
}
