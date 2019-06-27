using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Globalization;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Core.ORMapping;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;
using Siemens.Simatic.Platform.Data.DataAccess;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.Common.Persistence;

namespace Siemens.Simatic.ALT.DataAccess
{
    public partial interface IPM_ALT_LOGDAO : IDataAccessor<PM_ALT_LOG>
    {
        bool HasOpenEntityBySubject(Guid alertID, string logSubject);
        bool HasOpenEntityByContent(Guid alertID, string logContent);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_LOGDAO : IPM_ALT_LOGDAO
    {
        public bool HasOpenEntityBySubject(Guid alertID, string logSubject)
        {
            try
            {
                string sql = string.Format(@" select top 1 1 from PM_ALT_LOG where AlertID = '{0}' and LogContent = '{1}' and IsClosed = 0 and RowDeleted = 0", alertID, logSubject);

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                object objRlt = db.ExecuteScalar(dbCommand);
                if (objRlt == null || objRlt == DBNull.Value)
                    return false;

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
            //
            return true;
        }

        public bool HasOpenEntityByContent(Guid alertID, string logContent)
        {
            try
            {
                string sql = string.Format(@" select top 1 1 from PM_ALT_LOG where AlertID = '{0}' and AlertDesc = '{1}' and IsClosed = 0 and RowDeleted = 0", alertID, logContent);

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                object objRlt = db.ExecuteScalar(dbCommand);
                if (objRlt == null || objRlt == DBNull.Value)
                    return false;

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
            //
            return true;
        }
    }
}