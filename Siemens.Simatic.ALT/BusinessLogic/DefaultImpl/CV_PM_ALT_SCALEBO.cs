
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
    public class CV_PM_ALT_SCALEBO : ICV_PM_ALT_SCALEBO
    {
        private ICV_PM_ALT_SCALEDAO _CV_PM_ALT_SCALEDAO;

        public CV_PM_ALT_SCALEBO()
        {
            _CV_PM_ALT_SCALEDAO = ObjectContainer.BuildUp<ICV_PM_ALT_SCALEDAO>();
        }
        //
        public CV_PM_ALT_SCALE GetEntity(Guid criterionID, string scales)
        {
            return _CV_PM_ALT_SCALEDAO.GetEntity(criterionID, scales);
        }

    }
}
