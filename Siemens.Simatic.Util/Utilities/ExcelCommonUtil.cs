using System;
using System.Collections.Generic;
using System.Text;
using Siemens.Simatic.Util.Utilities.Excel;
using System.Data;
using System.IO;
using System.Collections;

namespace Siemens.Simatic.Util.Utilities
{
    public class ExcelCommonUtil
    {
        #region XmlToDataSet

        /// <summary>
        /// xml contains schema information converts to DataSet
        /// </summary>
        /// <param name="xml">converted xml</param>
        /// <returns>return a DataSet</returns>
        public static DataSet XmlToDataSet(string xmlPath)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(xmlPath);
            return ds;
        }

        #endregion

        #region DataSetToXml

        /// <summary>
        /// DataSet converts to xml that contains schema information
        /// </summary>
        /// <param name="ds">converted DataSet</param>
        /// <returns></returns>
        public static string DataSetToXml(DataSet ds)
        {
            StringBuilder sb = new StringBuilder();
            ds.WriteXml(new StringWriter(sb));

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// read xml as datatable
        /// </summary>
        /// <param name="xmlPath">xml path</param>
        /// <returns>return xml configuration as datatable</returns>
        public static DataTable XmlToFirstDataTable(string xmlPath)
        {
            DataSet ds = XmlToDataSet(xmlPath);
            DataTable dt = null;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        /// <summary>
        /// get the xml Mapping fields
        /// </summary>
        /// <param name="xmlPath">the xml path</param>
        /// <param name="keyName">the key name, unique of the row</param>
        /// <returns>return the mapping fields</returns>
        public static IDictionary<string, string> GetXmlMappingField(string xmlPath, string keyName)
        {
            IDictionary<string, string> columns = new Dictionary<string, string>();
            DataTable dt = XmlToFirstDataTable(xmlPath);
            DataRow row = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = dt.Rows[i]["KeyName"].ToString();
                if (name.ToLower() == keyName.ToLower())
                {
                    row = dt.Rows[i];
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (!DataChecker.IsEmpty(row[dc.ColumnName].ToString()))
                        {
                            columns.Add(dc.ColumnName, row[dc.ColumnName].ToString());
                        }
                    }
                    break;
                }
            }
            return columns;
        }

        /// <summary>
        /// get mandantory field and configure value 
        /// </summary>
        /// <param name="xmlPath">congigure xml file</param>
        /// <param name="keyName">the unique of read object</param>
        /// <returns>return the configuration object</returns>
        public static ParserEntity GetReadingResult(string xmlPath, string keyName)
        {
            ParserEntity result = new ParserEntity();
            IDictionary<string, string> columns = GetXmlMappingField(xmlPath, keyName);
            result.KeyName = ActualResult("KeyName", ref columns);
            result.KeyName = keyName;
            result.TitleRowNo = Convert.ToInt32(ActualResult("TitleRowNo", ref columns));
            result.StartRowNo = Convert.ToInt32(ActualResult("StartRowNo", ref columns));
            result.SheetName = ActualResult("SheetName", ref columns).ToLower();
            result.SplitRegex = ActualResult("SplitRegex", ref columns);
            result.IsExcel = Convert.ToBoolean(ActualResult("IsExcel", ref columns));
            result.Results = columns;
            return result;
        }

