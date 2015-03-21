using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace ias.shared
{

        [ValueConversion(typeof(int), typeof(Brush))]
        public class statusToBackgroundConv : IValueConverter
        {
           
            Brush background = Brushes.White;
            bool backgroundFlag = false;
            public object Convert(object value, Type targetType, object obj, CultureInfo culInfo)
            {
                if (targetType != typeof(Brush)) return null;
                if ((int)value > 0)
                {
                    if (backgroundFlag == true)
                    {
                        background = Brushes.Red;
                        backgroundFlag = false;
                    }
                    else
                    {
                        background = Brushes.White;
                        backgroundFlag = true;
                    }


                }
                else
                    background = Brushes.LimeGreen ;

                return background;
            }


            public object ConvertBack(object value, Type targetType, object obj, CultureInfo culInfo)
            {
                throw new NotImplementedException();
            }
        }


        public class Shift
        {
            public int ID { get; set; }
            public string Name { get; set; }
            TimeSpan startTime;
            public string StartTime
            {
                get { return startTime.ToString(); }
                set
                {
                    if (value == String.Empty)
                    {

                    }
                    else
                    {
                        try
                        {
                            String[] timeparams = value.Split(':');
                            startTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                                int.Parse(timeparams[2]));
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
            }

            TimeSpan endTime;
            public string EndTime
            {
                get { return endTime.ToString(); }
                set
                {
                    if (value == String.Empty)
                    {

                    }
                    else
                    {
                        try
                        {
                            String[] timeparams = value.Split(':');
                            endTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                                int.Parse(timeparams[2]));
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
            }

            public SessionCollection Sessions;

            public Shift()
            {
            }

            public Shift(int id, string description, string startTime, string endTime)
            {
                ID = id;
                Name = description;
                StartTime = startTime;
                EndTime = endTime;
                Sessions = new SessionCollection();



            }


            public Shift(int id, string description, TimeSpan startTime, TimeSpan endTime)
            {
                ID = id;
                Name = description;
                this.startTime = startTime;
                this.endTime = endTime;
                Sessions = new SessionCollection();
  


            }

   

            public Session getSession(TimeSpan time)
            {
                foreach (Session s in Sessions)
                {
                    if (s.IsWithin(time) == true)
                        return s;
                }
                return null;
            }

            public bool IsWithin(TimeSpan ts)
            {
                TimeSpan start = startTime;
                TimeSpan end = endTime;


                if (end < startTime)
                {
                    if (ts <= startTime && ts < endTime)
                        return true;
                    return false;
                }

                if (ts >= startTime && ts < endTime)
                    return true;
                return false;
        
            }
        }

        public class ShiftCollection : ObservableCollection<Shift>
        {
            public List<Shift> getShifts(TimeSpan time)
            {
                List<Shift> shiftList = new List<Shift>();
                IEnumerator<Shift> enumerator = this.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsWithin(time))
                    {
                        shiftList.Add(enumerator.Current);
                    }

                }
                return shiftList;
            }
        }


           
        public class shiftInfo
        {
            public string Name { get; set; }

            public string StartTime { get; set; }
            public string EndTime { get; set; }


            public shiftInfo()
            {
            }
        }

        public class Session
        {
            public int Shift { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            TimeSpan startTime;
            public string StartTime
            {
                get { return startTime.ToString(); }
                set
                {
                    if (value == String.Empty)
                    {

                    }
                    else
                    {
                        try
                        {
                            String[] timeparams = value.Split(':');
                            startTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                                int.Parse(timeparams[2]));
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
            }

            TimeSpan endTime;
            public string EndTime
            {
                get { return endTime.ToString(); }
                set
                {
                    if (value == String.Empty)
                    {

                    }
                    else
                    {
                        try
                        {
                            String[] timeparams = value.Split(':');
                            endTime = new TimeSpan(int.Parse(timeparams[0]), int.Parse(timeparams[1]),
                                                int.Parse(timeparams[2]));
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
            }

            public Session()
            {
            }
            public Session(int shift, int id, string description, TimeSpan startTime, TimeSpan endTime)
            {
                Shift = shift;
                ID = id;
                Name = description;
                this.startTime = startTime;
                this.endTime = endTime;
            }


            public Session(int shift, int id, string description, String startTime, String endTime)
            {
                Shift = shift;
                ID = id;
                Name = description;
                StartTime = startTime;
                EndTime = endTime;
            }

            public bool IsWithin(TimeSpan ts)
            {
                TimeSpan start = startTime;
                TimeSpan end = endTime;


                if (end < startTime)
                {
                    if (ts <= startTime && ts < endTime)
                        return true;
                    return false;
                }

                if (ts >= startTime && ts < endTime)
                    return true;
                return false;
        
            }
        }

        public class SessionCollection : ObservableCollection<Session>
        {
            public Session getTargetSession(TimeSpan ts)
            {
                IEnumerator<Session> enumerator = this.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsWithin(ts))
                    {
                        return enumerator.Current;
                    }

                }
                return null;
            }

        }

        public class sessionInfo
        {
            public int ShiftIndex { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }

            public sessionInfo()
            {
            }
        }

        public class Availability
        {
            public int Line { get; set; }
            public List<IssueDetails> issues;

            public TimeSpan EffectiveDowntime;

            public Availability()
            {
                issues = new List<IssueDetails>();
                EffectiveDowntime = new TimeSpan(0, 0, 0);
            }

            public void Add(IssueDetails issue)
            {
                if (issues.Count == 0)
                {
                    issues.Add(issue);
                    return;
                }
                foreach (IssueDetails id in issues)
                {

                    if ((issue.Raised >= id.Raised) && (issue.Resolved <= id.Resolved)) //overlap case 3
                    {
                        continue;
                    }
                    else if ((issue.Raised < id.Raised)
                        && (issue.Resolved > id.Raised) && (issue.Resolved <= id.Resolved)) // overlap case 1
                    {
                        issue.Resolved = id.Raised;
                        issues.Add(issue);
                        break;
                    }
                    else if ((issue.Raised > id.Raised) && (issue.Raised < id.Resolved) && (issue.Resolved > id.Resolved)) // overlap case 2
                    {
                        issue.Raised = id.Resolved;
                        issues.Add(issue);
                        break;
                    }
        
                    
                    else
                    {
                        issues.Add(issue);
                        break;
                    }
                }

            }

            public double getAvailability(TimeSpan from, TimeSpan to)
            {
                double availability = 0;
                foreach (IssueDetails i in issues)
                {
                    EffectiveDowntime = EffectiveDowntime.Add( i.Resolved - i.Raised);
                   // availability += EffectiveDowntime.Hours * 60 * 60 + EffectiveDowntime.Minutes * 60 + EffectiveDowntime.Seconds;
                }
                availability += EffectiveDowntime.Hours * 60 * 60 + EffectiveDowntime.Minutes * 60 + EffectiveDowntime.Seconds;
                double totalavailability = (to - from).Hours * 60 * 60 + (to - from).Minutes * 60 + (to - from).Seconds;


                double  availabilityPercentage = ((totalavailability - availability) * 100 / totalavailability);

                if (availabilityPercentage < 0) availabilityPercentage = 0;

                return availabilityPercentage;
            }

        }

        public class IssueDetails
        {
            public int Line { get; set; }
            public int Station { get; set; }
            public int Tolerance { get; set; }

            public TimeSpan Raised { get; set; }
            public TimeSpan Resolved { get; set; }

            public IssueDetails()
            {
            }
        }


        public class AvailabilityRecord
        {
            public String Date { get; set; }
            public String LineName { get; set; }
        
            
            public String Session { get; set; }
            public String EffectiveDowmtime { get; set; }
            public String AvailabilityPercentage { get; set;}

            public AvailabilityRecord(String LineName, String Date, String Session, String AvailabilityPercentage)
            {
                this.LineName = LineName;
                this.Date = Date;
                this.Session = Session;
                this.AvailabilityPercentage = AvailabilityPercentage;
            }

            public AvailabilityRecord(String LineName, String Date, String Session, String AvailabilityPercentage,
                string effectiveDowntime)
            {
                this.LineName = LineName;
                this.Date = Date;
                this.Session = Session;
                this.AvailabilityPercentage = AvailabilityPercentage;
                this.EffectiveDowmtime = effectiveDowntime;
            }


            public override string ToString()
            {

                return Date + "," + LineName + "," + Session + "," + EffectiveDowmtime+","+ AvailabilityPercentage;
               
            }
        }

        public class AvailabilityReport : ObservableCollection<AvailabilityRecord>
        {
        }


}
