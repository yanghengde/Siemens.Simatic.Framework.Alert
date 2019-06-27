//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
//{
//    public class CO_BPM_WORKSHOP_LINEController
//    {
//    }
//}


using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/workshop_line")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CO_BPM_WORKSHOP_LINEController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CO_BPM_WORKSHOP_LINEController));

        //IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        IPM_BPM_WORKSHOP_LINEBO workshop_lineBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOP_LINEBO>();

        #endregion

        #region Public Methods

        [Route("")]
        public IList<PM_BPM_WORKSHOP_LINE> GetWorkshop()
        {

            IList<PM_BPM_WORKSHOP_LINE> list = new List<PM_BPM_WORKSHOP_LINE>();
            list = workshop_lineBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpPost]
        [Route("Getworkshop_line")]
        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<PM_BPM_WORKSHOP_LINE> Getworkshop_line(PM_BPM_WORKSHOP_LINE_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {
            //return JsonConvert.SerializeObject(User);


            IList<PM_BPM_WORKSHOP_LINE> list = new List<PM_BPM_WORKSHOP_LINE>();
            if (User != null)
            {
                list = workshop_lineBO.GetEntities(line);
                return list;
            }
            return list;
        }

        #endregion
    }
}
