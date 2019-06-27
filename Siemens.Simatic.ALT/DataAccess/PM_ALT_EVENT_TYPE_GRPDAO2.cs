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
    public partial interface IPM_ALT_EVENT_TYPE_GRPDAO : IDataAccessor<PM_ALT_EVENT_TYPE_GRP>
    {
        void DeletedByType(Guid typeID);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_EVENT_TYPE_GRPDAO : IPM_ALT_EVENT_TYPE_GRPDAO
    {
        public void DeletedByType(Guid typeID)
        {
            try
            {
                string sql = @" delete from dbo.PM_ALT_EVENT_TYPE_GRP where EventTypeID = @EventTypeID";

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                db.AddInParameter(dbCommand, "@EventTypeID", DbType.Guid, typeID);
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