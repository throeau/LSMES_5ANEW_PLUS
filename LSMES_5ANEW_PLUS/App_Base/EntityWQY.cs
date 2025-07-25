using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class EntityWQY
    {
    }
    public class EntityProcess
    {
        public string HANDLE { set; get; }
        public string PROCESS { set; get; }
        public string STATE { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    public class RELATION
    {
        public string HANDLE { set; get; }
        public string HANDLE_PROCESS { set; get; }
        public string FIELD { set; get; }
        public string PARAMETER { set; get; }
        public string STATE { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    public class BATTERY
    {
        public string HANDLE { set; get; }
        public string BARCODE { set; get; }
        public string STATE { set; get; }
    }
    public class BATTERY_DATA
    {
        public string PRIMARY_KEY { set; get; }
        public string HANDLE_RELATION { set; get; }
        public string HANDLE_BARCODE { set; get; }
        public string VALUE { set; get; }
        public string STATE { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    public class BATTERY_SERIES
    {
        public string HANDLE_BARCODE { set; get; }
        public string BARCODE { set; get; }
        public List<BATTERY_DATA> DATA = new List<BATTERY_DATA>();
    }
    public class RESULT_WQY
    {
        public string FILE_NAME { set; get; }
        public string TOTAL { set; get; }
    }
    public class Batch
    {
        public string BATCH { set; get; }
        public string BATCH_NO { set; get; }
        public string ITEM_NO { set; get; }
        public string ORDER_NO { set; get; }
    }
    public class WIP
    {
        public string MATERIAL_CODE { set; get; }
        public string ORDER_NO { set; get; }
        public string BATCH { set; get; }
        public string GRADE { set; get; }
        public string GRADE_BATCH { set; get; }
        public string P_SLURRY { set; get; }
        public string N_SLURRY { set; get; }
    }
}