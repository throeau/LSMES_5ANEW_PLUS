using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;
using System.Data;


namespace LSMES_5ANEW_PLUS.Business
{
    public class Person
    {
        private string mEmployee;
        private string mEmployeeNo;
        private string mSex;
        private string mShift;
        private string mJoinTime;
        private string mDepartment;
        private string mState;
        public string Empolyee
        {
            set
            {
                mEmployee = value;
            }
            get
            {
                return mEmployee;
            }
        }
        public string EmployeeNo
        {
            set
            {
                mEmployeeNo = value;
            }
            get
            {
                return mEmployeeNo;
            }
        }
        public string Sex
        {
            set
            {
                mSex = value;
            }
            get
            {
                return mSex;
            }
        }
        public string Shift
        {
            set
            {
                mShift = value;
            }
            get
            {
                return mShift;
            }
        }
        public string JoinTime
        {
            set
            {
                mJoinTime = value;
            }
            get
            {
                return mJoinTime;
            }
        }
        public string Department
        {
            set
            {
                mDepartment = value;
            }
            get
            {
                return mDepartment;
            }
        }
        public string State
        {
            set
            {
                State = value;
            }
            get
            {
                return State;
            }
        }
    }
    public class Authentication
    {
        private string mUID;
        private string mPWD;
        private Person mPserson;
        public Authentication(ref Person person)
        {
            mPserson = person;
        }
        public string Login(string uid, string pwd)
        {
            try
            {
                if (string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(pwd))
                {
                    throw new Exception("用户名或密码为空！");
                }
                else
                {
                    string sqlStr = "select b.employeeno,b.employeename,b.sex,b.shift,b.jointime,b.state,a.departmentname From m_department a right join (select employeeno,employeename,sex,shift,jointime,state,departmentid From m_employee where userid='" + uid + "' and password='" + pwd + "') b on a.departmentid=b.departmentid";
                    using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sqlStr, conn);
                        DataTable dt=new DataTable();
                        dt.Load(comm.ExecuteReader());
                        foreach (DataRow dr in dt.Rows)
                        {
                            mPserson.EmployeeNo = dr["employeeno"].ToString();
                            mPserson.Empolyee = dr["employeename"].ToString();
                            mPserson.Sex = dr["sex"].ToString();
                            mPserson.Shift = dr["shift"].ToString();
                            mPserson.JoinTime = dr["jointime"].ToString();
                            mPserson.Department = dr["departmentname"].ToString();
                        }
                    }
                    return mPserson.EmployeeNo + "|" + mPserson.Empolyee + "|" + mPserson.Sex + "|" + mPserson.Shift + "|" + mPserson.JoinTime + "|" + mPserson.Department;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
    }
}