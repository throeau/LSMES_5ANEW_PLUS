using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Authentication;
using System.Web.Security;



namespace LSMES_5ANEW_PLUS.App_Base
{
    /// <summary>
    /// 回传管理：任务中唯一标识
    /// </summary>
    public class TaskSFC
    {
        private string created_date_time = null;
        public string HANDLE { set; get; }
        public string HANDLE_TASK { set; get; }
        public string SFC { set; get; }
        public string ITEM_NO { set; get; }
        public string CATEGORY { set; get; }
        public int QTY { set; get; }
        public string CUSTOMER__ID { set; get; } // SAP ME 中发货客户代码
        public string CUSTOMER { set; get; } // SAP ME 中发货客户
        public string STATE { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME
        {
            set
            {
                created_date_time = value;
            }
            get
            {
                return string.IsNullOrEmpty(created_date_time) ? DateTime.Now.ToString("s") : created_date_time;
            }
        }
    }
    /// <summary>
    /// 回传管理：数据回传总任务
    /// </summary>
    public class Tasks
    {
        private string created_date_time = null;
        private string updated_date_time = null;
        public string SN { set; get; }
        public string HANDLE { set; get; }
        public string HANDLE_CUSTOMER { set; get; }
        public string HANDLE_SCHEDULE_GROUP { set; get; }
        public string HANDLE_SYNC { set; get; }
        public string CUSTOMER { set; get; }
        public string ITEM_NO { set; get; }
        public string STATE { set; get; }
        public int QTY_TOTAL { set; get; }
        public List<TaskSFC> SFC_LIST = new List<TaskSFC>();
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME
        {
            set
            {
                created_date_time = value;
            }
            get
            {
                return string.IsNullOrEmpty(created_date_time) ? DateTime.Now.ToString("s") : created_date_time;
            }
        }
        public string UPDATED_USER { set; get; }
        public string UPDATED_DATE_TIME
        {
            set
            {
                updated_date_time = null;
            }
            get
            {
                return string.IsNullOrEmpty(updated_date_time) ? DateTime.Now.ToString("s") : updated_date_time;
            }
        }
    }
    public class Tray
    {
        public string ITEM_NO { set; get; }
        public string CUSTOMER_ID { set; get; }
        public string CUSTOMER { set; get; }
        public string TRAY_NO{ set; get; }
        public string QTY { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    abstract public class SyncTask
    {
        abstract public string Handle { set; get; }
        abstract public string CODE { set; get; }
        abstract public string APPID { set; get; }
        abstract public string SECRET { set; get; }
        abstract public string URI { set; get; }
        abstract public string TIMESTAMP { set; get; }
        abstract public string SIGN { set; get; }
        abstract public string VERSION { set; get; }
        abstract public DateTime CREATED_DATE_TIME { set; get; }
        abstract public string STATE { set; get; }
        abstract public int COUNT { set; get; }
        abstract public string ID { set; get; }
        abstract public string TOKEN { set; get; }
    }
    public class SMPTask: SyncTask
    {
        private DateTime mDt;
        private string mHandle;
        private string mCode;
        private string mAppid;
        private string mSecret;
        private string mURI;
        private string mSign;
        private string mVersion;
        private string mTimeStamp;
        private string mState;
        private int mCount;
        private string mID;
        private string mToken;

        public SMPTask()
        {
            mDt = DateTime.Now;
        }
        override public string Handle
        {
            set
            {
                mHandle = value;
            }
            get
            {
                return mHandle;
            }
        }
        override public string CODE
        {
            set
            {
                mCode = value;
            }
            get
            {
                return mCode;
            }
        }
        override public string APPID
        {
            set
            {
                mAppid = value;
            }
            get
            {
                return mAppid;
            }
        }
        override public string SECRET
        {
            set
            {
                mSecret = value;
            }
            get
            {
                return mSecret;
            }
        }
        override public string URI
        {
            set
            {
                mURI = value;
            }
            get
            {
                return mURI;
            }
        }
        override public string TIMESTAMP
        {
            set
            {
                mTimeStamp = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mTimeStamp))
                {
                    mTimeStamp = ((mDt.Ticks - 621355968000000000) / 10000000).ToString();
                }
                return mTimeStamp;
            }
        }
        override public string SIGN
        {
            set
            {
                mSign = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mSign))
                {
                    string secString = mSecret.Substring(0, mSecret.Length - 1);
                    string signString = string.Format("appid={0}&timestamp={1}&{2}^_^", mAppid, mTimeStamp, secString);
                    //MD5加密
                    signString = FormsAuthentication.HashPasswordForStoringInConfigFile(signString, "MD5").ToLower();
                    //MD5第二次加密
                    mSign = FormsAuthentication.HashPasswordForStoringInConfigFile(signString, "MD5").ToLower();
                }
                return mSign;
            }
        }
        override public string VERSION
        {
            set
            {
                mVersion = value;
            }
            get
            {
                return mVersion;
            }
        }
        override public DateTime CREATED_DATE_TIME
        {
            set
            {
                mDt = value;
            }
            get
            {
                return mDt;
            }
        }
        public override string STATE
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }
        public override int COUNT
        {
            get
            {
                return mCount;
            }

            set
            {
                mCount = value;
            }
        }
        public override string ID
        {
            get
            {
                return mID;
            }

            set
            {
                mToken = value;
            }
        }
        public override string TOKEN
        {
            get
            {
                return mToken;
            }

            set
            {
                mToken = value;
            }
        }
    }
    public class ZebraTask : SyncTask
    {
        private DateTime mDt;
        private string mHandle;
        private string mCode;
        private string mAppid;
        private string mSecret;
        private string mURI;
        private string mSign;
        private string mVersion;
        private string mTimeStamp;
        private string mState;
        private int mCount;
        private string mID;
        private string mToken;

        public ZebraTask()
        {
            mDt = DateTime.Now;
        }
        override public string Handle
        {
            set
            {
                mHandle = value;
            }
            get
            {
                return mHandle;
            }
        }
        override public string CODE
        {
            set
            {
                mCode = value;
            }
            get
            {
                return mCode;
            }
        }
        override public string APPID
        {
            set
            {
                mAppid = value;
            }
            get
            {
                return mAppid;
            }
        }
        override public string SECRET
        {
            set
            {
                mSecret = value;
            }
            get
            {
                return mSecret;
            }
        }
        override public string URI
        {
            set
            {
                mURI = value;
            }
            get
            {
                return mURI;
            }
        }
        override public string TIMESTAMP
        {
            set
            {
                mTimeStamp = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mTimeStamp))
                {
                    mTimeStamp = ((mDt.Ticks - 621355968000000000) / 10000000).ToString();
                }
                return mTimeStamp;
            }
        }
        override public string SIGN
        {
            set
            {
                mSign = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mSign))
                {
                    string secString = mSecret.Substring(0, mSecret.Length - 1);
                    string signString = string.Format("appid={0}&timestamp={1}&{2}^_^", mAppid, mTimeStamp, secString);
                    //MD5加密
                    signString = FormsAuthentication.HashPasswordForStoringInConfigFile(signString, "MD5").ToLower();
                    //MD5第二次加密
                    mSign = FormsAuthentication.HashPasswordForStoringInConfigFile(signString, "MD5").ToLower();
                }
                return mSign;
            }
        }
        override public string VERSION
        {
            set
            {
                mVersion = value;
            }
            get
            {
                return mVersion;
            }
        }
        override public DateTime CREATED_DATE_TIME
        {
            set
            {
                mDt = value;
            }
            get
            {
                return mDt;
            }
        }
        override public string STATE
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }
        override public int COUNT
        {
            get
            {
                return mCount;
            }

            set
            {
                mCount = value;
            }
        }
        override public string ID
        {
            get
            {
                return mID;
            }
            set
            {
                mID = value;
            }
        }
        override public string TOKEN
        {
            get
            {
                return mToken;
            }
            set
            {
                mToken = value;
            }
        }
    }
    public class HWTask : SyncTask
    {
        private DateTime mDt;
        private string mHandle;
        private string mCode;
        private string mAppid;
        private string mSecret;
        private string mURI;
        private string mSign;
        private string mVersion;
        private string mTimeStamp;
        private string mState;
        private int mCount;
        private string mID;
        private string mToken;

        public HWTask()
        {
            mDt = DateTime.Now;
        }
        override public string Handle
        {
            set
            {
                mHandle = value;
            }
            get
            {
                return mHandle;
            }
        }
        override public string CODE
        {
            set
            {
                mCode = value;
            }
            get
            {
                return mCode;
            }
        }
        override public string APPID
        {
            set
            {
                mAppid = value;
            }
            get
            {
                return mAppid;
            }
        }
        override public string SECRET
        {
            set
            {
                mSecret = value;
            }
            get
            {
                return mSecret;
            }
        }
        override public string URI
        {
            set
            {
                mURI = value;
            }
            get
            {
                return mURI;
            }
        }
        override public string TIMESTAMP
        {
            set
            {
                mTimeStamp = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mTimeStamp))
                {
                    mTimeStamp = ((mDt.Ticks - 621355968000000000) / 10000000).ToString();
                }
                return mTimeStamp;
            }
        }
        override public string SIGN
        {
            set
            {
                mSign = value;
            }
            get
            {
                return mSign;
            }
        }
        override public string VERSION
        {
            set
            {
                mVersion = value;
            }
            get
            {
                return mVersion;
            }
        }
        override public DateTime CREATED_DATE_TIME
        {
            set
            {
                mDt = value;
            }
            get
            {
                return mDt;
            }
        }
        public override string STATE
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }
        public override int COUNT
        {
            get
            {
                return mCount;
            }

            set
            {
                mCount = value;
            }
        }
        public override string ID
        {
            get
            {
                return mID;
            }

            set
            {
                mID = value;
            }
        }
        public override string TOKEN
        {
            get
            {
                return mToken;
            }

            set
            {
                mToken = value;
            }
        }
    }
    public class DesayTask
    {
        private DateTime mDt;
        private string mHandle;
        private string mHandleConfig;
        private string mHadnleProductConfig;
        private string mBomno;
        private string mAppid;
        private string mAppkey;
        private string mURI_Login;
        private string mURI_Logout;
        private string mURI_Post;
        private string mTimeStamp;
        private string mState;
        private int mCount;
        private string mTicket;
        private string mModule;
        private int mExpress;
        private string mCustomer;
        private string mVersion;
        //private string mCreated_Date_Time;
        //private string mNumSuccess;
        //private string mNumFail;
        public DateTime CREATED_DATE_TIME
        {
            set
            {
                mDt = value;
            }
            get
            {
                return mDt;
            }
        }
        public string HANDLE
        {
            get
            {
                return mHandle;
            }
            set
            {
                mHandle = value;
            }
        }
        public string HANDLE_CONFIG
        {
            set
            {
                mHandleConfig = value;
            }
            get
            {
                return mHandleConfig;
            }
        }
        public string HANDLE_PRODUCT_CONFIG
        {
            set
            {
                mHadnleProductConfig = value;
            }
            get
            {
                return mHadnleProductConfig;
            }
        }
        public string APPID
        {
            set
            {
                mAppid = value;
            }
            get
            {
                return mAppid;
            }
        }
        public string APPKEY
        {
            set
            {
                mAppkey = value;
            }
            get
            {
                return mAppkey;
            }
        }
        public string URI_LOGIN
        {
            set
            {
                mURI_Login = value;
            }
            get
            {
                return mURI_Login;
            }
        }
        public string URI_LOGOUT
        {
            set
            {
                mURI_Logout = value;
            }
            get
            {
                return mURI_Logout;
            }
        }
        public string URI_POST
        {
            set
            {
                mURI_Post = value;
            }
            get
            {
                return mURI_Post;
            }
        }
        public string TIMESTAMP
        {
            set
            {
                mTimeStamp = value;
            }
            get
            {
                if (string.IsNullOrEmpty(mTimeStamp))
                {
                    mTimeStamp = ((mDt.Ticks - 621355968000000000) / 10000000).ToString();
                }
                return mTimeStamp;
            }
        }
        public string STATE
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }
        public int COUNT
        {
            get
            {
                return mCount;
            }

            set
            {
                mCount = value;
            }
        }
        public string TICKET
        {
            set
            {
                mTicket = value;
            }
            get
            {
                return mTicket;
            }
        }
        public string BOMNO
        {
            set
            {
                mBomno = value;
            }
            get
            {
                return mBomno;
            }
        }
        public string MODULE
        {
            set
            {
                mModule = value;
            }
            get
            {
                return mModule;
            }
        }
        public int EXPRESS
        {
            set
            {
                mExpress = value;
            }
            get
            {
                return mExpress;
            }
        }
        public string CUSTOMER
        {
            set
            {
                mCustomer = value;
            }
            get
            {
                return mCustomer;
            }
        }
        public string VERSION
        {
            set
            {
                mVersion = value;
            }
            get
            {
                return mVersion;
            }
        }
        public string HANDLE_SEND_INFO { set; get; }
    }
    public class SunwodaTask
    {
        public string HANDLE { get; set; }
        public string HANDLE_CONFIG { get; set; }
        public string DATATYPE { get; set; }
        public string SUPPLIERID { get; set; }
        public string APPKEY { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string SUPPLIERCODE { get; set; }
        public string PASSWORD { get; set; }
        public string PRODUCTLINE { get; set; }
        public string URI { get; set; }
        public string STATE { get; set; }
        public string CREATED_DATE_TIME { get; set; }
        public string EXPRESS_QTY { get; set; }
        public string TOKEN { get; set; }
        public string URI_TOKEN { get; set; }
        public string TOKEN_PARA { get; set; }
    }
    public class SCUDTask
    {
        public string HANDLE { get; set; }
        public string HANDLE_CONFIG { get; set; }
        public string BOMNO { get; set; }
        public string ITEM_NO { get; set; }
        public int COMPLETED { get; set; }
        public int TOTAL { get; set; }
        public string APPID { get; set; }
        public string PWD { get; set; }
        public string DBTYPE { get; set; }
        public string TYPE { get; set; }
        public string URI { get; set; }
        public string DEPT { get; set; }
        public string STATE { get; set; }
        public string CO_ITEM_CODE { get; set; }
        public string CO_ITEM_NAME { get; set; }
        public string CO_ITEM_SPEC { get; set; }
        public string CREATED_DATE_TIME { get; set; }
    }
    public class TWSTask
    {
        public string HANDLE { get; set; }
        public string HANDLE_CONFIG { get; set; }
        public string URI { get; set; }
        public string TOKEN { get; set; }
        public string ITEM { get; set; }
        public string BOM { get; set; }
        public string STATE { get; set; }
    }
    public class AnkerTask
    {
        /// <summary>
        /// Anker 任务 handle
        /// </summary>
        public string HANDLE { get; set; }
        /// <summary>
        /// 任务管理器 handle
        /// </summary>
        public string HANDLE_TASK { get; set; }
        /// <summary>
        /// Anker 配置
        /// </summary>
        public string HANDLE_CONFIG { get; set; }
        /// <summary>
        /// 回传地址
        /// </summary>
        public string URI { get; set; }
        public string APPID { get; set; }
        public string SECRET { get; set; }
        public string TYPE { get; set; }
        public string TOKEN { get; set; }
        public int COMPLETED { get; set; }
        public int TOTAL { get; set; }
        public string STATE { get; set; }
        public string URI_TOKEN { get; set; }
    }
}