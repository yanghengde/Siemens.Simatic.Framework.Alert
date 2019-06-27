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
    [RoutePrefix("api/Userinformation")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class User_PersonalInformationController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(User_PersonalInformationController));
        IPM_WECHAT_USERBO pm_WECHAT_USERBO = ObjectContainer.BuildUp<IPM_WECHAT_USERBO>();
        ICV_PM_WECHAT_USER_DEPARTMENTBO pm_WECHAT_USER_DEPARTMENTBO = ObjectContainer.BuildUp<ICV_PM_WECHAT_USER_DEPARTMENTBO>();
        [HttpGet]
        [Route("GetuserName")]
        public IList<PM_WECHAT_USER> GetuserName(string userid)
        {
            IList<PM_WECHAT_USER> list = new List<PM_WECHAT_USER>();
            PM_WECHAT_USER pm_WECHAT_USER = new PM_WECHAT_USER()
            {
                UserID = userid,
            };
            list = pm_WECHAT_USERBO.GetuserEntities(pm_WECHAT_USER);
            return list;
        }
        [HttpGet]
        [Route("Getuserinformation")]
        public IList<CV_PM_WECHAT_USER_DEPARTMENT> Getuserinformation(string userid)
        {
            IList<CV_PM_WECHAT_USER_DEPARTMENT> list = new List<CV_PM_WECHAT_USER_DEPARTMENT>();
            CV_PM_WECHAT_USER_DEPARTMENT pm_WECHAT_USER_DEPARTMENT = new CV_PM_WECHAT_USER_DEPARTMENT()
            {
                UserID = userid,
            };
            list = pm_WECHAT_USER_DEPARTMENTBO.GetUserDepartmentbyuserID(pm_WECHAT_USER_DEPARTMENT);
            if (list != null && list.Count > 0)
                return list;
            return null;
        }
        
    }
    }      
   
 