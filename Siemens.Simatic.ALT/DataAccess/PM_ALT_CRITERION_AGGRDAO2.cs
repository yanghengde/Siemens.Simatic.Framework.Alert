﻿using System;
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

namespace Siemens.Simatic.ALT.DataAccess
{
    public partial interface IPM_ALT_CRITERION_AGGRDAO : IDataAccessor<PM_ALT_CRITERION_AGGR>
    {
        void SetDeletedByAlert(Guid alertID);
        void ClearDeletedByAlert(Guid alertID);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_CRITERION_AGGRDAO : IPM_ALT_CRITERION_AGGRDAO
    {
        public void SetDeletedByAlert(Guid alertID)
        {
            try
            {
                string sql = string.Format(@" update dbo.PM_ALT_CRITERION_AGGR set RowDeleted = 1 where AlertID = @AlertID", alertID);

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);

                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, alertID);
                //
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        public void ClearDeletedByAlert(Guid alertID)
        {
            try
            {
                string sql = @" delete from dbo.PM_ALT_CRITERION_AGGR where AlertID = @AlertID and RowDeleted = 1";

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, alertID);
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