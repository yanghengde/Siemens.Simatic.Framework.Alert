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
using System.Transactions;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/equiprelate")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_RELATEController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_RELATEController));

        //private IEQM_EQUIP_RELATEBO eQM_EQUIP_RELATEBO = ObjectContainer.BuildUp<IEQM_EQUIP_RELATEBO>();
        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_RELATEBO relateBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_RELATEBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_RELATE> GetRelates()
        {
            IList<EQM_EQUIP_RELATE> list = new List<EQM_EQUIP_RELATE>();
            list = relateBO.GetAll();
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
        public IList<EQM_EQUIP_RELATE> GetRelateByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_RELATE> list = new List<EQM_EQUIP_RELATE>();
            list = relateBO.GetRelateByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }


        [HttpPost]
        [Route("GetEntities")]
        public IList<EQM_EQUIP_RELATE> GetEntities(EQM_EQUIP_RELATE_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            IList<EQM_EQUIP_RELATE> list = new List<EQM_EQUIP_RELATE>();
            if (qp != null)
            {
                list = relateBO.GetEntities(qp);
            }
            return list;
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRelate")]
        public HttpResponseMessage AddUser(IList<EQM_EQUIP_RELATE> relate)
        {
                                       //获得
            EQM_EQUIP_RELATE newRelate = null;
            using (TransactionScope ts = new TransactionScope())
            {
                
                for (int i = 0; i < relate.Count; i++)
                {
                    newRelate = this.relateBO.Insert(relate[i]);
                }
                ts.Complete();
            }
            if (newRelate != null)
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
        [Route("UpdateRelate")]
        public HttpResponseMessage UpdateUser(EQM_EQUIP_RELATE user)
        {
            try
            {
                relateBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveRelate")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                relateBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "移除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
        //获取大类下拉框
        [HttpGet]
        [Route("getRelate")]
        public DataTable getRelate(EQM_EQUIP_RELATE tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct Equiprelate 
                              from EQM_EQUIP_RELATE 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        //下拉框值改变调用方法查询
        [HttpPost]
        [Route("Getpartdata")]
        public IList<EQM_EQUIP_RELATE> Getpartdata(EQM_EQUIP_RELATE_QueryParam param)
        {
            //EQM_EQUIP_RELATE
//            DataTable list = null;

//            string Sql = @" select *
//                              from EQM_EQUIP_RELATE 
//                             where 1=1 and EquipTypeID = N'" + value + "'";
//            list = BSCBO.GetDataTableBySql(Sql);
//            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
//            return list;
            IList<EQM_EQUIP_RELATE> entities = relateBO.GetEntities(param);
            return entities;

        }

        [HttpPost]
        [Route("GetCount")]
        public DataTable getcount(EQM_EQUIP_RELATE_QueryParam qp)
        {
            DataTable list = null;

            string Sql = @" select count(*)
                              from EQM_EQUIP_RELATE 
                             where 1=1 
                               and EquipTypeID = N'" + qp.EquipTypeID + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

    }
}
