using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Newtonsoft.Json;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Platform.Core;

using System;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace Siemens.Simatic.Web.PortalApi.Controllers.MM
{
    [RoutePrefix("api/feedinglocation")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MM_FEEDING_LOCATIONController : ApiController
    {

        public static string userName = "";
        ILog log = LogManager.GetLogger(typeof(MM_FEEDING_LOCATIONController));        
        IMM_FEEDING_LOCATIONBO definitionBO = ObjectContainer.BuildUp<IMM_FEEDING_LOCATIONBO>();


        [HttpPost]
        [Route("Addfeedinglocation")]
        public HttpResponseMessage Addfeedinglocation(MM_FEEDING_LOCATION definitions)
        {
            definitions.UpdatedOn = DateTime.Now;
            try
            {
                MM_FEEDING_LOCATION_QueryParam feeQueray = new MM_FEEDING_LOCATION_QueryParam();
                feeQueray.FeedingLocation = definitions.FeedingLocation;
                IList<MM_FEEDING_LOCATION> list = definitionBO.GetEntities(feeQueray);
                if (list.Count != 0)
                {
                    //return Request.CreateResponse(HttpStatusCode.OK, "投料口ID已经存在");
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "投料口ID已经存在");
                }
                else
                {
                    MM_FEEDING_LOCATION mmExt = definitionBO.Insert(definitions);
                    return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败:" + e.Message);
            }
        }


        [HttpPost]
        [Route("Updatefeedinglocation")]
        public object Updatefeedinglocation(MM_FEEDING_LOCATION definitions)
        {
            definitions.UpdatedOn = DateTime.Now;
            try
            {
                definitionBO.Update(definitions);
                return "OK";
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "修改失败:" + e.Message);
            }
        }
        

        //#region 获取全部数据用的-张伟光-2017年11月17日
        [Route("getAllFeedingLocation")]
        public object GetAllFeedingLocation()
        {
            IList<MM_FEEDING_LOCATION> list = new List<MM_FEEDING_LOCATION>();
            list = definitionBO.GetAll();
            return list;
        }
        //根据现有的工位ID（前7位）查询投料口ID（前7位相同）
        [HttpPost]
        [Route("GetFeedingLocationByTerminalID")]
        public object GetFeedingLocationByTerminalID(string terminalID)
        {
            IList<MM_FEEDING_LOCATION> list = new List<MM_FEEDING_LOCATION>();
            ICO_BSC_BO co_BSC_BO = ObjectContainer.BuildUp<ICO_BSC_BO>();
            string sql;

            if (terminalID.Length > 7)
            {
                terminalID = terminalID.Substring(0, 7);
                sql = @"SELECT * FROM dbo.MM_FEEDING_LOCATION  WHERE FeedingLocation like '{0}_%'";
                sql = string.Format(sql, terminalID);

            }
            else
            {
                sql = @"SELECT *  FROM dbo.MM_FEEDING_LOCATION ";
                sql = string.Format(sql);
            }
            DataTable dtMaterial = co_BSC_BO.GetDataTableBySql(sql);
            if (dtMaterial == null)
            {
                sql = @"SELECT *  FROM dbo.MM_FEEDING_LOCATION ";
                sql = string.Format(sql);
                dtMaterial = co_BSC_BO.GetDataTableBySql(sql);
            }
            foreach (DataRow dr in dtMaterial.Rows) 
            {
                MM_FEEDING_LOCATION model = new MM_FEEDING_LOCATION();
                model.ID = Convert.ToInt32(dr["ID"]);
                model.FeedingLocation = dr["FeedingLocation"].ToString();
                model.Note = dr["Note"].ToString();
                model.FeedingLocationDesc = dr["FeedingLocationDesc"].ToString();
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 查询投料口
        /// </summary>
        /// <param name="definitions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getfeedingLocation")]
        public object getfeedingLocation(MM_FEEDING_LOCATION_QueryParam definitions)
        {
            IList<MM_FEEDING_LOCATION> list = new List<MM_FEEDING_LOCATION>();
            try
            {
                list = definitionBO.GetEntities(definitions);
                return list;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "查询失败:" + e.Message);
            }
        }

        /// <summary>
        /// 投料口同步给WMS
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SetWMS")]
        public ReturnValue SetWMS()
        {
            IAPI_WMS_BO apiwms = ObjectContainer.BuildUp<IAPI_WMS_BO>();
            return apiwms.LineFeedingLoc();
        }

     


    }
}