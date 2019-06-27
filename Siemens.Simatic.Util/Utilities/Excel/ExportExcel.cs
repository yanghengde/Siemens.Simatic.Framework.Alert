using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

namespace Siemens.Simatic.Util.Utilities.Excel
{
    public class ExportExcel
    {
        const string SPREADSHEETSTRING = "urn:schemas-microsoft-com:office:spreadsheet";
        const string OFFICESTRING = "urn:schemas-microsoft-com:office:office";
        const string EXCELSTRING = "urn:schemas-microsoft-com:office:excel";

        private string _FilePath;
        /// <summary>
        /// the path of excel file
        /// </summary>
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }

        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        private string _Sort;

        public string Sort
        {
            get { return _Sort; }
            set { _Sort = value; }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public List<string> IgnoreColumns = null;


        /// <summary>
        /// creat a new instance of DataSetToExcel
        /// </summary>
        public ExportExcel() : this(string.Empty, string.Empty) { }
        public ExportExcel(string FilePath) : this(FilePath, string.Empty) { }
        public ExportExcel(string FilePath, string title)
        {
            this.FilePath = FilePath;
            this.Title = title;
            _filter = string.Empty;
            _Sort = string.Empty;
        }

        public void ExportToExcel(DataTable table)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(table.Copy());
            ExportToExcel(ds);
        }
        public void ExportToExcel(DataSet ds)
        {
            XmlDocument xml = CreateFile();
            XmlNode nodeworkbook = xml.SelectSingleNode("Workbook");


            for (int k = 0; k < ds.Tables.Count; k++)
            {
                DataTable table = new DataTable();
                table = ds.Tables[k].Copy();
                Hashtable tableDD = GetDDTable(table);
                DataRow[] dr = table.Select(Filter, Sort);
                int maxrowcount = this.Title.Length > 0 ? 65534 : 65535;
                XmlNode nodetable = CreateWorkSheet(table.TableName, nodeworkbook, table, tableDD);

                for (int i = 0; i < dr.Length; i++)
                {
                    if (i != 0 && i % maxrowcount == 0)
                    {
                        nodetable = CreateWorkSheet(string.Format("{0}{1}", table.TableName, i / maxrowcount), nodeworkbook, table, tableDD);
                    }
                    XmlNode noderow = xml.CreateElement("Row");
                    nodetable.AppendChild(noderow);
                    foreach (DataColumn dc in table.Columns)
                    {
                        if (IgnoreColumns == null || !IgnoreColumns.Contains(dc.ColumnName))
                        {
                            ToExcel(noderow, dr[i][dc], dc.DataType);
                        }
                    }
                }
            }
            xml.Save(FilePath);
        }

        private Hashtable GetDDTable(DataTable table)
        {
            Hashtable tableDD = new Hashtable();
            foreach (DataColumn dc in table.Columns)
            {
                tableDD.Add(dc.ColumnName, dc.ColumnName);

            }
            return tableDD;
        }

