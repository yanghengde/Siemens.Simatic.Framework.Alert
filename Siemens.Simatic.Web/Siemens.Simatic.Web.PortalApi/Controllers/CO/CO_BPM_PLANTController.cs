using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.Common;
using Newtonsoft.Json;
using Siemens.Simatic.Web.PortalApi.Controllers.MM;

namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
{    
    [RoutePrefix("api/co")]
    [EnableCors(origins:"*",headers:"*",methods:"*")]
    public class CO_BPM_PLANTController:ApiController
    {
        MM_FEEDING_LOCATIONController mc = new MM_FEEDING_LOCATIONController();
       
       
        
        #region Private Fileds

        ILog log = LogManager.GetLogger(typeof(CO_BPM_PLANTController));
        IPM_BPM_PLANTBO bpmPlantBO = ObjectContainer.BuildUp<IPM_BPM_PLANTBO>();
        
        #endregion

        #region  Public Methods
        /// <summary>         
        /// 查询所有工厂-张婷-2017-11-22 09:26:13
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("")]
        public IList<PM_BPM_PLANT> getAllPlant() {
            IList<PM_BPM_PLANT> list = new List<PM_BPM_PLANT>();
           // MM_FEEDING_LOCATIONController.GetName();
            list = bpmPlantBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        #endregion
        
    }
}