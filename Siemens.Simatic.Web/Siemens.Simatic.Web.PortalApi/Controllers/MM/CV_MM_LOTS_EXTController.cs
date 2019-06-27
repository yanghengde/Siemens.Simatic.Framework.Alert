//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Siemens.Simatic.Web.PortalApi.Controllers.MM
//{
//    public class CV_MM_LOTS_EXTController
//    {
//    }
//}

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
    [RoutePrefix("api/cvmmLotsExt")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_MM_LOTS_EXTController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(MM_ClassesController));
        ICV_MM_LOTS_EXTBO LotsExtBO = ObjectContainer.BuildUp<ICV_MM_LOTS_EXTBO>();

        #endregion
        /// <summary>
        /// 查询所有序列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAllLots")]
        public IList<CV_MM_LOTS_EXT> getAllLots()
        {
            IList<CV_MM_LOTS_EXT> list = new List<CV_MM_LOTS_EXT>();
            list = LotsExtBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        /// <summary>
        /// 根据条件查序列表实体。。。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getLotsEntity")]
        public IList<CV_MM_LOTS_EXT> getLotsEntity(CV_MM_LOTS_EXT_QueryParam entity)
        {
            IList<CV_MM_LOTS_EXT> list = new List<CV_MM_LOTS_EXT>();
            list = LotsExtBO.GetEntities(entity);
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
    }
}