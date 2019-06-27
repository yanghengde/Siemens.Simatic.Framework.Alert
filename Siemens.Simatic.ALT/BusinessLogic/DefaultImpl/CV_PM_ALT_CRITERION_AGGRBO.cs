
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
    public class CV_PM_ALT_CRITERION_AGGRBO : ICV_PM_ALT_CRITERION_AGGRBO
    {
        private ICV_PM_ALT_CRITERION_AGGRDAO _CV_PM_ALT_CRITERION_AGGRDAO;

        public CV_PM_ALT_CRITERION_AGGRBO()
        {
            _CV_PM_ALT_CRITERION_AGGRDAO = ObjectContainer.BuildUp<ICV_PM_ALT_CRITERION_AGGRDAO>();
        }
        //
        public CV_PM_ALT_CRITERION_AGGR GetEntity(Guid aggregationID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_AGGR> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AggregationID", aggregationID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_CRITERION_AGGRDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_CRITERION_AGGR> GetEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_AGGR> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERION_AGGRDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_ALT_CRITERION_AGGR> GetActiveEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_AGGR> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("IsActive", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERION_AGGRDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
