using System;
using System.Collections.Generic;
using System.Text;

namespace Siemens.Simatic.Util.Utilities.Excel
{
    public class ParserEntity
    {
        private int titleRowNo;

        private int startRowNo;

        private string sheetName;

        private string keyName;

        private bool isExcel;

        private string splitRegex;

        private IDictionary<string, string> results;

        /// <summary>
        /// tile row number
        /// </summary>
        public int TitleRowNo
        {
            get { return titleRowNo; }
            set { titleRowNo = value; }
        }

        /// <summary>
        /// actual data row number
        /// </summary>
        public int StartRowNo
        {
            get { return startRowNo; }
            set { startRowNo = value; }
        }

        /// <summary>
        /// excel sheet name
        /// </summary>
        public string SheetName
        {
            get { return sheetName; }
            set { sheetName = value; }
        }

        /// <summary>
        /// unique of the xml template
        /// </summary>
        public string KeyName
        {
            get { return keyName; }
            set { keyName = value; }
        }

        /// <summary>
        /// if excel, will parse with excel, if not excel, will parse with txt file.
        /// </summary>
        public bool IsExcel
        {
            get { return isExcel; }
            set { isExcel = value; }
        }

        /// <summary>
        /// split regex, like \s+
        /// </summary>
        public string SplitRegex
        {
            get { return splitRegex; }
            set { splitRegex = value; }
        }

        /// <summary>
        /// actual columns of the business
        /// </summary>
        public IDictionary<string, string> Results
        {
            get { return results; }
            set { results = value; }
        }

    }
}
