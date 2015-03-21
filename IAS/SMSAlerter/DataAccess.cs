using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;

namespace gsm.sms
{
    class DataAccess
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

        public DataTable getOpenSMSAlerts()
        {
            String qry = String.Empty;
            qry = @"select * from sms_trigger where status = 1 and DATEDiff(MINUTE,timestamp,GetDate()) < 60
                order by priority desc";


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            qry = "update sms_trigger set status = 2 where status = 1 ";
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();



            return dt;
        }
        ~DataAccess()
        {

        }

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