        /// <summary>
        /// get customized datatable, the mandantory fields are defined in the xmlPath
        /// </summary>
        /// <param name="filePath">the source file path, contain data, txt or excel</param>
        /// <param name="xmlPath">template file path</param>
        /// <param name="keyName">ensure that it's unique string from template</param>
        /// <returns>customized datatable</returns>
        public static DataTable GetCustomizedDataTable(string filePath, string xmlPath, string keyName)
        {
            try
            {
                ParserEntity result = GetReadingResult(xmlPath, keyName);
                DataTable dataTable = GetCustomizedDataTable(filePath, result);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// get customized datatable, the mandantory fields are defined in the xmlPath
        /// </summary>
        /// <param name="filePath">the source file path, contain data, txt or excel</param>
        /// <param name="xmlPath">template file path</param>
        /// <param name="keyName">ensure that it's unique string from template</param>
        /// <param name="pattern">regex of the file, replace some string to the other string</param>
        /// <param name="replacement">replacement string</param>
        /// <returns>customized datatable</returns>
        public static DataTable GetCustomizedDataTable(string filePath, string xmlPath, string keyName, string pattern, string replacement)
        {
            ParserEntity result = GetReadingResult(xmlPath, keyName);
            DataTable dataTable = GetCustomizedDataTable(filePath, result, pattern, replacement);
            return dataTable;
        }

        /// <summary>
        /// customized datatable
        /// </summary>
        /// <param name="filePath">the source file path, contain data, txt or excel</param>
        /// <param name="result">template of xml that we setup with ourself</param>
        /// <returns>customized datatable</returns>
        public static DataTable GetCustomizedDataTable(string filePath, ParserEntity result)
        {
            try
            {
                DataTable dataTable = null;
                if (result.IsExcel)
                {
                    try
                    {
                        dataTable = ExcelTextUtil.ReadSheet(filePath, result.SheetName);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            dataTable = ExcelTextUtil.ReadFirstSheet(filePath);
                        }
                        catch (Exception exp)
                        {
                            throw new Exception("Excel 文件格式不正确,请检查!");
                        }
                    }

                }
                else
                {
                    dataTable = ExcelTextUtil.ReadText(filePath, result.SplitRegex);
                }
                DataTable dt = GetCustomizedDataTable(result, dataTable);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception("Excel 文件格式不正确，请检查。");
            }
        }

        /// <summary>
        /// get customized datatable
        /// </summary>
        /// <param name="filePath">the source file path, contain data, txt or excel</param>
        /// <param name="result">template of xml that we setup with ourself</param>
        /// <param name="pattern">regex of the file, replace some string to the other string</param>
        /// <param name="replacement">replacement string</param>
        /// <returns>customized datatable</returns>
        public static DataTable GetCustomizedDataTable(string filePath, ParserEntity result, string pattern, string replacement)
        {
            DataTable dataTable = ExcelTextUtil.ReadText(filePath, result.SplitRegex, pattern, replacement);
            DataTable dt = GetCustomizedDataTable(result, dataTable);
            return dt;
        }

        /// <summary>
        /// Optimize DataTable and get customized datatable
        /// </summary>
        /// <param name="result">template of xml that we setup with ourself</param>
        /// <param name="dt">the original datatable and</param>
        /// <returns>customized datatable</returns>
        public static DataTable GetCustomizedDataTable(ParserEntity result, DataTable dt)
        {
            IDictionary<string, string> dict = result.Results;
            DataTable dataTable = new DataTable();
            DataRow titleRow = null;

            //title in a line
            titleRow = dt.Rows[result.TitleRowNo - 1];
            foreach (string key in dict.Keys)
            {
                for (int i = 0; i < titleRow.ItemArray.Length; i++)
                {
                    if (dict[key].ToLower().Trim() == titleRow.ItemArray[i].ToString().ToLower().Trim())
                    {
                        dt.Columns[i].ColumnName = key;
                    }
                }
            }

            //remove nonsense rows data
            for (int i = 0; i < result.StartRowNo - 1; i++)
            {
                dt.Rows.Remove(dt.Rows[0]);
            }

            //remove nosense columns data
            int j = dt.Columns.Count;
            for (int i = 0; i < j; i++)
            {
                if (!dict.ContainsKey(dt.Columns[i].ToString()))
                {
                    dt.Columns.Remove(dt.Columns[i]);
                    --j; --i;
                }
            }
            return dt;
        }

        /// <summary>
        /// remove the nosense columns from dictionary
        /// </summary>
        /// <param name="columnName">column of xml</param>
        /// <param name="columns">the column of dictionary</param>
        /// <returns>return the actual value of title</returns>
        private static string ActualResult(string columnName, ref IDictionary<string, string> columns)
        {
            if (columns.ContainsKey(columnName))
            {
                string value = columns[columnName].ToString();
                if (!DataChecker.IsEmpty(value))
                {
                    columns.Remove(columnName);
                    return value;
                }
                columns.Remove(columnName);
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// return Select distinct DataTable
        /// </summary>
        /// <param name="SourceTable">original datatable</param>
        /// <param name="FieldNames">column collections</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(DataTable SourceTable, params string[] FieldNames)
        {
            object[] lastValues;
            DataTable newTable;
            DataRow[] orderedRows;

            if (FieldNames == null || FieldNames.Length == 0)
                throw new ArgumentNullException("FieldNames");

            lastValues = new object[FieldNames.Length];
            newTable = new DataTable();

            foreach (string fieldName in FieldNames)
                newTable.Columns.Add(fieldName, SourceTable.Columns[fieldName].DataType);

            orderedRows = SourceTable.Select("", string.Join(",", FieldNames));

            foreach (DataRow row in orderedRows)
            {
                if (!fieldValuesAreEqual(lastValues, row, FieldNames))
                {
                    newTable.Rows.Add(createRowClone(row, newTable.NewRow(), FieldNames));

                    setLastValues(lastValues, row, FieldNames);
                }
            }

            return newTable;
        }

        private static bool fieldValuesAreEqual(object[] lastValues, DataRow currentRow, string[] fieldNames)
        {
            bool areEqual = true;

            for (int i = 0; i < fieldNames.Length; i++)
            {
                if (lastValues[i] == null || !lastValues[i].Equals(currentRow[fieldNames[i]]))
                {
                    areEqual = false;
                    break;
                }
            }

            return areEqual;
        }

        private static DataRow createRowClone(DataRow sourceRow, DataRow newRow, string[] fieldNames)
        {
            foreach (string field in fieldNames)
                newRow[field] = sourceRow[field];

            return newRow;
        }

        private static void setLastValues(object[] lastValues, DataRow sourceRow, string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
                lastValues[i] = sourceRow[fieldNames[i]];
        }
    }
}
