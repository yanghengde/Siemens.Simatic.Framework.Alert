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


namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/Workshop")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CO_BPM_WORKSHOPController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CO_BPM_WORKSHOPController));
        IPM_BPM_WORKSHOPBO workshopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();

        #endregion

        #region Public Methods
        [HttpPost]
        [Route("Addworkshop")]
        public HttpResponseMessage Addworkshop(PM_BPM_WORKSHOP definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            PM_BPM_WORKSHOP_QueryParam workQueray = new PM_BPM_WORKSHOP_QueryParam();
            workQueray.WorkshopID = definitions.WorkshopID;

            IList<PM_BPM_WORKSHOP> list = workshopBO.GetEntities(workQueray);
            if (list.Count != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该车间已经存在");
            }
            else
            {
                definitions.WorkshopGuid = Guid.NewGuid();
                PM_BPM_WORKSHOP mmExt = workshopBO.Insert(definitions);
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
        [HttpPost]
        [Route("Updateworkshop")]
        public void Updateworkshop(PM_BPM_WORKSHOP definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            try
            {
                workshopBO.Update(definitions);
            }
            catch (Exception e)
            {
                throw e;
            }


        }
        /// <summary>
        /// 查询所有车间-田成荣-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [HttpGet]
        [Route("")]
        public IList<PM_BPM_WORKSHOP> GetWorkshop()
        {

            IList<PM_BPM_WORKSHOP> list = new List<PM_BPM_WORKSHOP>();
            list = workshopBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }
        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public static string model;
        [HttpPost]
        [Route("GetWorkshop")]
        public IList<PM_BPM_WORKSHOP> GetWorkshop(PM_BPM_WORKSHOP_QueryParam workshop) //传入的参数是对象，用Post，不能用Get
        {
            //return JsonConvert.SerializeObject(User);

            //return suppliersBO.GetAll();
            //return User.ID.ToString();

            IList<PM_BPM_WORKSHOP> list = new List<PM_BPM_WORKSHOP>();
            if (workshop.PlantGuid != null)
            {
                model = JsonConvert.SerializeObject(workshop);
                list = workshopBO.GetEntities(workshop);
                return list;
            }
            else{
                workshop = JsonConvert.DeserializeObject<PM_BPM_WORKSHOP_QueryParam>(model);
                list = workshopBO.GetEntities(workshop);
                return list;
            }
            
            
        }
         #endregion
    }
}
