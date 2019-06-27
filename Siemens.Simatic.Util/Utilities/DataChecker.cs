using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;

namespace Siemens.Simatic.Util.Utilities
{
    /// <summary>
    /// General function library.
    /// </summary>
    public static class DataChecker
    {
        #region IsEmpty functions

        #region DataSet

        /// <summary>
        /// DataSet is null or empty
        /// </summary>
        /// <param name="dataSet">checking DataSet</param>
        /// <returns>if null or empty, then return<c>true</c>; else return <c>false</c>.</returns>
        public static bool IsEmpty(DataSet dataSet)
        {
            return !(IsNotEmpty(dataSet));
        }

        #endregion

        #region DataTable

        /// <summary>
        /// DataTable is null or empty
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <returns>if null or empty, then return<c>true</c><c>true</c>;else return <c>false</c>.</returns>
        public static bool IsEmpty(DataTable dataTable)
        {
            return !(IsNotEmpty(dataTable));
        }

        #endregion

        #region String

        /// <summary>
        /// input string is null or String.Empty
        /// </summary>
        /// <param name="str">checking string</param>
        /// <param name="removeSpace">remove front/end spaces or not</param>
        /// <returns>if null or empty, then return<c>true</c>; else return <c>false</c>.</returns>
        public static bool IsEmpty(string str, bool removeSpace)
        {
            if (str == null)
            {
                return true;
            }

            if (removeSpace)
            {
                return (str.Trim().Length == 0);
            }

            return (str.Length == 0);
        }

        /// <summary>
        ///input string is null or String.Empty，remove front and end spaces.
        /// </summary>
        /// <param name="str">checking string</param>
        /// <returns>if null or empty, then return<c>true</c>; else return <c>false</c>.</returns>
        public static bool IsEmpty(string str)
        {
            return IsEmpty(str, true);
        }

        public static bool IsRealEmpty(string str, bool removeSpace)
        {
            if (str == null)
            {
                return true;
            }

            if (removeSpace)
            {
                return (str.Replace("&nbsp;", " ").Trim().Length == 0);
            }

            return (str.Length == 0);
        }

        public static bool IsReadEmpty(string str)
        {
            return IsRealEmpty(str, true);
        }

        #endregion

        #region ICollection

        /// <summary>
        /// Determines whether the specified value is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(ICollection value)
        {
            return (value == null || value.Count == 0);
        }

        /// <summary>
        /// Determines whether the specified value is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty<T>(ICollection<T> value)
        {
            return (value == null || value.Count == 0);
        }

        #endregion

        #region Object

        /// <summary>
        /// Determines whether the specified value is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string)
            {
                return IsEmpty((string)value);
            }

            if (value is ICollection)
            {
                return IsEmpty((ICollection)value);
            }

            if (value is DataSet)
            {
                return IsEmpty((DataSet)value);
            }

            if (value is DataTable)
            {
                return IsEmpty((DataTable)value);
            }

            return false;
        }

        #endregion

        #endregion

        #region IsNotEmpty functions

        #region DataSet

