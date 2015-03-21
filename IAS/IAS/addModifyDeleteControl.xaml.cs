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


namespace IAS
{
    /// <summary>
    /// Interaction logic for addModifyDeleteControl.xaml
    /// </summary>
    public partial class addModifyDeleteControl : UserControl 
    {
        public event EventHandler<AddModifyDeleteSelectionChangedEventArgs> selectionChanged;

        public event EventHandler<EventArgs> addClicked;

        public event EventHandler<EventArgs> modifyClicked;

        public event EventHandler<EventArgs> deleteClicked;

        public String Tag = String.Empty;
            
        public addModifyDeleteControl()
        {
            InitializeComponent();
           


        }


        private void dgItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddModifyDeleteSelectionChangedEventArgs eventArgs;

            eventArgs = new AddModifyDeleteSelectionChangedEventArgs(dgItem.SelectedIndex);

            if( selectionChanged != null )
                selectionChanged(this,eventArgs);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (addClicked != null)
                addClicked(this, new EventArgs());

        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (modifyClicked != null)
                modifyClicked(this, new EventArgs());
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (deleteClicked != null)
                deleteClicked(this, new EventArgs());


        }


        public void allowModification()
        {
            btnModify.Visibility = System.Windows.Visibility.Visible;
        }

    }
    #region EVENT ARGS
    public class AddModifyDeleteSelectionChangedEventArgs : EventArgs
    {
        public int SelectedIndex{ get; set;}
        

        public AddModifyDeleteSelectionChangedEventArgs(int selectedIndex )
        {
            SelectedIndex = selectedIndex;
            
        }
    }




    #endregion
}
