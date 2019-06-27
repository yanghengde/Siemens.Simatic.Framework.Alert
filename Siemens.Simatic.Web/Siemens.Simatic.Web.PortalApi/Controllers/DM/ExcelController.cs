using Siemens.Simatic.DM.BusinessLogic;
using Siemens.Simatic.DM.Common;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Web.PortalApi.Infrustrues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Siemens.Simatic.PM.BusinessLogic;
using System.Data;
using NPOI.HSSF.UserModel;
using System.Web;
using System.IO;
using NPOI.SS.UserModel;

namespace Siemens.Simatic.Web.PortalApi.Controller.DM
{
    [RoutePrefix("api/excel")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExcelController : ApiController
    {
        IDM_Equipment_MaintenanceBO DM_Equipment_MaintenanceBO = ObjectContainer.BuildUp<IDM_Equipment_MaintenanceBO>();
        IEQM_EQUIP_PRO_BASE_INFOBO EQM_EQUIP_PRO_BASE_INFOBO = ObjectContainer.BuildUp<IEQM_EQUIP_PRO_BASE_INFOBO>();
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("DM_Equipment_Maintenance")]
        public string DM_Equipment_Maintenance()
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
                    string resul = DM_Equipment_MaintenanceBO.Input(savePath); //导入数据库

                    return resul;

                }
            }
            else
            {
                return "文件不存在！";
            }
            return "true";
        }

        [HttpPost]
        [Route("EQM_EQUIP_PRO_BASE_INFO")]
        public string EQM_EQUIP_PRO_BASE_INFO() {

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
                    string resul = EQM_EQUIP_PRO_BASE_INFOBO.Input(savePath); //导入数据库

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