        /// <summary>
        /// data in DataSet
        /// </summary>
        /// <param name="dataSet">checking DataSet</param>
        /// <returns>if null or empty, then return<c>false</c>; otherwise <c>true</c>.</returns>
        public static bool IsNotEmpty(DataSet dataSet)
        {
            if (dataSet == null)
            {
                return false;
            }
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                if (IsNotEmpty(dataSet.Tables[i]))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region DataTable

        /// <summary>
        /// data in DataTable
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <returns>if null or empty, then return<c>false</c>; else return <c>true</c>.</returns>
        public static bool IsNotEmpty(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return false;
            }

            if (dataTable.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region String

        /// <summary>
        /// string is null or String.Empty。
        /// </summary>
        /// <param name="str">checking string</param>
        /// <param name="removeSpace">remove front and end spaces or not</param>
        /// <returns>if null or empty, then return<c>false</c>; else return <c>true</c>.</returns>
        public static bool IsNotEmpty(string str, bool removeSpace)
        {
            return (!IsEmpty(str, removeSpace));
        }

        /// <summary>
        /// input string is null or String.Empty，remove the front and end spaces.
        /// </summary>
        /// <param name="str">checking string</param>
        /// <returns>if null or empty, then return<c>false</c>; else return <c>true</c>.</returns>
        public static bool IsNotEmpty(string str)
        {
            return IsNotEmpty(str, true);
        }

        #endregion

        #region ICollection

        /// <summary>
        /// Determines whether [is not empty] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is not empty] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty(ICollection value)
        {
            return !IsEmpty(value);
        }

        /// <summary>
        /// Determines whether [is not empty] [the specified value].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is not empty] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty<T>(ICollection<T> value)
        {
            return !IsEmpty(value);
        }

        #endregion

        #region Object

        /// <summary>
        /// Determines whether [is not empty] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is not empty] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty(object value)
        {
            return !IsEmpty(value);
        }

        #endregion

        #endregion

        #region HasEmpty

        /// <summary>
        /// Determines whether the specified args has empty.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>
        /// 	<c>true</c> if the specified args has empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasEmpty(params object[] args)
        {
            foreach (object arg in args)
            {
                if (IsEmpty(arg))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region AllAreEmpty

        /// <summary>
        /// Alls the are empty.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static bool AllAreEmpty(params object[] args)
        {
            foreach (object arg in args)
            {
                if (IsNotEmpty(arg))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region AllAreNotEmpty

        /// <summary>
        /// Alls the are not empty.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static bool AllAreNotEmpty(params object[] args)
        {
            foreach (object arg in args)
            {
                if (IsEmpty(arg))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Digit checking

        #region IsUInt

        /// <summary>
        /// Decimal and unsigned data
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool IsUInt(string input)
        {
            Regex regUInt = new Regex("^[0-9]+$");

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regUInt.Match(input);
            return match.Success;
        }

        #endregion

        #region IsInt

        /// <summary>
        /// Decimal integer string.
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool IsInt(string input)
        {
            Regex regInt = new Regex("^[+-]?[0-9]+$");

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regInt.Match(input);
            return match.Success;
        }

        #endregion

        #region IsUDecimal

        /// <summary>
        /// Whether it is an unsigned decimal number, including decimal.
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool IsUDecimal(string input)
        {
            Regex regUDecimal = new Regex("^[0-9]+[.]?[0-9]*$");

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regUDecimal.Match(input);
            return match.Success;
        }

        #endregion

        #region IsDecimal

        /// <summary>
        /// Is a decimal number or not, including decimal.
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool IsDecimal(string input)
        {
            Regex regDecimal = new Regex("^[+-]?[0-9]+[.]?[0-9]*$"); //==>  ^[+-]?\d+[.]?\d+$

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regDecimal.Match(input);
            return match.Success;
        }

        #endregion

        #endregion

        #region Chinese checking

        /// <summary>
        /// check chinese is exist or not
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool HasChinese(string input)
        {
            Regex regChinese = new Regex("[\u4e00-\u9fa5]");

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regChinese.Match(input);
            return match.Success;
        }

        #endregion

        #region Mail Address check

        /// <summary>
        /// Is a floating-point numbers, may have + and -
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            Regex regEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");

            if (!IsNotEmpty(input))
            {
                return false;
            }

            Match match = regEmail.Match(input);
            return match.Success;
        }

        #endregion

        ///转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///        
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }


        ///
        ///转半角的函数(DBC case)
        ///
        ///任意字符串
        ///半角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// Trim Start Zero
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>返回去掉前面0的结果</returns>
        public static string TrimStartZero(string input)
        {
            if (!string.IsNullOrEmpty(input))
                return input.TrimStart('0');
            return input;
        }
    }
}
