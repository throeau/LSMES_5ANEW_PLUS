using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class E6
    {
        public string BATTERYNO { get; set; }
        public string BOMNO { get; set; }
        public string E6KVALUE { get; set; }
        public string E6LEVEL { get; set; }
        public string STATUS { get; set; }
        public string E6TESTTIME2 { get; set; }
        public string E6VOLTAGE2 { get; set; }
        public string E6RESISTANCE2 { get; set; }
        public string E6DELTAV { get; set; }
        public string E6DELTAT { get; set; }
        public string E8KVALUE { get; set; }
    }
    public class E6Performance
    {
        //public IDictionary<string, IList<Data>> data { get; set; }
        public List<E6> Data { get; set; }
    }
    public class E5
    {
        public string BATTERYNO { get; set; }
        public string BOMNO { get; set; }
        public string E5LEVEL { get; set; }
        public string STATUS { get; set; }
        public string E5TESTTIME1 { get; set; }
        public string E5VOLTAGE1 { get; set; }
        public string E5RESISTANCE1 { get; set; }

    }
    public class E5Performance
    {
        //public IDictionary<string, IList<Data>> data { get; set; }
        public List<E5> Data { get; set; }
    }
    public class E3
    {
        public string BATTERYNO { get; set; }
        public string BOMNO { get; set; }
        public string E13EQUNO { get; set; }
        public string E13ENDTIME { get; set; }
        public string E13OUT_CAPACITY2 { get; set; }
        public string E13END_VOLTAGE { get; set; }
        public string E13AREANO { get; set; }
        public string E13POSNO { get; set; }
        public string E13LEVEL { get; set; }
        public string E13START_VOLTAGE { get; set; }
        public string STATUS { get; set; }

    }
    public class E3Performance
    {
        //public IDictionary<string, IList<Data>> data { get; set; }
        public List<E3> Data { get; set; }
    }
    public class E16
    {
        public string BATTERYNO { get; set; }
        public string BOMNO { get; set; }
        public string E16FLAG { get; set; }
        public string E16TESTTIME { get; set; }
        public string E16BATTERYWEIGH { get; set; }
        public string E16CHANO { get; set; }
        public string E16JYLWEIGH { get; set; }
        public string STATUS { get; set; }
    }
    public class E16Performance
    {
        //public IDictionary<string, IList<Data>> data { get; set; }
        public List<E16> Data { get; set; }
    }
}