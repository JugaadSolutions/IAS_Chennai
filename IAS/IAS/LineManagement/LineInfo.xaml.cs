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
    /// Interaction logic for LineInfo.xaml
    /// </summary>
    public partial class LineInfo : PageFunction<lineInfo>
    {
        lineInfo _changedLine = null;
        public LineInfo(lineInfo changedLine)
        {
            InitializeComponent();
            _changedLine = changedLine;

            if (changedLine != null)
            {
                IdGrid.DataContext = changedLine;
                NameGrid.DataContext = changedLine;
            }
            tbLineID.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if( _changedLine == null)
                    _changedLine = new lineInfo();
                _changedLine.ID = Convert.ToInt32(tbLineID.Text);
                _changedLine.Name = tbLineName.Text;
                OnReturn(new ReturnEventArgs<lineInfo>(_changedLine));
            }
            catch (Exception s)
            {
                OnReturn(new ReturnEventArgs<lineInfo>(null));
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(new ReturnEventArgs<lineInfo>(null));
        }
    }
}
