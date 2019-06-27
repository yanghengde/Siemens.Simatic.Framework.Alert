
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
    public class CV_PM_ALT_CRITERION_CONDBO : ICV_PM_ALT_CRITERION_CONDBO
    {
        private ICV_PM_ALT_CRITERION_CONDDAO _CV_PM_ALT_CRITERION_CONDDAO;

        public CV_PM_ALT_CRITERION_CONDBO()
        {
            _CV_PM_ALT_CRITERION_CONDDAO = ObjectContainer.BuildUp<ICV_PM_ALT_CRITERION_CONDDAO>();
        }
        //
        public CV_PM_ALT_CRITERION_COND GetEntity(Guid conditionID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_COND> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("ConditionID", conditionID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_CRITERION_CONDDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_CRITERION_COND> GetEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_COND> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERION_CONDDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_ALT_CRITERION_COND> GetActiveEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION_COND> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("IsActive", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERION_CONDDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
