using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Globalization;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Core.ORMapping;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;
using Siemens.Simatic.Platform.Data.DataAccess;

namespace Siemens.Simatic.Util.Utilities
{
    public class SSLogManager
    {
        private static CO_SYSTEM_LOGDAO _co_SYSTEM_LOGDAO = new CO_SYSTEM_LOGDAO();
        private static string _systemArchitectureType = ConfigurationManager.AppSettings["SystemArchitectureType"];
        private static bool _systemLog = Convert.ToBoolean(ConfigurationManager.AppSettings["SystemLog"]); 

        public static void WriteLog(string logType, string logSource, LogSeverityEnum logSeverityEnum, string logContent, string logOperator)
        {
            if (_systemLog == false) return;
            //
            string hostName = String.Empty;
            string hostIP = String.Empty;
            string hostMac = String.Empty;

            if (string.IsNullOrEmpty(_systemArchitectureType))
            {
                throw new Exception("系统配置项[SystemArchitectureType]配置有误，请联系管理员！");
            }
            else if (_systemArchitectureType.ToUpper() == "CS")
            {
                hostName = System.Environment.UserDomainName + "\\" + System.Environment.MachineName;
                //
                System.Net.IPAddress[] addressList = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
                for (int i = 0; i < addressList.Length; i++)
                {
                    if (addressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        hostIP += ":" + addressList[i].ToString();
                }
                hostIP = hostIP.TrimStart(':');
                //
                System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                    {
                        hostMac = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }
            else if (_systemArchitectureType.ToUpper() == "BS")
            {
                hostName = System.Web.HttpContext.Current.Request.UserHostName;
                hostIP = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                throw new Exception("系统配置项[SystemArchitectureType]配置有误，请联系管理员！");
            }

            SSLogManager.WriteLog(logType, logSource,logSeverityEnum, logContent, logOperator, hostName, hostIP, hostMac);
        }

        public static void WriteLog(string logType, string logSource, LogSeverityEnum logSeverityEnum, string logContent, string logOperator, string hostName, string hostIP, string hostMac)
        {
            if (_systemLog == false) return;
            //
            CO_SYSTEM_LOG log = new CO_SYSTEM_LOG();
            log.LogGuid = Guid.NewGuid();
            log.LogType = logType;
            log.LogSource = logSource;
            log.LogSeverity = Convert.ToInt32(logSeverityEnum);
            log.LogContent = logContent;
            log.LogDate = DAO.UtilDAO.GetDatabaseDatetime();
            log.LogDateUtc = DAO.UtilDAO.GetDatabaseUtcDatetime();
            log.LogOperator = logOperator;
            log.HostName = hostName;
            log.HostIP = hostIP;
            log.HostMac = hostMac;

            SSLogManager.WriteLog(log);
        }

        public static void WriteLog(CO_SYSTEM_LOG log)
        {
            if (_systemLog == false) return;
            //
            _co_SYSTEM_LOGDAO.Insert(log);
        }
    }

    [Serializable]
    public class CO_SYSTEM_LOG
    {

        #region Private Fields

        private Guid? _LogGuid;
        private String _LogSource;
        private String _LogType;
        private Int32? _LogSeverity;
        private String _LogContent;
        private DateTime? _LogDate;
        private DateTime? _LogDateUtc;
        private String _LogOperator;
        private String _HostName;
        private String _HostIP;
        private String _HostMac;

        #endregion

        #region Public Properties

        public Guid? LogGuid
        {
            get { return _LogGuid; }
            set
            {
                _LogGuid = value;
            }
        }
        public String LogSource
        {
            get { return _LogSource; }
            set
            {
                _LogSource = value;
            }
        }
        public String LogType
        {
            get { return _LogType; }
            set
            {
                _LogType = value;
            }
        }
        public Int32? LogSeverity
        {
            get { return _LogSeverity; }
            set
            {
                _LogSeverity = value;
            }
        }
        public String LogContent
        {
            get { return _LogContent; }
            set
            {
                _LogContent = value;
            }
        }
        public DateTime? LogDate
        {
            get { return _LogDate; }
            set
            {
                _LogDate = value;
            }
        }
        public DateTime? LogDateUtc
        {
            get { return _LogDateUtc; }
            set
            {
                _LogDateUtc = value;
            }
        }
        public String LogOperator
        {
            get { return _LogOperator; }
            set
            {
                _LogOperator = value;
            }
        }
        public String HostName
        {
            get { return _HostName; }
            set
            {
                _HostName = value;
            }
        }
        public String HostIP
        {
            get { return _HostIP; }
            set
            {
                _HostIP = value;
            }
        }
        public String HostMac
        {
            get { return _HostMac; }
            set
            {
                _HostMac = value;
            }
        }

        #endregion
    }

    public class CO_SYSTEM_LOGDAO
    {


        #region SQl
        #region SqlInsert
        private const string SqlInsert = @"INSERT INTO dbo.CO_SYSTEM_LOG (
                                                [LogGuid],
                                                [LogSource],
                                                [LogType],
                                                [LogSeverity],
                                                [LogContent],
                                                [LogDate],
                                                [LogDateUtc],
                                                [LogOperator],
                                                [HostName],
                                                [HostIP],
                                                [HostMac]
                                                ) VALUES ( 
                                                @LogGuid,
                                                @LogSource,
                                                @LogType,
                                                @LogSeverity,
                                                @LogContent,
                                                @LogDate,
                                                @LogDateUtc,
                                                @LogOperator,
                                                @HostName,
                                                @HostIP,
                                                @HostMac
                                            )";
        #endregion

        #region SqlDelete
        private const string SqlDelete = @"DELETE FROM dbo.CO_SYSTEM_LOG 
                                           WHERE 1 = 1 
                                           AND LogGuid = @LogGuid";
        #endregion

        #region SqlUpdateAll
        private const string SqlUpdate = @"UPDATE dbo.CO_SYSTEM_LOG SET
                                            [LogSource] = @LogSource,
                                            [LogType] = @LogType,
                                            [LogSeverity] = @LogSeverity,
                                            [LogContent] = @LogContent,
                                            [LogDate] = @LogDate,
                                            [LogDateUtc] = @LogDateUtc,
                                            [LogOperator] = @LogOperator,
                                            [HostName] = @HostName,
                                            [HostIP] = @HostIP,
                                            [HostMac] = @HostMac
                                            WHERE 1 = 1
                                            AND [LogGuid] = @LogGuid";
        #endregion

        #region SqlGet
        private const string SqlGet = @"SELECT
                                            [LogGuid],
                                            [LogSource],
                                            [LogType],
                                            [LogSeverity],
                                            [LogContent],
                                            [LogDate],
                                            [LogDateUtc],
                                            [LogOperator],
                                            [HostName],
                                            [HostIP],
                                            [HostMac]
                                        FROM dbo.CO_SYSTEM_LOG
                                        WHERE 1 = 1 
                                        AND [LogGuid] = @LogGuid";
        #endregion

        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.CO_SYSTEM_LOG 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [LogGuid],
                                                    [LogSource],
                                                    [LogType],
                                                    [LogSeverity],
                                                    [LogContent],
                                                    [LogDate],
                                                    [LogDateUtc],
                                                    [LogOperator],
                                                    [HostName],
                                                    [HostIP],
                                                    [HostMac]
                                                FROM dbo.CO_SYSTEM_LOG 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [LogGuid],
                                                [LogSource],
                                                [LogType],
                                                [LogSeverity],
                                                [LogContent],
                                                [LogDate],
                                                [LogDateUtc],
                                                [LogOperator],
                                                [HostName],
                                                [HostIP],
                                                [HostMac]
                                           FROM dbo.CO_SYSTEM_LOG 
                                           WHERE ( 1 = 1 {1} 
                                           AND LogGuid 
                                           NOT IN 
                                           (SELECT TOP {2} LogGuid 
                                           FROM dbo.CO_SYSTEM_LOG 
                                           WHERE (1 = 1 {3}) 
                                           {4} ) ) {5} 
                                           ";
        #endregion SqlSelect
        #endregion SQL


        #region IDataAccessor members

        #region Insert
        /// <summary>
        /// Insert 
        /// </summary>  
        public CO_SYSTEM_LOG Insert(CO_SYSTEM_LOG entity)
        {
            //ArgumentValidator.CheckForNullArgument(entity, "entity");
            CO_SYSTEM_LOG CO_SYSTEM_LOGEntity = entity as CO_SYSTEM_LOG;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, CO_SYSTEM_LOGEntity.LogGuid);
                db.AddInParameter(dbCommand, "@LogSource", DbType.String, CO_SYSTEM_LOGEntity.LogSource);
                db.AddInParameter(dbCommand, "@LogType", DbType.String, CO_SYSTEM_LOGEntity.LogType);
                db.AddInParameter(dbCommand, "@LogSeverity", DbType.Int32, CO_SYSTEM_LOGEntity.LogSeverity);
                db.AddInParameter(dbCommand, "@LogContent", DbType.String, CO_SYSTEM_LOGEntity.LogContent);
                db.AddInParameter(dbCommand, "@LogDate", DbType.DateTime, CO_SYSTEM_LOGEntity.LogDate);
                db.AddInParameter(dbCommand, "@LogDateUtc", DbType.DateTime, CO_SYSTEM_LOGEntity.LogDateUtc);
                db.AddInParameter(dbCommand, "@LogOperator", DbType.String, CO_SYSTEM_LOGEntity.LogOperator);
                db.AddInParameter(dbCommand, "@HostName", DbType.String, CO_SYSTEM_LOGEntity.HostName);
                db.AddInParameter(dbCommand, "@HostIP", DbType.AnsiString, CO_SYSTEM_LOGEntity.HostIP);
                db.AddInParameter(dbCommand, "@HostMac", DbType.AnsiString, CO_SYSTEM_LOGEntity.HostMac);

                int result = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGEntity as CO_SYSTEM_LOG;
        }

