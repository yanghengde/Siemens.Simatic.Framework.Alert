using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;
using Siemens.Simatic.PM.BusinessLogic;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.PM.Common;
using Newtonsoft.Json;

namespace Siemens.Simatic.Web.PortalApi.Controllers.CO
{
    [RoutePrefix("api/terminal")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CV_BPM_TERMINALController : ApiController
    {
        #region Private Fields

        ILog log = LogManager.GetLogger(typeof(CV_BPM_TERMINALController));
        ICV_BPM_TERMINALBO terminalBO = ObjectContainer.BuildUp<ICV_BPM_TERMINALBO>();

        #endregion

        /// <summary>
        /// 查询所有工位  -  张婷-2017-11-22 10:37:23
        /// </summary>
        /// <returns>IList</returns>
        [Route("")]
        public IList<CV_BPM_TERMINAL> GetTerminal()
        {

            IList<CV_BPM_TERMINAL> list = new List<CV_BPM_TERMINAL>();
            list = terminalBO.GetAll();
            //log.Debug(JsonConvert.SerializeObject(list));
            return list;
        }

        [HttpPost]
        [Route("GetTerminals")]
        public IList<CV_BPM_TERMINAL> GetTerminals(CV_BPM_TERMINAL_QueryParam terminal)
        {
            IList<CV_BPM_TERMINAL> list = new List<CV_BPM_TERMINAL>();
            if (terminal != null)
            {
                //获取工位视图相关信息
                list = terminalBO.GetEntities(terminal);
                return list;
            }
            else
            {
                return null;
            }
        }


    }
}