        private void ToExcel(XmlNode nodeRow, object value, Type type)
        {
            XmlDocument xml = nodeRow.OwnerDocument;
            XmlNode nodecell = xml.CreateElement("Cell");
            nodeRow.AppendChild(nodecell);
            XmlNode nodedata = xml.CreateElement("Data");
            XmlAttribute atttype = xml.CreateAttribute("ss", "Type", SPREADSHEETSTRING);
            if (type == typeof(uint) || type == typeof(UInt16) || type == typeof(UInt32)
                         || type == typeof(UInt64) || type == typeof(int) || type == typeof(Int16)
                         || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Single)
                         || type == typeof(Double) || type == typeof(Decimal))
            {
                atttype.Value = "Number";
                if (value == DBNull.Value)
                {
                    nodedata.InnerText = "0";      //empty value can not set to number column
                }
                else
                {
                    nodedata.InnerText = value.ToString();
                }
            }
            else
            {
                atttype.Value = "String";
                nodedata.InnerText = value.ToString().Replace(">", "&gt").Replace("<", "&lt");
            }
            nodedata.Attributes.Append(atttype);
            nodecell.AppendChild(nodedata);
        }
        private XmlNode CreateWorkSheet(string sheetName, XmlNode nodeWorkbook, DataTable table, Hashtable tableDD)
        {
            XmlDocument xml = nodeWorkbook.OwnerDocument;
            XmlNode nodeworksheet = xml.CreateElement("Worksheet");

            XmlAttribute attname = xml.CreateAttribute("ss", "Name", SPREADSHEETSTRING);
            attname.Value = sheetName;
            nodeworksheet.Attributes.Append(attname);
            XmlAttribute attxmln = xml.CreateAttribute("xmlns");
            attxmln.Value = SPREADSHEETSTRING;
            nodeworksheet.Attributes.Append(attxmln);
            nodeWorkbook.AppendChild(nodeworksheet);

            XmlNode nodetable = xml.CreateElement("Table");
            nodeworksheet.AppendChild(nodetable);
            if (this.Title.Length > 0)
            {
                XmlNode noderowtitle = xml.CreateElement("Row");
                nodetable.AppendChild(noderowtitle);

                XmlNode nodecell = xml.CreateElement("Cell");
                noderowtitle.AppendChild(nodecell);
                if (table.Columns.Count > 1)
                {
                    XmlAttribute attmerge = xml.CreateAttribute("ss", "MergeAcross", SPREADSHEETSTRING);
                    attmerge.Value = (table.Columns.Count - 1).ToString();
                    nodecell.Attributes.Append(attmerge);
                }
                XmlAttribute attstyle = xml.CreateAttribute("ss", "StyleID", SPREADSHEETSTRING);
                attstyle.Value = "title";
                nodecell.Attributes.Append(attstyle);

                XmlNode nodedata = xml.CreateElement("Data");
                XmlAttribute atttype = xml.CreateAttribute("ss", "Type", SPREADSHEETSTRING);
                atttype.Value = "String";
                nodedata.InnerText = this.Title;
                nodedata.Attributes.Append(atttype);
                nodecell.AppendChild(nodedata);
            }

            XmlNode noderow = xml.CreateElement("Row");
            nodetable.AppendChild(noderow);
            foreach (DataColumn dc in table.Columns)
            {
                if (IgnoreColumns == null || !IgnoreColumns.Contains(dc.ColumnName))
                {
                    ToExcel(noderow, tableDD[dc.ColumnName], typeof(string));
                }
            }

            return nodetable;
        }
        private XmlDocument CreateFile()
        {
            string directoryname = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directoryname))
            {
                Directory.CreateDirectory(directoryname);
            }
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));

            XmlNode nodeworkbook = xml.CreateElement("Workbook");
            XmlAttribute attxmlns = xml.CreateAttribute("xmlns");
            attxmlns.Value = SPREADSHEETSTRING;
            XmlAttribute attxmlnso = xml.CreateAttribute("xmlns:o");
            attxmlnso.Value = OFFICESTRING;
            XmlAttribute attxmlnsx = xml.CreateAttribute("xmlns:x");
            attxmlnsx.Value = EXCELSTRING;
            XmlAttribute attxmlnsss = xml.CreateAttribute("xmlns:ss");
            attxmlnsss.Value = SPREADSHEETSTRING;
            nodeworkbook.Attributes.Append(attxmlns);
            nodeworkbook.Attributes.Append(attxmlnso);
            nodeworkbook.Attributes.Append(attxmlnsx);
            nodeworkbook.Attributes.Append(attxmlnsss);
            xml.AppendChild(nodeworkbook);

            XmlNode nodestyles = xml.CreateElement("Styles");
            nodeworkbook.AppendChild(nodestyles);

            XmlNode nodestyle = xml.CreateElement("Style");
            XmlAttribute attid = xml.CreateAttribute("ss", "ID", SPREADSHEETSTRING);
            attid.Value = "title";
            nodestyle.Attributes.Append(attid);
            nodestyles.AppendChild(nodestyle);

            XmlElement nodealignment = xml.CreateElement("Alignment");
            XmlAttribute atthorizontal = xml.CreateAttribute("ss", "Horizontal", SPREADSHEETSTRING);
            atthorizontal.Value = "Center";
            nodealignment.Attributes.Append(atthorizontal);
            nodestyle.AppendChild(nodealignment);

            return xml;
        }
    }
}
