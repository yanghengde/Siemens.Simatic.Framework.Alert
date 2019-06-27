
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
    public class CV_PM_ALT_NOTI_GROUP_DETAILBO : ICV_PM_ALT_NOTI_GROUP_DETAILBO
    {
        private ICV_PM_ALT_NOTI_GROUP_DETAILDAO _CV_PM_ALT_NOTI_GROUP_DETAILDAO;

        public CV_PM_ALT_NOTI_GROUP_DETAILBO()
        {
            _CV_PM_ALT_NOTI_GROUP_DETAILDAO = ObjectContainer.BuildUp<ICV_PM_ALT_NOTI_GROUP_DETAILDAO>();
        }
        //
        public CV_PM_ALT_NOTI_GROUP_DETAIL GetEntity(Guid groupDtlID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP_DETAIL> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("NotiGroupDetailID", groupDtlID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_NOTI_GROUP_DETAILDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_NOTI_GROUP_DETAIL> GetEntitiesByGroup(Guid groupID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI_GROUP_DETAIL> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("NotiGroupID", groupID);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("MemberID", Sort.Direction.ASC);

                entities = _CV_PM_ALT_NOTI_GROUP_DETAILDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
