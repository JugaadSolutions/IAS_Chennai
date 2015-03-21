using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace IAS
{
    public class DataAccess
    {
        public enum TransactionStatus
        {
            NONE = 0, OPEN = 1, INPROCESS = 2,
            COMPLETE = 3, TIMEOUT = 4
        };
        public static String conStr;

        #region CONSTRUCTORS
        public DataAccess()
        {
            if( conStr ==String.Empty)
            conStr = ConfigurationSettings.AppSettings["DBConStr"];
        }

        public DataAccess(String conStr)
        {
            DataAccess.conStr = conStr;
        }
        #endregion

        #region LINEMANAGEMENT
        public lineCollection getLines()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            lineCollection lines = new lineCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description FROM lines ORDER BY id";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                lines.add(new line((int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"]));
            }

            con.Close();
            con.Dispose();
            return lines;
        }


        public void addLine(int ID, string description)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into lines(id ,description) values({0},'{1}')";
            qry = String.Format(qry, ID, description);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void deleteLine(int ID)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from lines where id={0}";
            qry = String.Format(qry, ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateLine(int currentID, int modifiedID, string description)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update lines set id = {0} , description ='{1}' where id ={2}";
            qry = String.Format(qry, modifiedID, description, currentID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public StationCollection getStations(int line)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            StationCollection stations = new StationCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description,tolerance FROM stations where line={0} ORDER by id";

            qry = String.Format(qry, line);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                stations.Add(new Station(line,(int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"],(int)dt.Rows[i]["tolerance"]));
            }

            con.Close();
            con.Dispose();
            return stations;
        }

        public void removeStation(int line, int station)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from stations where line = {0} and id= {1}";
            qry = String.Format(qry, line, station);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = @"delete from class where line ={0} and id = {0}";
            qry = String.Format(qry, line, station);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void addStation(int line, int ID, string description,int tolerance)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into stations(line , id , description,tolerance)
                    values({0},{1},'{2}',{3})";
            qry = String.Format(qry, line, ID, description,tolerance);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateStation(int line, int ID, string description, int tolerance)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update stations set [description]='{0}',tolerance={1} where line ={2} and ID = {3} ";
            qry = String.Format(qry,  description, tolerance,line, ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }



        public ClassCollection getClass(int line, int station, int department)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ClassCollection Class = new ClassCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description FROM class where line={0} and station = {1} and
                department={2} ORDER by id";

            qry = String.Format(qry, line,station , department);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Class.Add(new Class(department, (int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"]));
            }

            con.Close();
            con.Dispose();
            return Class;
        }

        public void addClass(int line, int station, int department, int code, string description)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into class(line ,station, department , id, description)
                    values({0},{1},{2},{3},'{4}')";
            qry = String.Format(qry, line, station, department, code, description);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void removeClass( int line , int station , int department , int code)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from class where line={0} and station={1}
                    and department={2} and  id = {3}";
            qry = String.Format(qry, line, station, department, code);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        #endregion

        #region SHIFT MANAGEMENT

        public ShiftCollection getShifts()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ShiftCollection shifts= new ShiftCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM shift ORDER BY id";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                shifts.Add(new Shift((int)dt.Rows[i]["id"],(string)dt.Rows[i]["description"],
                   ((TimeSpan) dt.Rows[i]["from"]),((TimeSpan) dt.Rows[i]["to"])));
            }

            con.Close();
            con.Dispose();
            return shifts;
        }


        public void addShift(string name ,string startTime ,string endTime)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"SELECT Max(id) FROM shift ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            int id = (dt.Rows[0][0] == DBNull.Value) ?0 : (int) dt.Rows[0][0];
            id += 1;
            
            qry = @"insert into shift(id ,description,[from],[to]) values({0},'{1}','{2}','{3}')";
            qry = String.Format(qry,id, name, startTime,endTime);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }
        public void deleteShift(int ID)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from shift where id={0}";
            qry = String.Format(qry, ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public SessionCollection getSessions(int shift)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SessionCollection sessions = new SessionCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM sessions where shift={0}";

            qry = String.Format(qry, shift);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sessions.Add(new Session(shift, (int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"],
                    ((TimeSpan)dt.Rows[i]["from"]).ToString(),((TimeSpan) dt.Rows[i]["to"]).ToString()));
            }

            con.Close();
            con.Dispose();
            return sessions;
        }
        

        public void addSession(int shift , int id , string description, string start, string end)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into sessions(shift,id ,description,[from],[to]) values({0},{1},'{2}','{3}','{4}')";
            qry = String.Format(qry,shift ,id, description, start,end);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void deleteSession(int shift, int id)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"delete from sessions where shift = {0} and id = {1}";
            qry = String.Format(qry, shift, id);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();



            con.Close();
            con.Dispose();
        }

        #endregion


        #region SUPPORT GROUP MANAGEMENT

        public ContactCollection getContacts()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ContactCollection contacts = new ContactCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM contacts order by name";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable contactTable = new DataTable();
            contactTable.Load(dr);
            dr.Close();

            qry = String.Empty;
            qry = @"SELECT * FROM lines";

            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable lineTable = new DataTable();
            lineTable.Load(dr);
            dr.Close();

            cmd.Dispose();

            for (int i = 0; i < contactTable.Rows.Count; i++)
            {
                Contact c = new Contact((int)contactTable.Rows[i]["slNo"], (string)contactTable.Rows[i]["name"],
                    (string)contactTable.Rows[i]["number"]);
                

                createAssociation(c, 
                    contactTable.Rows[i]["lineAssociation"] == DBNull.Value ? String.Empty :(String) contactTable.Rows[i]["lineAssociation"],
                    contactTable.Rows[i]["shiftAssociation"] == DBNull.Value ? String.Empty :(String) contactTable.Rows[i]["shiftAssociation"],
                    contactTable.Rows[i]["departmentAssociation"] == DBNull.Value ? String.Empty :(String) contactTable.Rows[i]["departmentAssociation"],
                    contactTable.Rows[i]["escalationAssociation"] == DBNull.Value ? String.Empty :(String) contactTable.Rows[i]["escalationAssociation"]);

                if ((bool)contactTable.Rows[i]["hourlySummary"] == true)
                    c.LineSummary = true;
                else
                    c.LineSummary = false;

                if ((bool)contactTable.Rows[i]["shiftSummary"] == true)
                    c.ShiftSummary = true;
                else
                    c.ShiftSummary = false;
                

                contacts.Add(c);
            }

            con.Close();
            con.Dispose();
            return contacts;
        }




        private ShiftAssociationCollection createShiftAssociation(DataTable lineTable, String association)
        {
            ShiftAssociationCollection Association = new ShiftAssociationCollection();
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            SqlCommand cmd;
            String qry;
            qry = String.Empty;


            qry = @"SELECT * FROM shift";
            cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            
            DataTable shiftTable = new DataTable();
            shiftTable.Load(dr);
            dr.Close();

            for (int i = 0; i < shiftTable.Rows.Count; i++)
            {
                Association.Add(new ShiftAssociationInfo((int)shiftTable.Rows[i]["id"],
                     (string)lineTable.Rows[i]["description"],
                     (association.ToCharArray())[i] == '0' ? false : true));
            }
            return Association;
        }



        public static void createAssociation(Contact c)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry;
            qry = String.Empty;
            qry = @"SELECT * FROM lines";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable lineTable = new DataTable();
            lineTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM shift";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable shiftTable = new DataTable();
            shiftTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM departments";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable departmentTable = new DataTable();
            departmentTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM escalation";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable escalationTable = new DataTable();
            escalationTable.Load(dr);
            dr.Close();

            cmd.Dispose();

            LineAssociationCollection lineAssociation = new LineAssociationCollection();

            for (int i = 0; i < lineTable.Rows.Count; i++)
            {
                lineAssociation.Add(new LineAssociationInfo((int)lineTable.Rows[i]["id"],
                     (string)lineTable.Rows[i]["description"], 
                     true));
            }
            c.LineAssociation = lineAssociation;

            ShiftAssociationCollection shiftAssociation = new ShiftAssociationCollection();

            for (int i = 0; i < shiftTable.Rows.Count; i++)
            {
                shiftAssociation.Add(new ShiftAssociationInfo((int)shiftTable.Rows[i]["id"],
                     (string)shiftTable.Rows[i]["description"],
                     false));
            }
            c.ShiftAssociation = shiftAssociation;

            DepartmentAssociationCollection departmentAssociation = new DepartmentAssociationCollection();

            for (int i = 0; i < departmentTable.Rows.Count; i++)
            {
                departmentAssociation.Add(new DepartmentAssociationInfo((int)departmentTable.Rows[i]["id"],
                     (string)departmentTable.Rows[i]["description"],
                     false));
            }
            c.DepartmentAssociation = departmentAssociation;


            EscalationAssociationCollection escalationAssociation = new EscalationAssociationCollection();

            for (int i = 0; i < escalationTable.Rows.Count; i++)
            {
                escalationAssociation.Add(new EscalationAssociationInfo((int)escalationTable.Rows[i]["id"],
                     (string)escalationTable.Rows[i]["description"],
                     false));
            }
            c.EscalationAssociation = escalationAssociation;



            con.Close();
            con.Dispose();
            
        }

         public static void createAssociation(Contact c,String lineTag,string shiftTag,string departmentTag,string escalationTag)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry;
            qry = String.Empty;
            qry = @"SELECT * FROM lines";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable lineTable = new DataTable();
            lineTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM shift";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable shiftTable = new DataTable();
            shiftTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM departments";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable departmentTable = new DataTable();
            departmentTable.Load(dr);
            dr.Close();


            qry = @"SELECT * FROM escalation";
            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable escalationTable = new DataTable();
            escalationTable.Load(dr);
            dr.Close();

            cmd.Dispose();

            LineAssociationCollection lineAssociation = new LineAssociationCollection();

            for (int i = 0; i < lineTable.Rows.Count; i++)
            {
                lineAssociation.Add(new LineAssociationInfo((int)lineTable.Rows[i]["id"],
                     (string)lineTable.Rows[i]["description"],
                     (lineTag.ToCharArray())[i] == '0' ? false : true));
            }
            c.LineAssociation = lineAssociation;

            ShiftAssociationCollection shiftAssociation = new ShiftAssociationCollection();

            for (int i = 0; i < shiftTable.Rows.Count; i++)
            {
                shiftAssociation.Add(new ShiftAssociationInfo((int)shiftTable.Rows[i]["id"],
                     (string)shiftTable.Rows[i]["description"],
                     (shiftTag.ToCharArray())[i] == '0' ? false : true));
            }
            c.ShiftAssociation = shiftAssociation;

            DepartmentAssociationCollection departmentAssociation = new DepartmentAssociationCollection();

            for (int i = 0; i < departmentTable.Rows.Count; i++)
            {
                departmentAssociation.Add(new DepartmentAssociationInfo((int)departmentTable.Rows[i]["id"],
                     (string)departmentTable.Rows[i]["description"],
                     (departmentTag.ToCharArray())[i] == '0' ? false : true));
            }
            c.DepartmentAssociation = departmentAssociation;


            EscalationAssociationCollection escalationAssociation = new EscalationAssociationCollection();

            for (int i = 0; i < escalationTable.Rows.Count; i++)
            {
                escalationAssociation.Add(new EscalationAssociationInfo((int)escalationTable.Rows[i]["id"],
                     (string)escalationTable.Rows[i]["description"],
                     (escalationTag.ToCharArray())[i] == '0' ? false : true));
            }
            c.EscalationAssociation = escalationAssociation;



            con.Close();
            con.Dispose();
            
        }


        public void updateContact(Contact c)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            SqlCommand cmd;
            if (c.ID == -1)
            {
                qry = @"insert into contacts(name,number) values('{0}','{1}') ";
                qry = String.Format(qry, c.Name, c.Number);
                cmd = new SqlCommand(qry, con);
                cmd.ExecuteNonQuery();

                qry = @"select * from contacts where name='{0}'and number='{1}' ";
                qry = String.Format(qry, c.Name, c.Number);
                cmd = new SqlCommand(qry, con);

                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);

                dr.Close();

                c.ID = (int)dt.Rows[0]["slNo"];
            }
            else
            {
                qry = @"update contacts set name='{0}', number='{1}' where slNo={2}";
                qry = String.Format(qry, c.Name, c.Number, c.ID);
                cmd = new SqlCommand(qry, con);

                cmd.ExecuteNonQuery();
            }
            qry = @"update contacts set lineAssociation ='{0}',shiftAssociation = '{1}',
                       departmentAssociation='{2}',escalationAssociation='{3}' where slNo= {4}";
            qry = String.Format(qry, getLineAssociationTag(c.LineAssociation),
                                    getShiftAssociationTag(c.ShiftAssociation),
                                    getDepartmentAssociationTag(c.DepartmentAssociation),
                                    getEscalationAssociationTag(c.EscalationAssociation),
                                    c.ID);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            con.Close();
            con.Dispose();
        }

        public string getLineAssociationTag(LineAssociationCollection line)
        {
            Char[] a = new Char[line.Count];
            int i = 0;
            foreach (LineAssociationInfo l in line)
            {
                a[i++] = (l.IsAssociated==false)? '0':'1';
            }

            return new String(a);
        
        }


        public string getShiftAssociationTag(ShiftAssociationCollection shift)
        {
            Char[] a = new Char[shift.Count];
            int i = 0;
            foreach (ShiftAssociationInfo l in shift)
            {
                a[i++] = (l.IsAssociated == false) ? '0' : '1';
            }

            return new String(a);

        }

        public string getDepartmentAssociationTag(DepartmentAssociationCollection dep)
        {
            Char[] a = new Char[dep.Count];
            int i = 0;
            foreach (DepartmentAssociationInfo l in dep)
            {
                a[i++] = (l.IsAssociated == false) ? '0' : '1';
            }

            return new String(a);

        }


        public string getEscalationAssociationTag(EscalationAssociationCollection esc)
        {
            Char[] a = new Char[esc.Count];
            int i = 0;
            foreach (EscalationAssociationInfo l in esc)
            {
                a[i++] = (l.IsAssociated == false) ? '0' : '1';
            }

            return new String(a);

        }


        public void deleteContact(Contact c)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"delete from contacts where slNo= {0} ";
            qry = String.Format(qry, c.ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();



            con.Close();
            con.Dispose();
        }

                
