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
namespace Siemens.Simatic.Web.PortalApi.Controllers.PM
{
    [RoutePrefix("api/CVRule")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_PM_BARCODE_RULE_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CV_PM_BARCODE_RULE_Controller));
        ICV_PM_BARCODE_RULEBO CV_PM_BARCODE_RULEBOBO =ObjectContainer.BuildUp<ICV_PM_BARCODE_RULEBO>();
        IPM_BARCODE_RULEBO PM_BARCODE_RULEBOBO = ObjectContainer.BuildUp<IPM_BARCODE_RULEBO>();
        IPM_BARCODE_RULE_DETAILSBO PM_BARCODE_RULE_DETAILSBO = ObjectContainer.BuildUp<IPM_BARCODE_RULE_DETAILSBO>();
        ICV_PM_BARCODE_RULE_DETAILS_TYPEBO CV_PM_BARCODE_RULE_DETAILS_TYPEBO = ObjectContainer.BuildUp<ICV_PM_BARCODE_RULE_DETAILS_TYPEBO>();
        ICV_PM_BARCODE_RULE_TYPEBO CV_PM_BARCODE_RULE_TYPEBO = ObjectContainer.BuildUp<ICV_PM_BARCODE_RULE_TYPEBO>();
        #endregion

        #region Public Methods  RULE

        /// <summary>
        /// 查询所有规则-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns> 、
        [HttpGet]
        [Route("GetRuleAll")]
        public IList<CV_PM_BARCODE_RULE> GetRuleAll()
        {

            IList<CV_PM_BARCODE_RULE> list = new List<CV_PM_BARCODE_RULE>();
            list = CV_PM_BARCODE_RULEBOBO.GetAll();
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
        public IList<CV_PM_BARCODE_RULE> GetRuleEntities(CV_PM_BARCODE_RULE_QueryParam workshop) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_PM_BARCODE_RULE> list = new List<CV_PM_BARCODE_RULE>();
            list = CV_PM_BARCODE_RULEBOBO.GetEntities(workshop);
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

        #region Public Methods  Details

        /// <summary>
        /// 查询所有规则-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [HttpPost]
        [Route("GetByRule")]
        public IList<CV_PM_BARCODE_RULE_DETAILS_TYPE> GetByRule(int rulePK)
        {

            IList<CV_PM_BARCODE_RULE_DETAILS_TYPE> list = new List<CV_PM_BARCODE_RULE_DETAILS_TYPE>();
            list = CV_PM_BARCODE_RULE_DETAILS_TYPEBO.GetByRule(rulePK);
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
        public HttpResponseMessage AddRuleDetails(PM_BARCODE_RULE_DETAILS Rule)
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

        #region Public Methods Type
        [HttpGet]
        [Route("GetTypeAll")]
        public IList<CV_PM_BARCODE_RULE_TYPE> GetTypeAll()
        {

            IList<CV_PM_BARCODE_RULE_TYPE> list = new List<CV_PM_BARCODE_RULE_TYPE>();
            list = CV_PM_BARCODE_RULE_TYPEBO.GetAll();
            log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        #endregion
    }
}