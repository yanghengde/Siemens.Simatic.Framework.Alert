
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
    public class CV_PM_ALT_CRITERIONBO : ICV_PM_ALT_CRITERIONBO
    {
        private ICV_PM_ALT_CRITERIONDAO _CV_PM_ALT_CRITERIONDAO;

        public CV_PM_ALT_CRITERIONBO()
        {
            _CV_PM_ALT_CRITERIONDAO = ObjectContainer.BuildUp<ICV_PM_ALT_CRITERIONDAO>();
        }
        //
        public CV_PM_ALT_CRITERION GetEntity(Guid criterionID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("CriterionID", criterionID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_CRITERIONDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_CRITERION> GetEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERIONDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_ALT_CRITERION> GetActiveEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_CRITERION> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("IsActive", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_CRITERIONDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
