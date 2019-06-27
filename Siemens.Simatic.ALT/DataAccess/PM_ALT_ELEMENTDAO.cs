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
    [DefaultImplementationAttreibute(typeof(DefaultImpl.PM_ALT_ELEMENTDAO))]
    public partial interface IPM_ALT_ELEMENTDAO : IDataAccessor<PM_ALT_ELEMENT>
    {
    }
}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: PM_ALT_ELEMENTDAO
    /// Copyright: LiXiao Info Tech Ltd.
    /// Version: 1.0  
    public partial class PM_ALT_ELEMENTDAO : IPM_ALT_ELEMENTDAO
    {

        #region SQl
        #region SqlInsert
        private const string SqlInsert = @"INSERT INTO dbo.PM_ALT_ELEMENT (
                                                [ElementID],
                                                [AlertID],
                                                [ElementField],
                                                [ElementAlias],
                                                [ElementType],
                                                [Sequence],
                                                [IsActive],
                                                [RowDeleted],
                                                [CreatedBy],
                                                [CreatedOn],
                                                [ModifiedBy],
                                                [ModifiedOn]
                                                ) VALUES ( 
                                                @ElementID,
                                                @AlertID,
                                                @ElementField,
                                                @ElementAlias,
                                                @ElementType,
                                                @Sequence,
                                                @IsActive,
                                                @RowDeleted,
                                                @CreatedBy,
                                                @CreatedOn,
                                                @ModifiedBy,
                                                @ModifiedOn
                                            )";
        #endregion

        #region SqlDelete
        private const string SqlDelete = @"DELETE FROM dbo.PM_ALT_ELEMENT 
                                           WHERE 1 = 1 
                                           AND ElementID = @ElementID";
        #endregion

        #region SqlUpdateAll
        private const string SqlUpdate = @"UPDATE dbo.PM_ALT_ELEMENT SET
                                            [AlertID] = @AlertID,
                                            [ElementField] = @ElementField,
                                            [ElementAlias] = @ElementAlias,
                                            [ElementType] = @ElementType,
                                            [Sequence] = @Sequence,
                                            [IsActive] = @IsActive,
                                            [RowDeleted] = @RowDeleted,
                                            [CreatedBy] = @CreatedBy,
                                            [CreatedOn] = @CreatedOn,
                                            [ModifiedBy] = @ModifiedBy,
                                            [ModifiedOn] = @ModifiedOn
                                            WHERE 1 = 1
                                            AND [ElementID] = @ElementID";
        #endregion

        #region SqlGet
        private const string SqlGet = @"SELECT
                                            [ElementID],
                                            [AlertID],
                                            [ElementField],
                                            [ElementAlias],
                                            [ElementType],
                                            [Sequence],
                                            [IsActive],
                                            [RowDeleted],
                                            [CreatedBy],
                                            [CreatedOn],
                                            [ModifiedBy],
                                            [ModifiedOn]
                                        FROM dbo.PM_ALT_ELEMENT
                                        WHERE 1 = 1 
                                        AND [ElementID] = @ElementID";
        #endregion

        #region SqlCount
        private const string SqlCount = @"SELECT COUNT(*) 
                                  FROM dbo.PM_ALT_ELEMENT 
                                  WHERE ( 1 = 1 {0} ) 
                                          ";
        #endregion SqlCount

        #region SqlSelectAll
        private const string SqlSelectAll = @"SELECT
                                                    [ElementID],
                                                    [AlertID],
                                                    [ElementField],
                                                    [ElementAlias],
                                                    [ElementType],
                                                    [Sequence],
                                                    [IsActive],
                                                    [RowDeleted],
                                                    [CreatedBy],
                                                    [CreatedOn],
                                                    [ModifiedBy],
                                                    [ModifiedOn]
                                                FROM dbo.PM_ALT_ELEMENT 
                                                WHERE (1 = 1 {0} ) {1}
                                        ";
        #endregion SqlSelectAll

        #region SqlSelect
        private const string SqlSelect = @"SELECT TOP {0} 
                                                [ElementID],
                                                [AlertID],
                                                [ElementField],
                                                [ElementAlias],
                                                [ElementType],
                                                [Sequence],
                                                [IsActive],
                                                [RowDeleted],
                                                [CreatedBy],
                                                [CreatedOn],
                                                [ModifiedBy],
                                                [ModifiedOn]
                                           FROM dbo.PM_ALT_ELEMENT 
                                           WHERE ( 1 = 1 {1} 
                                           AND ElementID 
                                           NOT IN 
                                           (SELECT TOP {2} ElementID 
                                           FROM dbo.PM_ALT_ELEMENT 
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
        public PM_ALT_ELEMENT Insert(PM_ALT_ELEMENT entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, PM_ALT_ELEMENTEntity.ElementID);
                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, PM_ALT_ELEMENTEntity.AlertID);
                db.AddInParameter(dbCommand, "@ElementField", DbType.String, PM_ALT_ELEMENTEntity.ElementField);
                db.AddInParameter(dbCommand, "@ElementAlias", DbType.String, PM_ALT_ELEMENTEntity.ElementAlias);
                db.AddInParameter(dbCommand, "@ElementType", DbType.String, PM_ALT_ELEMENTEntity.ElementType);
                db.AddInParameter(dbCommand, "@Sequence", DbType.Int32, PM_ALT_ELEMENTEntity.Sequence);
                db.AddInParameter(dbCommand, "@IsActive", DbType.Boolean, PM_ALT_ELEMENTEntity.IsActive);
                db.AddInParameter(dbCommand, "@RowDeleted", DbType.Boolean, PM_ALT_ELEMENTEntity.RowDeleted);
                db.AddInParameter(dbCommand, "@CreatedBy", DbType.String, PM_ALT_ELEMENTEntity.CreatedBy);
                db.AddInParameter(dbCommand, "@CreatedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.CreatedOn);
                db.AddInParameter(dbCommand, "@ModifiedBy", DbType.String, PM_ALT_ELEMENTEntity.ModifiedBy);
                db.AddInParameter(dbCommand, "@ModifiedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.ModifiedOn);

                int result = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_ELEMENTEntity as PM_ALT_ELEMENT;
        }

        /// <summary>
        /// Insert with transaction
        /// </summary>  
        public PM_ALT_ELEMENT Insert(PM_ALT_ELEMENT entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");
            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlInsert);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, PM_ALT_ELEMENTEntity.ElementID);
                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, PM_ALT_ELEMENTEntity.AlertID);
                db.AddInParameter(dbCommand, "@ElementField", DbType.String, PM_ALT_ELEMENTEntity.ElementField);
                db.AddInParameter(dbCommand, "@ElementAlias", DbType.String, PM_ALT_ELEMENTEntity.ElementAlias);
                db.AddInParameter(dbCommand, "@ElementType", DbType.String, PM_ALT_ELEMENTEntity.ElementType);
                db.AddInParameter(dbCommand, "@Sequence", DbType.Int32, PM_ALT_ELEMENTEntity.Sequence);
                db.AddInParameter(dbCommand, "@IsActive", DbType.Boolean, PM_ALT_ELEMENTEntity.IsActive);
                db.AddInParameter(dbCommand, "@RowDeleted", DbType.Boolean, PM_ALT_ELEMENTEntity.RowDeleted);
                db.AddInParameter(dbCommand, "@CreatedBy", DbType.String, PM_ALT_ELEMENTEntity.CreatedBy);
                db.AddInParameter(dbCommand, "@CreatedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.CreatedOn);
                db.AddInParameter(dbCommand, "@ModifiedBy", DbType.String, PM_ALT_ELEMENTEntity.ModifiedBy);
                db.AddInParameter(dbCommand, "@ModifiedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.ModifiedOn);

                int result = db.ExecuteNonQuery(dbCommand, transaction);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_ELEMENTEntity as PM_ALT_ELEMENT;
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
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, entityId);
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
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlDelete);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, entityId);
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
        public PM_ALT_ELEMENT Get(object entityId)
        {
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");

            PM_ALT_ELEMENT PM_ALT_ELEMENTEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlGet);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand))
                {
                    if (dataReader.Read())
                    {
                        PM_ALT_ELEMENTEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_ELEMENTEntity;
        }


        /// <summary>
        /// Get with transaction
        /// </summary>
        public PM_ALT_ELEMENT Get(object entityId, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entityId, "entityId");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PM_ALT_ELEMENT PM_ALT_ELEMENTEntity = null;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlGet);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, entityId);
                using (IDataReader dataReader = db.ExecuteReader(dbCommand, transaction))
                {
                    if (dataReader.Read())
                    {
                        PM_ALT_ELEMENTEntity = ReadEntity(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }

            return PM_ALT_ELEMENTEntity;
        }

        #endregion Get

        #region Update

        /// <summary>
        /// Update
        /// </summary>

        public void Update(PM_ALT_ELEMENT entity)
        {
            Update(entity, true);
        }

        public void Update(PM_ALT_ELEMENT entity, bool updateAll)
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

        private void UpdateAll(PM_ALT_ELEMENT entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");

            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlUpdate);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, PM_ALT_ELEMENTEntity.ElementID);
                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, PM_ALT_ELEMENTEntity.AlertID);
                db.AddInParameter(dbCommand, "@ElementField", DbType.String, PM_ALT_ELEMENTEntity.ElementField);
                db.AddInParameter(dbCommand, "@ElementAlias", DbType.String, PM_ALT_ELEMENTEntity.ElementAlias);
                db.AddInParameter(dbCommand, "@ElementType", DbType.String, PM_ALT_ELEMENTEntity.ElementType);
                db.AddInParameter(dbCommand, "@Sequence", DbType.Int32, PM_ALT_ELEMENTEntity.Sequence);
                db.AddInParameter(dbCommand, "@IsActive", DbType.Boolean, PM_ALT_ELEMENTEntity.IsActive);
                db.AddInParameter(dbCommand, "@RowDeleted", DbType.Boolean, PM_ALT_ELEMENTEntity.RowDeleted);
                db.AddInParameter(dbCommand, "@CreatedBy", DbType.String, PM_ALT_ELEMENTEntity.CreatedBy);
                db.AddInParameter(dbCommand, "@CreatedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.CreatedOn);
                db.AddInParameter(dbCommand, "@ModifiedBy", DbType.String, PM_ALT_ELEMENTEntity.ModifiedBy);
                db.AddInParameter(dbCommand, "@ModifiedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.ModifiedOn);
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

        private void UpdateSome(PM_ALT_ELEMENT entity)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");

            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            StringBuilder sqlUpdateSome = new StringBuilder();
            sqlUpdateSome.Append("UPDATE dbo.PM_ALT_ELEMENT SET ");

            PropertyInfo[] propertyInfos = PM_ALT_ELEMENTEntity.GetType().GetProperties();
            Hashtable propertyValues = new System.Collections.Hashtable();
            int columnCountForUpdate = 0;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (EntityMapping.ContainsProperty(propertyInfo.Name))
                {
                    object propertyValue = propertyInfo.GetValue(PM_ALT_ELEMENTEntity, null);
                    ORProperty property = EntityMapping[propertyInfo.Name];
                    if (!property.IsPrimaryKey)
                    {
                        if (!PM_ALT_ELEMENTEntity.IsDefaultValue(propertyInfo.Name))
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
            sqlUpdateSome.Append(" AND ElementID = @ElementID ");

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

        public void Update(PM_ALT_ELEMENT entity, DbTransaction transaction)
        {
            Update(entity, true, transaction);
        }

        public void Update(PM_ALT_ELEMENT entity, bool updateAll, DbTransaction transaction)
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

        private void UpdateAll(PM_ALT_ELEMENT entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(PM_ALT_ELEMENTDAO.SqlUpdate);

                db.AddInParameter(dbCommand, "@ElementID", DbType.Guid, PM_ALT_ELEMENTEntity.ElementID);
                db.AddInParameter(dbCommand, "@AlertID", DbType.Guid, PM_ALT_ELEMENTEntity.AlertID);
                db.AddInParameter(dbCommand, "@ElementField", DbType.String, PM_ALT_ELEMENTEntity.ElementField);
                db.AddInParameter(dbCommand, "@ElementAlias", DbType.String, PM_ALT_ELEMENTEntity.ElementAlias);
                db.AddInParameter(dbCommand, "@ElementType", DbType.String, PM_ALT_ELEMENTEntity.ElementType);
                db.AddInParameter(dbCommand, "@Sequence", DbType.Int32, PM_ALT_ELEMENTEntity.Sequence);
                db.AddInParameter(dbCommand, "@IsActive", DbType.Boolean, PM_ALT_ELEMENTEntity.IsActive);
                db.AddInParameter(dbCommand, "@RowDeleted", DbType.Boolean, PM_ALT_ELEMENTEntity.RowDeleted);
                db.AddInParameter(dbCommand, "@CreatedBy", DbType.String, PM_ALT_ELEMENTEntity.CreatedBy);
                db.AddInParameter(dbCommand, "@CreatedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.CreatedOn);
                db.AddInParameter(dbCommand, "@ModifiedBy", DbType.String, PM_ALT_ELEMENTEntity.ModifiedBy);
                db.AddInParameter(dbCommand, "@ModifiedOn", DbType.DateTime, PM_ALT_ELEMENTEntity.ModifiedOn);
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

        private void UpdateSome(PM_ALT_ELEMENT entity, DbTransaction transaction)
        {
            ArgumentValidator.CheckForNullArgument(entity, "entity");
            ArgumentValidator.CheckForNullArgument(transaction, "transaction");

            PersistentPM_ALT_ELEMENT PM_ALT_ELEMENTEntity = entity as PersistentPM_ALT_ELEMENT;

            StringBuilder sqlUpdateSome = new StringBuilder();
            sqlUpdateSome.Append("UPDATE dbo.PM_ALT_ELEMENT SET ");

            PropertyInfo[] propertyInfos = PM_ALT_ELEMENTEntity.GetType().GetProperties();
            Hashtable propertyValues = new System.Collections.Hashtable();
            int columnCountForUpdate = 0;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (EntityMapping.ContainsProperty(propertyInfo.Name))
                {
                    object propertyValue = propertyInfo.GetValue(PM_ALT_ELEMENTEntity, null);
                    ORProperty property = EntityMapping[propertyInfo.Name];
                    if (!property.IsPrimaryKey)
                    {
                        if (!PM_ALT_ELEMENTEntity.IsDefaultValue(propertyInfo.Name))
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
            sqlUpdateSome.Append(" AND ElementID = @ElementID ");

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
            entityMapping.Name = "PM_ALT_ELEMENT";
            entityMapping.TableName = "dbo.PM_ALT_ELEMENT";

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
            new object[] {"ElementID", DbType.Guid, false, true, "ElementID",typeof(Guid?)},
            new object[] {"AlertID", DbType.Guid, false, false, "AlertID",typeof(Guid?)},
            new object[] {"ElementField", DbType.String, false, false, "ElementField",typeof(String)},
            new object[] {"ElementAlias", DbType.String, false, false, "ElementAlias",typeof(String)},
            new object[] {"ElementType", DbType.String, false, false, "ElementType",typeof(String)},
            new object[] {"Sequence", DbType.Int32, false, false, "Sequence",typeof(Int32?)},
            new object[] {"IsActive", DbType.Boolean, false, false, "IsActive",typeof(Boolean?)},
            new object[] {"RowDeleted", DbType.Boolean, false, false, "RowDeleted",typeof(Boolean?)},
            new object[] {"CreatedBy", DbType.String, false, false, "CreatedBy",typeof(String)},
            new object[] {"CreatedOn", DbType.DateTime, false, false, "CreatedOn",typeof(DateTime?)},
            new object[] {"ModifiedBy", DbType.String, true, false, "ModifiedBy",typeof(String)},
            new object[] {"ModifiedOn", DbType.DateTime, true, false, "ModifiedOn",typeof(DateTime?)},
        };


        #endregion Update

        #region Find
        /// <summary>
        /// Find
        /// </summary>
        public IList<PM_ALT_ELEMENT> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < -1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            IList<PM_ALT_ELEMENT> PM_ALT_ELEMENTList = new List<PM_ALT_ELEMENT>();
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
                sortClause = "ORDER BY ElementID";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, PM_ALT_ELEMENTDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       PM_ALT_ELEMENTDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            PM_ALT_ELEMENTList.Add(ReadEntity(dataReader));
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

            return PM_ALT_ELEMENTList;
        }

        /// <summary>
        /// Find with transaction
        /// </summary>
        public IList<PM_ALT_ELEMENT> Find(int pageIndex, int pageSize, IFilter filter, ISort sort, out long totalRecords, DbTransaction transaction)
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

            IList<PM_ALT_ELEMENT> PM_ALT_ELEMENTList = new List<PM_ALT_ELEMENT>();
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
                sortClause = "ORDER BY ElementID";
            }
            else
            {
                sortClause = sort.ToSortString();
            }

            cmdCountText = string.Format(CultureInfo.InvariantCulture, SqlCount, filterClause);

            if (pageSize == -1)
            {
                cmdText = string.Format(
                CultureInfo.InvariantCulture, PM_ALT_ELEMENTDAO.SqlSelectAll, filterClause, sortClause);
            }
            else
            {
                cmdText = String.Format(
                       CultureInfo.InvariantCulture,
                       PM_ALT_ELEMENTDAO.SqlSelect, pageSize.ToString(), filterClause, beginIndex.ToString(), filterClause, sortClause, sortClause);
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
                            PM_ALT_ELEMENTList.Add(ReadEntity(dataReader));
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

            return PM_ALT_ELEMENTList;
        }

        #endregion Find

        #endregion IDataAccessor members


        #region Private Method

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        private static PM_ALT_ELEMENT ReadEntity(IDataReader dataReader)
        {
            PM_ALT_ELEMENT PM_ALT_ELEMENTEntity = new PM_ALT_ELEMENT();
            object value;


            value = dataReader["ElementID"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ElementID = (Guid?)value;
            }

            value = dataReader["AlertID"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.AlertID = (Guid?)value;
            }

            value = dataReader["ElementField"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ElementField = (String)value;
            }

            value = dataReader["ElementAlias"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ElementAlias = (String)value;
            }

            value = dataReader["ElementType"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ElementType = (String)value;
            }

            value = dataReader["Sequence"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.Sequence = (Int32?)value;
            }

            value = dataReader["IsActive"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.IsActive = (Boolean?)value;
            }

            value = dataReader["RowDeleted"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.RowDeleted = (Boolean?)value;
            }

            value = dataReader["CreatedBy"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.CreatedBy = (String)value;
            }

            value = dataReader["CreatedOn"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.CreatedOn = (DateTime?)value;
            }

            value = dataReader["ModifiedBy"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ModifiedBy = (String)value;
            }

            value = dataReader["ModifiedOn"];
            if (value != DBNull.Value)
            {
                PM_ALT_ELEMENTEntity.ModifiedOn = (DateTime?)value;
            }

            return PM_ALT_ELEMENTEntity;
        }

        #endregion Private Method

    }
}
