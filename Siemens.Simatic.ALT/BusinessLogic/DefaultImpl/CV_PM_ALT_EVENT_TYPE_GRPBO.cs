
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
    public class CV_PM_ALT_EVENT_TYPE_GRPBO : ICV_PM_ALT_EVENT_TYPE_GRPBO
    {
        private ICV_PM_ALT_EVENT_TYPE_GRPDAO _CV_PM_ALT_EVENT_TYPE_GRPDAO;

        public CV_PM_ALT_EVENT_TYPE_GRPBO()
        {
            _CV_PM_ALT_EVENT_TYPE_GRPDAO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_TYPE_GRPDAO>();
        }
        //
        public IList<CV_PM_ALT_EVENT_TYPE_GRP> GetEntities(Guid eventTypeID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_EVENT_TYPE_GRP> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("EventTypeID", eventTypeID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("GroupName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_EVENT_TYPE_GRPDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
