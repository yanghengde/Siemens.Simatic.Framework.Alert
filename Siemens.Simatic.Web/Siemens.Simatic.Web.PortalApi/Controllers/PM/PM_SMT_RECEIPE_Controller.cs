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
using Siemens.Simatic.PM.Common.QueryParams;


namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/Receipe")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_SMT_RECEIPE_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(PM_SMT_RECEIPE_Controller));
        IPM_SMT_RECEIPEBO PM_SMT_RECEIPEBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPEBO>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 查询所有-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns> 
        [HttpGet]
        [Route("GetReceipeAll")]
        public IList<PM_SMT_RECEIPE> GetReceipeAll()
        {

            IList<PM_SMT_RECEIPE> list = new List<PM_SMT_RECEIPE>();
            list = PM_SMT_RECEIPEBO.GetAll();
            log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetReceipeEntities")]
        public IList<PM_SMT_RECEIPE> GetReceipeEntities(PM_SMT_RECEIPE_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_SMT_RECEIPE> list = new List<PM_SMT_RECEIPE>();
            list = PM_SMT_RECEIPEBO.GetEntities(Entitie);
            return list;
        }
        #endregion
    }
}

