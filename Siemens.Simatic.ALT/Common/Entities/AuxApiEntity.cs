using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.ALT.Common
{
    [Serializable]
    public class AuxApiEntity
    {
        public AuxApiEntity()
        {
        }

        public string agentId { get; set; }
        public List<string> userIds { get; set; }
        public string content { get; set; }
    }
}
