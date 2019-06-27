using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.Util.Utilities
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

