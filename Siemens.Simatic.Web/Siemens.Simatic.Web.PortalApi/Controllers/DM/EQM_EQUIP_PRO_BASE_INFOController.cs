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
    [RoutePrefix("api/equipprobase")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_PRO_BASE_INFOController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_PRO_BASE_INFOController));


        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_PRO_BASE_INFOBO BaseBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_PRO_BASE_INFOBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_PRO_BASE_INFO> GetClasss()
        {
            IList<EQM_EQUIP_PRO_BASE_INFO> list = new List<EQM_EQUIP_PRO_BASE_INFO>();
            list = BaseBO.GetAll();
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
        public IList<EQM_EQUIP_PRO_BASE_INFO> GetClassByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_PRO_BASE_INFO> list = new List<EQM_EQUIP_PRO_BASE_INFO>();
            list = BaseBO.GetClassByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }

        //获得项目
        [HttpPost]
        [Route("filterProBasePage")]
        public EQM_Page_Return filterProBasePage(EQM_EQUIP_PRO_BASE_INFO_QueryParam param)
        {
            return BaseBO.GetEntitiesByParam(param);
        }


        [HttpPost]
        [Route("GetEntities")]
        public IList<EQM_EQUIP_PRO_BASE_INFO> GetEntities(EQM_EQUIP_PRO_BASE_INFO_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            IList<EQM_EQUIP_PRO_BASE_INFO> list = new List<EQM_EQUIP_PRO_BASE_INFO>();
            if (qp != null)
            {
                list = BaseBO.GetEntities(qp);
            }
            return list;
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddBase")]
        public HttpResponseMessage AddUser(EQM_EQUIP_PRO_BASE_INFO Base)
        {

            EQM_EQUIP_PRO_BASE_INFO newBase = this.BaseBO.Insert(Base);
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
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateBase")]
        public HttpResponseMessage UpdateUser(EQM_EQUIP_PRO_BASE_INFO user)
        {
            try
            {
                BaseBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveBase")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                BaseBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
        //获取项目类型
        [HttpGet]
        [Route("getBase")]
        public DataTable getBase(EQM_EQUIP_PRO_BASE_INFO tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct service_type 
                              from EQM_EQUIP_PRO_BASE_INFO 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        //下拉框值改变调用方法查询
        [HttpPost]
        [Route("Getpartdata")]
        public IList<EQM_EQUIP_PRO_BASE_INFO> Getpartdata(EQM_EQUIP_PRO_BASE_INFO_QueryParam qp)
        {
            IList<EQM_EQUIP_PRO_BASE_INFO> entities = BaseBO.GetEntities(qp);
            return entities;
        }
        [HttpPost]
        [Route("AddDate")]
        public HttpResponseMessage AddDate(EQM_EQUIP_PRO_BASE_INFO qp)
        {
            try
            {
                BaseBO.Insert(qp);
                return Request.CreateResponse(HttpStatusCode.OK, "添加成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "添加出错:" + ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateDate")]
        public HttpResponseMessage UpdateDate(EQM_EQUIP_PRO_BASE_INFO qp)
        {
            try
            {
                qp.UpdateTime = SSGlobalConfig.Now;
                BaseBO.UpdateSome(qp);
                return Request.CreateResponse(HttpStatusCode.OK, "修改成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "修改出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveDate")]
        public HttpResponseMessage RemoveDate(string KId)
        {
            try
            {
                BaseBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }
        [HttpPost]
        [Route("GetCount")]
        public DataTable getcount(EQM_EQUIP_PRO_BASE_INFO tp)
        {

            DataTable list = null;

            string Sql = @" select count(*) count
                              from EQM_EQUIP_PRO_BASE_INFO 
                             where service_type = '" + tp.service_type + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateBase")]
        public HttpResponseMessage UpdateBase(EQM_EQUIP_PRO_BASE_INFO item)
        {
            try
            {
                BaseBO.UpdateSome(item);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

    }
}
