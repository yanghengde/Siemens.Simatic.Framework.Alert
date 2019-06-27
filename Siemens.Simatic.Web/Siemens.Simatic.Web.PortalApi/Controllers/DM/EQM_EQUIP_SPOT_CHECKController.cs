using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.DM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.DM.Common;
using System.Data;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Platform.Core;

namespace Siemens.Simatic.Web.PortalApi.Controller.DM
{
    [RoutePrefix("api/spotcheck")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_SPOT_CHECKController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_SPOT_CHECKController));

        Siemens.Simatic.DM.BusinessLogic.ICV_EQM_EQUIP_SPOT_CHECKBO CheckBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.ICV_EQM_EQUIP_SPOT_CHECKBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        [HttpPost]
        [Route("Getpartdata")]
        public EQM_Page_Return Getpartdata(CV_EQM_EQUIP_SPOT_CHECK_QueryParam param)
        {
            EQM_Page_Return entities = CheckBO.GetEntities(param);
            return entities;

        }

        [HttpPost]
        [Route("GetCount")]
        public DataTable getcount(CV_EQM_EQUIP_SPOT_CHECK_QueryParam qp)
        {
            DataTable list = null;

            string Sql = @" select count(*)
                              from CV_EQM_EQUIP_SPOT_CHECK 
                             where 1=1 ";
            if (!string.IsNullOrEmpty(qp.DeviceType))
            {
                Sql += " and DeviceType = N'" + qp.DeviceType + "' ";
            }
            if (!string.IsNullOrEmpty(qp.DeviceName))
            {
                Sql += " and DeviceName = N'" + qp.DeviceName + "' ";
            }
            if (!string.IsNullOrEmpty(qp.DeviceSpecID))
            {
                Sql += " and DeviceSpecID = N'" + qp.DeviceSpecID + "' ";
            }
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

        //[HttpPost]
        //[Route("Addcheck")]
        //public DataTable Addcheck(CV_EQM_EQUIP_SPOT_CHECK_QueryParam qp)
        //{
        //    DataTable List = null;


        //    return List;

        //}
    }
}