#endregion


        #region SETTINGS MANAGEMENT


        public String getMarquee()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from config where [key]='marquee'";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            return (String)dt.Rows[0]["value"];
        }

        public bool updateMarquee(String msg)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"update config set value = '{0}' where [key] = 'marquee'";

            qry = String.Format(qry, msg);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);
        }




        public String getMarqueeSpeed()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from config where [key]='marqueeSpeed'";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            return (String)dt.Rows[0]["value"];
        }



        public bool updateMarqueeSpeed(String msg)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"update config set value = '{0}' where [key] = 'marqueeSpeed'";

            qry = String.Format(qry, msg);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);
        }

        public String getIssuemarqueeSpeed()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from config where [key]='issueMarqueeSpeed'";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            return (String)dt.Rows[0]["value"];
        }



        public bool updateIssuemarqueeSpeed(String msg)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"update config set value = '{0}' where [key] = 'issueMarqueeSpeed'";

            qry = String.Format(qry, msg);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);
        }

        public String getPassword()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from config where [key]='Password'";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            return (String)dt.Rows[0]["value"];
        }



        public bool updatePassword(String msg)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"update config set value = '{0}' where [key] = 'password'";

            qry = String.Format(qry, msg);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);
        }

        public ObservableCollection<escalationInfo> getEscalationSettings()
        {
            ObservableCollection<escalationInfo> escalationSettings = new ObservableCollection<escalationInfo>();

            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from escalation where id > 0 ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                escalationSettings.Add(new escalationInfo((string)dt.Rows[i]["description"],
                    (int)dt.Rows[i]["id"], (int)dt.Rows[i]["timeout"]));
            }

            return escalationSettings;
        }

        public List<double> getEscalationTimeout()
        {
            List<double> escalationTimeout = new List<double>();

            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from escalation where id > 0 ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                escalationTimeout.Add((int)dt.Rows[i]["timeout"]);
            }

            return escalationTimeout;
        }




        public void updateEscalationSettings(ObservableCollection<escalationInfo> escalationSettings)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            foreach (escalationInfo e in escalationSettings)
            {
                int result = 0;
                String qry = @"update escalation set timeout = '{0}' where id = {1}";

                qry = String.Format(qry,Convert.ToInt32(e.Duration), e.ID);
                SqlCommand cmd = new SqlCommand(qry, con);
                result = cmd.ExecuteNonQuery();

            }
        }



        #endregion


        #region HOURLY_SHIFT_SUMMARY_MANAGEMENT

        public ContactCollection getHourlyUpdateContacts()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ContactCollection contacts = new ContactCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM contacts where hourlySummary = 1 order by name ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable contactTable = new DataTable();
            contactTable.Load(dr);
            dr.Close();

            qry = String.Empty;
            qry = @"SELECT * FROM lines";

            cmd = new SqlCommand(qry, con);
            dr = cmd.ExecuteReader();
            DataTable lineTable = new DataTable();
            lineTable.Load(dr);
            dr.Close();

            cmd.Dispose();

            for (int i = 0; i < contactTable.Rows.Count; i++)
            {
                Contact c = new Contact((int)contactTable.Rows[i]["slNo"], (string)contactTable.Rows[i]["name"],
                    (string)contactTable.Rows[i]["number"]);


                createAssociation(c,
                    contactTable.Rows[i]["lineAssociation"] == DBNull.Value ? String.Empty : (String)contactTable.Rows[i]["lineAssociation"],
                    contactTable.Rows[i]["shiftAssociation"] == DBNull.Value ? String.Empty : (String)contactTable.Rows[i]["shiftAssociation"],
                    contactTable.Rows[i]["departmentAssociation"] == DBNull.Value ? String.Empty : (String)contactTable.Rows[i]["departmentAssociation"],
                    contactTable.Rows[i]["escalationAssociation"] == DBNull.Value ? String.Empty : (String)contactTable.Rows[i]["escalationAssociation"]);

                if ((bool)contactTable.Rows[i]["hourlySummary"] == true)
                    c.LineSummary = true;
                else
                    c.LineSummary = false;

                if ((bool)contactTable.Rows[i]["shiftSummary"] == true)
                    c.ShiftSummary = true;
                else
                    c.ShiftSummary = false;


                contacts.Add(c);
            }

            con.Close();
            con.Dispose();
            return contacts;
        }


       

        public void updateNP(int line, int shift)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update lines_shifts set np='true' where line = {0} and shift = {1}";

            qry = String.Format(qry, line, shift);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        #endregion


        public int addContact(string number, string name)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into contacts(number , name)
                    values('{0}','{1}')";
            qry = String.Format(qry, number, name);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = String.Empty;
            qry = @"select slNo from contacts where number = '{0}' and name ='{1}'";
            qry = String.Format(qry, number, name);
            cmd = new SqlCommand(qry, con);

            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();




            con.Close();
            con.Dispose();

            return (int)dt.Rows[0][0];
        }


        public void removeContact(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from contacts where slNo = {0}";
            qry = String.Format(qry, slNo);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = @"delete from sms_entity_map where contact= {0}";
            qry = String.Format(qry, slNo);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void addToEscalationList(int id, int line, int department, int level)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into sms_entity_map(contact ,line , department,level)
                    values({0},{1},{2},{3})";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void removeContactFromEscalationList(int id, int line, int department, int level)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from sms_entity_map where contact={0} and line={1} and department={2}and level={3}";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public DataTable getStationsInfo(String stations)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT stations.id , stations.description AS STATION_DESCRIPTION 
                    FROM stations ";
            String qry1 = String.Empty;
            if (stations != String.Empty)
            {
                qry1 = " where id in ({0})";
                qry1 = String.Format(qry1, stations);
            }

            qry = String.Format(qry, stations);
            qry += qry1;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            return dt;
        }

        public DataTable getDepartmentInfo(String departments)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select * from departments";
            if (departments != String.Empty)
            {
                qry += " where id in ({0})";
                qry = String.Format(qry, departments);
            }

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            return dt;
        }

        public int findIssueRecord(int line, int station, int department, String data)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select slNo from issues where line = {0} and
                    station = {1} and department = {2} and data = '{3}' 
                    and status = 'raised'";
            qry = String.Format(qry,line, station, department, data);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if ((dt.Rows.Count <= 0) || (dt.Rows.Count > 1))
                return -1;

            else return (int)dt.Rows[0][0];
        }

        public void updateIssueStatus(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), slNo);
            SqlCommand cmd = new SqlCommand(qry, con);
             
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void updateIssueStatus(DataTable dt)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            for(int i = 0 ;i < dt.Rows.Count;i++)

            {
                String qry = String.Empty;
                qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
                qry = String.Format(qry, DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"),(int) dt.Rows[i]["SlNo"]);
                SqlCommand cmd = new SqlCommand(qry, con);
             
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            con.Close();
            con.Dispose();
        }


        public List<Byte> getIssueInfo(int slNo, out int l)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            l = -1;
            String qry = @"select * from issues where [slNo] = {0}";

            qry = String.Format(qry, slNo);


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            if (dt.Rows.Count == 0)
                return null;
            short lineNO = Convert.ToInt16((int)dt.Rows[0]["line"]);

            Byte[] line = new byte[2];
            line = BitConverter.GetBytes(lineNO);
            l = lineNO;


            Byte[] stn =
                BitConverter.GetBytes(Convert.ToInt16((int)dt.Rows[0]["station"]));
            Byte dep = Convert.ToByte((int)dt.Rows[0]["department"]);



            List<Byte> data = new List<byte>();

            data.Add(stn[1]);
            data.Add(stn[0]);

            data.Add(dep);


            for (int i = 0; i < data.Count; i++)
                data[i] += (Byte)'0';


            if (dt.Rows[0]["data"] != DBNull.Value)
            {
                Byte[] msg = new byte[40];
                char[] d = ((String)dt.Rows[0]["data"]).ToCharArray();
                foreach (char c in d)
                    data.Add((Byte)c);


            }


            return data;



        }

        public String getActualQuantity(int lines)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
           

            qry = @"select Top(1)  quantity , np from actual 
                    inner join lines_shifts on actual.line = lines_shifts.line 
                    where actual.line = {0} and DATEDIFF(DD,timestamp,GETDATE()) < 1 order by timestamp desc";

            qry = String.Format(qry, lines);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            if (dt.Rows.Count == 0)
                return String.Empty;
            else if ((bool)dt.Rows[0]["np"] == true)
                return "NP";
            else return ((int)dt.Rows[0][0]).ToString();    

        }

        public void updateActualQuantity(int line, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into actual(line,quantity,timestamp)
                    values({0},{1},'{2}')";
            qry = String.Format(qry,line,quantity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void updateTargetQuantity(int line, int shift,int session, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into target(line,shift,session,quantity,timestamp)
                    values({0},{1},{2},{3},'{4}')";
            qry = String.Format(qry, line, shift, session, quantity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        public String getTargetQuantity(int shift , int session, int id)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            List<int> stationList = new List<int>();

            qry = @"select quantity from target where shift in ({0})
                        and session in ({1}) and line = {2} order by timestamp desc";

            qry = String.Format(qry, shift, session, id);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            

            int quantity = (dt.Rows.Count == 0) ? -1 : (int) dt.Rows[0][0];
            return (quantity == -1) ?  string.Empty : quantity.ToString();

        }


        public int insertIssueRecord(int line ,int station, int department, String data,String message)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;

            SqlCommand cmd = new SqlCommand(qry, con);
            DataTable dt = new DataTable();

            cmd = new SqlCommand("insertIssueRecord", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@line", line);
            cmd.Parameters.AddWithValue("@station", station);
            cmd.Parameters.AddWithValue("@department", (int)department);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
            cmd.Parameters.AddWithValue("@message", message);
            SqlDataReader dr = cmd.ExecuteReader();
            dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            int slNo = (int)dt.Rows[0][0];
            return slNo;

        }

        public DataTable getCommands()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = @"select * from Command where [status] = {0} and request_date='{1}'";

            qry = String.Format(qry, (int)TransactionStatus.OPEN, DateTime.Today.ToString("MM-dd-yyyy"));

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public bool updateCommandStatus(int id, TransactionStatus status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update Command set [status] = {0} where id = {1}";
            qry = String.Format(qry, (int)status, id);


            SqlCommand cmd = new SqlCommand(qry, con);
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException s)
            {
                result = 0;
            }
            return (result == 1);
        }

        public void updateIssueMarquee()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            string issueMarquee = String.Empty;
            String qry = @"select message from issues where [status] = 'raised' and DATEDIFF(hh,timestamp,GETDATE())<=24";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                issueMarquee += (String)dt.Rows[i]["message"] + "; ";
            }
            qry = @"update config set value ='{0}' where [key]='issueMarquee'";
            qry = String.Format(qry, issueMarquee);

            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();
        }


        #region ManageTab

        public DataTable getReceivers(int line,int department , int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select receiver as Receivers from sms_receiver 
                inner join sms_entity_map on sms_receiver.slNo = sms_entity_map.receiver_id where entity_1_id = {0}
                and entity_2_id = {1} and parameter_1 = {2}";

            qry = String.Format(qry, department,line, escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public Dictionary<String, int> loadReceiverList()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver";

            Dictionary<String, int> receiverList = new Dictionary<string, int>();

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                receiverList.Add((String)dt.Rows[i]["receiver"], (int)dt.Rows[i]["slNo"]);
            }

            return receiverList;
        }

        public int getReceiverSlNo(String receiver)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver where receiver = '{0}'";
            qry = String.Format(qry, receiver);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            int slNo = (dt.Rows.Count > 0) ? (int)(dt.Rows[0]["slNo"]) : -1;
            return slNo;
        }
        public String getMessage(int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select message as Message from departments
                 where id = {0}";

            qry = String.Format(qry, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return (String)dt.Rows[0][0];

        }


        public bool updateMessage(String message, int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update departments set message = '{0}' where [id] = {1}";

            qry = String.Format(qry, message, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }


        public void addReceiver(String receiver,int line, int department, int slNo,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = String.Empty;
            SqlCommand cmd = null;


            if (slNo == -1)
            {
                qry = @"insert into sms_receiver(receiver) values('{0}')";
                qry = String.Format(qry, receiver);

                cmd = new SqlCommand(qry, con);
                result = cmd.ExecuteNonQuery();

                qry = @"select Count(*) from sms_receiver";
                cmd = new SqlCommand(qry, con);

                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dr.Close();

                slNo = (int)dt.Rows[0][0];
            }

            qry = @"insert into sms_entity_map(receiver_id , entity_1_id,entity_2_id,parameter_1) values({0},{1},{2},{3})";
            qry = String.Format(qry, slNo, department,line, escalationLevel);

            cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();
        }

        public bool removeReceiver(int slNo,int line, int department,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"delete from sms_entity_map where receiver_id = {0} and entity_1_id = {1} 
            and entity_2_id = {2} and parameter_1 = {3} ";

            qry = String.Format(qry, slNo, department,line,escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not
        }

        public bool insertSmsTrigger(string receiver, string message, int status,String timestamp)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"insert into sms_trigger(receiver , message , status,timestamp) values('{0}','{1}',{2},'{3}') ";

            qry = String.Format(qry, receiver, message, status,timestamp);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }



        public bool insertSmsTrigger(string receiver, string message, int status, String timestamp,int priority)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"insert into sms_trigger(receiver , message , status,timestamp,priority) 
                        values('{0}','{1}',{2},'{3}',{4}) ";

            qry = String.Format(qry, receiver, message, status, timestamp,priority);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }



        #endregion

        #region ReportTab

        public DataTable GetReportData(DateTime from, DateTime to)
        {

            SqlConnection localCon = new SqlConnection(conStr);
            
            String qry = @"select distinct 
                        
                        lines.id as LINE , 
                        stations.id as STATION,
                        stations.tolerance as Tolerance,
                        CONVERT(TIME(0), raised.timestamp,0) as RAISED , 
                        CONVERT(TIME(0), resolved.timestamp,0) as RESOLVED
                        from issues
                        inner join stations on (stations.id = issues.station and stations.line = issues.line)
                        inner join lines on lines.id = issues.line
                        inner join departments on issues.department = departments.id
                        inner join ( select issue, timestamp from issue_tracker where status = 'raised') as raised 
                        on raised.issue = issues.slNo

                        left outer join (select issue , timestamp from issue_tracker where status = 'resolved')
                        as resolved on resolved.issue = issues.slNo 
                        where raised.timestamp >= '{0}' and raised.timestamp <= '{1}' ";

            qry = String.Format(qry, from.ToString("MM-dd-yyyy HH:mm:ss"), to.ToString("MM-dd-yyyy HH:mm:ss"));

            localCon.Open();

            SqlCommand cmd = new SqlCommand(qry, localCon);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            localCon.Close();
            localCon.Dispose();

            return dt;


        }


        public DataTable GetOpenIssues()
        {
            SqlConnection localCon = new SqlConnection(conStr);
            String qry = @"select distinct Substring(Convert(nvarchar,issues.timestamp,0),0,12) as DATE, 
                        issues.slNo as SlNo,
                        lines.description as LINE , 
                        issues.station as STATION_NO,
                        stations.description as STATION,
                        departments.description as ISSUE , 
                        issues.data as DETAILS,
                        CONVERT(TIME(0), issues.timestamp,0) as RAISED 
                        from issues
                        LEFT OUTER join stations on (stations.id = issues.station and stations.line = issues.line)
                        inner join lines on lines.id = issues.line
                        inner join departments on issues.department = departments.id
                        where status='raised'";


            localCon.Open();

            SqlCommand cmd = new SqlCommand(qry, localCon);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            localCon.Close();
            localCon.Dispose();

            return dt;
        }

        #endregion


        ~DataAccess()
        {

        }

        public void close()
        {

        }
    }
}
