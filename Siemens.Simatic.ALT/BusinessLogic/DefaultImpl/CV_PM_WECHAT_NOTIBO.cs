
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
    public class CV_PM_WECHAT_NOTIBO : ICV_PM_WECHAT_NOTIBO
    {
        private ICV_PM_WECHAT_NOTIDAO cV_PM_WECHAT_NOTIDAO;

        public CV_PM_WECHAT_NOTIBO()
        {
            cV_PM_WECHAT_NOTIDAO = ObjectContainer.BuildUp<ICV_PM_WECHAT_NOTIDAO>();
        }

        public IList<CV_PM_WECHAT_NOTI> GetEntity(string alertName)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_NOTI> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertName);
                af.AddFilter(mf);

                entities = cV_PM_WECHAT_NOTIDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;

        }

        public IList<CV_PM_WECHAT_NOTI> GetEntities(CV_PM_WECHAT_NOTI entity)
        {
            long totalRecords = 0;
            IList<CV_PM_WECHAT_NOTI> entities = new List<CV_PM_WECHAT_NOTI>();

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();

                if (entity.AlertID != null)
                {
                    mf.AddMatching("AlertID", entity.AlertID);
                }

                af.AddFilter(mf);

                entities = cV_PM_WECHAT_NOTIDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }





    }
}
