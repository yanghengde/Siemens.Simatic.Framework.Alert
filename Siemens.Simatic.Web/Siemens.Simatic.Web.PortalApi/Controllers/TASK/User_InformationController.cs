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
using Siemens.Simatic.ALT.BusinessLogic;
using Siemens.Simatic.ALT.BusinessLogic.DefaultImpl;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.ALT.Common;
using Siemens.MES.Model.EntityModel.SysMgt;


namespace Siemens.Simatic.Web.PortalApi.Controllers
{
    [RoutePrefix("api/UserLoginName")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class User_InformationController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(User_PersonalInformationController));
        ICV_HRM_PERSBO cv_HRM_PERSBO = ObjectContainer.BuildUp<ICV_HRM_PERSBO>();
       
        [HttpGet]
        [Route("GetUserLoginName")]
        public IList<CV_HRM_PERS> GetuserName(string userid)
        {
            IList<CV_HRM_PERS> list = new List<CV_HRM_PERS>();
            CV_HRM_PERS cv_HRM_PERS = new CV_HRM_PERS()
            {
                id = userid,
            };

            list = cv_HRM_PERSBO.GetuserEntities(cv_HRM_PERS);
            return list;
        }
       
    }
    }      
   
 