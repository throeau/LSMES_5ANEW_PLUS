using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync_Pack
{
    class Entity
    {
    }
    public class Configure
    {
        public string WEBSITE { get; set; }
        public string EQUIPMENT { get; set; }
        public string LINENO { get; set; }
        public string BOMNO { get; set; }
        public string ITEMNO { get; set; }
        public string CUSTOMERNO { get; set; }
        public string LABELCODE { get; set; }
        public string BOXID { get; set; }
        public string OPERATORS { get; set; }
        public string ORDERNO { get; set; }
    }
    public class EntityException
    {
        public bool IsException { get; set; }
        public string ExpMessage { get; set; }
    }
}
