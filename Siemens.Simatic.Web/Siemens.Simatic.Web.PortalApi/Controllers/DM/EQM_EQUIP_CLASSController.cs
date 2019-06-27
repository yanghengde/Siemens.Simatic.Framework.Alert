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
    [RoutePrefix("api/equipmentclass")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EQM_EQUIP_CLASSController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(EQM_EQUIP_CLASSController));


        Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_CLASSBO ClassBO = ObjectContainer.BuildUp<Siemens.Simatic.DM.BusinessLogic.IEQM_EQUIP_CLASSBO>();
        ICO_BSC_BO BSCBO = ObjectContainer.BuildUp<ICO_BSC_BO>();
        #endregion

        #region Public Methods

        [Route("")]
        public IList<EQM_EQUIP_CLASS> GetClasss()
        {
            IList<EQM_EQUIP_CLASS> list = new List<EQM_EQUIP_CLASS>();
            list = ClassBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpGet]
        [Route("Login")]
        public string Login()
        {
            string str = "登录成功";
            return str;
        }


        [Route("paged"), HttpGet]
        /// <summary>
        /// 按分页查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>        
        public IList<EQM_EQUIP_CLASS> GetClassByPage(/*DateTime start, DateTime end, */int pageIndex, int pageSize) //Get可以传入多个参数
        {
            IList<EQM_EQUIP_CLASS> list = new List<EQM_EQUIP_CLASS>();
            list = ClassBO.GetClassByPage(pageIndex, pageSize);
            return list;
            //JsonConvert.SerializeObject(user);
        }

        //获得中类
        [HttpPost]
        [Route("filterDeviceClassPage")]
        public EQM_Page_Return filterDeviceClassPage(EQM_EQUIP_CLASS_QueryParam param)
        {
            return ClassBO.GetEntitiesByParam(param);
        }

        [HttpPost]
        [Route("GetEntities")]
        public IList<EQM_EQUIP_CLASS> GetEntities(EQM_EQUIP_CLASS_QueryParam qp)
        //传入的参数是对象，用Post，不能用Get
        {
            IList<EQM_EQUIP_CLASS> list = new List<EQM_EQUIP_CLASS>();
            if (qp != null)
            {
                list = ClassBO.GetEntities(qp);
            }
            return list;
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddClass")]
        public HttpResponseMessage AddUser(EQM_EQUIP_CLASS Class)
        {

            EQM_EQUIP_CLASS newClass = this.ClassBO.Insert(Class);
            if (newClass != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateClass")]
        public HttpResponseMessage UpdateUser(EQM_EQUIP_CLASS user)
        {
            try
            {
                ClassBO.UpdateSome(user);
                return Request.CreateResponse(HttpStatusCode.OK, "更新成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "更新出错:" + ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveClass")]
        public HttpResponseMessage DeleteUser(string KId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            try
            {
                ClassBO.Delete(Convert.ToInt32(KId));

                return Request.CreateResponse(HttpStatusCode.OK, "删除成功");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "删除出错:" + ex.Message);
            }
        }

        #endregion
        //获取大类下拉框
        [HttpGet]
        [Route("getClass")]
        public DataTable getClass(EQM_EQUIP_CLASS tp)
        {
            //get后，post传输数据量大，网址后面不会接数据
            DataTable list = null;

            string Sql = @" select distinct EquipClass 
                              from EQM_EQUIP_CLASS 
                             where 1=1 ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;


        }

        //下拉框值改变调用方法查询
        [HttpGet]
        [Route("Getpartdata")]
        public DataTable Getpartdata(string Class)
        {
            //EQM_EQUIP_CLASS
            DataTable list = null;

            string Sql = @" select *
                              from EQM_EQUIP_CLASS 
                             where 1=1 and EquipClass = N'" + Class + "'";
            list = BSCBO.GetDataTableBySql(Sql);
            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
            return list;

        }

        [HttpGet]
        [Route("getcount")]
        public DataTable getcount(EQM_EQUIP_CLASS tp)
        {

            DataTable list = null;

            string Sql = @" select count(*)
                              from EQM_EQUIP_CLASS ";
            list = BSCBO.GetDataTableBySql(Sql);
            return list;

            //return Request.CreateResponse(HttpStatusCode.OK, Sql);
        }

        [HttpPost]
        [Route("upload")]
        public string upload(int equipClassID)
        {
            //string path = System.Web.HttpContext.Current.Server.MapPath(".");
            EQM_EQUIP_CLASS entity=ClassBO.GetEntity(equipClassID);
            string path = "C:/hans/Siemens/sopFiles";//155服务器本地文件夹
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            try
            {
                if (FileCollect.Count > 0)          //如果集合的数量大于0
                {
                    foreach (string str in FileCollect)
                    {
                        HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                        // string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string savePath = path + "/" + FileSave.FileName;     //通过此对象获取文件名

                        FileSave.SaveAs(savePath);//上传     
                        entity.SopFile = "http://172.16.6.15/Siemens/sopFiles/" + FileSave.FileName;//存放http地址可以直接下载查看
                        ClassBO.UpdateSome(entity);
                        //InputExcel(savePath);//导入数据库
                    }
                }
                return "上传成功！";
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }


    }
}
