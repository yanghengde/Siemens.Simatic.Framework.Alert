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
    public partial interface IPM_ALT_ELEMENTDAO : IDataAccessor<PM_ALT_ELEMENT>
    {
        void SetDeletedByAlert(Guid alertID);
        void ClearDeletedByAlert(Guid alertID);
        IList<DataFilterField> GetSourceFieldsFromDatabase(string databaseObject);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_ELEMENTDAO : IPM_ALT_ELEMENTDAO
    {
        public void SetDeletedByAlert(Guid alertID)
        {
            try
            {
                string sql = string.Format(@" update dbo.PM_ALT_ELEMENT set RowDeleted = 1 where AlertID = @AlertID", alertID);

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
                string sql = @" delete from dbo.PM_ALT_ELEMENT where AlertID = @AlertID and RowDeleted = 1";

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

        public IList<DataFilterField> GetSourceFieldsFromDatabase(string databaseObject)
        {
            IList<DataFilterField> entities = new List<DataFilterField>();
            string sql = string.Format("select top 1 * from {0}", databaseObject);
            //
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);

                DataSet ds = db.ExecuteDataSet(dbCommand);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        DataFilterField dff = new DataFilterField();
                        dff.KeyField = dc.ColumnName;
                        dff.KeyType = dc.DataType.Name;
                        dff.IsAttribute = false;
                        dff.KeyDesc = string.Empty;
                        entities.Add(dff);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
            //
            return entities;
        }
    }
}