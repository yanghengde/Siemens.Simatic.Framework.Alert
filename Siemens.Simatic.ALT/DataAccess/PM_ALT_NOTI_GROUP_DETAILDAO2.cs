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
    public partial interface IPM_ALT_NOTI_GROUP_DETAILDAO : IDataAccessor<PM_ALT_NOTI_GROUP_DETAIL>
    {
        void SetDeletedByGroup(Guid groupID);
        void ClearDeletedByGroup(Guid groupID);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_NOTI_GROUP_DETAILDAO : IPM_ALT_NOTI_GROUP_DETAILDAO
    {
        public void SetDeletedByGroup(Guid groupID)
        {
            try
            {
                string sql = string.Format(@" update dbo.PM_ALT_NOTI_GROUP_DETAIL set RowDeleted = 1 where NotiGroupID = '{0}'", groupID);

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        public void ClearDeletedByGroup(Guid groupID)
        {
            try
            {
                string sql = @" delete from dbo.PM_ALT_NOTI_GROUP_DETAIL where NotiGroupID = @NotiGroupID and RowDeleted = 1";

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                db.AddInParameter(dbCommand, "@NotiGroupID", DbType.Guid, groupID);
                //
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }
    }
}