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
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_CONFIG_KEYDAO))]
    public interface IPM_ALT_CONFIG_KEYDAO : IDataAccessor<PM_ALT_CONFIG_KEY>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: PM_ALT_CONFIG_KEYDAO
    /// Copyright: Siemens
    /// Version: 1.0  
    public partial class PM_ALT_CONFIG_KEYDAO : IPM_ALT_CONFIG_KEYDAO
    {

        #region SQl
        #region SqlInsert
        private const string SqlInsert = @"INSERT INTO dbo.PM_ALT_CONFIG_KEY (
                                                [sOwner],
                                                [sKey],
                                                [sValue],
                                                [sDescription]
                                                ) VALUES ( 
                                                @sOwner,
                                                @sKey,
                                                @sValue,
                                                @sDescription
                                            )";
        #endregion

        #region SqlDelete
        private const string SqlDelete = @"DELETE FROM dbo.PM_ALT_CONFIG_KEY 
                                           WHERE 1 = 1 
                                           AND ID = @ID";
        #endregion

        #region SqlUpdateAll
        private const string SqlUpdate = @"UPDATE dbo.PM_ALT_CONFIG_KEY SET
                                            [sOwner] = @sOwner,
                                            [sKey] = @sKey,
                                            [sValue] = @sValue,
                                            [sDescription] = @sDescription
                                            WHERE 1 = 1
                                            AND [ID] = @ID";
        #endregion

        #region SqlGet
        private const string SqlGet = @"SELECT
                                            [ID],
                                            [sOwner],
                                            [sKey],
                                            [sValue],
                                            [sDescription]
                                        FROM dbo.PM_ALT_CONFIG_KEY
                                        WHERE 1 = 1 
                                        AND [ID] = @ID";
        #endregion

        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.PM_ALT_CONFIG_KEY 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [ID],
                                                    [sOwner],
                                                    [sKey],
                                                    [sValue],
                                                    [sDescription]
                                                FROM dbo.PM_ALT_CONFIG_KEY 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [ID],
                                                [sOwner],
                                                [sKey],
                                                [sValue],
                                                [sDescription]
                                           FROM dbo.PM_ALT_CONFIG_KEY 
                                           WHERE ( 1 = 1 {1} 
                                           AND ID 
                                           NOT IN 
                                           (SELECT TOP {2} ID 
                                           FROM dbo.PM_ALT_CONFIG_KEY 
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
        public PM_ALT_CONFIG_KEY Insert(PM_ALT_CONFIG_KEY entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@sOwner", DbType.String, PM_ALT_CONFIG_KEYEntity.sOwner);
                db.AddInParameter(dbCommand, "@sKey", DbType.String, PM_ALT_CONFIG_KEYEntity.sKey);
                db.AddInParameter(dbCommand, "@sValue", DbType.String, PM_ALT_CONFIG_KEYEntity.sValue);
                db.AddInParameter(dbCommand, "@sDescription", DbType.String, PM_ALT_CONFIG_KEYEntity.sDescription);

                int result = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_CONFIG_KEYEntity as PM_ALT_CONFIG_KEY;
        }

        /// <summary>
        /// Insert with transaction
        /// </summary>  
        public PM_ALT_CONFIG_KEY Insert(PM_ALT_CONFIG_KEY entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");
            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@sOwner", DbType.String, PM_ALT_CONFIG_KEYEntity.sOwner);
                db.AddInParameter(dbCommand, "@sKey", DbType.String, PM_ALT_CONFIG_KEYEntity.sKey);
                db.AddInParameter(dbCommand, "@sValue", DbType.String, PM_ALT_CONFIG_KEYEntity.sValue);
                db.AddInParameter(dbCommand, "@sDescription", DbType.String, PM_ALT_CONFIG_KEYEntity.sDescription);

                int result = db.ExecuteNonQuery(dbCommand, transaction);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_CONFIG_KEYEntity as PM_ALT_CONFIG_KEY;
        }

        #endregion Insert

        #region Delete
        /// <summary>
        /// Delete 
        /// </summary>  
        public void Delete(object entityId)
        {
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, entityId);
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
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, entityId);
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
        public PM_ALT_CONFIG_KEY Get(object entityId)
        {
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");

            PM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlGet);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand))
                {
                    if (dataReader.Read())
                    {
                        PM_ALT_CONFIG_KEYEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_CONFIG_KEYEntity;
        }


        /// <summary>
        /// Get with transaction
        /// </summary>
        public PM_ALT_CONFIG_KEY Get(object entityId, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlGet);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand, transaction))
                {
                    if (dataReader.Read())
                    {
                        PM_ALT_CONFIG_KEYEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_CONFIG_KEYEntity;
        }

        #endregion Get

        #region Update

        /// <summary>
        /// Update
        /// </summary>

        public void Update(PM_ALT_CONFIG_KEY entity)
        {
            Update(entity, true);
        }

        public void Update(PM_ALT_CONFIG_KEY entity, bool updateAll)
        {
            if (!updateAll)
            {
                UpdateSome(entity);
            }
            else
            {
                UpdateAll(entity);
            }
        }

        private void UpdateAll(PM_ALT_CONFIG_KEY entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");

            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlUpdate);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, PM_ALT_CONFIG_KEYEntity.ID);
                db.AddInParameter(dbCommand, "@sOwner", DbType.String, PM_ALT_CONFIG_KEYEntity.sOwner);
                db.AddInParameter(dbCommand, "@sKey", DbType.String, PM_ALT_CONFIG_KEYEntity.sKey);
                db.AddInParameter(dbCommand, "@sValue", DbType.String, PM_ALT_CONFIG_KEYEntity.sValue);
                db.AddInParameter(dbCommand, "@sDescription", DbType.String, PM_ALT_CONFIG_KEYEntity.sDescription);
                int result = db.ExecuteNonQuery(dbCommand);

                if (result == 0)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        private void UpdateSome(PM_ALT_CONFIG_KEY entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");

            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            StringBuilder sqlUpdateSome = new StringBuilder();
            sqlUpdateSome.Append("UPDATE dbo.PM_ALT_CONFIG_KEY SET ");

            PropertyInfo[] propertyInfos = PM_ALT_CONFIG_KEYEntity.GetType().GetProperties();
            Hashtable propertyValues = new System.Collections.Hashtable();
            int columnCountForUpdate = 0;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (EntityMapping.ContainsProperty(propertyInfo.Name))
                {
                    object propertyValue = propertyInfo.GetValue(PM_ALT_CONFIG_KEYEntity, null);
                    ORProperty property = EntityMapping[propertyInfo.Name];
                    if (!property.IsPrimaryKey)
                    {
                        if (!PM_ALT_CONFIG_KEYEntity.IsDefaultValue(propertyInfo.Name))
                        {
                            propertyValues[propertyInfo.Name] = propertyValue;

                            sqlUpdateSome.Append(" " + property.ColumnName + " = @" + property.ColumnName + ",");
                            columnCountForUpdate++;
                        }
                    }
                    else
                    {
                        propertyValues[propertyInfo.Name] = propertyValue;
                    }
                }
            }
            if (columnCountForUpdate == 0)
            {
                return;
            }

            sqlUpdateSome.Remove(sqlUpdateSome.Length - 1, 1);
            sqlUpdateSome.Append(" WHERE 1 = 1 ");
            sqlUpdateSome.Append(" AND ID = @ID ");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sqlUpdateSome.ToString());

                foreach (DictionaryEntry de in propertyValues)
                {
                    ORProperty property = EntityMapping[de.Key.ToString()];
                    db.AddInParameter(dbCommand, "@" + property.ColumnName, property.DatabaseType, de.Value);
                }

                int result = db.ExecuteNonQuery(dbCommand);

                if (result == 0)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        /// <summary>
        /// Update with transaction
        /// </summary>

        public void Update(PM_ALT_CONFIG_KEY entity, DbTransaction transaction)
        {
            Update(entity, true, transaction);
        }

        public void Update(PM_ALT_CONFIG_KEY entity, bool updateAll, DbTransaction transaction)
        {
            if (!updateAll)
            {
                UpdateSome(entity, transaction);
            }
            else
            {
                UpdateAll(entity, transaction);
            }
        }

        private void UpdateAll(PM_ALT_CONFIG_KEY entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_CONFIG_KEYDAO.SqlUpdate);

                db.AddInParameter(dbCommand, "@ID", DbType.Int32, PM_ALT_CONFIG_KEYEntity.ID);
                db.AddInParameter(dbCommand, "@sOwner", DbType.String, PM_ALT_CONFIG_KEYEntity.sOwner);
                db.AddInParameter(dbCommand, "@sKey", DbType.String, PM_ALT_CONFIG_KEYEntity.sKey);
                db.AddInParameter(dbCommand, "@sValue", DbType.String, PM_ALT_CONFIG_KEYEntity.sValue);
                db.AddInParameter(dbCommand, "@sDescription", DbType.String, PM_ALT_CONFIG_KEYEntity.sDescription);
                int result = db.ExecuteNonQuery(dbCommand, transaction);

                if (result == 0)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        private void UpdateSome(PM_ALT_CONFIG_KEY entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PersistentPM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = entity as PersistentPM_ALT_CONFIG_KEY;

            StringBuilder sqlUpdateSome = new StringBuilder();
            sqlUpdateSome.Append("UPDATE dbo.PM_ALT_CONFIG_KEY SET ");

            PropertyInfo[] propertyInfos = PM_ALT_CONFIG_KEYEntity.GetType().GetProperties();
            Hashtable propertyValues = new System.Collections.Hashtable();
            int columnCountForUpdate = 0;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (EntityMapping.ContainsProperty(propertyInfo.Name))
                {
                    object propertyValue = propertyInfo.GetValue(PM_ALT_CONFIG_KEYEntity, null);
                    ORProperty property = EntityMapping[propertyInfo.Name];
                    if (!property.IsPrimaryKey)
                    {
                        if (!PM_ALT_CONFIG_KEYEntity.IsDefaultValue(propertyInfo.Name))
                        {
                            propertyValues[propertyInfo.Name] = propertyValue;

                            sqlUpdateSome.Append(" " + property.ColumnName + " = @" + property.ColumnName + ",");
                            columnCountForUpdate++;
                        }
                    }
                    else
                    {
                        propertyValues[propertyInfo.Name] = propertyValue;
                    }
                }
            }
            if (columnCountForUpdate == 0)
            {
                return;
            }

            sqlUpdateSome.Remove(sqlUpdateSome.Length - 1, 1);
            sqlUpdateSome.Append(" WHERE 1 = 1 ");
            sqlUpdateSome.Append(" AND ID = @ID ");

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sqlUpdateSome.ToString());

                foreach (DictionaryEntry de in propertyValues)
                {
                    ORProperty property = EntityMapping[de.Key.ToString()];
                    db.AddInParameter(dbCommand, "@" + property.ColumnName, property.DatabaseType, de.Value);
                }

                int result = db.ExecuteNonQuery(dbCommand, transaction);

                if (result == 0)
                {
                    throw new EntityNotFoundException();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }

        /// <summary>
        /// ORMapping
        /// </summary>

        private OREntity entityMapping;

        public OREntity EntityMapping
        {
            get
            {
                if (entityMapping == null)
                {
                    LoadMappings();
                }

                return entityMapping;
            }
        }

        private void LoadMappings()
        {
            entityMapping = new OREntity();
            entityMapping.Name = "PM_ALT_CONFIG_KEY";
            entityMapping.TableName = "dbo.PM_ALT_CONFIG_KEY";

            foreach (object[] propertyMapping in this.PropertyMappings)
            {
                ORProperty property = new ORProperty();

                property.ColumnName = (string)propertyMapping[0];
                property.DatabaseType = (DbType)propertyMapping[1];
                property.IsNullable = (bool)propertyMapping[2];
                property.IsPrimaryKey = (bool)propertyMapping[3];
                property.Name = (string)propertyMapping[4];
                property.Type = (Type)propertyMapping[5];

                entityMapping.Properties.Add(property);
            }
        }

        private object[][] PropertyMappings = new object[][] 
        {
            new object[] {"ID", DbType.Int32, false, true, "ID",typeof(Int32?)},
            new object[] {"sOwner", DbType.String, false, false, "sOwner",typeof(String)},
            new object[] {"sKey", DbType.String, false, false, "sKey",typeof(String)},
            new object[] {"sValue", DbType.String, false, false, "sValue",typeof(String)},
            new object[] {"sDescription", DbType.String, true, false, "sDescription",typeof(String)},
        };


        #endregion Update

        #region Find
        /// <summary>
        /// Find
        /// </summary>
        public IList<PM_ALT_CONFIG_KEY> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<PM_ALT_CONFIG_KEY> PM_ALT_CONFIG_KEYList = new List<PM_ALT_CONFIG_KEY>();
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
                sortClause = "ORDER BY ID";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, PM_ALT_CONFIG_KEYDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       PM_ALT_CONFIG_KEYDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            PM_ALT_CONFIG_KEYList.Add(ReadEntity(dataReader));
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

            return PM_ALT_CONFIG_KEYList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<PM_ALT_CONFIG_KEY> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
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

            IList<PM_ALT_CONFIG_KEY> PM_ALT_CONFIG_KEYList = new List<PM_ALT_CONFIG_KEY>();
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
                sortClause = "ORDER BY ID";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, PM_ALT_CONFIG_KEYDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       PM_ALT_CONFIG_KEYDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            PM_ALT_CONFIG_KEYList.Add(ReadEntity(dataReader));
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

            return PM_ALT_CONFIG_KEYList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        private static PM_ALT_CONFIG_KEY ReadEntity(IDataReader dataReader)
        {
            PM_ALT_CONFIG_KEY PM_ALT_CONFIG_KEYEntity = new PM_ALT_CONFIG_KEY();
            object value;


            value = dataReader["ID"];
            if (value != DBNull.Value)
            {
                PM_ALT_CONFIG_KEYEntity.ID = (Int32?)value;
            }

            value = dataReader["sOwner"];
            if (value != DBNull.Value)
            {
                PM_ALT_CONFIG_KEYEntity.sOwner = (String)value;
            }

            value = dataReader["sKey"];
            if (value != DBNull.Value)
            {
                PM_ALT_CONFIG_KEYEntity.sKey = (String)value;
            }

            value = dataReader["sValue"];
            if (value != DBNull.Value)
            {
                PM_ALT_CONFIG_KEYEntity.sValue = (String)value;
            }

            value = dataReader["sDescription"];
            if (value != DBNull.Value)
            {
                PM_ALT_CONFIG_KEYEntity.sDescription = (String)value;
            }

            return PM_ALT_CONFIG_KEYEntity;
        }

        #endregion Private Method

    }
}
