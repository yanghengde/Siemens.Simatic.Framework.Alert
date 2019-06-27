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
    [RoutePrefix("api/RuleDetails")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_BARCODE_RULE_DETAILS_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(PM_BARCODE_RULE_DETAILS_Controller));
        IPM_BARCODE_RULE_DETAILSBO PM_BARCODE_RULE_DETAILSBO =ObjectContainer.BuildUp<IPM_BARCODE_RULE_DETAILSBO>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 查询所有规则-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [HttpPost]
        [Route("GetByRule")]
        public IList<PM_BARCODE_RULE_DETAILS> GetByRule(int rulePK)
        {

            IList<PM_BARCODE_RULE_DETAILS> list = new List<PM_BARCODE_RULE_DETAILS>();
            list = PM_BARCODE_RULE_DETAILSBO.GetByRule(rulePK);
            log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="Rule"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRuleDetails")]
        public HttpResponseMessage AddRule(PM_BARCODE_RULE_DETAILS Rule)
        {
            //User.CreatedOn = DateTime.Now;
            PM_BARCODE_RULE_DETAILS newAddRule = this.PM_BARCODE_RULE_DETAILSBO.Insert(Rule);
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
        /// 删除
        /// </summary>
        /// <param name="detailsPK"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Remove")]
        public HttpResponseMessage Delete(string detailsPK)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                PM_BARCODE_RULE_DETAILSBO.Delete(int.Parse(detailsPK));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
    }
}

