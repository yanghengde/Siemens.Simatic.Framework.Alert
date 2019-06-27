using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.QM.BusinessLogic;
using Siemens.Simatic.QM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.Util.Utilities;
using System.Transactions;
using log4net;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using Newtonsoft.Json;

namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/QMShowScrap")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QMShowScrapController : ApiController
    {

        private IQM_SCRAP_TOPBO scrap_topbo = ObjectContainer.BuildUp<IQM_SCRAP_TOPBO>();//报废单表
        private ILog log = LogManager.GetLogger(typeof(QMRepairScrapController));

        [HttpPost]
        [Route("getScrapInfo")]
        public QM_Page_Return getScrapInfo(QM_SCRAP_TOP_Param param)
        {
            return scrap_topbo.GetEntitiesByQueryParam(param);
        }
    }


}