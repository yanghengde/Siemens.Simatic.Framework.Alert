﻿
//=====================================================================
// This file was generated by Siemens.Simatic Platform
// 
// Siemens Copyright (c) 2014 All rights reserved. 
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
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_WECHAT_NOTIDAO))]
    public interface ICV_PM_WECHAT_NOTIDAO : IDataAccessor<CV_PM_WECHAT_NOTI>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: CV_PM_WECHAT_NOTIDAO
    /// Copyright: Siemens
    /// Version: 1.0  
    public partial class CV_PM_WECHAT_NOTIDAO : ICV_PM_WECHAT_NOTIDAO
    {

        #region SQl
        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.CV_PM_WECHAT_NOTI 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [NotiGuid],
                                                    [AlertID],
                                                    [AgentGuid],
                                                    [AgentID],
                                                    [SecretID],
                                                    [UserIDs]
                                                FROM dbo.CV_PM_WECHAT_NOTI 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [NotiGuid],
                                                [AlertID],
                                                [AgentGuid],
                                                [AgentID],
                                                [SecretID],
                                                [UserIDs]
                                           FROM dbo.CV_PM_WECHAT_NOTI 
                                           WHERE ( 1 = 1 {1} 
                                           AND NotiGuid 
                                           NOT IN 
                                           (SELECT TOP {2} NotiGuid 
                                           FROM dbo.CV_PM_WECHAT_NOTI 
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
        public IList<CV_PM_WECHAT_NOTI> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<CV_PM_WECHAT_NOTI> CV_PM_WECHAT_NOTIList = new List<CV_PM_WECHAT_NOTI>();
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
                sortClause = "ORDER BY NotiGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_WECHAT_NOTIDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_WECHAT_NOTIDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_WECHAT_NOTIList.Add(ReadEntity(dataReader));
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

            return CV_PM_WECHAT_NOTIList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<CV_PM_WECHAT_NOTI> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
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

            IList<CV_PM_WECHAT_NOTI> CV_PM_WECHAT_NOTIList = new List<CV_PM_WECHAT_NOTI>();
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
                sortClause = "ORDER BY NotiGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_WECHAT_NOTIDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_WECHAT_NOTIDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_WECHAT_NOTIList.Add(ReadEntity(dataReader));
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

            return CV_PM_WECHAT_NOTIList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        private static CV_PM_WECHAT_NOTI ReadEntity(IDataReader dataReader)
        {
            CV_PM_WECHAT_NOTI CV_PM_WECHAT_NOTIEntity = new CV_PM_WECHAT_NOTI();
            object value;


            value = dataReader["NotiGuid"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.NotiGuid = (Guid?)value;
            }

            value = dataReader["AlertID"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.AlertID = (Guid?)value;
            }

            value = dataReader["AgentGuid"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.AgentGuid = (Guid?)value;
            }

            value = dataReader["AgentID"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.AgentID = (Int32?)value;
            }

            value = dataReader["SecretID"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.SecretID = (String)value;
            }

            value = dataReader["UserIDs"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_NOTIEntity.UserIDs = (String)value;
            }

            return CV_PM_WECHAT_NOTIEntity;
        }

        #endregion Private Method


        #region NotImplemented Methods

        public CV_PM_WECHAT_NOTI Insert(CV_PM_WECHAT_NOTI entity)
        {
            throw new NotImplementedException();
        }
        public CV_PM_WECHAT_NOTI Insert(CV_PM_WECHAT_NOTI entity, DbTransaction transaction)
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
        public CV_PM_WECHAT_NOTI Get(object entityId)
        {
            throw new NotImplementedException();
        }
        public CV_PM_WECHAT_NOTI Get(object entityId, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_NOTI entity)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_NOTI entity, bool updateAll)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_NOTI entity, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_NOTI entity, bool updateAll, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        #endregion NotImplemented Methods

    }
}
