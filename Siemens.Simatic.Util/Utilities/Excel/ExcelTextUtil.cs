using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Siemens.Simatic.Util.Utilities.Excel
{
    public static class ExcelTextUtil
    {

        #region ReadExcel

        private static OleDbConnection conn = null;

        private static string connectionString = string.Empty;

        private const string connectionStringFormatForRead2003 =
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";

        private const string connectionStringFormatForRead2007 =
            "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";


        /// <summary>
        /// verify the file path and file is correct or not
        /// </summary>
        /// <param name="filePath">file path</param>
        private static void ValidateExcelFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (extension.ToLower() != ".xls" && extension.ToLower() != ".xlsx")
            {
                //throw new Exception(Resources.ExcelFileSupport);
                throw new Exception("Excel file not found.");
            }

            if (!File.Exists(filePath))
            {
                //throw new FileNotFoundException(Resources.ExcelFileNotFound, filePath);
                throw new FileNotFoundException("File not found.", filePath);
            }
        }

        /// <summary>
        /// get sheet name, if sheet name is not end with "$", will append it.
        /// </summary>
        /// <param name="sheetName">sheet name</param>
        /// <returns>sheet name of format</returns>
        private static string GetSheetName(string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                //throw new Exception(Resources.SheetNameNotEmpty);
                throw new Exception("Excel sheet name is not empty.");
            }
            if (!sheetName.EndsWith("$"))
            {
                sheetName += "$";
            }
            return sheetName;
        }

        /// <summary>
        /// create connection of excel
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>return connection of excel</returns>
        public static OleDbConnection CreateConnection(string filePath)
        {
            ValidateExcelFile(filePath);
            try
            {
                connectionString = string.Format(connectionStringFormatForRead2003, filePath);
                conn = new OleDbConnection(connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                try
                {
                    connectionString = string.Format(connectionStringFormatForRead2007, filePath);
                    conn = new OleDbConnection(connectionString);
                    conn.Open();
                    return conn;
                }
                catch (Exception exp)
                {
                    throw ex;
                    //throw new Exception("Create Excel connection failed.");
                }
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// get all sheet name
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>return all sheet names</returns>
        public static IList<string> GetAllSheetNames(string filePath)
        {
            conn = CreateConnection(filePath);
            return GetAllSheetNames(conn);
        }

        /// <summary>
        /// get all excel sheet name
        /// </summary>
        /// <param name="connection">Excel file connection</param>
        /// <returns>sheet names</returns>
        private static IList<string> GetAllSheetNames(OleDbConnection connection)
        {
            IList<string> sheetNames = new List<string>();
            //get all tables                   
            DataTable tablesInfo = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (tablesInfo != null && tablesInfo.Rows.Count > 0)
            {
                foreach (DataRow row in tablesInfo.Rows)
                {
                    sheetNames.Add(row["TABLE_NAME"].ToString());
                }
            }
            return sheetNames;
        }

        /// <summary>
        /// 获取SheetNames
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns>IList<string></returns>
        private static IList<string> GetAllSheetNamesByNPOI(string filePath)
        {
            IList<string> sheetNames = new List<string>();

            string extension = Path.GetExtension(filePath);

            if (extension.ToLower() == ".xls")
            {
                #region 2003
                using (FileStream fs = File.OpenRead(@filePath))
                {
                    HSSFWorkbook wk = new HSSFWorkbook(fs);
                    int sheets = wk.NumberOfSheets;
                    for (int i = 0; i < sheets; i++)
                    {
                        string setSheetName = wk.GetSheetName(i);
                        sheetNames.Add(setSheetName);
                    }
                }
                #endregion
            }
            else if (extension.ToLower() == ".xlsx")
            {
                #region 2007
                using (FileStream fs = new FileStream(@filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook wb = WorkbookFactory.Create(fs);
                    int sheets = wb.NumberOfSheets;
                    for (int i = 0; i < sheets; i++)
                    {
                        string setSheetName = wb.GetSheetName(i);
                        sheetNames.Add(setSheetName);
                    }
                }
                #endregion
            }

            return sheetNames;
        }

        /// <summary>
        /// Reads the sheets.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="sheetNames">The sheet names.</param>
        /// <returns></returns>
        public static DataSet ReadSheets(string filePath, params string[] sheetNames)
        {

            OleDbConnection connection = CreateConnection(filePath);
            DataSet result = new DataSet();
            IList<string> selectedSheets;
            if (sheetNames == null || sheetNames.Length == 0)
            {
                selectedSheets = GetAllSheetNames(connection);
            }
            else
            {
                selectedSheets = sheetNames;
            }
            foreach (string sheet in selectedSheets)
            {
                result.Tables.Add(ReadSheet(connection, sheet));
            }

            return result;
        }

        /// <summary>
        /// Read the first sheet of excel
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>return datatable</returns>
        public static DataTable ReadFirstSheet(string filePath)
        {
            //OleDbConnection connection = CreateConnection(filePath);
            //IList<string> sheets = GetAllSheetNames(connection);

            IList<string> sheets = GetAllSheetNamesByNPOI(filePath);
            if (sheets.Count > 0)
            {
                string firstSheetName = sheets[0];
                return GetDataTabeByNPOI(filePath, firstSheetName);
                //return ReadSheet(connection, firstSheetName);
            }
            return null;
        }



        /// <summary>
        /// read sheet by sheet name
        /// </summary>
        /// <param name="filePath">the file path</param>
        /// <param name="sheetName">sheet name</param>
        /// <returns>return sheet values as datable</returns>
        public static DataTable ReadSheet(string filePath, string sheetName)
        {
            //OleDbConnection connection = CreateConnection(filePath);
            //return ReadSheet(connection, sheetName);

            return GetDataTabeByNPOI(filePath, sheetName);
        }


        /// <summary>
        /// 获取导入Excel记录
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">sheetname</param>
        /// <returns></returns>
        private static DataTable GetDataTabeByNPOI(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();
            string extension = Path.GetExtension(filePath);

            if (extension.ToLower() == ".xls")
            {
                #region 2003 
                using (FileStream fs = File.OpenRead(@filePath))
                {
                    HSSFWorkbook wk = new HSSFWorkbook(fs);
                    ISheet sheet = null;
                    sheet = wk.GetSheet(sheetName);

                    IEnumerator rows = sheet.GetRowEnumerator();
                    for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
                    {
                        dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                    }

                    while (rows.MoveNext())
                    {
                        HSSFRow row = null;
                        row = (HSSFRow)rows.Current;
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            ICell cell = row.GetCell(i);
                            if (cell == null)
                            {
                                dr[i] = null;
                            }
                            else
                            {
                                dr[i] = cell.ToString();
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
                #endregion
            }
            else if (extension.ToLower() == ".xlsx")
            {
                #region 2007
                using (FileStream fs = new FileStream(@filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs);
                    ISheet sheet = null;
                    sheet = workbook.GetSheet(sheetName);

                    IEnumerator rows = sheet.GetRowEnumerator();
                    for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
                    {
                        dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
                    }

                    while (rows.MoveNext())
                    {
                        XSSFRow row = null;
                        row = (XSSFRow)rows.Current;

                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            ICell cell = row.GetCell(i);
                            if (cell == null)
                            {
                                dr[i] = null;
                            }
                            else
                            {
                                dr[i] = cell.ToString();
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
                #endregion
            }
            return dt;
        }

        /// <summary>
        /// Reads the sheet.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns></returns>
        private static DataTable ReadSheet(OleDbConnection connection, string sheetName)
        {
            OleDbDataAdapter adapter = GetAdapter(sheetName, connection);
            DataTable result = new DataTable(sheetName);
            adapter.Fill(result);
            return result;
        }


        /// <summary>
        /// Gets the adapter.
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        private static OleDbDataAdapter GetAdapter(string sheetName, OleDbConnection connection)
        {
            sheetName = GetSheetName(sheetName);
            string sql = string.Format(@"select * from [{0}]", sheetName);
            OleDbCommand command = new OleDbCommand(sql, connection);
            return new OleDbDataAdapter(command);
        }

        #endregion

        #region Read Text File

        /// <summary>
        /// read txt file and return datatable
        /// </summary>
        /// <param name="filePath">the source file path, contain data, txt or excel</param>
        /// <param name="splitRegex">split line in the txt file</param>
        /// <param name="pattern">regex of the file, replace some string to the other string</param>
        /// <param name="replacement">replacement string</param>
        /// <returns>return datatable</returns>
        public static DataTable ReadText(string filePath, string splitRegex, string pattern, string replacement)
        {
            StreamReader reader = PreReader(filePath, pattern, replacement);
            DataTable dt = ReadText(reader, splitRegex);
            return dt;
        }

        /// <summary>
        /// read txt file and return datatable
        /// </summary>
        /// <param name="filePath">the source txt filepath+filename</param>
        /// <param name="splitRegex">regex</param>
        /// <returns>return datatable</returns>
        public static DataTable ReadText(string filePath, string strRegex)
        {
            StreamReader reader = PreReader(filePath);
            DataTable dt = ReadText(reader, strRegex);
            return dt;
        }

        /// <summary>
        /// read txt and return datatable
        /// </summary>
        /// <param name="objReader">stream reader</param>
        /// <param name="splitRegex">regex</param>
        /// <returns>return datatable</returns>
        public static DataTable ReadText(StreamReader objReader, string splitRegex)
        {
            string sLine = "";
            DataTable dt = new DataTable();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    DataRow row = dt.NewRow();
                    //use Trim to remove front/end spaces
                    string[] txtLine = Regex.Split(sLine.Trim(), splitRegex);
                    for (int i = 0; i < txtLine.Length; i++)
                    {
                        try
                        {
                            row[i] = txtLine[i];
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            //if no this column, then add this
                            dt.Columns.Add();
                            row[i] = txtLine[i];
                        }
                    }
                    dt.Rows.Add(row);
                }
            }
            objReader.Close();

            return dt;
        }

        /// <summary>
        /// return stream reader
        /// </summary>
        /// <param name="filePath">the source txt filepath+filename</param>
        /// <returns>return stream reader</returns>
        public static StreamReader PreReader(string filePath)
        {
            ValidateTextFile(filePath);
            StreamReader objReader = new StreamReader(filePath);
            return objReader;
        }

        /// <summary>
        /// use regex to format the txt file
        /// </summary>
        /// <param name="filePath">the source txt filepath+filename</param>
        /// <param name="pattern">regex of the file, replace some string to the other string</param>
        /// <param name="replacement">replacement string</param>
        /// <returns>return stream reader</returns>
        public static StreamReader PreReader(string filePath, string pattern, string replacement)
        {
            StreamReader objReader = PreReader(filePath);
            string str = Regex.Replace(objReader.ReadToEnd(), pattern, replacement);
            byte[] array = Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(array);
            StreamReader reader = new StreamReader(stream);
            return reader;
        }

        /// <summary>
        /// verify the file path and file is correct or not
        /// </summary>
        /// <param name="filePath">file path</param>
        private static void ValidateTextFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (extension.ToLower() != ".txt" && extension.ToLower() != ".csv")
            {
                //throw new Exception(Resources.ExcelFileSupport);
                throw new Exception("Just support txt or csv file.");
            }

            if (!File.Exists(filePath))
            {
                //throw new FileNotFoundException(Resources.ExcelFileNotFound, filePath);
                throw new FileNotFoundException("File not found.", filePath);
            }
        }

        #endregion
    }
}
