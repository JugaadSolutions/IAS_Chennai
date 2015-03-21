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
    /// Interaction logic for ShiftInfo.xaml
    /// </summary>
    public partial class ShiftInfo : PageFunction<shiftInfo>
    {
        shiftInfo _shiftInfo = null;
        public ShiftInfo(shiftInfo shiftInfo)
        {
            InitializeComponent();

            _shiftInfo = shiftInfo;

            if (_shiftInfo != null)
            {
                nameGrid.DataContext = _shiftInfo;
                startTimeGrid.DataContext = _shiftInfo;
                endTimeGrid.DataContext = _shiftInfo;

            }
            tbLineID.Focus();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_shiftInfo == null)
                    _shiftInfo = new shiftInfo();
                _shiftInfo.Name = tbLineID.Text;
                _shiftInfo.StartTime = tbStartTime.Text;
                _shiftInfo.EndTime = tbEndTime.Text;
                OnReturn(new ReturnEventArgs<shiftInfo>(_shiftInfo));
            }
            catch (Exception s)
            {
                OnReturn(new ReturnEventArgs<shiftInfo>(null));
            }
            

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(new ReturnEventArgs<shiftInfo>(null));
        }


        private void textChangeEventHandler(object sender, TextChangedEventArgs e)
        {
            
            TextBox tb = (TextBox)sender;
            if ((tb.Text.Length == 2) ||(tb.Text.Length == 5))
            {
                tb.Text += ":";
            }
            tb.CaretIndex = tb.Text.Length;
            e.Handled = true;
        }

        private void tbStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
