using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Siemens.Simatic.ALT.Common;

namespace Siemens.Simatic.ALT.Common
{
    public class CV_PM_ALT_BASE_QuaryParam : CV_PM_ALT_BASE
    {
        private string _alertTypeName;
       // private CV_PM_ALT_BASE _cV_PM_ALT_BASE;
        public string alertTypeName
        {
            get { return _alertTypeName; }
            set { _alertTypeName = value; }
        }

        //public CV_PM_ALT_BASE cV_PM_ALT_BASE
        //{
        //    get { return _cV_PM_ALT_BASE; }
        //    set { _cV_PM_ALT_BASE = value; }
        //}
    }
}
