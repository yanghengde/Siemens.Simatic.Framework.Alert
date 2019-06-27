
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
    public class CV_PM_ALT_USERBO : ICV_PM_ALT_USERBO
    {
        private ICV_PM_ALT_USERDAO _CV_PM_ALT_USERDAO;

        public CV_PM_ALT_USERBO()
        {
            _CV_PM_ALT_USERDAO = ObjectContainer.BuildUp<ICV_PM_ALT_USERDAO>();
        }
        //
        public IList<CV_PM_ALT_USER> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_USER> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("IsActiveEmployee", true);
                af.AddFilter(mf);

                Sort sort = new Sort();
                sort.OrderBy("EmployeeCode", Sort.Direction.ASC);

                entities = _CV_PM_ALT_USERDAO.Find(0, -1, af, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