        /// <summary>
        /// Insert with transaction
        /// </summary>  
        public CO_SYSTEM_LOG Insert(CO_SYSTEM_LOG entity, DbTransaction transaction)
        {
            //ArgumentValidator.CheckForNullArgument(entity, "entity");
            //ArgumentValidator.CheckForNullArgument(transaction, "transaction");
            CO_SYSTEM_LOG CO_SYSTEM_LOGEntity = entity as CO_SYSTEM_LOG;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, CO_SYSTEM_LOGEntity.LogGuid);
                db.AddInParameter(dbCommand, "@LogSource", DbType.String, CO_SYSTEM_LOGEntity.LogSource);
                db.AddInParameter(dbCommand, "@LogType", DbType.String, CO_SYSTEM_LOGEntity.LogType);
                db.AddInParameter(dbCommand, "@LogSeverity", DbType.Int32, CO_SYSTEM_LOGEntity.LogSeverity);
                db.AddInParameter(dbCommand, "@LogContent", DbType.String, CO_SYSTEM_LOGEntity.LogContent);
                db.AddInParameter(dbCommand, "@LogDate", DbType.DateTime, CO_SYSTEM_LOGEntity.LogDate);
                db.AddInParameter(dbCommand, "@LogDateUtc", DbType.DateTime, CO_SYSTEM_LOGEntity.LogDateUtc);
                db.AddInParameter(dbCommand, "@LogOperator", DbType.String, CO_SYSTEM_LOGEntity.LogOperator);
                db.AddInParameter(dbCommand, "@HostName", DbType.String, CO_SYSTEM_LOGEntity.HostName);
                db.AddInParameter(dbCommand, "@HostIP", DbType.AnsiString, CO_SYSTEM_LOGEntity.HostIP);
                db.AddInParameter(dbCommand, "@HostMac", DbType.AnsiString, CO_SYSTEM_LOGEntity.HostMac);

