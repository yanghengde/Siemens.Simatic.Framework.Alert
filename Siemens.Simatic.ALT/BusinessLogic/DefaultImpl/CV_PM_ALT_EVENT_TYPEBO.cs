
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
    public class CV_PM_ALT_EVENT_TYPEBO : ICV_PM_ALT_EVENT_TYPEBO
    {
        private ICV_PM_ALT_EVENT_TYPEDAO _CV_PM_ALT_EVENT_TYPEDAO;

        public CV_PM_ALT_EVENT_TYPEBO()
        {
            _CV_PM_ALT_EVENT_TYPEDAO = ObjectContainer.BuildUp<ICV_PM_ALT_EVENT_TYPEDAO>();
        }
        //
        public CV_PM_ALT_EVENT_TYPE GetEntity(Guid typeID)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_EVENT_TYPE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("EventTypeID", typeID);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_EVENT_TYPEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public CV_PM_ALT_EVENT_TYPE GetEntity(string typeName)
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_EVENT_TYPE> entities = null;

            try
            {
                AndFilter af = new AndFilter();
                MatchingFilter mf = new MatchingFilter();
                mf.AddMatching("EventTypeName", typeName);
                af.AddFilter(mf);

                entities = _CV_PM_ALT_EVENT_TYPEDAO.Find(0, -1, af, null, out totalRecords);
                if (entities.Count > 0)
                    return entities[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return null;
        }

        public IList<CV_PM_ALT_EVENT_TYPE> GetEntities()
        {
            long totalRecords = 0;
            IList<CV_PM_ALT_EVENT_TYPE> entities = null;

            try
            {
                Sort sort = new Sort();
                sort.OrderBy("Category", Sort.Direction.ASC);
                sort.OrderBy("EventTypeName", Sort.Direction.ASC);

                entities = _CV_PM_ALT_EVENT_TYPEDAO.Find(0, -1, null, sort, out totalRecords);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.BusinessLogicDefaultPolicy);
            }

            return entities;
        }
    }
}
