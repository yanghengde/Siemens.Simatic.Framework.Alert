using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data.DataAccess;
using System.Data.SqlClient;
using System.Data;

namespace Siemens.Simatic.Util.Utilities.DAO
{
    public class UtilDAO
    {
        internal static DateTime? GetDatabaseDatetime()
        {
            DateTime? now = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand("select getdate()");

                now = (DateTime?)db.ExecuteScalar(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return now;
        }

        public static DateTime? GetDatabaseUtcDatetime()
        {
            DateTime? now = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand("select getUtcdate()");

                now = (DateTime?)db.ExecuteScalar(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return now;
        }

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(Platform.Data.DatabaseEnum.SITBusinessDB); //用SITBusinessDB的连接
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="TableName"></param>
        /// <param name="dt"></param>
        public static void InsertEntities(string connectionString, string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                //增加事务
                SqlTransaction trans = conn.BeginTransaction();
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                        trans.Commit();
                        conn.Close();
                    }
                    catch (System.Exception ex)
                    {
                        trans.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
            }
        }



    }
}
