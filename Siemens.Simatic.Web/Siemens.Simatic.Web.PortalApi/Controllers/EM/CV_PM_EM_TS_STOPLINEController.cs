using log4net;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Text;
using Siemens.Simatic.Web.PortalApi.Controllers.CO;
using Siemens.Simatic.Util.Utilities;
using System.IO;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers.EM
{
    [RoutePrefix("api/employee")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_PM_EM_TS_STOPLINEController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(CV_PM_EM_TS_STOPLINEController));
        ICV_PM_EM_TS_STOPLINEBO cV_PM_EM_TS_STOPLINEBO = ObjectContainer.BuildUp<ICV_PM_EM_TS_STOPLINEBO>();
        IPM_EM_TS_STOPLINEBO pM_EM_TS_STOPLINEBO = ObjectContainer.BuildUp<IPM_EM_TS_STOPLINEBO>();
        IPM_EM_EMPLOYEEBO PM_EM_EMPLOYEEBO = ObjectContainer.BuildUp<IPM_EM_EMPLOYEEBO>();

        /// <summary>
        /// 查询试图
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCVEntitis")]
        public IList<CV_PM_EM_TS_STOPLINE> GetCVEntitis(CV_PM_EM_TS_STOPLINE_QueryParam entity)
        {
            IList<CV_PM_EM_TS_STOPLINE> list = new List<CV_PM_EM_TS_STOPLINE>();
            if (entity != null)
            {
                try
                {
                    list = cV_PM_EM_TS_STOPLINEBO.GetEntities(entity);
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

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public HttpResponseMessage Add(PM_EM_TS_STOPLINE definitions)
        { 
            definitions.CreatedOn = DateTime.Now;

            CV_PM_EM_TS_STOPLINE_QueryParam employee = new CV_PM_EM_TS_STOPLINE_QueryParam();

            IList<CV_PM_EM_TS_STOPLINE> list = new List<CV_PM_EM_TS_STOPLINE>();
            list = cV_PM_EM_TS_STOPLINEBO.GetEntities(employee);
            if (list.Count != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该用户已经存在");
            }
            else
            {
                PM_EM_TS_STOPLINE mmExt = pM_EM_TS_STOPLINEBO.Insert(definitions);
                if (mmExt != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Rule"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public HttpResponseMessage Update(PM_EM_TS_STOPLINE Rule)
        {
            try
            {
                pM_EM_TS_STOPLINEBO.UpdateSome(Rule);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpPost]
        [Route("uploadPosition")]
        public string uploadPosition(string user)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(".");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            //int i = request.Cookies.Count;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                    FileSave.SaveAs(savePath);//上传
                    //InputExcel(savePath);//导入数据库
                    string resul = PM_EM_EMPLOYEEBO.InputPosition(savePath, user); //导入数据库

                    return resul;
                }
            }
            else
            {
                return "文件不存在！";
            }
            return "true";
        }
    }
}