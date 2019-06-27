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

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/DM_Device_MaintenanceOrder")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DM_Device_MaintenanceOrderController : ApiController
    {
        private IDM_Device_MaintenanceOrderBO OrderBO = ObjectContainer.BuildUp<IDM_Device_MaintenanceOrderBO>();

        private string DM_Device_MaintenanceOrder_DB = "DM_Device_MaintenanceOrder";//维修单 
            //查询条件查询
        [HttpPost]
        [Route("filterMaintenanceOrderPage")]
        public DM_Page_Return filterMaintenanceOrderPage(DM_Device_MaintenanceOrder_QueryParam queryParam)  //CustPageContent
        {
            return OrderBO.filterMaintenanceOrderPage(queryParam);
        }
    }
}