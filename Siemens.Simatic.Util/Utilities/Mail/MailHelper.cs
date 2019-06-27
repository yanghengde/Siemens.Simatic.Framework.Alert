using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.IO;
using System.Data;

namespace Siemens.Simatic.Util.Utilities
{
    /// <summary>
    /// Mail Helper class
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// Get Mail Table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetMailTable(DataTable data, string title)
        {
            string LargeMailBody = string.Empty;
            if (title != "")
                LargeMailBody += "<p style=\"font-size: 15pt\">" + title + "</p>";

            LargeMailBody += "<div align=\"left\">";  //<div align=\"center\">
            LargeMailBody += "<table cellspacing=\"1\" cellpadding=\"3\" border=\"1\" bgcolor=\"000000\" style=\"font-size: 10pt;line-height: 15px;\">";

            LargeMailBody += "<tr>";
            for (int hcol = 0; hcol < data.Columns.Count; hcol++)
            {
                LargeMailBody += "<td bgcolor=\"dddddd\">&nbsp;";
                LargeMailBody += "<b>" + data.Columns[hcol].ColumnName + "</b>";
                LargeMailBody += "</td>"; //&nbsp;&nbsp;&nbsp;
            }
            LargeMailBody += "</tr>";

            for (int row = 0; row < data.Rows.Count; row++)
            {
                LargeMailBody += "<tr>";
                for (int col = 0; col < data.Columns.Count; col++)
                {
                    LargeMailBody += "<td bgcolor=\"ffffff\">&nbsp;";
                    LargeMailBody += data.Rows[row][col].ToString();
                    LargeMailBody += "</td>";  //&nbsp;&nbsp;&nbsp;
                }
                LargeMailBody += "</tr>";
            }
            LargeMailBody += "</table><br>";
            LargeMailBody += "</div>";
            return LargeMailBody;
        }

    }
}