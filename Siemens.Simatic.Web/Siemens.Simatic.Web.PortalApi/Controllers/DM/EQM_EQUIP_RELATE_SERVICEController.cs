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


namespace Siemens.Simatic.Web.PortalApi.Controller.DM
{
    [RoutePrefix("api/relateservice")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_RELATE_SERVICEController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_RELATE_SERVICEController));


        Siemens.Simatic.DM.BusinessLogic.ICV_EQM_EQUIP_RELATE_SERVICEBO ServiceBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.ICV_EQM_EQUIP_RELATE_SERVICEBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        [HttpPost]
        [Route("Getpartdata")]
        public IList<CV_EQM_EQUIP_RELATE_SERVICE> Getpartdata(CV_EQM_EQUIP_RELATE_SERVICE_QueryParam param)
        {
            IList<CV_EQM_EQUIP_RELATE_SERVICE> entities = ServiceBO.GetEntities(param);
            return entities;

        }

        [HttpPost]
        [Route("GetCount")]
        public DataTable getcount(CV_EQM_EQUIP_RELATE_SERVICE_QueryParam qp)
        {
            DataTable list = null;

            string Sql = @" select count(*)
                              from CV_EQM_EQUIP_RELATE_SERVICE 
                             where 1=1 
                               and EquipTypeID = N'" + qp.EquipTypeID + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

    }
}