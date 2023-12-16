using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener_Pack
{
    class Entity
    {
        public string RESULT { get; set; }
        public string MESSAGE { get; set; }
        public string REQ_ID { get; set; }
        public string CARRIER_BATCH_NO { get; set; }
        public List<CellSn> SN_LIST { get; set; }
    }
    class CellSn
    {
        public string SN { get; set; }
    }
    class LotEntity
    {
        public string MESSAGE_ID { get; set; }
        public string REQ_ID { get; set; }
        public string SITE { get; set; }
        public string RESOURCE { get; set; }
        public string LINE_ID { get; set; }
        public string STATION { get; set; }
        public string OPERATION { get; set; }
        public string SEND_TIME { get; set; }
        public string LOT_NO { get; set; }
    }
}
