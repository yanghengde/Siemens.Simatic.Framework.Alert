using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;
using Siemens.Simatic.Web.PortalApi.EquipRepair;
using Newtonsoft.Json;
using Siemens.Simatic.Web.PortalApi.EquipRepairResult;
using Siemens.Simatic.Util.Utilities;
using Siemens.Simatic.DM.Common.QueryParams;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/EquipmentMaintenance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EquipmentMaintenanceController : ApiController
    {
        private IDM_Equipment_MaintenanceBO EMBO = ObjectContainer.BuildUp<IDM_Equipment_MaintenanceBO>();
        private IEQM_EQUIP_CLASSBO eec = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();

        [HttpPost]
        [Route("getTwoDirectory")]
        public EQM_Page_Return getTwoDirectory(CV_EQM_EQUIP_RELATE_SERVICE_QueryParam qp)
        {
            DM_Equipment_Maintenance_QueryParam parm = new DM_Equipment_Maintenance_QueryParam
            {
                parentDirection = qp.EquipType,
                type = qp.EquipTypeDes,
                PageIndex = qp.PageIndex,
                PageSize = qp.PageSize
            };
            return EMBO.GetEntitiesByParam(parm);
        }
//       [HttpPost]
//        [Route("getThreeDirectory")]
//        public DataTable getThreeDirectory(DM_Equipment_Maintenance qp)
//        {
//            DataTable list = null;

//            string Sql = @"SELECT TOP 1000 [parentID]
//                          ,[kid]
//                          ,[type]
//                          ,[direction]
//                          ,[parentDirection]
//                          ,[code]
//                      FROM [SitMesDbExt].[dbo].[DM_Equipment_Maintenance]
//                      where parentID=N'" + qp.parentID + "'";
//            list = EMBO.GetDataTableBySql(Sql);
//            return list;
//        }
        
        [HttpGet]
        [Route("RemoveEntity")]
        public string RemoveEntity(int kid)
        {
            try
            {
                EMBO.Delete(kid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "成功";
        }
        //获取中类类型
        [HttpGet]
        [Route("getParentID")]
        public DataTable getPraentID()
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @"SELECT TOP 1000 [EquipClassID] as parentID
      ,[EquipClass] as parentDirection
  FROM [SitMesDbExt].[dbo].[EQM_EQUIP_CLASS] 
                             where 1=1 AND [EquipClass]  is not null";
            list = eec.GetDataTableBySql(Sql);
            return list;
        }
        //获取维护类别
        [HttpGet]
        [Route("getMaintenanceTypes")]
        public DataTable getMaintenanceTypes()
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;
                string Sql = @"SELECT  distinct  [type]
  FROM [SitMesDbExt].[dbo].[DM_Equipment_Maintenance]";
                list = EMBO.GetDataTableBySql(Sql);

            return list;
        }
        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddBase")]
        public HttpResponseMessage AddBase(DM_Equipment_Maintenance Base)
        {
            try
            {
                Base.CreateTime = SSGlobalConfig.Now;
                DM_Equipment_Maintenance newBase = this.EMBO.Insert(Base);
                if (newBase != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateBase")]
        public HttpResponseMessage UpdateBase(DM_Equipment_Maintenance user)
        {
            try
            {
                user.UpdateTime = SSGlobalConfig.Now;
                EMBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }
    }
}