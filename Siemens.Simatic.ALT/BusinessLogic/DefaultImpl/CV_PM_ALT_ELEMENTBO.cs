
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
    public class CV_PM_ALT_ELEMENTBO : ICV_PM_ALT_ELEMENTBO
    {
        private ICV_PM_ALT_ELEMENTDAO _CV_PM_ALT_ELEMENTDAO;

        public CV_PM_ALT_ELEMENTBO()
        {
            _CV_PM_ALT_ELEMENTDAO = ObjectContainer.BuildUp<ICV_PM_ALT_ELEMENTDAO>();
        }
        //
        public CV_PM_ALT_ELEMENT GetEntity(Guid elementID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_ELEMENT> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("ElementID", elementID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_ELEMENTDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public CV_PM_ALT_ELEMENT GetEntity(Guid alertID, string column)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_ELEMENT> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("ElementColumn", column);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_ELEMENTDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_ELEMENT> GetEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_ELEMENT> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_ELEMENTDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_ALT_ELEMENT> GetActiveEntitiesByAlert(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_ELEMENT> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                mf.AddMatching("IsActive", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_ELEMENTDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        //
        #region IDataList
        public IList<CV_PM_ALT_ELEMENT> GetDataList(Dictionary<string, object> filter)
        {
            if (filter == null || filter.Count <= 0)
                return new List<CV_PM_ALT_ELEMENT>();

            //
            long totalRecords = 0;
            IList<CV_PM_ALT_ELEMENT> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                foreach (string key in filter.Keys)
                {
                    mf.AddMatching(key, filter[key]);
                }
                //
                mf.AddMatching("IsActive", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("Sequence", Sort.Direction.ASC);

                entities = _CV_PM_ALT_ELEMENTDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
        #endregion
    }
}
