
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
using Siemens.Simatic.PM.BusinessLogic.DefaultImpl;
using Siemens.MES.Model.EntityModel.SysMgt;
using Siemens.Simatic.PM.Common.QueryParams;
using System.Text;
using System.IO;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/external")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PM_EM_EXTERNALController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(PM_EM_EXTERNALController));


        IPM_EM_TS_EXTERNALBO externalBO = ObjectContainer.BuildUp<IPM_EM_TS_EXTERNALBO>();
        ICV_PM_EM_TS_EXTERNALBO cvexternalBO = ObjectContainer.BuildUp<ICV_PM_EM_TS_EXTERNALBO>();
        
        /// <summary>
        /// 获取所有的员工信息
        /// </summary>
        /// <returns></returns>
        [Route("GetAllExternal")]
        public IList<CV_PM_EM_TS_EXTERNAL> GetAllExternal()
        {
            CV_PM_EM_TS_EXTERNAL definition = new CV_PM_EM_TS_EXTERNAL();
            definition.RowDeleted = "否";
            
            
            IList<CV_PM_EM_TS_EXTERNAL> list = new List<CV_PM_EM_TS_EXTERNAL>();

            list = cvexternalBO.GetEntities(definition);
            return list;
        }

        /// <summary>
        /// 按条件查询外部损失信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetExternal")]
        public IList<CV_PM_EM_TS_EXTERNAL> GetExternal(CV_PM_EM_TS_EXTERNAL definitions)
        {
            IList<CV_PM_EM_TS_EXTERNAL> list = new List<CV_PM_EM_TS_EXTERNAL>();
            definitions.RowDeleted = "否";
            if (definitions != null)
            {
                try
                {
                    list = cvexternalBO.GetEntities(definitions);


                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                return null;
            }

        }
       
        [HttpPost]
        [Route("AddExternal")]
        public HttpResponseMessage AddExternal(PM_EM_TS_EXTERNAL definitions)
        {
            definitions.ExternalGuid = Guid.NewGuid();
            definitions.CreatedOn = DateTime.Now;
            CV_PM_EM_TS_EXTERNAL external = new CV_PM_EM_TS_EXTERNAL();


            external.ExternalGuid = definitions.ExternalGuid;

            PM_EM_TS_EXTERNAL mmExt = externalBO.Insert(definitions);
                if (mmExt != null)
                {
                 
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            
        }

        /// <summary>
        /// 更新菜单栏信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("UpdateExternal")]
        public void UpdateExternal(PM_EM_TS_EXTERNAL definitions)
        {
            try
            {
                externalBO.Update(definitions);
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }


    }
}