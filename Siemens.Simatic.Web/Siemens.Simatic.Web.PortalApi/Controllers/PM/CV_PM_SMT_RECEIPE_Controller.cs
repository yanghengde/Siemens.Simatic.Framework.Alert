//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Siemens.Simatic.Web.PortalApi.Controllers.PM
//{
//    public class CV_PM_SMT_RECEIPE_Controller
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
using Siemens.Simatic.PM.Common.QueryParams;
using System.IO;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;


namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/CVReceipe")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_PM_SMT_RECEIPE_Controller : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CV_PM_SMT_RECEIPE_Controller));
        IPM_SMT_RECEIPEBO ReceipeBO = ObjectContainer.BuildUp<IPM_SMT_RECEIPEBO>();
        ICV_PM_SMT_RECEIPE_exportBO receipeExpBo = ObjectContainer.BuildUp<ICV_PM_SMT_RECEIPE_exportBO>();
        #endregion

        #region Public Methods

        /// <summary>
        /// 查询所有-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("GetCVReceipeAll")]
        public IList<CV_PM_SMT_RECEIPE_export> GetCVReceipeAll()
        {
            //IList<PM_SMT_RECEIPE> list = new List<PM_SMT_RECEIPE>();
            //list = ReceipeBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            //return list;
            IList<CV_PM_SMT_RECEIPE_export> list = new List<CV_PM_SMT_RECEIPE_export>();
            list = receipeExpBo.GetAllEntities();
            return list;
        }

        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetCVReceipeEntities")]
        public IList<CV_PM_SMT_RECEIPE_export> GetReceipeEntities(PM_SMT_RECEIPE_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            if (Entitie ==null)
            {
                return null;
            }
            IList<CV_PM_SMT_RECEIPE_export> list = new List<CV_PM_SMT_RECEIPE_export>();
            list = receipeExpBo.GetAllEntities(Entitie);
            return list;
        }
        /// <summary>
        /// 按条件查询未导入工单
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("GetNonReceipe")]
        public IList<CV_PM_SMT_RECEIPE_export> GetNonReceipe(PM_SMT_RECEIPE_QueryParam Entitie) //传入的参数是对象，用Post，不能用Get
        {
            IList<CV_PM_SMT_RECEIPE_export> list = new List<CV_PM_SMT_RECEIPE_export>();
            Entitie.Status = null;
            list = receipeExpBo.GetEntities(Entitie);
            return list;
        }

        public string userName;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user")]
        public string user(string user)
        {
            userName = user;
            return user;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadReceipe")]
        public string uploadReceipe(string user)
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
                    string savePath = path + "/"+ FileSave.FileName;     //通过此对象获取文件名

                    FileSave.SaveAs(savePath);//上传
                    //InputExcel(savePath);//导入数据库
                    string resul = ReceipeBO.RecipeInput(savePath, user); //导入数据库
                    
                    return resul;                    
                }
            }
            else
            {
                return "文件不存在！";
            }
            return "true";
        }

       
        #endregion
    }
}

