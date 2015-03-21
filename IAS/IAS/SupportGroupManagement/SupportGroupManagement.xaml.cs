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
    /// Interaction logic for SupportGroupManagement.xaml
    /// </summary>
    public partial class SupportGroupManagement : PageFunction<ContactCollection>
    {
        ContactCollection contacts = null;
        DataAccess dataAccess = null;

        Contact currentContact = null;

        public SupportGroupManagement(ContactCollection contacts)
        {
            InitializeComponent();
            dataAccess = new DataAccess();
            this.contacts= contacts;

            contactControl.DataContext = contacts;

            ((Label)contactControl.aMDGroupBox.Header).Content = "CONTACTS";
                
        }

        private void contactControl_selectionChanged(object sender, AddModifyDeleteSelectionChangedEventArgs e)
        {
            
            contactDetailsControl.DataContext = null;

            if (contactControl.dgItem.SelectedIndex == -1)
            {
                ContactDetails.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            currentContact = contacts[contactControl.dgItem.SelectedIndex];

            contactDetailsControl.DataContext = currentContact;
            ContactDetails.Visibility = System.Windows.Visibility.Visible;
         }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if( contactDetailsControl.tbContactName.Text.Length == 0)
            {
                 MessageBox.Show("Please Enter Contact Name ","Info",
                     MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }

            if( contactDetailsControl.tbContactNumber.Text.Length == 0 )
            {
                 MessageBox.Show("Please Enter Contact Number ","Info",
                     MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }


            dataAccess.updateContact(currentContact);

            if (!contacts.Contains(currentContact))
            {
                contacts.Add(currentContact);
                
            }
            MessageBox.Show("Contact Saved ", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            contactDetailsControl.DataContext = null;
            contactControl.dgItem.SelectedIndex = -1;
            ContactDetails.Visibility = System.Windows.Visibility.Hidden;

        }

        private void contactControl_addClicked(object sender, EventArgs e)
        {
            currentContact = new Contact();
            contactDetailsControl.DataContext = null;
            DataAccess.createAssociation(currentContact);
            contactDetailsControl.DataContext = currentContact;
            contactDetailsControl.tbContactName.Focus();
            ContactDetails.Visibility = System.Windows.Visibility.Visible;
        }

        private void contactControl_deleteClicked(object sender, EventArgs e)
        {
            if (contactControl.dgItem.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Contact", "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            dataAccess.deleteContact(currentContact);
            contacts.Remove(currentContact);
            contactDetailsControl.DataContext = null;
                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void PageFunction_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        

    }



    public class Contact
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public LineAssociationCollection LineAssociation { get; set; }
        public ShiftAssociationCollection ShiftAssociation { get; set; }
        public DepartmentAssociationCollection DepartmentAssociation { get; set; }
        public EscalationAssociationCollection EscalationAssociation { get; set; }
        public Boolean LineSummary { get; set; }
        public Boolean ShiftSummary { get; set; }

        
        public Contact()
        {
            ID = -1;
            Name = String.Empty;
            Number = String.Empty;
            
            

        }

        public Contact(int id, string name,string number)
        {
            ID = id;
            Name = name;
            Number = number;
            
            
        }

        public bool IsAssociatedWith(int line,int shift,int department ,int escalation)
        {
            bool lineAsso = false;
            bool shiftAsso = false;
            bool departmentAsso = false;
            bool escalationAsso = false;
            foreach( LineAssociationInfo l in LineAssociation)
            {
                if( l.ID == line)
                {
                    lineAsso = l.IsAssociated;
                    break;
                }

            }

            foreach( ShiftAssociationInfo s in ShiftAssociation)
            {
                if( s.ID == shift)
                {
                    shiftAsso = s.IsAssociated;
                    break;
                }
            }

            foreach( DepartmentAssociationInfo d in DepartmentAssociation)
            {
                if( d.ID == department)
                {
                    departmentAsso= d.IsAssociated;
                    break;
                }
            }
            foreach( EscalationAssociationInfo e in EscalationAssociation)
            {
                if( e.ID == escalation)
                {
                    escalationAsso = e.IsAssociated;
                    break;
                }
            }

            return ( (lineAsso) && (shiftAsso) &&(departmentAsso) && (escalationAsso));
        }

        
    }

    public class ContactCollection : ObservableCollection<Contact>
    {

        public List<Contact> getContactList(int line, List<int> shifts, int department, int escalation)
        {
            List<Contact> contactList = new List<Contact>();
            foreach (int id in shifts)
            {
                IEnumerator<Contact> ce = this.GetEnumerator();
                while (ce.MoveNext())
                {
                    if (ce.Current.IsAssociatedWith(line, id, department, escalation) == true)
                    {
                        if(!contactList.Contains(ce.Current))
                        {
                            contactList.Add(ce.Current);
                        }
                    }

                }
            }
            return contactList;
        }
    }

}
