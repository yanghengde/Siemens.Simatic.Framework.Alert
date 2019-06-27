﻿
//=====================================================================
// This file was generated by Siemens.Simatic Platform
// 
// LiXiao Info Tech Ltd. Copyright (c) 2014 All rights reserved. 
//=====================================================================

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
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_ALT_LOGDAO))]
    public interface ICV_PM_ALT_LOGDAO : IDataAccessor<CV_PM_ALT_LOG>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: CV_PM_ALT_LOGDAO
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0  
    public partial class CV_PM_ALT_LOGDAO : ICV_PM_ALT_LOGDAO
    {

        #region SQl
        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.CV_PM_ALT_LOG 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [LogPK],
                                                    [AlertID],
                                                    [AlertName],
                                                    [AlertAlias],
                                                    [LogTitle],
                                                    [LogContent],
                                                    [NotifiedCnt],
                                                    [IsClosed],
                                                    [RespondedBy],
                                                    [RespondedOn],
                                                    [ResponseAction],
                                                    [ResponseActionName],
                                                    [ResponseCause],
                                                    [ResponseNotes],
                                                    [AuditedBy],
                                                    [AuditedOn],
                                                    [CreatedBy],
                                                    [CreatedOn],
                                                    [NotifiedBy],
                                                    [NotifiedOn]
                                                FROM dbo.CV_PM_ALT_LOG 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [LogPK],
                                                [AlertID],
                                                [AlertName],
                                                [AlertAlias],
                                                [LogTitle],
                                                [LogContent],
                                                [NotifiedCnt],
                                                [IsClosed],
                                                [RespondedBy],
                                                [RespondedOn],
                                                [ResponseAction],
                                                [ResponseActionName],
                                                [ResponseCause],
                                                [ResponseNotes],
                                                [AuditedBy],
                                                [AuditedOn],
                                                [CreatedBy],
                                                [CreatedOn],
                                                [NotifiedBy],
                                                [NotifiedOn]
                                           FROM dbo.CV_PM_ALT_LOG 
                                           WHERE ( 1 = 1 {1} 
                                           AND LogPK 
                                           NOT IN 
                                           (SELECT TOP {2} LogPK 
                                           FROM dbo.CV_PM_ALT_LOG 
                                           WHERE (1 = 1 {3}) 
                                           {4} ) ) {5} 
                                           ";
        #endregion SqlSelect
        #endregion SQL


        #region IDataAccessor members

        #region Find
        /// <summary>
        /// Find
        /// </summary>
        public IList<CV_PM_ALT_LOG> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<CV_PM_ALT_LOG> CV_PM_ALT_LOGList = new List<CV_PM_ALT_LOG>();
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
                sortClause = "ORDER BY LogPK";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_ALT_LOGDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_ALT_LOGDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_ALT_LOGList.Add(ReadEntity(dataReader));
                        }
                    }
                }

                //DbCommand dbCommandCount = db.GetSqlStringCommand(cmdCountText);
                //totalRecords = Convert.ToInt64(db.ExecuteScalar(dbCommandCount));

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CV_PM_ALT_LOGList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<CV_PM_ALT_LOG> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
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

            IList<CV_PM_ALT_LOG> CV_PM_ALT_LOGList = new List<CV_PM_ALT_LOG>();
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
                sortClause = "ORDER BY LogPK";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_ALT_LOGDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_ALT_LOGDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_ALT_LOGList.Add(ReadEntity(dataReader));
                        }
                    }
                }

                //DbCommand dbCommandCount = db.GetSqlStringCommand(cmdCountText);
                //totalRecords = Convert.ToInt64(db.ExecuteScalar(dbCommandCount,transaction));

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return CV_PM_ALT_LOGList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        private static CV_PM_ALT_LOG ReadEntity(IDataReader dataReader)
        {
            CV_PM_ALT_LOG CV_PM_ALT_LOGEntity = new CV_PM_ALT_LOG();
            object value;


            value = dataReader["LogPK"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.LogPK = (Int64?)value;
            }

            value = dataReader["AlertID"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.AlertID = (Guid?)value;
            }

            value = dataReader["AlertName"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.AlertName = (String)value;
            }

            value = dataReader["AlertAlias"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.AlertAlias = (String)value;
            }

            value = dataReader["LogTitle"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.LogTitle = (String)value;
            }

            value = dataReader["LogContent"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.LogContent = (String)value;
            }

            value = dataReader["NotifiedCnt"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.NotifiedCnt = (Int32?)value;
            }

            value = dataReader["IsClosed"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.IsClosed = (Boolean?)value;
            }

            value = dataReader["RespondedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.RespondedBy = (String)value;
            }

            value = dataReader["RespondedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.RespondedOn = (DateTime?)value;
            }

            value = dataReader["ResponseAction"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.ResponseAction = (String)value;
            }

            value = dataReader["ResponseActionName"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.ResponseActionName = (String)value;
            }

            value = dataReader["ResponseCause"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.ResponseCause = (String)value;
            }

            value = dataReader["ResponseNotes"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.ResponseNotes = (String)value;
            }

            value = dataReader["AuditedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.AuditedBy = (String)value;
            }

            value = dataReader["AuditedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.AuditedOn = (DateTime?)value;
            }

            value = dataReader["CreatedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.CreatedBy = (String)value;
            }

            value = dataReader["CreatedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.CreatedOn = (DateTime?)value;
            }

            value = dataReader["NotifiedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.NotifiedBy = (String)value;
            }

            value = dataReader["NotifiedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_ALT_LOGEntity.NotifiedOn = (DateTime?)value;
            }

            return CV_PM_ALT_LOGEntity;
        }

        #endregion Private Method


        #region NotImplemented Methods

        public CV_PM_ALT_LOG Insert(CV_PM_ALT_LOG entity)
        {
            throw new NotImplementedException();
        }
        public CV_PM_ALT_LOG Insert(CV_PM_ALT_LOG entity, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Delete(object entityId)
        {
            throw new NotImplementedException();
        }
        public void Delete(object entityId, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public CV_PM_ALT_LOG Get(object entityId)
        {
            throw new NotImplementedException();
        }
        public CV_PM_ALT_LOG Get(object entityId, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_ALT_LOG entity)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_ALT_LOG entity, bool updateAll)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_ALT_LOG entity, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_ALT_LOG entity, bool updateAll, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        #endregion NotImplemented Methods

    }
}
