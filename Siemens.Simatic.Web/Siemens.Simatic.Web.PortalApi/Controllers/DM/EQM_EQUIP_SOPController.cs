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
using System.IO;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/equipmentsop")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_SOPController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_CLASSController));
        IEQM_EQUIP_CLASSBO eqm_EQUIP_CLASSBO = ObjectContainer.BuildUp<IEQM_EQUIP_CLASSBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods
        //获得中类及SOP文件地址
        [HttpPost]
        [Route("filterDeviceClassPage")]
        public EQM_Page_Return filterDeviceClassPage(EQM_EQUIP_CLASS_QueryParam param)
        {
            return eqm_EQUIP_CLASSBO.GetEntitiesByParam(param);
        }
  
        //获取中类名称
        [HttpGet]
        [Route("getClass")]
        public DataTable getClass()
        {
            DataTable list = null;
            string Sql = @" select distinct EquipClass 
                              from EQM_EQUIP_CLASS 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;
        }

        //上传SOP
        [HttpPost]
        [Route("upload")]
        public void upload(int equipClassID)
        {
            HttpRequest request = HttpContext.Current.Request;
            string urlPath = request.Url.GetLeftPart(UriPartial.Path).Replace("/upload", "");//IIS
            string path = System.Web.HttpContext.Current.Server.MapPath(".");//155服务器本地文件夹
            EQM_EQUIP_CLASS entity = eqm_EQUIP_CLASSBO.GetEntity(equipClassID);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)          //如果集合的数量大于0
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名
                    FileSave.SaveAs(savePath);//上传     
                    entity.SopFile = urlPath + "/" + FileSave.FileName;//存放http地址可以直接下载查看
                    eqm_EQUIP_CLASSBO.UpdateSome(entity);
                }
            }
        }
        #endregion
    }
}
