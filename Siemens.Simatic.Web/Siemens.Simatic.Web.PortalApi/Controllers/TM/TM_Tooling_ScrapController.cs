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
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/TM_Tooling_Scrap")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TM_Tooling_ScrapController : ApiController
    {
        private ITM_TOOLING_PARAMETERBO ParameterBO = ObjectContainer.BuildUp<ITM_TOOLING_PARAMETERBO>();
        private ITM_TOOLING_SCRAPBO scrapBO = ObjectContainer.BuildUp<ITM_TOOLING_SCRAPBO>();
        private IPOM_ORDER_EXTBO pOM_ORDER_EXT = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();

        [HttpPost]
        [Route("getScrapPage")]
        public QM_Page_Return getScrapPage(TM_TOOLING_SCRAP_QueryParam param)
        {
            return scrapBO.GetTooling_SCRAPByQueryParam(param);
        }
      
        [HttpPost]
        [Route("createScrap")]
        public string createScrap(TM_TOOLING_SCRAP param)
        {
            try
            {
                TM_TOOLING_PARAMETER entiy = ParameterBO.GetEntity(param.ItemID);
                entiy.Status = "报废";
                ParameterBO.Update(entiy);

                DateTime now = SSGlobalConfig.Now;
                param.CreateTime = now;
                param.UpdateTime = now;
                scrapBO.Insert(param);
                return "创建成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}