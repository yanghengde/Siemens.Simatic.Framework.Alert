using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.ALT.Common
{
    public class CV_PM_ALT_LOGQueryParam : CV_PM_ALT_LOG
    {
        private DateTime? _respondedOnBegin;
        public DateTime? Z_RespondedOnBegin
        {
            get { return _respondedOnBegin; }
            set { _respondedOnBegin = value; this.SetNotDefaultValue("Z_RespondedOnBegin"); }
        }

        private DateTime? _respondedOnEnd;
        public DateTime? Z_RespondedOnEnd
        {
            get { return _respondedOnEnd; }
            set { _respondedOnEnd = value; this.SetNotDefaultValue("Z_RespondedOnEnd"); }
        }
    }
}
