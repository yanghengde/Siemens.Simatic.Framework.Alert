
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
    public class CV_PM_ALT_EVENT_LOGBO : ICV_PM_ALT_EVENT_LOGBO
    {
        private ICV_PM_ALT_EVENT_LOGDAO _CV_PM_ALT_EVENT_LOGDAO;
        //
        public CV_PM_ALT_EVENT_LOGBO()
        {
            _CV_PM_ALT_EVENT_LOGDAO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_LOGDAO>();
        }
        //
        //
        public IList<CV_PM_ALT_EVENT_LOG> GetEntities(CV_PM_ALT_EVENT_LOGQueryParam qp)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_EVENT_LOG> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("EventPriority", Sort.Direction.ASC);
                sort.OrderBy("CreatedOn", Sort.Direction.ASC);
                //
                entities = _CV_PM_ALT_EVENT_LOGDAO.Find(0, -1, this.CreateFilter(qp), sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        private IFilter CreateFilter(CV_PM_ALT_EVENT_LOGQueryParam qp)
        {
            ArgumentValidator.CheckForNullArgument(qp, "CV_PM_ALT_EVENT_LOGQueryParam");

            AndFilter af = new AndFilter();
            MatchingFilter mf = new MatchingFilter();

            if (!qp.IsDefaultValue("CreatedOn"))
            {
                mf.AddGreatEqualThan("CreatedOn", qp.CreatedOn.Value.Date);
                mf.AddLessThan("CreatedOn", qp.CreatedOn.Value.Date.AddDays(1));
            }

            if (!qp.IsDefaultValue("EventTypeID"))
                mf.AddMatching("EventTypeID", qp.EventTypeID);

            if (!qp.IsDefaultValue("EventTypeName"))
                mf.AddMatching("EventTypeName", qp.EventTypeName);

            if (!qp.IsDefaultValue("IsFinished"))
                mf.AddMatching("IsFinished", qp.IsFinished);

            if (!qp.IsDefaultValue("CreatedBy"))
                mf.AddMatching("CreatedBy", qp.CreatedBy);

            //
            af.AddFilter(mf);
            //
            return af;
        }
    }
}
