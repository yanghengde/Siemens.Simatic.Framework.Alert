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
using Siemens.Simatic.Web.PortalApi.Controllers;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/inspection")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SAMPLING_PLANController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(SAMPLING_PLANController));
        IQM_INFRA_INSPECTION_PLANBO planBO = ObjectContainer.BuildUp<IQM_INFRA_INSPECTION_PLANBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion
        
        #region Public Methods

        [HttpPost]
        [Route("filterInspectionPlanPage")]
        public QM_Page_Return filterInspectionPlanPage(QM_INFRA_INSPECTION_PLAN_QueryParam param)
        {
            return planBO.filterInspectionPlanPage(param);
        }


        [HttpGet]
        [Route("GetPlant")]
        public DataTable GetPlant(QM_INFRA_INSPECTION_PLAN_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            DataTable list = null;
            string sql = @"select [PlantName] 
                             from PM_BPM_PLANT 
                            where 1=1";

            list = BSCBO.GetDataTableBySql(sql);

            return list;
        }


        [HttpGet]
        [Route("GetWorkshop")]
        public DataTable GetWorkshop(string plant)
        //传入的参数是对象，用Post，不能用Get
        {
            DataTable list = null;
            string sql = @"select PM_BPM_WORKSHOP.WorkshopName 
                             from PM_BPM_WORKSHOP , PM_BPM_PLANT 
                            where 1=1 
							 and PM_BPM_WORKSHOP.SAPPlantID = PM_BPM_PLANT.SAPPlantID and PM_BPM_PLANT.PlantName = '" + plant + "' ";
            list = BSCBO.GetDataTableBySql(sql);
            //qp.Plant
            //qp.Prod_Line
            return list;
        }

        [HttpGet]
        [Route("GetProdline")]
        public DataTable GetProdline(string Workshop)
        //传入的参数是对象，用Post，不能用Get
        {
            DataTable list = null;
            string sql = @" select m.LineName 
                              from CV_BPM_TERMINAL m 
                             where m.WorkshopName=N'" + Workshop + "'  ";
            list = BSCBO.GetDataTableBySql(sql);

            return list;
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddPlan")]
        public HttpResponseMessage AddUser(QM_INFRA_INSPECTION_PLAN Plan)
        {
            Plan.creation_date = DateTime.Now;
            QM_INFRA_INSPECTION_PLAN newPlan = this.planBO.Insert(Plan);
            if (newPlan != null)
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
        [Route("UpdatePlan")]
        public HttpResponseMessage UpdateUser(QM_INFRA_INSPECTION_PLAN user)
        {
            try
            {
                planBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemovePlan")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                
                planBO.Delete( Convert.ToInt32(KId));

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
