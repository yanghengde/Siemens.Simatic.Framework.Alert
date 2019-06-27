using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.QM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.QM.Common;
using System.Data;
using Siemens.Simatic.PM.BusinessLogic;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/sampling")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SAMPLING_RULEController : ApiController
    {
        #region Private Fields
        ILog log = LogManager.GetLogger(typeof(SAMPLING_RULEController));
        IQM_INFRA_SAMPLING_RULEBO ruleBO = ObjectContainer.BuildUp<IQM_INFRA_SAMPLING_RULEBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        [HttpPost]
        [Route("filterSamplingRulePage")]
        //根据质检类型获得质检规则
        public QM_Page_Return filterSamplingRulePage(QM_INFRA_SAMPLING_RULE_QueryParam param)
        {
            return ruleBO.filterSamplingRulePage(param);
        }

        [HttpGet]
        [Route("GetSamplingRuleType")]
        //获得质检类型
        public DataTable GetSamplingRuleType()
        {
            //string sql = @"select [sValue] from SM_CONFIG_KEY where sOwner = 'hand' and sKey = 'SAMPLING_RULE'";
            string sql = @"select [sValue] from SM_CONFIG_KEY where sKey = 'SAMPLING_RULE'";
            return BSCBO.GetDataTableBySql(sql);
        }

        [HttpGet]
        [Route("GetLowerBound")]
        public DataTable GetLowerBound(string examType)
        //传入的参数是对象，用Post，不能用Get
        {
            DataTable list = null;
//            string sql = @"select isnull(max(UpperBound),0) as maxvalue 
//                             from dbo.QM_INFRA_SAMPLING_RULE ru 
//                            WHERE 1 = 1 and ExamType=N'" + examType + "'";
            string sql = @"select top 1 isnull(UpperBound,0) as maxvalue 
                             from dbo.QM_INFRA_SAMPLING_RULE ru 
                            WHERE 1 = 1 
                              and ExamType=N'" + examType + @"'
                              and UpperBound not in(select LowerBound 
                                                      from dbo.QM_INFRA_SAMPLING_RULE
													 WHERE 1 = 1 
													   and ExamType=N'" + examType + @"')
                             order by UpperBound";
            list = BSCBO.GetDataTableBySql(sql);

            return list;
        }

        [HttpGet]
        [Route("GetUpperBound")]
        public DataTable GetUpperBound(string examType)
        //传入的参数是对象，用Post，不能用Get
        {
            DataTable list = null;
            string sql = @"select max(LowerBound) as maxvalue 
                             from dbo.QM_INFRA_SAMPLING_RULE ru 
                            WHERE 1 = 1 
                              and ExamType=N'" + examType + @"'
                              and LowerBound not in(select UpperBound 
                                                      from dbo.QM_INFRA_SAMPLING_RULE)
                             ";
            list = BSCBO.GetDataTableBySql(sql);
            return list;
        }


        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRule")]
        public HttpResponseMessage AddUser(QM_INFRA_SAMPLING_RULE Rule)
        {
            IList<QM_INFRA_SAMPLING_RULE> list = new List<QM_INFRA_SAMPLING_RULE>();
            list = ruleBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            for (int i = 0; i < list.Count; i++)
            {
                if (Rule.ExamType == list[i].ExamType)//如果类型相同则对比
                {
                    if (Rule.LowerBound < list[i].UpperBound && Rule.LowerBound >= list[i].LowerBound)
                        //return Request.CreateResponse(HttpStatusCode.InternalServerError, "下限值输入有误！");
                        return Request.CreateResponse(HttpStatusCode.OK, "下限值输入有误！");

                    if(Rule.UpperBound<=list[i].UpperBound&&Rule.UpperBound>list[i].LowerBound)
                        //return Request.CreateResponse(HttpStatusCode.InternalServerError, "上限值输入有误！");
                        return Request.CreateResponse(HttpStatusCode.OK, "上限值输入有误！");
                }
            }

                Rule.creation_date = DateTime.Now;
            QM_INFRA_SAMPLING_RULE newRule = this.ruleBO.Insert(Rule);
            if (newRule != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateRule")]
        public HttpResponseMessage UpdateUser(QM_INFRA_SAMPLING_RULE user)
        {
            try
            {
                ruleBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveRule")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                
                ruleBO.Delete( Convert.ToInt32(KId));

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
