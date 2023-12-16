using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class EntityAssemble
    {
    }
    public class ResultAssemble
    {
        public string Result { set; get; }
        public string Message { set; get; }
    }
    public class EntityHipot
    {
        public string BARCODE { set; get; }
        public double VOLTAGE { set; get; }
        public double RESISTANCE { set; get; }
        public string BOMNO { set; get; }
        public string COMMENTS { set; get; }
        public string STATE { set; get; }
        public string CREATED_DATE_TIME{set;get;}
    }
}