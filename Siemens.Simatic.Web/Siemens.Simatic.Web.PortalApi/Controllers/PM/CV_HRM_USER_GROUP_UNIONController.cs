using Siemens.Simatic.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/UserGroupUnion")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_HRM_USER_GROUP_UNIONController : ApiController
    {
        #region Private Fields

        //ILog log = LogManager.GetLogger(typeof(CV_HRM_USER_GROUP_UNIONController));

        ICV_HRM_USER_GROUP_UNIONBO cv_HRM_USER_GROUP_UNIONBO = ObjectContainer.BuildUp<ICV_HRM_USER_GROUP_UNIONBO>();
        ICV_HRM_PERSBO cv_HRM_PERSBO = ObjectContainer.BuildUp<ICV_HRM_PERSBO>();

        #endregion

        #region Public Methods

        [HttpGet]
        [Route("GetUserGroup")]
        public CV_HRM_USER_GROUP_UNION GetUserGroup(string userID)
        {
            IList<CV_HRM_USER_GROUP_UNION> list = new List<CV_HRM_USER_GROUP_UNION>();
            CV_HRM_USER_GROUP_UNION cV_HRM_USER_GROUP_UNION = new CV_HRM_USER_GROUP_UNION() { UserID = userID };
            list = cv_HRM_USER_GROUP_UNIONBO.GetEntities(cV_HRM_USER_GROUP_UNION);
            //log.Debug(JsonConvert.SerializeObject(list));
            if (list != null && list.Count > 0)
            return list[0];
            return null;
        }

        [HttpPost]
        [Route("GetTime")]
        public DateTime Getdatebasetime()
        {
            DateTime time = SSGlobalConfig.Now;
            return time;
        }
        [HttpPost]
        [Route("UpdateLoginTime")]
        public HttpResponseMessage UpdateLoginTime(CV_HRM_PERS cv_HRM_pers)
        {
            try
            {
                CV_HRM_PERS cV_HRM_pers = new CV_HRM_PERS()
                {
                    id = cv_HRM_pers.id,
                    email3 = cv_HRM_pers.email3, 
                };
                cv_HRM_PERSBO.UpdateSome(cV_HRM_pers);
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
