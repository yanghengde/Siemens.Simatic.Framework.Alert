
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public class CV_PM_WECHAT_AGENTBO : ICV_PM_WECHAT_AGENTBO
    {
        private ICV_PM_WECHAT_AGENTDAO cV_PM_WECHAT_AGENTDAO;

        public CV_PM_WECHAT_AGENTBO()
        {
            cV_PM_WECHAT_AGENTDAO = ObjectContainer.BuildUp<ICV_PM_WECHAT_AGENTDAO>();
        }
        public IList<CV_PM_WECHAT_AGENT> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_AGENT> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("AgentID", Sort.Direction.ASC);
                sort.OrderBy("Name", Sort.Direction.ASC);

                entities = cV_PM_WECHAT_AGENTDAO.Find(0, -1, null, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
