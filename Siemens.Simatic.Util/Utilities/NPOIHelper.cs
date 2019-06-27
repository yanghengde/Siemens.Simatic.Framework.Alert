using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;

namespace Siemens.Simatic.Util.Utilities
{
    public class NPOIHelper
    {
        public HSSFWorkbook Workbook2003;
        public IWorkbook Workbook2007;
        public ISheet iSheet;
        public string ExcelVersion;

        /// <summary>
        /// NPOIHelper
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="sheetName">Sheet Name</param>
        public NPOIHelper(string filePath, string sheetName)
        {
            try
            {
                string extension = Path.GetExtension(filePath);
                if (extension.ToLower() == ".xls")
                {
                    ExcelVersion = "2003";
                    using (FileStream fs = new FileStream(@filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        Workbook2003 = new HSSFWorkbook(fs);
                        iSheet = Workbook2003.GetSheet(sheetName);
                    }
                }
                else
                {
                    ExcelVersion = "2007";
                    using (FileStream fs = new FileStream(@filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        Workbook2007 = WorkbookFactory.Create(fs);
                        iSheet = Workbook2007.GetSheet(sheetName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="rowIndex">行数 从0开始</param>
        /// <param name="cellIndex">列数 从0开始</param>
        /// <param name="val">值</param>
        public void SetValue(int rowIndex, int cellIndex, string val)
        {
            IRow dataRow = iSheet.GetRow(rowIndex);
            if (dataRow == null)
                dataRow = iSheet.CreateRow(rowIndex);

            ICell newCell = dataRow.GetCell(cellIndex);
            if (newCell == null)
                newCell = dataRow.CreateCell(cellIndex);

            iSheet.GetRow(rowIndex).GetCell(cellIndex).SetCellValue(val);
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="rowIndex">行数 从0开始</param>
        /// <param name="cellIndex">列数 从0开始</param>
        /// <returns>值</returns>
        public string GetValue(int rowIndex, int cellIndex)
        {
            string _tempData=null;
	        {
		 
	        }
            IRow dataRow = iSheet.GetRow(rowIndex);
            if (dataRow == null)
                return string.Empty;

            ICell dataCell = dataRow.GetCell(cellIndex);
            if (dataCell == null)
                return string.Empty;

            switch(dataCell.CellType)
            {
                case CellType.Numeric:
                   _tempData= dataCell.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    _tempData =dataCell.StringCellValue;
                    break;
                //case CellType.Formula:
                //    _tempData= dataCell.StringCellValue;;
                //    break;

                //added by hans on 2018-08-28 公式的处理
                case CellType.Formula:
                    switch (dataCell.CachedFormulaResultType)
                    {
                        case CellType.Boolean:
                            _tempData = dataCell.BooleanCellValue.ToString();
                            break;
                        case CellType.Error:
                            _tempData = ErrorEval.GetText(dataCell.ErrorCellValue);
                            break;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(dataCell))
                            {
                                _tempData = dataCell.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                            }
                            else
                            {
                                _tempData = dataCell.NumericCellValue.ToString();
                            }
                            break;
                        case CellType.String:
                            string str = dataCell.StringCellValue;
                            if (!string.IsNullOrEmpty(str))
                            {
                                _tempData = str.ToString();
                            }
                            else
                            {
                                _tempData = null;
                            }
                            break;
                        case CellType.Unknown:
                        case CellType.Blank:
                        default:
                            _tempData = string.Empty;
                            break;
                    }
                    break;
                //end added

                case CellType.Blank:
                    _tempData="";
                    break;
                case CellType.Unknown:
                    _tempData="未知数据";
                    break;
            }

            return _tempData;
        }
    }
}
