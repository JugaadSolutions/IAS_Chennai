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
    /// Interaction logic for ClassInfo.xaml
    /// </summary>
    public partial class ClassInfo : PageFunction<classInfo>
    {
        classInfo classInfo = null;
        public ClassInfo(classInfo classInfo)
        {
            InitializeComponent();
            tbLineID.Focus();
            this.classInfo = classInfo;
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (classInfo == null)
                    classInfo = new classInfo();
                classInfo.ID = Convert.ToInt32(tbLineID.Text);
                classInfo.Name = tbLineName.Text;
                OnReturn(new ReturnEventArgs<classInfo>(classInfo));
            }
            catch (Exception s)
            {
                OnReturn(new ReturnEventArgs<classInfo>(null));
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnReturn(new ReturnEventArgs<classInfo>(null));

        }
    }
}
