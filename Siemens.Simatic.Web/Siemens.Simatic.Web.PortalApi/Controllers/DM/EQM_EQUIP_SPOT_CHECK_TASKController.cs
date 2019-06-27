using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.DM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.DM.Common;
using System.Data;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/spotchecktask")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_SPOT_CHECK_TASKController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_SPOT_CHECK_TASKController));

        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPOT_CHECK_PROBO PROBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPOT_CHECK_PROBO>();
        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPOT_CHECK_TASKBO CheckBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPOT_CHECK_TASKBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion


        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_SPOT_CHECK_TASK> GetClasss()
        {
            IList<EQM_EQUIP_SPOT_CHECK_TASK> list = new List<EQM_EQUIP_SPOT_CHECK_TASK>();
            list = CheckBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpGet]
        [Route("Login")]
        public string Login()
        {
            string str = "登录成功";
            return str;
        }


        [Route("paged"), HttpGet]
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>        
        public IList<EQM_EQUIP_SPOT_CHECK_TASK> GetClassByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_SPOT_CHECK_TASK> list = new List<EQM_EQUIP_SPOT_CHECK_TASK>();
            list = CheckBO.GetCheckByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }


        //[HttpPost]
        //[Route("GetEntities")]
        //public IList<EQM_EQUIP_SPOT_CHECK_TASK> GetEntities(EQM_EQUIP_SPOT_CHECK_TASK_QueryParam qp)
        ////传入的参数是对象，用Post，不能用Get
        //{
        //    IList<EQM_EQUIP_SPOT_CHECK_TASK> list = new List<EQM_EQUIP_SPOT_CHECK_TASK>();
        //    if (qp != null)
        //    {
        //        list = CheckBO.GetEntities(qp);
        //    }
        //    return list;
        //}
        [HttpGet]
        [Route("GetCheckPRO")]
        public IList<EQM_EQUIP_SPOT_CHECK_PRO> GetCheckPRO(string equipCheckNum)
        {
            return PROBO.GetByEquipCheckNum(equipCheckNum);
        }
        [HttpPost]
        [Route("UpdateCheckPRO")]
        public HttpResponseMessage UpdateCheckPRO(EQM_EQUIP_SPOT_CHECK_PRO check)
        {
            try
            {
                PROBO.UpdateSome(check);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        /// <summary>
        /// 添加检验任务
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddCheck")]
        public HttpResponseMessage AddCheck(EQM_EQUIP_SPOT_CHECK_TASK Base)
        {
            EQM_EQUIP_SPOT_CHECK_TASK newBase = this.CheckBO.Insert(Base);
            if (newBase != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        /// <summary>
        /// 更新点检项目
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateCheck")]
        public HttpResponseMessage UpdateCheck(EQM_EQUIP_SPOT_CHECK_TASK check)
        {
            try
            {
                //TimeSpan startSpan = DateTime.Parse(check.taskDate).TimeOfDay;
                if (check.taskDate == SSGlobalConfig.Now.ToString("yyyy-MM-dd"))
                {
                    check.UpdateTime = SSGlobalConfig.Now;
                    check.checkTime = SSGlobalConfig.Now.ToString();
                    CheckBO.UpdateSome(check);

                    return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该点检单已失效");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveCheck")]
        public HttpResponseMessage DeleteCheck(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                CheckBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion


        //获取设备类型
        [HttpGet]
        [Route("getType")]
        public DataTable getType(EQM_EQUIP_SPOT_CHECK_TASK tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct  
                              from EQM_EQUIP_SPOT_CHECK_TASK 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        /// <summary>
        /// 更新点检设备信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateBase")]
        public HttpResponseMessage UpdateBase(EQM_EQUIP_SPOT_CHECK_TASK item)
        {
            try
            {
                CheckBO.UpdateSome(item);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

    }
}