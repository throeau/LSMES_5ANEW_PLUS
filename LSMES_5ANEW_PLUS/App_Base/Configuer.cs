using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Configuration;

namespace LSMES_5ANEW_PLUS
{
    public class Configuer
    {
        public static string ConnectionStringByLSMES_5ANEW
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string ConnectionStringByLSMES_5ANEW_PLUS
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["LSMES_5ANEW_PLUS"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string ConnectionStringByLSMES_PACK
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["LSMES_PACK"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string ConnectionStringBySyncRemote
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string ConnectionStringBySAP
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string ConnectionStringBySAP_ME
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["SAP_ME"].ConnectionString;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }

    }
}