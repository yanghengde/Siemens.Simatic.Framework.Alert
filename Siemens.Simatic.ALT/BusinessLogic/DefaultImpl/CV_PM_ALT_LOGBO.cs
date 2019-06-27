
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
    public class CV_PM_ALT_LOGBO : ICV_PM_ALT_LOGBO
    {
        private ICV_PM_ALT_LOGDAO _CV_PM_ALT_LOGDAO;

        public CV_PM_ALT_LOGBO()
        {
            _CV_PM_ALT_LOGDAO = ObjectContainer.BuildUp<ICV_PM_ALT_LOGDAO>();
        }
        //
        public CV_PM_ALT_LOG GetEntity(long logPK)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_LOG> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("LogPK", logPK);

                entities = _CV_PM_ALT_LOGDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_LOG> GetEntities(CV_PM_ALT_LOGQueryParam qp)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_LOG> entities = null;

            try
            {
                AndFilter af = this.CreateFilter(qp) as AndFilter;

                Sort sort = new Sort();
                sort.OrderBy("CreatedBy", Sort.Direction.ASC);

                entities = _CV_PM_ALT_LOGDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        private IFilter CreateFilter(CV_PM_ALT_LOGQueryParam qp)
        {
            ArgumentValidator.CheckForNullArgument(qp, "CV_PM_ALT_LOGQueryParam");

            AndFilter af = new AndFilter();
            MatchingFilter mf = new MatchingFilter();

            if (!qp.IsDefaultValue("AlertAlias"))
                mf.AddMatching("AlertAlias", qp.AlertAlias);

            if (!qp.IsDefaultValue("LogTitle"))
                mf.AddLike("LogTitle", qp.LogTitle);

            if (!qp.IsDefaultValue("LogContent"))
                mf.AddLike("LogContent", qp.LogContent);

            if (!qp.IsDefaultValue("RespondedBy"))
                mf.AddMatching("RespondedBy", qp.RespondedBy);

            if (!qp.IsDefaultValue("IsClosed"))
                mf.AddMatching("IsClosed", qp.IsClosed.Value);

            if (!qp.IsDefaultValue("Z_RespondedOnBegin"))
                mf.AddGreatEqualThan("RespondedOn", qp.Z_RespondedOnBegin.Value);

            if (!qp.IsDefaultValue("Z_RespondedOnEnd"))
                mf.AddLessEqualThan("RespondedOn", qp.Z_RespondedOnEnd.Value);

            af.AddFilter(mf);

            return af;
        }
    }
}
