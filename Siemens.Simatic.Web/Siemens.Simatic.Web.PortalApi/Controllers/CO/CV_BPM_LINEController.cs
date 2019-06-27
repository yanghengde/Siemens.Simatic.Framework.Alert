//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
//{
//    public class CV_BPM_LINEController
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

namespace Siemens.Simatic.Web.PortalApi.Controller
{
    [RoutePrefix("api/Vline")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_BPM_LINEController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CV_BPM_LINEController));

        //IPOM_TEMP_ERP_ORDERBO orderBO = ObjectContainer.BuildUp<IPOM_TEMP_ERP_ORDERBO>();
        ICV_BPM_LINEBO lineBO = ObjectContainer.BuildUp<ICV_BPM_LINEBO>();
        IPM_BPM_LINEBO linebo = ObjectContainer.BuildUp<IPM_BPM_LINEBO>();
        #endregion

        #region Public Methods

        [HttpPost]
        [Route("Addline")]
        public HttpResponseMessage Addline(PM_BPM_LINE definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            CV_BPM_LINE_QueryParam lineQueray = new CV_BPM_LINE_QueryParam();
            lineQueray.LineID = definitions.LineID;

            IList<CV_BPM_LINE> list = lineBO.GetEntities(lineQueray);
            if (list.Count != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该产线已经存在");
            }
            else
            {
                definitions.CreatedOn = DateTime.Now;
                definitions.LineGuid = Guid.NewGuid();
                if (string.IsNullOrEmpty(definitions.PlantID))
                    definitions.PlantID = "H006";
                definitions.UpdatedOn = DateTime.Now;
                if (string.IsNullOrEmpty(definitions.UpdatedBy))
                    definitions.UpdatedBy = definitions.CreatedBy;
                if (string.IsNullOrEmpty(definitions.WorkshopGuid.ToString()))
                {
                    IPM_BPM_WORKSHOPBO WorkShopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
                    PM_BPM_WORKSHOP_QueryParam pm = new PM_BPM_WORKSHOP_QueryParam();
                    pm.WorkshopID = definitions.DepartID;
                    IList<PM_BPM_WORKSHOP> nn = WorkShopBO.GetEntities(pm);

                    if (nn.Count > 0)

                        definitions.WorkshopGuid = nn[0].WorkshopGuid;
                }

                PM_BPM_LINE mmExt = linebo.Insert(definitions);
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
        [Route("Updateline")]
        public void Updateline(PM_BPM_LINE definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            try
            {
                if (string.IsNullOrEmpty(definitions.PlantID))
                    definitions.PlantID = "H006";
                definitions.UpdatedOn = DateTime.Now;
                if (string.IsNullOrEmpty(definitions.UpdatedBy))
                    definitions.UpdatedBy = definitions.CreatedBy;
                if (string.IsNullOrEmpty(definitions.WorkshopGuid.ToString()))
                {
                    IPM_BPM_WORKSHOPBO WorkShopBO = ObjectContainer.BuildUp<IPM_BPM_WORKSHOPBO>();
                    PM_BPM_WORKSHOP_QueryParam pm = new PM_BPM_WORKSHOP_QueryParam();
                    pm.WorkshopID = definitions.DepartID;
                    IList<PM_BPM_WORKSHOP> nn = WorkShopBO.GetEntities(pm);

                   if(nn.Count>0)
                    
                    definitions.WorkshopGuid = nn[0].WorkshopGuid;
                }
                //definitions.WorkshopGuid = definitions.CreatedBy;
                linebo.Update(definitions);
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        [HttpPost]
        [Route("GetVLine")]
        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<CV_BPM_LINE> GetLine(CV_BPM_LINE_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {
            //return JsonConvert.SerializeObject(User);

            //return suppliersBO.GetAll();
            //return User.ID.ToString();
            IList<CV_BPM_LINE> list = new List<CV_BPM_LINE>();
            if (line != null)
            {
                list = lineBO.GetEntities(line);
                List<CV_BPM_LINE> _list = (from l in list
                                           orderby l.LineID
                                           select l).ToList();
                return _list;
            }
            return list;
        }

        #endregion
    }
}
