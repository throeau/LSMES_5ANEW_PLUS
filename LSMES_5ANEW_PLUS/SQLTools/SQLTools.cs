using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.SQLTools
{
    public class SQLTools
    {
        private string mConnectionString;
        private SqlCommand mCommand;
        private SqlDataReader mReader;
        private SqlConnection mConnection;
        private DataTable mDataTable;
        private SqlTransaction sqlTransaction;

        public SQLTools(string connectionString)
        {
            mConnectionString = connectionString;
            using (mConnection = new SqlConnection())
            {
                mCommand = new SqlCommand();
                mDataTable = new DataTable("Data");
            }
        }
        ~SQLTools()
        {
            mDataTable.Clear();
        }
        public DataTable ExecuteQuery(string mSQL)
        {
            try
            {
                mConnection.ConnectionString = mConnectionString;
                mConnection.Open();

                mCommand.Connection = mConnection;
                sqlTransaction = mConnection.BeginTransaction();
                mCommand.Transaction = sqlTransaction;

                mCommand.CommandText = mSQL;
                sqlTransaction.Commit();

                mReader = mCommand.ExecuteReader();
                mDataTable.Clear();
                mDataTable.Load(mReader);
                mReader.Close();
                mConnection.Close();

                return mDataTable;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " \\ SQLTools::ExcuteQuery");
                sqlTransaction.Rollback();
                mDataTable.Clear();
                mReader.Close();
                mConnection.Close();
                return null;
            }
        }
    }
}