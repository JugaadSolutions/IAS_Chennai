using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Media;


namespace ias.client
{
    public class LineStatus : INotifyPropertyChanged
    {

        String lineDescription = String.Empty;
        public int id = -1;
        bool breakDownFlag = false;
        bool qualityFlag = false;
        bool materialShortageFlag = false;
        public String LineDescription
        {
            get { return lineDescription; }
            set
            {
                lineDescription = value;
                OnPropertyChanged("LineDescription");
            }
        }
        String actualQuantity = String.Empty;
        public String ActualQuantity
        {
            get { return actualQuantity; }

            set
            {
                actualQuantity = value;
                OnPropertyChanged("ActualQuantity");
            }
        }

        String targetQuantity = String.Empty;
        public String TargetQuantity
        {
            get { return targetQuantity; }
            set
            {
                targetQuantity = value;
                OnPropertyChanged("TargetQuantity");
            }
        }

        int breakdown = 0;
        public int Breakdown
        {
            get { return breakdown; }
            set
            {
                breakdown = value;


                OnPropertyChanged("Breakdown");
            }
        }

        SolidColorBrush breakdownBrush;
        public SolidColorBrush BreakdownBrush
        {
            get 
            {
                return breakdownBrush;


            }
            set
            {
                breakdownBrush = value;
                OnPropertyChanged("BreakdownBrush");
            }
        }

        int quality = 0;
        public int Quality
        {
            get { return quality; }
            set
            {
                quality = value;
                OnPropertyChanged("Quality");
            }
        }

        SolidColorBrush qualityBrush;
        public SolidColorBrush QualityBrush
        {
            get
            {
                return qualityBrush;


            }
            set
            {
                qualityBrush = value;
                OnPropertyChanged("QualityBrush");
            }
        }



        int materialShortage = 0;
        public int MaterialShortage
        {
            get { return materialShortage; }
            set
            {
                materialShortage = value;
                OnPropertyChanged("MaterialShortage");
            }
        }

        SolidColorBrush materialShortageBrush;
        public SolidColorBrush MaterialShortageBrush
        {
            get
            {
                return materialShortageBrush;


            }
            set
            {
                materialShortageBrush = value;
                OnPropertyChanged("MaterialShortageBrush");
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


        public LineStatus(String description , int id)
        {
            LineDescription = description;
            this.id = id;

        }

        public void  updateStatus()
        {
            
            if (breakdown > 0)
            {

                if (breakDownFlag == true)
                {
                    breakDownFlag = false;
                    BreakdownBrush = Brushes.Red;


                }
                else
                {
                    breakDownFlag = true;
                    BreakdownBrush = Brushes.Black;
                }

            }
            else
            {
                breakDownFlag = false;
                BreakdownBrush = Brushes.LimeGreen;
            }

            if (quality > 0)
            {

                if (qualityFlag == true)
                {
                    qualityFlag = false;
                    QualityBrush = Brushes.Red;


                }
                else
                {
                    qualityFlag = true;
                    QualityBrush = Brushes.Black;
                }

            }
            else
            {
                qualityFlag = false;
                QualityBrush = Brushes.LimeGreen;
            }


            if (materialShortage > 0)
            {

                if (materialShortageFlag == true)
                {
                    materialShortageFlag = false;
                    MaterialShortageBrush = Brushes.Red;


                }
                else
                {
                    materialShortageFlag = true;
                    MaterialShortageBrush = Brushes.Black;
                }

            }
            else
            {
                materialShortageFlag = false;
                MaterialShortageBrush = Brushes.LimeGreen;
            }




            
            
        }


    }

    public class LineStatusCollection : ObservableCollection<LineStatus>
    {

    }

}
