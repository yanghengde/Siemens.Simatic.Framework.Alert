//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Siemens.Simatic.Web.PortalApi.Controllers.PM
//{
//    public class CV_PM_SMT_RECEIPE_DETAIL_Controller
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
using Siemens.Simatic.PM.Common.QueryParams;


namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/CVReceipeDetail")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_PM_SMT_RECEIPE_DETAIL_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CV_PM_SMT_RECEIPE_DETAIL_Controller));
        IPM_SMT_RECEIPE_DETAILBO receipeDeatailBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPE_DETAILBO>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 查询所有-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("GetCVReceipeDetailAll")]
        public IList<PM_SMT_RECEIPE_DETAIL> GetCVReceipeAll()
        {
            IList<PM_SMT_RECEIPE_DETAIL> list = new List<PM_SMT_RECEIPE_DETAIL>();
            list = receipeDeatailBO.GetAll();
            log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetCVReceipeDetailEntities")]
        public IList<PM_SMT_RECEIPE_DETAIL> GetReceipeEntities(PM_SMT_RECEIPE_DETAIL_QueryParam entitie) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_SMT_RECEIPE_DETAIL> list = new List<PM_SMT_RECEIPE_DETAIL>();
            list = receipeDeatailBO.GetEntities(entitie);
            return list;
        }
        #endregion
    }
}

