using log4net;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.PM.Common;
using Siemens.Simatic.Util.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
{
    [RoutePrefix("api/co_bmp_step")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CO_BPM_STEPController : ApiController
    {
        #region Private Fileds

        ILog log = LogManager.GetLogger(typeof(CO_BPM_PLANTController));
        IPM_BPM_STEPBO stepBO = ObjectContainer.BuildUp<IPM_BPM_STEPBO>();
        IPM_BPM_STEP_ATTRIBUTEBO saBO = ObjectContainer.BuildUp<IPM_BPM_STEP_ATTRIBUTEBO>();
        IList<PM_BPM_STEP_ATTRIBUTE> list = new List<PM_BPM_STEP_ATTRIBUTE>();
        #endregion

        [HttpPost]
        [Route("GetStepAtt")]
        public IList<PM_BPM_STEP_ATTRIBUTE> GetStepAtt(PM_BPM_STEP_ATTRIBUTE sa) 
        {

            
            if (sa != null)
            {
                list = saBO.GetEntities(sa);
                return list;
            }
            return list;
        }

        [HttpPost]
        [Route("updateStepAtt")]
        public void updateStepAtt(PM_BPM_STEP_ATTRIBUTE sa)
        {
            PM_BPM_STEP_ATTRIBUTE nn = new PM_BPM_STEP_ATTRIBUTE();
            nn.StepAtrrGuid = sa.StepAtrrGuid;

            list = saBO.GetEntities(nn);
            try
            {
                if (list!=null &&list.Count> 0 )
                {
                   sa.UpdatedOn = SSGlobalConfig.Now;
                   sa.CreatedBy = list[0].CreatedBy;
                   saBO.UpdateSome(sa);
                }
                else
                {
                    sa.StepAtrrGuid = Guid.NewGuid();
                    sa.CreatedOn = SSGlobalConfig.Now;         
                    saBO.Insert(sa);
                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }
        [HttpPost]
        [Route("Addstep")]
        public HttpResponseMessage Addstep(PM_BPM_STEP definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            PM_BPM_STEP_QueryParam stepQueray = new PM_BPM_STEP_QueryParam();
            stepQueray.StepID = definitions.StepID;

            IList<PM_BPM_STEP> list = stepBO.GetEntities(stepQueray);
            if (list.Count != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该工序已经存在");
            }
            else
            {
                definitions.StepGuid = Guid.NewGuid();
                PM_BPM_STEP mmExt = stepBO.Insert(definitions);
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
        [Route("Updatestep")]
        public HttpResponseMessage Updatestep(PM_BPM_STEP definitions)
        {
            definitions.CreatedOn = DateTime.Now;
            try
            {
               
                PM_BPM_STEP_QueryParam stepQueray = new PM_BPM_STEP_QueryParam();
                stepQueray.StepID = definitions.StepID;

                IList<PM_BPM_STEP> list = stepBO.GetEntities(stepQueray);
                if (list.Count != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "该工序已经存在");
                }
                else
                {
                    stepBO.Update(definitions);
                    return Request.CreateResponse(HttpStatusCode.OK, "修改成功");
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "修改失败");
                throw e;
            }


        }
        [HttpDelete]
        [Route("Deletestep")]
        public void Deletestep(Guid stepGuid)
        {
            
            try
            {
                stepBO.Delete(stepGuid);
            }
            catch (Exception e)
            {
                throw e;
            }


        }
    }
}