
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
    public class CV_PM_ALT_NOTIBO : ICV_PM_ALT_NOTIBO
    {
        private ICV_PM_ALT_NOTIDAO cV_PM_ALT_NOTIDAO;

        public CV_PM_ALT_NOTIBO()
        {
            cV_PM_ALT_NOTIDAO = ObjectContainer.BuildUp<ICV_PM_ALT_NOTIDAO>();
        }
        public IList<CV_PM_ALT_NOTI> GetEntityByAlertID(Guid alertID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_NOTI> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("AlertID", alertID);
                af.AddFilter(mf);


                entities = cV_PM_ALT_NOTIDAO.Find(0, -1, af, null, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        
        }
    }
}
