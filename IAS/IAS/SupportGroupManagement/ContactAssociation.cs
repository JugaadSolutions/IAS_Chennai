using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace IAS
{
    public class LineAssociationInfo :INotifyPropertyChanged
    {
        public int ID { get; set; }
        public String Name { get; set; }
        bool isAssociated;
        public Boolean IsAssociated
        {
            get { return isAssociated; }
            set
            {
                isAssociated = value;
                OnPropertyChanged("IsAssociated");
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
        public LineAssociationInfo()
        {
        }

        public LineAssociationInfo(int id, string name, Boolean asso)
        {
            ID = id;
            Name = name;
            IsAssociated = asso;
        }
    }

    public class LineAssociationCollection : ObservableCollection<LineAssociationInfo>
    {
        public LineAssociationCollection()
        {
        }

        public LineAssociationCollection(String tag)
        {
            Array a = tag.ToCharArray();

            foreach (char c in a)
            {

                this.Add(new LineAssociationInfo(-1, String.Empty,
                    c == '0' ? false : true));
            }
        }
    }


    public class ShiftAssociationInfo : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public String Name { get; set; }
        bool isAssociated;
        public Boolean IsAssociated
        {
            get { return isAssociated; }
            set
            {
                isAssociated = value;
                OnPropertyChanged("IsAssociated");
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
        public ShiftAssociationInfo()
        {
        }

        public ShiftAssociationInfo(int id, string name, Boolean asso)
        {
            ID = id;
            Name = name;
            IsAssociated = asso;
        }
    }

    public class ShiftAssociationCollection : ObservableCollection<ShiftAssociationInfo>
    {
        public ShiftAssociationCollection()
        {
        }

        public ShiftAssociationCollection(String tag)
        {
            Array a = tag.ToCharArray();

            foreach (char c in a)
            {

                this.Add(new ShiftAssociationInfo(-1, String.Empty,
                    c == '0' ? false : true));
            }
        }
    }


    public class DepartmentAssociationInfo : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public String Name { get; set; }
        bool isAssociated;
        public Boolean IsAssociated
        {
            get { return isAssociated; }
            set
            {
                isAssociated = value;
                OnPropertyChanged("IsAssociated");
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
        public DepartmentAssociationInfo()
        {
        }

        public DepartmentAssociationInfo(int id, string name, Boolean asso)
        {
            ID = id;
            Name = name;
            IsAssociated = asso;
        }
    }

    public class DepartmentAssociationCollection : ObservableCollection<DepartmentAssociationInfo>
    {
        public DepartmentAssociationCollection()
        {
        }

        public DepartmentAssociationCollection(String tag)
        {
            Array a = tag.ToCharArray();

            foreach (char c in a)
            {

                this.Add(new DepartmentAssociationInfo(-1, String.Empty,
                    c == '0' ? false : true));
            }
        }
    }


    public class EscalationAssociationInfo : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public String Name { get; set; }
        bool isAssociated;
        public Boolean IsAssociated
        {
            get { return isAssociated; }
            set
            {
                isAssociated = value;
                OnPropertyChanged("IsAssociated");
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
        public EscalationAssociationInfo()
        {
        }

        public EscalationAssociationInfo(int id, string name, Boolean asso)
        {
            ID = id;
            Name = name;
            IsAssociated = asso;
        }
    }

    public class EscalationAssociationCollection : ObservableCollection<EscalationAssociationInfo>
    {
        public EscalationAssociationCollection()
        {
        }

        public EscalationAssociationCollection(String tag)
        {
            Array a = tag.ToCharArray();

            foreach (char c in a)
            {

                this.Add(new EscalationAssociationInfo(-1, String.Empty,
                    c == '0' ? false : true));
            }
        }
    }

}
