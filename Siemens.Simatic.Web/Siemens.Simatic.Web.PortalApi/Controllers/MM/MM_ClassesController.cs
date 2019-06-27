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

namespace Siemens.Simatic.Web.PortalApi.Controllers.MM
{
    [RoutePrefix("api/mmClass")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MM_ClassesController:ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(MM_ClassesController));

        //IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        IMMClassesBO classesBO = ObjectContainer.BuildUp<IMMClassesBO>();
        //IPOM_USERBO userBO = ObjectContainer.BuildUp<IPOM_USERBO>();


        #endregion

        #region 公共方法-王浩田-2017年11月14日13:57:28

        #region getClasses-直接请求“”-王浩田-2017年11月14日18:30:59
        [Route("GetClasses")]
        public IList<MMClasses> getClasses()
        {
            IList<MMClasses> list = new List<MMClasses>();
            list = classesBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        #endregion


        #region Ilist<MMClasses>-GetClasses-获取所有物料类别-王浩田-2017年11月14日18:41:31
        [HttpGet]
        [Route("GetClasses1")]
        public IList<MMClasses> GetClasses(MM_CLASSES_QueryParam classes)
        {
            IList<MMClasses> list = new List<MMClasses>();
            if (classes != null)
            {
                list = classesBO.GetEntities(classes);
            }
            return list;
        }
        #endregion
        


        


        #endregion
    }
}