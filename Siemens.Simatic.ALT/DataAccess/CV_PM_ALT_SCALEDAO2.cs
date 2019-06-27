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
    public partial interface ICV_PM_ALT_SCALEDAO : IDataAccessor<CV_PM_ALT_SCALE>
    {
        CV_PM_ALT_SCALE GetEntity(Guid criterionID, string scales);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class CV_PM_ALT_SCALEDAO : ICV_PM_ALT_SCALEDAO
    {
        public CV_PM_ALT_SCALE GetEntity(Guid criterionID, string scales)
        {
            CV_PM_ALT_SCALE entity = null;
            //
            try
            {
                string sql = "select * from CV_PM_ALT_SCALE where CriterionID = @CriterionID and Scales = @Scales";

                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);

                db.AddInParameter(dbCommand, "@CriterionID", DbType.Guid, criterionID);
                db.AddInParameter(dbCommand, "@Scales", DbType.String, scales);
                //
                using (IDataReader dataReader = db.ExecuteReader(dbCommand))
                {
                    if (dataReader.Read())
                    {
                        entity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
            //
            return entity;
        }
    }
}