using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.ALT.Common
{
    [Serializable]
    public class ProcModel
    {
        public ProcModel()
        {
        }

        public string DbType { get; set; }
        public bool IsOutPut { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