                int result = db.ExecuteNonQuery(dbCommand, transaction);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGEntity as CO_SYSTEM_LOG;
        }

        #endregion Insert

        #region Delete
        /// <summary>
        /// Delete 
        /// </summary>  
        public void Delete(object entityId)
        {
            //ArgumentValidator.CheckForNullArgument(entityId, "entityId");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, entityId);
                int result = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }


        /// <summary>
        /// Delete with transaction
        /// </summary>  
        public void Delete(object entityId, DbTransaction transaction)
        {
            //ArgumentValidator.CheckForNullArgument(entityId, "entityId");
            //ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, entityId);
                int result = db.ExecuteNonQuery(dbCommand, transaction);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        #endregion Delete

        #region Get
        /// <summary>
        /// Get
        /// </summary>
        public CO_SYSTEM_LOG Get(object entityId)
        {
            //ArgumentValidator.CheckForNullArgument(entityId, "entityId");

            CO_SYSTEM_LOG CO_SYSTEM_LOGEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlGet);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand))
                {
                    if (dataReader.Read())
                    {
                        CO_SYSTEM_LOGEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGEntity;
        }


        /// <summary>
        /// Get with transaction
        /// </summary>
        public CO_SYSTEM_LOG Get(object entityId, DbTransaction transaction)
        {
            //ArgumentValidator.CheckForNullArgument(entityId, "entityId");
            //ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            CO_SYSTEM_LOG CO_SYSTEM_LOGEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(CO_SYSTEM_LOGDAO.SqlGet);

                db.AddInParameter(dbCommand, "@LogGuid", DbType.Guid, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand, transaction))
                {
                    if (dataReader.Read())
                    {
                        CO_SYSTEM_LOGEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGEntity;
        }

        #endregion Get

        #region Find
        /// <summary>
        /// Find
        /// </summary>
        public IList<CO_SYSTEM_LOG> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<CO_SYSTEM_LOG> CO_SYSTEM_LOGList = new List<CO_SYSTEM_LOG>();
            totalRecords = 0;

            string cmdText;
            string cmdCountText;

            int beginIndex = pageSize * pageIndex;

            string filterClause = "";
            if (filter == null || String.IsNullOrEmpty(filter.ToQueryString()))
            {
            }
            else
            {
                filterClause = filter.ToQueryString();
            }

            string sortClause = "";
            if (sort == null || String.IsNullOrEmpty(sort.ToSortString()))
            {
                sortClause = "ORDER BY LogGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CO_SYSTEM_LOGDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CO_SYSTEM_LOGDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
            }

            try
            {
                Database db = GetDatabaseInstance();

                if (pageSize != 0)
                {
                    DbCommand dbCommand = db.GetSqlStringCommand(cmdText);
                    using (IDataReader dataReader = db.ExecuteReader(dbCommand))
                    {
                        while (dataReader.Read())
                        {
                            CO_SYSTEM_LOGList.Add(ReadEntity(dataReader));
                        }
                    }
                }

                DbCommand dbCommandCount = db.GetSqlStringCommand(cmdCountText);
                totalRecords = Convert.ToInt64(db.ExecuteScalar(dbCommandCount));

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<CO_SYSTEM_LOG> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<CO_SYSTEM_LOG> CO_SYSTEM_LOGList = new List<CO_SYSTEM_LOG>();
            totalRecords = 0;

            string cmdText;
            string cmdCountText;

            int beginIndex = pageSize * pageIndex;

            string filterClause = "";
            if (filter == null || String.IsNullOrEmpty(filter.ToQueryString()))
            {
            }
            else
            {
                filterClause = filter.ToQueryString();
            }

            string sortClause = "";
            if (sort == null || String.IsNullOrEmpty(sort.ToSortString()))
            {
                sortClause = "ORDER BY LogGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CO_SYSTEM_LOGDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CO_SYSTEM_LOGDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
            }

            try
            {
                Database db = GetDatabaseInstance();

                if (pageSize != 0)
                {
                    DbCommand dbCommand = db.GetSqlStringCommand(cmdText);
                    using (IDataReader dataReader = db.ExecuteReader(dbCommand, transaction))
                    {
                        while (dataReader.Read())
                        {
                            CO_SYSTEM_LOGList.Add(ReadEntity(dataReader));
                        }
                    }
                }

                DbCommand dbCommandCount = db.GetSqlStringCommand(cmdCountText);
                totalRecords = Convert.ToInt64(db.ExecuteScalar(dbCommandCount, transaction));

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CO_SYSTEM_LOGList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(Siemens.Simatic.Platform.Data.DatabaseEnum.SITBusinessDB);
        }

        private static CO_SYSTEM_LOG ReadEntity(IDataReader dataReader)
        {
            CO_SYSTEM_LOG CO_SYSTEM_LOGEntity = new CO_SYSTEM_LOG();
            object value;


            value = dataReader["LogGuid"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogGuid = (Guid?)value;
            }

            value = dataReader["LogSource"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogSource = (String)value;
            }

            value = dataReader["LogType"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogType = (String)value;
            }

            value = dataReader["LogSeverity"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogSeverity = (Int32?)value;
            }

            value = dataReader["LogContent"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogContent = (String)value;
            }

            value = dataReader["LogDate"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogDate = (DateTime?)value;
            }

            value = dataReader["LogDateUtc"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogDateUtc = (DateTime?)value;
            }

            value = dataReader["LogOperator"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.LogOperator = (String)value;
            }

            value = dataReader["HostName"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.HostName = (String)value;
            }

            value = dataReader["HostIP"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.HostIP = (String)value;
            }

            value = dataReader["HostMac"];
            if (value != DBNull.Value)
            {
                CO_SYSTEM_LOGEntity.HostMac = (String)value;
            }

            return CO_SYSTEM_LOGEntity;
        }

        #endregion Private Method


    }

    public class SSLogType
    {
        public const string Info = "Info";
        public const string Warning = "Warning";
        public const string Error = "Error";
    }

    public partial class SSLogSource
    {
        public const string OrderAudit = "OrderAudit";
        public const string Order = "Order";
        public const string OrderStatus = "OrderStatus";
        public const string Lot = "Lot";
        public const string LotStatus = "LotStatus";
    }

    public enum LogSeverityEnum
    {
        Low = 1,
        Middle = 2,
        High = 3
    }
}
