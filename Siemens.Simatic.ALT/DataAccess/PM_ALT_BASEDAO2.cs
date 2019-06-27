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
    public partial interface IPM_ALT_BASEDAO : IDataAccessor<PM_ALT_BASE>
    {
        DataTable Run(string sql);
        bool CheckDatabaseObject(string databaseObject);
        bool CallProcedure(string procName);
        string DuplicateAlert(string newAlertName, string oldAlertName, string updatedBy);
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    public partial class PM_ALT_BASEDAO : IPM_ALT_BASEDAO
    {
        public DataTable Run(string sql)
        {
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                //
                DataSet ds = db.ExecuteDataSet(dbCommand);
                if (ds != null && ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
            //
            return null;
        }

        public bool CheckDatabaseObject(string databaseObject)
        {
            IList<DataFilterField> entities = new List<DataFilterField>();
            string sql = string.Format("select top 1 * from {0}", databaseObject);
            //
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);

                DataSet ds = db.ExecuteDataSet(dbCommand);
            }
            catch (Exception ex)
            {
                return false;
            }
            //
            return true;
        }

        public bool CallProcedure(string procName)
        {
            string result = String.Empty;
            //
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetStoredProcCommand(procName);

                db.ExecuteNonQuery(dbCommand);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return true;
        }

        public string DuplicateAlert(string newAlertName, string oldAlertName,string updatedBy)
        {
            string sql = @"DECLARE @ReturnMsg nvarchar(255)
                           EXEC dbo.CP_BSC_ALERT_DUPLICATE  @NewAlertName,
                                                            @OldAlertName,
                                                            @UpdatedBy,@ReturnMsg OUTPUT
                           SELECT @ReturnMsg";
            string result = String.Empty;
            //
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);

                db.AddInParameter(dbCommand, "@NewAlertName", DbType.String, newAlertName);
                db.AddInParameter(dbCommand, "@OldAlertName", DbType.String, oldAlertName);
                db.AddInParameter(dbCommand, "@UpdatedBy", DbType.String, updatedBy);

                DataSet ds = db.ExecuteDataSet(dbCommand);

                return ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return null;
        }
    }
}