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

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/equipmentspec")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_SPECController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_SPECController));


        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPECBO specBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_SPECBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_SPEC> GetSpecs()
        {
            IList<EQM_EQUIP_SPEC> list = new List<EQM_EQUIP_SPEC>();
            list = specBO.GetAll();
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
        public IList<EQM_EQUIP_SPEC> GetspecByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_SPEC> list = new List<EQM_EQUIP_SPEC>();
            list = specBO.GetSpecByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }

        //获得小类
        [HttpPost]
        [Route("filterDeviceSpecPage")]
        public EQM_Page_Return filterDeviceSpecPage(EQM_EQUIP_SPEC_QueryParam param)
        {
            return specBO.GetEntitiesByParam(param);
        }

        [HttpPost]
        [Route("GetEntities")]
        public IList<EQM_EQUIP_SPEC> GetEntities(EQM_EQUIP_SPEC_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            IList<EQM_EQUIP_SPEC> list = new List<EQM_EQUIP_SPEC>();
            if (qp != null)
            {
                list = specBO.GetEntities(qp);
            }
            return list;
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddSpec")]
        public HttpResponseMessage AddUser(EQM_EQUIP_SPEC spec)
        {

            EQM_EQUIP_SPEC newSpec = this.specBO.Insert(spec);
            if (newSpec != null)
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
        [Route("UpdateSpec")]
        public HttpResponseMessage UpdateUser(EQM_EQUIP_SPEC user)
        {
            try
            {
                specBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveSpec")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                specBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
        //获取大类下拉框
        [HttpGet]
        [Route("getSpec")]
        public DataTable getSpec(EQM_EQUIP_SPEC tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct EquipSPEC 
                              from EQM_EQUIP_SPEC 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        //下拉框值改变调用方法查询
        [HttpGet]
        [Route("Getpartdata")]
        public DataTable Getpartdata(string Class)
        {
            //EQM_EQUIP_SPEC
            DataTable list = null;

            string Sql = @" select *
                              from EQM_EQUIP_SPEC 
                             where 1=1 and EquipSPEC = N'" + Class + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
            return list;

        }

        [HttpGet]
        [Route("getcount")]
        public DataTable getcount(EQM_EQUIP_SPEC tp)
        {
            DataTable list = null;

            string Sql = @" select count(*)
                              from EQM_EQUIP_SPEC ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

    }
}
