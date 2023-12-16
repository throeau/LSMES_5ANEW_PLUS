using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;
using System.Data;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Assemble
    {
        public static bool HiPot(EntityHipot entity)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("fail to database。");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO DATA_WINDING (BARCODE,BOMNO,VOLTAGE,RESISTANCE,COMMENTS) VALUES ('{0}','{1}',{2},{3},'{4}');", entity.BARCODE, entity.BOMNO, entity.VOLTAGE, entity.RESISTANCE, entity.COMMENTS), conn);
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("Assemble::HiPot => " + ex.Message);
                    return false;
                }
            }
        }
    }
}