using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using ias.shared;
namespace ias.client
{
    public class DataAccess
    {


        private String conStr;
        private SqlConnection con;

        public DataAccess()
        {
            conStr = ConfigurationSettings.AppSettings["DBConStr"];
            con = new SqlConnection(conStr);
            try
            {
                con.Open();
            }
            catch (SqlException s)
            {
                throw s;
            }
        }


        public DataTable getProductionLineInfo(String lines)
        {
            String qry = String.Empty;
            qry = @"SELECT lines.id , lines.description 
                    FROM lines where lines.id in ({0}) ";

            qry = String.Format(qry, lines);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            return dt;
        }



        public DataTable getProductionLineInfo()
        {
            String qry = String.Empty;
            qry = @"SELECT lines.id , lines.description 
                    FROM lines ";
            String qry1 = String.Empty;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            return dt;
        }


       public ShiftCollection getShifts()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ShiftCollection shifts= new ShiftCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM shift where id <> 4 ORDER BY id";

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

       public SessionCollection getSessions(int shift)
       {

           SqlConnection con = new SqlConnection(conStr);
           con.Open();

           SessionCollection sessions = new SessionCollection();

           String qry = String.Empty;
           qry = @"SELECT * FROM sessions where shift={0} ORDER BY id";

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
                   ((TimeSpan)dt.Rows[i]["from"]).ToString(), ((TimeSpan)dt.Rows[i]["to"]).ToString()));
           }

           con.Close();
           con.Dispose();
           return sessions;
       }


        public DataTable addCommand(int lineID, int command, int status, String cmdData, String date)
        {
            SqlCommand cmd = new SqlCommand("addCommand", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@line_id", lineID);
            cmd.Parameters.AddWithValue("@command", command);
            cmd.Parameters.AddWithValue("@commandData", cmdData);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@req_date", date);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public String getMarquee()
        {
            String qry = String.Empty;
            qry = @"select * from config where [key]='marquee'";
            SqlConnection prvCon = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(qry, prvCon);
            prvCon.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            prvCon.Close();
            prvCon.Dispose();

            return (String)dt.Rows[0]["value"];
        }


        public String getIssueMarquee()
        {
            String qry = String.Empty;
            qry = @"select * from config where [key]='issueMarquee'";
            SqlConnection prvCon = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(qry, prvCon);
            prvCon.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            prvCon.Close();
            prvCon.Dispose();

            return (String)dt.Rows[0]["value"];
        }



        public DataTable getProductionQuantity(int shift,String lines)
        {
            String qry = String.Empty;
            qry = @"select lines.description as [ProductionLine], 
                    target.quantity as [TargetQuantity],
                    actual.quantity as [ActualQuantity]
                    from actual inner join lines on actual.line = lines.id 
                    inner join [target] on target.line = lines.id
                    where shift = {0} and session = {1} ";

            String qry1 = String.Empty;
            if (lines != String.Empty)
            {
                qry1 = " and lines.id in ({0})";
                qry1 = String.Format(qry1, lines);
            }

            qry += qry1;

            qry = String.Format(qry,shift);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            return dt ;

        }

        public List<int> getBreakDownStatus(String lines)
        {
            String qry = String.Empty;
            List<int> stationList = new List<int>();
            qry = @"select distinct line from issues where department = 1 and status = 'raised' ";

            String qry1 = String.Empty;
            if (lines != String.Empty)
            {
                qry1 = " and line in ({0})";
                qry1 = String.Format(qry1, lines);
            }

            qry += qry1;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for(int i = 0; i< dt.Rows.Count; i++)
            {
                stationList.Add((int)dt.Rows[i]["line"]);
            }

            return stationList;

        }

        public List<int> getQualityStatus(String lines)
        {
            String qry = String.Empty;
            List<int> stationList = new List<int>();


            qry = @"select distinct line from issues where department = 2 and status = 'raised' ";


            String qry1 = String.Empty;
            if (lines != String.Empty)
            {
                qry1 = " and line in ({0})";
                qry1 = String.Format(qry1, lines);
            }

            qry += qry1;




            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                stationList.Add((int)dt.Rows[i]["line"]);
            }


            return stationList;

        }


        public List<int> getMaterialShortageStatus(String lines)
        {
            String qry = String.Empty;
            List<int> stationList = new List<int>();

            qry = @"select distinct line from issues where department = 3 and status = 'raised' ";

            String qry1 = String.Empty;
            if (lines != String.Empty)
            {
                qry1 = " and line in ({0})";
                qry1 = String.Format(qry1, lines);
            }

            qry += qry1;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                stationList.Add((int)dt.Rows[i]["line"]);
            }


            return stationList;

        }



        public String getTargetQuantity(int shift , int session, int id)
        {
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


        public String getActualQuantity(int lines )
        {
            String qry = String.Empty;
            List<int> stationList = new List<int>();

            qry = @"select Top(1)  quantity from actual where line = {0} and
                    DATEDIFF(DD,timestamp,GETDATE()) < 1 order by timestamp desc";

            qry = String.Format(qry, lines);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();


            int quantity = (dt.Rows.Count == 0) ? -1 : (int)dt.Rows[0][0];
            return (quantity == -1) ? string.Empty : quantity.ToString();
        }

            


        #region ReportTab

        public DataTable GetReportData(DateTime from, DateTime to)
        {

            SqlConnection localCon = new SqlConnection(conStr);
            to = to.AddDays(1);
            String qry = @"select  Substring(Convert(nvarchar,raised.timestamp,0),0,12) as DATE, 
                        
                        lines.description as LINE , 
                        departments.description as ISSUE , 
                        CONVERT(TIME(0), raised.timestamp,0) as RAISED , 
                        CONVERT(TIME(0), resolved.timestamp,0) as RESOLVED ,
                        CONVERT(Time(0) , resolved.timestamp - raised.timestamp , 0) as DOWNTIME 
                        from issues
                        inner join lines on lines.id = issues.line
                        inner join departments on issues.department = departments.id
                        inner join ( select issue, timestamp from issue_tracker where status = 'raised' ) as raised 
                        on raised.issue = issues.slNo

                        left outer join (select issue , timestamp from issue_tracker where status = 'resolved')
                        as resolved on resolved.issue = issues.slNo 
                        where raised.timestamp >= '{0}' and raised.timestamp <= '{1}' order by lines.description, raised.timestamp";
           qry =  string.Format(qry, from.ToString("MM-dd-yyyy"), to.ToString("MM-dd-yyyy"));

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

        public DataTable GetHourlyReportData(int line,DateTime from, DateTime to)
        {

            SqlConnection localCon = new SqlConnection(conStr);

            String qry = @"select distinct 
                        lines.description as LINENAME,
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
                        where raised.timestamp >= '{0}' and raised.timestamp <= '{1}' and lines.id={2} ";

            qry = String.Format(qry, from.ToString("MM-dd-yyyy HH:mm:ss"), to.ToString("MM-dd-yyyy HH:mm:ss"),line);

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




        public void close()
        {
            if (con != null)
            {
                con.Close();
                con = null;
            }
        }

    }
}
