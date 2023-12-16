using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.Business
{
    public class State
    {
        public int UpdateStateByEquipment(string equipmentno,string state,string recieved_date_time)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    string sql = "IF EXISTS(SELECT * FROM EQUIPMENT_STATE WHERE EQUIPMENT_NO = '{0}')	UPDATE EQUIPMENT_STATE SET STATE = '{1}',RECIEVED_DATE_TIME = '{2}' WHERE EQUIPMENT_NO = '{0}'; ELSE INSERT INTO EQUIPMENT_STATE (EQUIPMENT_NO,STATE,RECIEVED_DATE_TIME) VALUES ('{0}','{1}','{2}');INSERT INTO EQUIPMENT_STATE_LOG (EQUIPMENT_NO,STATE,RECIEVED_DATE_TIME) VALUES ('{0}','{1}','{2}');";
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format(sql, equipmentno, state, recieved_date_time);
                    //Notice notice = new Notice();
                    //notice.SendEquipmentState(equipmentno, state, recieved_date_time);
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
    }
}