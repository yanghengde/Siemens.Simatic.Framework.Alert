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
    [RoutePrefix("api/TM_Tooling_Maintain")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TM_Tooling_MaintainController : ApiController
    {
        private ITM_TOOLING_PARAMETERBO ParameterBO = ObjectContainer.BuildUp<ITM_TOOLING_PARAMETERBO>();
        private ITM_TOOLING_MAINTAINBO mAINTAINBO = ObjectContainer.BuildUp<ITM_TOOLING_MAINTAINBO>();
        private IPOM_ORDER_EXTBO pOM_ORDER_EXT = ObjectContainer.BuildUp<IPOM_ORDER_EXTBO>();

        [HttpPost]
        [Route("getMaintainPage")]
        public QM_Page_Return getMaintainPage(TM_TOOLING_MAINTAIN_QueryParam param)
        {
            return mAINTAINBO.GetTooling_MaintainByQueryParam(param);
        }
        [HttpPost]
        [Route("createMaintain")]
        public string createMaintain(TM_TOOLING_MAINTAIN param)
        {
            try
            {
                TM_TOOLING_PARAMETER entiy = ParameterBO.GetEntity(param.ItemID);
                entiy.Status = "保养";
                ParameterBO.Update(entiy);

                DateTime now = SSGlobalConfig.Now;
                param.CreateTime = now;
                param.UpdateTime = now;
                mAINTAINBO.Insert(param);
                return "创建成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
      
    }
}