using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Account.Controllers.DM
{
    [RoutePrefix("api/DMDeviceInstance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DMDeviceInstanceController : ApiController
    {
        #region Private Fields
        private IDM_BSC_BO utilityBo = ObjectContainer.BuildUp<IDM_BSC_BO>();
        private ICV_EQM_DEVICE_INSTANCE_TERMINALBO cv_device_ins_bo = ObjectContainer.BuildUp<ICV_EQM_DEVICE_INSTANCE_TERMINALBO>();
        private IDM_DEVICE_INSTANCEBO device_ins_bo = ObjectContainer.BuildUp<IDM_DEVICE_INSTANCEBO>();
        //表名
        //private string DM_DEVICE_TYPE_DB = "DM_DEVICE_TYPE";//设备大类
        //private string DM_DEVICE_CLASS_DB = "DM_DEVICE_CLASS";//设备中类
        //private string DM_DEVICE_DEFINITION_DB = "DM_DEVICE_DEFINITION";//设备小类
        //private string DM_DEVICE_STATUS_DB = "DM_DEVICE_STATUS";//设备状态
        private string DM_DEVICE_INSTANCE_DB = "DM_DEVICE_INSTANCE";//设备台账 
        //private string DM_DEVICE_INSTANCE_VIEW_DB = "a_test_view"; //设备台账查询视图
        #endregion


        #region Public Methods
        //获得所有大类
        [HttpGet]
        [Route("getAllDeviceType")]
        public DataTable getAllDeviceType() 
        {
            string querySql = "select DeviceType from " + DM_DEVICE_INSTANCE_DB + " group by DeviceType";
            return utilityBo.GetDataTableBySql(querySql);
        }

        [HttpGet]
        [Route("test")]
        public string test()
        {   
            return "";
        }

        //获得所有中类
        [HttpGet]
        [Route("getAllDeviceClass")]
        public DataTable getAllDeviceClass(string deviceType)
        {
            string querySql = "select DeviceName from " + DM_DEVICE_INSTANCE_DB + " where DeviceType = N'" + deviceType + "' group by DeviceName";
            return utilityBo.GetDataTableBySql(querySql);
        }

        //获得所有小类
        [HttpGet]
        [Route("getAllDeviceDefinition")]
        public DataTable getAllDeviceDefinition(string deviceType, string deviceClass)
        {
            string querySql = "select DeviceSpecID from " + DM_DEVICE_INSTANCE_DB + " where 1=1";
            if(!string.IsNullOrEmpty(deviceType))
            {
                querySql+=" and DeviceType = N'" + deviceType + "'";
            }
            if(!string.IsNullOrEmpty(deviceClass))
            {
                querySql += " and DeviceName = N'" + deviceClass + "' group by DeviceSpecID";
            }
            return utilityBo.GetDataTableBySql(querySql);
        }

        //获得所有状态
        [HttpGet]
        [Route("getAllDeviceStatus")]
        public DataTable getAllDeviceStatus()
        {
            string querySql = "select DeviceStatus from " + DM_DEVICE_INSTANCE_DB + " group by DeviceStatus";
            return utilityBo.GetDataTableBySql(querySql);
        }

        //获得adckz
        [HttpGet]
        [Route("getABCKZ")]
        public DataTable getABCKZ()
        {
            string querySql = "select distinct ABCKZ from " + DM_DEVICE_INSTANCE_DB + " WHERE ABCKZ IS NOT NULL ORDER BY ABCKZ";
            return utilityBo.GetDataTableBySql(querySql);
        }

        //查询条件查询
        [HttpPost]
        [Route("filterDeviceInstancePage")]
        public EQM_Page_Return filterDeviceInstancePage(DM_DEVICE_INSTANCE_QueryParam queryParam)  //CustPageContent
        {
            return cv_device_ins_bo.GetEntities(queryParam);
        }
        [HttpPost]
        [Route("updateDeviceInstance")]
        public HttpResponseMessage updateDeviceInstance(DM_DEVICE_INSTANCE Param)  //CustPageContent
        {
            try
            {
                device_ins_bo.Update(Param);
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