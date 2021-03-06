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

//using Siemens.Simatic.PM.Common;
//using Siemens.Simatic.PM.Common.Persistence;
using Siemens.Simatic.Util.Utilities;

namespace Siemens.Simatic.ALT.DataAccess
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.ALT_BSC_DAO))]
    public interface IALT_BSC_DAO
    {
        //string GetAttributeInstanceID(string AttributeName);

        DataTable GetDataTableBySql(string sql);

        void ExecuteNonQueryBySql(string sql);

        DataTable GetDataTableByStoredProcedure(string storedProcedureName);
    }


}

namespace Siemens.Simatic.ALT.DataAccess.DefaultImpl
{
    /// Title: CO_BSC_DAO
    /// Copyright: Siemens
    /// Version: 1.0  
    public partial class ALT_BSC_DAO : IALT_BSC_DAO
    {
        //public string GetAttributeInstanceID(string AttributeName)
        //{
        //    string sql = @"SELECT [AttributeInstanceID] FROM [dbo].[CO_BSC_ATTRIBUTE_INSTANCE] WITH(NOLOCK) WHERE AttributeName=@AttributeName";
        //    try
        //    {
        //        Database db = GetDatabaseInstance();
        //        DbCommand dbCommand = db.GetSqlStringCommand(sql);

        //        db.AddInParameter(dbCommand, "@AttributeName", DbType.String, AttributeName);

        //        DataSet result = db.ExecuteDataSet(dbCommand);
        //        if (result == null || result.Tables[0] == null || result.Tables[0].Rows.Count == 0) return string.Empty;
        //        return SafeConvert.ToString(result.Tables[0].Rows[0][0]);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
        //    }

        //    return string.Empty;
        //}

        private static Database GetDatabaseInstance()
        {
            return DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
        }

        public DataTable GetDataTableBySql(string sql)
        {
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                dbCommand.CommandTimeout = 600;
                DataSet ds = db.ExecuteDataSet(dbCommand);
                if (DataChecker.IsNotEmpty(ds))
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void ExecuteNonQueryBySql(string sql)
        {
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetSqlStringCommand(sql);
                dbCommand.CommandTimeout = 600;
                int ds = db.ExecuteNonQuery(dbCommand);
                //int ds = db.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, ExceptionPolicy.DataAccessDefaultPolicy);
            }
        }


        public DataTable GetDataTableByStoredProcedure(string storedProcedureName)
        {
            try
            {
                Database db = GetDatabaseInstance();
                DbCommand dbCommand = db.GetStoredProcCommand(storedProcedureName);
                dbCommand.CommandTimeout = 600;
                DataSet ds = db.ExecuteDataSet(dbCommand);
                if (DataChecker.IsNotEmpty(ds))
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



    }
}