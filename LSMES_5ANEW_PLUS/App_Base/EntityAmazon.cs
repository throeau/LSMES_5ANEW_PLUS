using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LSMES_5ANEW_PLUS.App_Base
{
    public class EntityAmazon
    {

    }
    public class ResultAmazon
    {
        public string RESULT { set; get; }
        public string MSG { set; get; }
        public string CREATED_DATE_TIME
        {
            get
            {
                return DateTime.Now.ToString();
            }
        }

    }
    /// <summary>
    /// Amazon 产品参数值
    /// </summary>
    public class EntityAmazonBatteryParameter
    {
        public string Serial { set; get; }
        public string Parameter { set; get; }
        public string Value { set; get; }
        public string DateTime { set; get; }
    }
    public class EntityAmazonConfig
    {
        public string handle { set; get; }
        public string item_no { set; get; }
        public string factory_id { set; get; }
        public string supplier { set; get; }
        public string project { set; get; }
        public string phase { set; get; }
        public string ptype { set; get; }
        public string state { set; get; }
        public string comments { set; get; }
        public string created_date_time { set; get; } 
    }
    public class EntityAmazonPerformanceCell
    {
        public string handle_config { get; set; }
        public string cell_sn { get; set; }
        public string cell_supplier { get; set; }
        public string cell_phase { get; set; }
        public string cell_model { get; set; }
        public string cell_capacity { get; set; }
        public string cell_capacity_time { get; set; }
        public string cell_ocv1 { get; set; }
        public string cell_ocv1_time { get; set; }
        public string cell_acr1 { get; set; }
        public string cell_acr1_time { get; set; }
        public string cell_ocv2 { get; set; }
        public string cell_ocv2_time { get; set; }
        public string cell_acr2 { get; set; }
        public string cell_acr2_time { get; set; }
        public string cell_k_value { get; set; }
        public string cell_thickness { get; set; }
        public string cell_width { get; set; }
        public string cell_length { get; set; }
        public string cell_weight { get; set; }
        public string cell_shipping_ocv { get; set; }
        public string cell_shipping_acr { get; set; }
        public string cell_shipping_time { get; set; }
        public string cell_carton_no { get; set; }
        public string cell_pallet_no { get; set; }
        public string cell_lot_no { get; set; }
        public string cell_shipping_no { get; set; }
        public string cell_shipping_date { get; set; }
        public string state { get; set; }
    }
    public class AmazonKazamStatistics
    {
        public string project { get; set; }
        public string item_no { get; set; }
        public string phase { get; set; }
        public string type { get; set; }
        public string state { get; set; }
        public string total { get; set; }
        public string ready { get; set; }
        public string pallet_no { get; set; }
    }
    public class AmazonBarcode
    {
        public string barcode { get; set; }
    }
    public class AmazonParameter
    {
        public string ITEM_NO { get; set; }
        public string PROJECT { get; set; }
        public string ITEM { get; set; }
        public string LSL { get; set; }
        public string USL { get; set; }
        public string UNIT { get; set; }
        public string ISEMPTY { get; set; }
    }
}