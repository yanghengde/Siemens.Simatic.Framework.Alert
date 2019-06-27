
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
    public class CV_PM_ALT_NOTI_GROUPBO : ICV_PM_ALT_NOTI_GROUPBO
    {
        private ICV_PM_ALT_NOTI_GROUPDAO _CV_PM_ALT_NOTI_GROUPDAO;

        public CV_PM_ALT_NOTI_GROUPBO()
        {
            _CV_PM_ALT_NOTI_GROUPDAO = ObjectContainer.BuildUp<ICV_PM_ALT_NOTI_GROUPDAO>();
        }
        //
        public CV_PM_ALT_NOTI_GROUP GetEntity(Guid groupID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("NotiGroupID", groupID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_NOTI_GROUPDAO.Find(0, -1, af, null, out totalRecords);
                if(entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public CV_PM_ALT_NOTI_GROUP GetEntity(string groupName)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("GroupName", groupName);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_NOTI_GROUPDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_NOTI_GROUP> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("GroupName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_NOTI_GROUPDAO.Find(0, -1, null, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }

        //
        #region IDataList
        public IList<CV_PM_ALT_NOTI_GROUP> GetDataList(Dictionary<string, object> filter)
        {
            if (filter == null || filter.Count <= 0)
                return this.GetEntities();

            //
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP> entities = null;

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
                sort.OrderBy("GroupLevel", Sort.Direction.ASC);
                sort.OrderBy("GroupCode", Sort.Direction.ASC);

                entities = _CV_PM_ALT_NOTI_GROUPDAO.Find(0, -1, af, sort, out totalRecords);
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
