using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;
using Siemens.Simatic.QM.Common;
using Siemens.Simatic.PM.Common;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/TM_Tooling_Resource")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TM_Tooling_ResourceController : ApiController
    {
        private ITM_TOOLING_RESOURCEBO rESOURCEBO = ObjectContainer.BuildUp<ITM_TOOLING_RESOURCEBO>();


        [HttpPost]
        [Route("getResourcePage")]
        public QM_Page_Return getParameterPage(TM_TOOLING_RESOURCE_QueryParam param)
        {
            return rESOURCEBO.GetTooling_ResourceByQueryParam(param);
        }
    }
}