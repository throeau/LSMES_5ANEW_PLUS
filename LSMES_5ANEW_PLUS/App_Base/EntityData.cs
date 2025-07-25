using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class ResultAvailability
    {
        public string Result { set; get; }
        public string Msg { set; get; }
    }
    public class BarcodeAvailable
    {
        public string Barcode { set; get; }
        public string Created_user { set; get; }
    }
    public class ProductionModel
    {
        public string RESRCE { set; get; }
        public string CORE_TYPE_CODE { set; get; }
        public string ITEM { set; get; }
        public string MAT_CODE { set; get; }
        public string SHOP_ORDER { set; get; }
        public string PROCESS_LOT { set; get; }
    }
}