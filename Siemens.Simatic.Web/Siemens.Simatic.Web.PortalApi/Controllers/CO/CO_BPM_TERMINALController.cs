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
using Siemens.Simatic.Util.Utilities;


namespace Siemens.Simatic.Web.PortalApi.Controllers.CO{

    [RoutePrefix("api/co_bmp_terminal")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class CO_BPM_TERMINALController:ApiController
    {

        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CO_BPM_TERMINALController));


        IPM_BPM_TERMINALBO terminalBO = ObjectContainer.BuildUp<IPM_BPM_TERMINALBO>();
        IPM_BPM_TERMINAL_FEEDING_LOCBO terminallocBO = ObjectContainer.BuildUp<IPM_BPM_TERMINAL_FEEDING_LOCBO>();
        IPM_BPM_TERMINAL_SECTIONBO terminalsecBO = ObjectContainer.BuildUp<IPM_BPM_TERMINAL_SECTIONBO>();
        IPM_BPM_STEP_TERMINALBO terminalstepBO = ObjectContainer.BuildUp<IPM_BPM_STEP_TERMINALBO>();
        #endregion

        #region Public Methods

        [HttpGet]
        [Route("GetDataCount")]
        public int GetDataCount()
        {
            
            return Int32.Parse(terminalBO.GetDataCount());
        }
        [HttpPost]
        [Route("Addterminal")]
        public HttpResponseMessage Addterminal(PM_BPM_TERMINAL definitions)
        {
            definitions.CreatedOn = SSGlobalConfig.Now;
            PM_BPM_TERMINAL_QueryParam terminalQueray = new PM_BPM_TERMINAL_QueryParam();
            terminalQueray.TerminalID = definitions.TerminalID;

            IList<PM_BPM_TERMINAL> list = terminalBO.GetEntities(terminalQueray);
            if (list.Count != 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "该工位已经存在");
            }
            else
            {
                definitions.TerminalGuid = Guid.NewGuid();
                PM_BPM_TERMINAL mmExt = terminalBO.Insert(definitions);
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
        [Route("Updateterminal")]
        public void Updateterminal(PM_BPM_TERMINAL definitions)
        {
            definitions.CreatedOn = SSGlobalConfig.Now;
            try
            {
                terminalBO.Update(definitions);
            }
            catch (Exception e)
            {
                throw e;
            }


        }
        [Route("")]
        public IList<PM_BPM_TERMINAL> getAll()
        {

            IList<PM_BPM_TERMINAL> list = new List<PM_BPM_TERMINAL>();
            list = terminalBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpPost]
        [Route("getTerminal")]
        /// <summary>
        /// 按条件查询，可分页
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<PM_BPM_TERMINAL> getTerminal(PM_BPM_TERMINAL_QueryParam qb) //传入的参数是对象，用Post，不能用Get
        {

            IList<PM_BPM_TERMINAL> list = new List<PM_BPM_TERMINAL>();
            if (qb != null)
            {
                list = terminalBO.GetEntities(qb);
                return list;
            }
            return list;
        }
        #endregion


        #region 投料口与工位
        /// <summary>
        /// 查询投料口与工位
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("getAllTerminalfeedingloc")] 
        public IList<PM_BPM_TERMINAL_FEEDING_LOC> getAllTerminalfeedingloc() //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_TERMINAL_FEEDING_LOC> list = new List<PM_BPM_TERMINAL_FEEDING_LOC>();
            list = terminallocBO.GetAll();
            return list;
        }

        /// <summary>
        /// 查询投料口与工位
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("getTerminalfeedingloc")]
        public IList<PM_BPM_TERMINAL_FEEDING_LOC> GetTerminalfeedingloc(PM_BPM_TERMINAL_FEEDING_LOC_QueryParam line) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_TERMINAL_FEEDING_LOC> list = new List<PM_BPM_TERMINAL_FEEDING_LOC>();
            if (User != null)
            {
                list = terminallocBO.GetEntities(line);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 更新绑定信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("updateTerminalfeedingloc")]
        public void updateTerminalfeedingloc(PM_BPM_TERMINAL_FEEDING_LOC definitions)
        {
            definitions.CreatedOn = SSGlobalConfig.Now;
            terminallocBO.Update(definitions);
        }

        /// <summary>
        /// 删除绑定的信息
        /// </summary>
        /// <param name="ID"></param>
        [HttpDelete]
        [Route("deleteTerminalfeedingloc")]
        public void deleteTerminalfeedingloc(string TerminalID)
        {
            IList<PM_BPM_TERMINAL_FEEDING_LOC> list = new List<PM_BPM_TERMINAL_FEEDING_LOC>();
            PM_BPM_TERMINAL_FEEDING_LOC_QueryParam qp = new PM_BPM_TERMINAL_FEEDING_LOC_QueryParam();
            qp.TerminalID = TerminalID.ToString();
            qp.PageIndex = 0;
            qp.PageSize = 10;
            try
            {
                list = this.GetTerminalfeedingloc(qp);
                if (list!=null && list.Count > 0)
                {
                    int id = (int)list[0].ID;
                    terminallocBO.Delete(id);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 添加投料口与工位的绑定
        /// </summary>
        /// <param name="User">
        [HttpPost]
        [Route("addTerminalfeedingloc")] 
        public HttpResponseMessage addTerminalfeedingloc(PM_BPM_TERMINAL_FEEDING_LOC definitions)
        {
            definitions.CreatedOn = SSGlobalConfig.Now;
            PM_BPM_TERMINAL_FEEDING_LOC bpmExt = terminallocBO.Insert(definitions);
            if (bpmExt != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
        #endregion


        #region 工位与工段
        /// <summary>
        /// 查询投料口与工位绑定的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("getAllTerminalsection")]
        public IList<PM_BPM_TERMINAL_SECTION> getAllTerminalsection() //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_TERMINAL_SECTION> list = new List<PM_BPM_TERMINAL_SECTION>();
            list = terminalsecBO.GetAll();
            return list;
        }


        /// <summary>
        /// 查询投料口与工位绑定的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("getTerminalsection")]         
        public IList<PM_BPM_TERMINAL_SECTION> getTerminalsection(PM_BPM_TERMINAL_SECTION_QueryParam terminalsec) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_TERMINAL_SECTION> list = new List<PM_BPM_TERMINAL_SECTION>();
            if (terminalsec != null)
            {
                list = terminalsecBO.GetEntities(terminalsec);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 更新绑定的信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("updateTerminalsection")]
        public void updateTerminalsection(PM_BPM_TERMINAL_SECTION terminalsec)
        {
            terminalsec.CreatedOn = SSGlobalConfig.Now;
            terminalsecBO.Update(terminalsec);
        }

        /// <summary>
        /// 删除绑定的信息
        /// </summary>
        /// <param name="ID"></param>
        [HttpDelete]
        [Route("deleteTerminalsection")]
        public void deleteTerminalsection(Guid TerminalGuid)
        {
            PM_BPM_TERMINAL_SECTION_QueryParam pq = new PM_BPM_TERMINAL_SECTION_QueryParam();
            pq.TerminalGuid = TerminalGuid;
            IList<PM_BPM_TERMINAL_SECTION> list = terminalsecBO.GetEntities(pq);
            try
            {
                if (list!=null && list.Count > 0)
                {
                    Guid terminal = (Guid)list[0].TermSectionGuid;
                    terminalsecBO.Delete(terminal);
                }
            }
            catch (Exception e)
            {
                throw e;
            }            
        }


        /// <summary>
        /// 添加工段与工位的绑定
        /// </summary>
        /// <param name="User">
        [HttpPost]
        [Route("addTerminalsection")]
        public HttpResponseMessage addTerminalsection(PM_BPM_TERMINAL_SECTION definitions)
        {
            definitions.CreatedOn = SSGlobalConfig.Now;
            definitions.TermSectionGuid = Guid.NewGuid();
            PM_BPM_TERMINAL_SECTION bpmExt = terminalsecBO.Insert(definitions);
            if (bpmExt != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
        #endregion


        #region 工位与工序
        [HttpPost]
        [Route("getAllTerminalstep")]

        /// <summary>
        /// 查询投料口与工位绑定的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<PM_BPM_STEP_TERMINAL> getAllTerminalstep() //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_STEP_TERMINAL> list = new List<PM_BPM_STEP_TERMINAL>();
            list = terminalstepBO.GetAll();
            return list;
        }


        [HttpPost]
        [Route("getTerminalstep")]
        /// <summary>
        /// 查询工位与工序绑定的信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        public IList<PM_BPM_STEP_TERMINAL> getTerminalstep(PM_BPM_STEP_TERMINAL_QueryParam terminalstep) //传入的参数是对象，用Post，不能用Get
        {
            IList<PM_BPM_STEP_TERMINAL> list = new List<PM_BPM_STEP_TERMINAL>();
            if (terminalstep != null)
            {
                list = terminalstepBO.GetEntities(terminalstep);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 更新绑定的信息
        /// </summary>
        /// <param name="definitions"></param>
        [HttpPost]
        [Route("updateTerminalstep")]
        public void updateTerminalstep(PM_BPM_STEP_TERMINAL terminalstep)
        {
            IList<PM_BPM_STEP_TERMINAL> list = new List<PM_BPM_STEP_TERMINAL>();
            Guid stepterminal = (Guid)terminalstep.TerminalGuid;
            PM_BPM_STEP_TERMINAL_QueryParam terminalstep1 =new PM_BPM_STEP_TERMINAL_QueryParam();
            terminalstep1.TerminalGuid = stepterminal;
            list = terminalstepBO.GetEntities(terminalstep1);
            terminalstep.StepTerminalGuid = list[0].StepTerminalGuid;
            terminalstep.CreatedOn = SSGlobalConfig.Now;
            terminalstepBO.Update(terminalstep);
        }

        /// <summary>
        /// 删除绑定的信息
        /// </summary>
        /// <param name="ID"></param>
        [HttpDelete]
        [Route("deleteTerminalstep")]
        public void deleteTerminalstep(Guid TerminalGuid)
        {
            PM_BPM_STEP_TERMINAL_QueryParam pq = new PM_BPM_STEP_TERMINAL_QueryParam();
            pq.TerminalGuid = TerminalGuid;
            IList<PM_BPM_STEP_TERMINAL> list = terminalstepBO.GetEntities(pq);
            try
            {
                if (list != null && list.Count > 0)
                {
                    Guid terminal = (Guid)list[0].StepTerminalGuid;
                    terminalstepBO.Delete(terminal);
                }
            }
            catch (Exception e)
            {                
                throw e;
            }           
        }

        [HttpPost]
        [Route("addTerminalstep")]
        /// <summary>
        /// 添加工序与工位的绑定
        /// </summary>
        /// <param name="User"></p>
        public HttpResponseMessage addTerminalstep(PM_BPM_STEP_TERMINAL terminalstep)
        {
            terminalstep.CreatedOn = SSGlobalConfig.Now;
            terminalstep.StepTerminalGuid = Guid.NewGuid();
            PM_BPM_STEP_TERMINAL bpmExt = terminalstepBO.Insert(terminalstep);
            if (bpmExt != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "新增成功");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "新增失败");
            }
        }
        #endregion   

    }
}