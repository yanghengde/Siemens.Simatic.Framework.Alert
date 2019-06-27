using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Infrustrues
{
    public class QMRepairScrapRequest
    {

        public string user 
        {
            set;
            get;
        }

        public List<CV_QM_REPAIR_TO_SCRAP_INFO> scrapList
        {
            set;
            get;
        }
    }
}