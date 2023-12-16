using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.Business
{
    public class StatisticsWIP
    {
        private DataTable mBomnoDT;
        public bool InitBomno()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW_PLUS))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = "";
                    return true;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
    }
}