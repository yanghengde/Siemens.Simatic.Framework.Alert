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
    [DefaultImplementationAttreibute(typeof(DefaultImpl.CV_PM_WECHAT_USERDAO))]
    public interface ICV_PM_WECHAT_USERDAO : IDataAccessor<CV_PM_WECHAT_USER>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: CV_PM_WECHAT_USERDAO
    /// Copyright: Siemens
    /// Version: 1.0  
    public partial class CV_PM_WECHAT_USERDAO : ICV_PM_WECHAT_USERDAO
    {

        #region SQl
        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.CV_PM_WECHAT_USER 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [UserGuid],
                                                    [UserID],
                                                    [Name],
                                                    [EnglishName],
                                                    [Mobile],
                                                    [Position],
                                                    [Gender],
                                                    [Email],
                                                    [IsLeader],
                                                    [Enable],
                                                    [AvatarMediaid],
                                                    [Telephone],
                                                    [CreatedBy],
                                                    [CreatedOn],
                                                    [UpdatedBy],
                                                    [UpdatedOn]
                                                FROM dbo.CV_PM_WECHAT_USER 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [UserGuid],
                                                [UserID],
                                                [Name],
                                                [EnglishName],
                                                [Mobile],
                                                [Position],
                                                [Gender],
                                                [Email],
                                                [IsLeader],
                                                [Enable],
                                                [AvatarMediaid],
                                                [Telephone],
                                                [CreatedBy],
                                                [CreatedOn],
                                                [UpdatedBy],
                                                [UpdatedOn]
                                           FROM dbo.CV_PM_WECHAT_USER 
                                           WHERE ( 1 = 1 {1} 
                                           AND UserGuid 
                                           NOT IN 
                                           (SELECT TOP {2} UserGuid 
                                           FROM dbo.CV_PM_WECHAT_USER 
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
        public IList<CV_PM_WECHAT_USER> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<CV_PM_WECHAT_USER> CV_PM_WECHAT_USERList = new List<CV_PM_WECHAT_USER>();
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
                sortClause = "ORDER BY UserGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_WECHAT_USERDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_WECHAT_USERDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_WECHAT_USERList.Add(ReadEntity(dataReader));
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

            return CV_PM_WECHAT_USERList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<CV_PM_WECHAT_USER> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
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

            IList<CV_PM_WECHAT_USER> CV_PM_WECHAT_USERList = new List<CV_PM_WECHAT_USER>();
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
                sortClause = "ORDER BY UserGuid";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, CV_PM_WECHAT_USERDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       CV_PM_WECHAT_USERDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            CV_PM_WECHAT_USERList.Add(ReadEntity(dataReader));
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

            return CV_PM_WECHAT_USERList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        private static CV_PM_WECHAT_USER ReadEntity(IDataReader dataReader)
        {
            CV_PM_WECHAT_USER CV_PM_WECHAT_USEREntity = new CV_PM_WECHAT_USER();
            object value;


            value = dataReader["UserGuid"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.UserGuid = (Guid?)value;
            }

            value = dataReader["UserID"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.UserID = (String)value;
            }

            value = dataReader["Name"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Name = (String)value;
            }

            value = dataReader["EnglishName"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.EnglishName = (String)value;
            }

            value = dataReader["Mobile"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Mobile = (String)value;
            }

            value = dataReader["Position"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Position = (String)value;
            }

            value = dataReader["Gender"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Gender = (Int32?)value;
            }

            value = dataReader["Email"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Email = (String)value;
            }

            value = dataReader["IsLeader"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.IsLeader = (Boolean?)value;
            }

            value = dataReader["Enable"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Enable = (Boolean?)value;
            }

            value = dataReader["AvatarMediaid"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.AvatarMediaid = (String)value;
            }

            value = dataReader["Telephone"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.Telephone = (String)value;
            }

            value = dataReader["CreatedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.CreatedBy = (String)value;
            }

            value = dataReader["CreatedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.CreatedOn = (DateTime?)value;
            }

            value = dataReader["UpdatedBy"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.UpdatedBy = (String)value;
            }

            value = dataReader["UpdatedOn"];
            if (value != DBNull.Value)
            {
                CV_PM_WECHAT_USEREntity.UpdatedOn = (DateTime?)value;
            }

            return CV_PM_WECHAT_USEREntity;
        }

        #endregion Private Method


        #region NotImplemented Methods

        public CV_PM_WECHAT_USER Insert(CV_PM_WECHAT_USER entity)
        {
            throw new NotImplementedException();
        }
        public CV_PM_WECHAT_USER Insert(CV_PM_WECHAT_USER entity, DbTransaction transaction)
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
        public CV_PM_WECHAT_USER Get(object entityId)
        {
            throw new NotImplementedException();
        }
        public CV_PM_WECHAT_USER Get(object entityId, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_USER entity)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_USER entity, bool updateAll)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_USER entity, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }
        public void Update(CV_PM_WECHAT_USER entity, bool updateAll, DbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        #endregion NotImplemented Methods

    }
}
