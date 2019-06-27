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
    [RoutePrefix("api/Rule")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_BARCODE_RULE_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(PM_BARCODE_RULE_Controller));
        IPM_BARCODE_RULEBO PM_BARCODE_RULEBOBO = ObjectContainer.BuildUp<IPM_BARCODE_RULEBO>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 查询所有规则-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns> 、
        [HttpGet]
        [Route("GetRuleAll")]
        public IList<PM_BARCODE_RULE> GetRuleAll()
        {

            IList<PM_BARCODE_RULE> list = new List<PM_BARCODE_RULE>();
            list = PM_BARCODE_RULEBOBO.GetAll();
            log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetRuleEntities")]
        public IList<PM_BARCODE_RULE> GetRuleEntities(PM_BARCODE_RULE_QueryParam workshop) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BARCODE_RULE> list = new List<PM_BARCODE_RULE>();
            list = PM_BARCODE_RULEBOBO.GetEntities(workshop);
            return list;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="Rule"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRule")]
        public HttpResponseMessage AddRule(PM_BARCODE_RULE Rule)
        {
            Rule.IsActive = true;
            Rule.IsSystem = true;
            PM_BARCODE_RULE newAddRule = this.PM_BARCODE_RULEBOBO.Insert(Rule);
            if (newAddRule != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateRule")]
        public HttpResponseMessage UpdateRule(PM_BARCODE_RULE Rule)
        {
            try
            {
                PM_BARCODE_RULEBOBO.UpdateSome(Rule);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }
        #endregion
    }
}

