
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
    public class CV_PM_ALT_BASEBO : ICV_PM_ALT_BASEBO
    {
        private ICV_PM_ALT_BASEDAO _CV_PM_ALT_BASEDAO;

        public CV_PM_ALT_BASEBO()
        {
            _CV_PM_ALT_BASEDAO = ObjectContainer.BuildUp<ICV_PM_ALT_BASEDAO>();
        }

        public CV_PM_ALT_BASE GetEntity(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("AlertName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_BASEDAO.Find(0, -1, af, sort, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public CV_PM_ALT_BASE GetEntity(string alertName)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertName", alertName);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_BASEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_BASE> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_BASE> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("AlertName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_BASEDAO.Find(0, -1, null, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        public IList<CV_PM_ALT_BASE> GetEntities(CV_PM_ALT_BASE entity)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                if (!string.IsNullOrEmpty(entity.Category))
                {
                    mf.AddLike("Category", entity.Category);
                }
               
                if (!string.IsNullOrEmpty(entity.AlertName))
                {
                    mf.AddLike("AlertName", entity.AlertName);
                }
                if (!string.IsNullOrEmpty(entity.AlertAlias))
                {
                    mf.AddLike("AlertAlias", entity.AlertAlias);
                }
                if (!string.IsNullOrEmpty(entity.AlertDesc))
                {
                    mf.AddLike("AlertDesc", entity.AlertDesc);

                }
                if (!string.IsNullOrEmpty(entity.AlertContent))
                {
                    mf.AddLike("AlertContent", entity.AlertContent);
                }
                if (entity.AlertType.HasValue && entity.AlertType != 0)
                {
                    mf.AddMatching("AlertType", entity.AlertType);
                }
                if (!string.IsNullOrEmpty(entity.Format))
                {
                    mf.AddMatching("Format", entity.Format);
                }
                if (!string.IsNullOrEmpty(entity.AlertObject))
                {
                    mf.AddMatching("AlertObject", entity.AlertObject);
                }
                if (!string.IsNullOrEmpty(entity.PreProcedure))
                {
                    mf.AddMatching("PreProcedure", entity.PreProcedure);
                }
                if (!string.IsNullOrEmpty(entity.PostProcedure))
                {
                    mf.AddMatching("PostProcedure", entity.PostProcedure);
                }
                if (entity.AlertInterval.HasValue && entity.AlertInterval != 0)
                {
                    mf.AddMatching("AlertInterval", entity.Category);
                }
                if (!string.IsNullOrEmpty(entity.AlertTimePoints))
                {
                    mf.AddMatching("AlertTimePoints", entity.AlertTimePoints);
                }
                //if (entity.LastAlertedTime.HasValue)
                //{
                //    mf.AddMatching("LastAlertedTime", entity.LastAlertedTime);
                //}
                if (entity.IsActive.HasValue)
                {
                    mf.AddMatching("IsActive", entity.IsActive);
                }
                if (entity.RowDeleted.HasValue)
                {
                    mf.AddMatching("RowDeleted", entity.RowDeleted);
                }
                if (!string.IsNullOrEmpty(entity.CreatedBy))
                {
                    mf.AddMatching("CreatedBy", entity.CreatedBy);
                }
                //if (entity.CreatedOn.HasValue)
                //{
                //    mf.AddMatching("CreatedOn", entity.CreatedOn);
                //}
                if (!string.IsNullOrEmpty(entity.ModifiedBy))
                {
                    mf.AddMatching("ModifiedBy", entity.ModifiedBy);
                }
                //if (entity.ModifiedOn.HasValue)
                //{
                //    mf.AddMatching("ModifiedOn", entity.ModifiedOn);
                //}

                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("AlertName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_BASEDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }


        #region IDataList
        public IList<CV_PM_ALT_BASE> GetDataList(Dictionary<string, object> filter)
        {
            if (filter == null || filter.Count <= 0)
                return this.GetEntities();

            //
            long totalRecords = 0;
            IList<CV_PM_ALT_BASE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                foreach (string key in filter.Keys)
                {
                    mf.AddMatching(key, filter[key]);
                }
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("AlertName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_BASEDAO.Find(0, -1, af, sort, out totalRecords);
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
