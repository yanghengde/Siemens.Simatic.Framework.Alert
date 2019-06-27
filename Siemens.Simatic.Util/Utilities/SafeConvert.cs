using System;
using System.Globalization;
namespace Siemens.Simatic.Util.Utilities
{
    public static class SafeConvert
    {
        public static double? ToRequiredUnitQty(double? requiredQty, int? orderQuantity)
        {
            if (requiredQty == null) return 0;
            if (requiredQty == (double)0.0) return 0;
            if (orderQuantity == 0) return 0;
            return requiredQty/orderQuantity;
        }

        public static int SapToMesInt32(object obj)
        {
            if (obj == null) return 0;
            obj = obj.ToString().Replace(".000", "");
            return ToInt32(obj);
        }
        /// <summary>
        /// 判断对像是否是空对象，例数据表中某列内某行为空
        /// </summary>
        /// <param name="obj">侍判断的对象</param>
        /// <returns>对像是否是空对象</returns>
        public static bool IsDBNull(object obj)
        {
            return Convert.IsDBNull(obj);
        }

        #region Conver To Methods

        /// <summary>
        /// 生成Guid
        /// </summary>
        /// <param name="obj">obj的ToString方法需要能够返回32位的数字字符串</param>
        /// <returns>生成的System.Guid对象,传入的对象为空，则返回Guid.Empty</returns>
        public static Guid ToGuid(object obj)
        {
            if (obj != null && obj != DBNull.Value)
            {
                try
                {
                    return new Guid(obj.ToString());
                }
                catch
                {
                    return Guid.Empty;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// 将一个对象转达换为一个TimeSpan对象
        /// </summary>
        /// <param name="obj">侍转换对象</param>
        /// <returns>一个TimeSpan对象</returns>
        public static TimeSpan ToTimeSpan(object obj)
        {
            return ToTimeSpan(obj, TimeSpan.Zero);
        }

        /// <summary>
        /// 将一个对象转换成TimeSpan,当参数对象为空时，返回缺省值
        /// </summary>
        /// <param name="obj">侍转换的对象</param>
        /// <param name="defaultValue">传入对象为空时，设置的缺省值</param>
        /// <returns>返回一个TimeSpan对象</returns>
        public static TimeSpan ToTimeSpan(object obj, TimeSpan defaultValue)
        {
            if (obj != null)
                return ToTimeSpan(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将一个字符串转换成TimeSpan,当参数为空时，返回缺省值
        /// </summary>
        /// <param name="s">侍转换的字符串</param>
        /// <param name="defaultValue">传入字符串为空时，设置的缺省值</param>
        /// <returns>返回一个TimeSpan对象</returns>
        public static TimeSpan ToTimeSpan(string s, TimeSpan defaultValue)
        {
            TimeSpan result;
            bool success = TimeSpan.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将一个字符串转换成TimeSpan
        /// </summary>
        /// <param name="s">侍转换的字符串</param>
        /// <returns>返回一个TimeSpan对象</returns>
        public static TimeSpan ToTimeSpan(string s)
        {
            return ToTimeSpan(s, TimeSpan.Zero);
        }

        /// <summary>
        /// 将一个对象转换为字符串
        /// </summary>
        /// <param name="obj">侍转换的对象</param>
        /// <returns>返回一个字符串，当对象为空时，返回string.Empty</returns>
        public static string ToString(object obj)
        {
            if (obj != null) return obj.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 判断字符串对象是否为空，如果是空，则返回string.Empty；
        /// </summary>
        /// <param name="s">侍转换的字符串</param>
        /// <returns>返回一个字符串</returns>
        public static string ToString(string s)
        {
            return ToString(s, string.Empty);
        }

        /// <summary>
        /// 判断字符串对象是否为空，如果是空，则返回一个缺省值
        /// </summary>
        /// <param name="s">侍转换字符串</param>
        /// <param name="defaultString">字符串缺省值</param>
        /// <returns>返回一个字符串，当对象为空时，返回缺省值</returns>
        public static string ToString(string s, string defaultString)
        {
            if (s == null) return defaultString;

            return s.ToString();
        }
        /// <summary>
        /// 判断对象是否为空，如果是空，则返回一个缺省值
        /// </summary>
        /// <param name="s">侍转换对象</param>
        /// <param name="defaultString">字符串缺省值</param>
        /// <returns>返回一个字符串，当对象为空时，返回缺省值</returns>
        public static string ToString(object s, string defaultString)
        {
            if (s == null) return defaultString;

            return s.ToString();
        }
        /// <summary>
        /// 将字符串转换为double类型，如果传入的字符串不能转换为double类型，返回一个缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">double缺省值</param>
        /// <returns>返回double类型值，如果传入的字符串不能转换为double类型，返回一个缺省值</returns>
        public static double ToDouble(string s, double defaultValue)
        {
            double result;
            bool success = double.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为double类型，如果传入的字符串不能转换为double类型，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回double类型值，如果传入的字符串不能转换为double类型，返回0</returns>
        public static double ToDouble(string s)
        {
            return ToDouble(s, 0);
        }

        /// <summary>
        /// 将对象转换为double类型，如果传入的对象不能转换为double类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回double类型值，如果传入的对象不能转换为double类型，返回0</returns>
        public static double ToDouble(object obj)
        {
            return ToDouble(obj, 0);
        }

        /// <summary>
        /// 将对象转换为double类型，如果传入的对象不能转换为double类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">缺省的double值</param>
        /// <returns>返回double类型值，如果传入的对象不能转换为double类型，返回缺省值</returns>
        public static double ToDouble(object obj, double defaultValue)
        {
            if (obj != null)
                return ToDouble(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串对象</param>
        /// <param name="defaultValue">缺省的float值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回缺省值</returns>
        public static float ToSingle(string s, float defaultValue)
        {
            float result;
            bool success = float.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回0
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回0</returns>
        public static float ToSingle(string s)
        {
            return ToSingle(s, 0);
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回0</returns>
        public static float ToSingle(object obj)
        {
            return ToSingle(obj, 0);
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">float缺省值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回缺省值</returns>
        public static float ToSingle(object obj, float defaultValue)
        {
            if (obj != null)
                return ToSingle(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换为decimal类型，如果传入的对象不能转换为decimal类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <param name="defaultValue">decimal缺省值</param>
        /// <returns>返回decimal类型值，如果传入的对象不能转换为decimal类型，返回缺省值</returns>
        public static decimal ToDecimal(string s, decimal defaultValue)
        {
            decimal result;
            bool success = decimal.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为decimal类型，如果传入的对象不能转换为decimal类型，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回decimal类型值，如果传入的对象不能转换为decimal类型，返回0</returns>
        public static decimal ToDecimal(string s)
        {
            return ToDecimal(s, 0);
        }

        /// <summary>
        /// 将对象转换为decimal类型，如果传入的对象不能转换为decimal类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回decimal类型值，如果传入的对象不能转换为decimal类型，返回0</returns>
        public static decimal ToDecimal(object obj)
        {
            return ToDecimal(obj, 0);
        }

        /// <summary>
        /// 将对象转换为decimal类型，如果传入的对象不能转换为decimal类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">decimal缺省值</param>
        /// <returns>返回decimal类型值，如果传入的对象不能转换为decimal类型，返回缺省值</returns>
        public static decimal ToDecimal(object obj, decimal defaultValue)
        {
            if (obj != null)
                return ToDecimal(obj.ToString(), defaultValue);

            return defaultValue;
        }


        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <param name="defaultValue">float缺省值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回缺省值</returns>
        public static float ToFloat(string s, float defaultValue)
        {
            float result;
            bool success = float.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回0
        /// </summary>
        /// <param name="s">待转换字符串对象</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回0</returns>
        public static float ToFloat(string s)
        {
            return ToFloat(s, 0);
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回0</returns>
        public static float ToFloat(object obj)
        {
            return ToFloat(obj, 0);
        }

        /// <summary>
        /// 将对象转换为float类型，如果传入的对象不能转换为float类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">float缺省值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为float类型，返回缺省值</returns>
        public static float ToFloat(object obj, float defaultValue)
        {
            if (obj != null)
                return ToFloat(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换为bool类型，如果传入的对象不能转换为bool类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <param name="defaultValue">bool缺省值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为bool类型，返回缺省值</returns>
        public static bool ToBoolean(string s, bool defaultValue)
        {
            //修复1被转换为false的BUG
            if (s == "1")
                return true;

            bool result;
            bool success = bool.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为bool类型，如果传入的对象不能转换为bool类型，返回false
        /// </summary>
        /// <param name="s">待转换字符串对象</param>
        /// <returns>返回bool类型值，如果传入的对象不能转换为bool类型，返回false</returns>
        public static bool ToBoolean(string s)
        {
            return ToBoolean(s, false);
        }

        /// <summary>
        /// 将对象转换为bool类型，如果传入的对象不能转换为bool类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为bool类型，返回缺省值</returns>
        public static bool ToBoolean(object obj)
        {
            return ToBoolean(obj, false);
        }

        /// <summary>
        /// 将对象转换为bool类型，如果传入的对象不能转换为bool类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">bool缺省值</param>
        /// <returns>返回float类型值，如果传入的对象不能转换为bool类型，返回缺省值</returns>
        public static bool ToBoolean(object obj, bool defaultValue)
        {
            if (obj != null)
                return ToBoolean(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换为char类型，如果传入的对象不能转换为char类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">char缺省值</param>
        /// <returns>返回char类型值，如果传入的对象不能转换为char类型，返回缺省值</returns>
        public static char ToChar(string s, char defaultValue)
        {
            char result;
            bool success = char.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为char类型，如果传入的对象不能转换为char类型，返回'\0'
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回char类型值，如果传入的对象不能转换为char类型，返回'\0'</returns>
        public static char ToChar(string s)
        {
            return ToChar(s, '\0');
        }

        /// <summary>
        /// 将对象转换为char类型，如果传入的对象不能转换为char类型，返回'\0'
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回char类型值，如果传入的对象不能转换为char类型，返回'\0'</returns>
        public static char ToChar(object obj)
        {
            return ToChar(obj, '\0');
        }

        /// <summary>
        /// 将对象转换为char类型，如果传入的对象为空，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">char缺省值</param>
        /// <returns>返回char类型值，如果传入的对象为空，返回缺省值</returns>
        public static char ToChar(object obj, char defaultValue)
        {
            if (obj != null)
                return ToChar(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将字符串转换为byte类型，如果传入的对象不能转换为byte类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">byte缺省值</param>
        /// <returns>返回byte类型值，如果传入的对象不能转换为byte类型，返回缺省值</returns>
        public static byte ToByte(string s, byte defaultValue)
        {
            byte result;
            bool success = byte.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为byte类型，如果传入的对象不能转换为byte类型，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">char缺省值</param>
        /// <returns>返回byte类型值，如果传入的对象不能转换为byte类型，返回0</returns>
        public static byte ToByte(string s)
        {
            return ToByte(s, 0);
        }

        /// <summary>
        /// 将对象转换为byte类型，如果传入的对象不能转换为byte类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">char缺省值</param>
        /// <returns>返回byte类型值，如果传入的对象不能转换为byte类型，返回0</returns>
        public static byte ToByte(object obj)
        {
            return ToByte(obj, 0);
        }

        /// <summary>
        /// 将对象转换为byte类型，如果传入的对象不能转换为byte类型，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">byte缺省值</param>
        /// <returns>返回byte类型值，如果传入的对象不能转换为byte类型，返回缺省值</returns>
        public static byte ToByte(object obj, byte defaultValue)
        {
            if (obj != null)
                return ToByte(obj.ToString(), defaultValue);

            return defaultValue;
        }


        /// <summary>
        /// 将字符串对象转换为sbyte类型，如果传入字符串的对象不能转换为sbyte类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <param name="defaultValue">sbyte缺省值</param>
        /// <returns>返回sbyte类型值，如果传入的对象不能转换为sbyte类型，返回缺省值</returns>
        public static sbyte ToSByte(string s, sbyte defaultValue)
        {
            sbyte result;
            bool success = sbyte.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为sbyte类型，如果传入的对象不能转换为sbyte类型，返回0
        /// </summary>
        /// <param name="s">待转换对象</param>
        /// <returns>返回sbyte类型值，如果传入的对象不能转换为sbyte类型，返回0</returns>
        public static sbyte ToSByte(string s)
        {
            return ToSByte(s, 0);
        }

        /// <summary>
        /// 将对象转换为sbyte类型，如果传入的对象不能转换为sbyte类型，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回sbyte类型值，如果传入的对象不能转换为sbyte类型，返回0</returns>
        public static sbyte ToSByte(object obj)
        {
            return ToSByte(obj, 0);
        }

        /// <summary>
        /// 将对象转换为sbyte类型，如果传入的对象为空，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">sbyte缺省值</param>
        /// <returns>返回sbyte类型值，如果传入的对象为空，返回缺省值</returns>
        public static sbyte ToSByte(object obj, sbyte defaultValue)
        {
            if (obj != null)
                return ToSByte(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换为short类型，如果传入的对象不能转换为short类型，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">short缺省值</param>
        /// <returns>返回short类型值，如果传入的对象不能转换为short类型，返回缺省值</returns>
        public static short ToInt16(string s, short defaultValue)
        {
            short result;
            bool success = short.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为short类型，如果传入的字符串不能转换为short类型，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回short类型值，如果传入的字符串不能转换为short类型，返回0</returns>
        public static short ToInt16(string s)
        {
            return ToInt16(s, 0);
        }

        /// <summary>
        /// 将对象转换为short类型，如果传入的对象不能转换为short类型，返回0
        /// </summary>
        /// <param name="obj">待转换字符串</param>
        /// <returns>返回short类型值，如果传入的对象不能转换为short类型，返回0</returns>
        public static short ToInt16(object obj)
        {
            return ToInt16(obj, 0);
        }

        /// <summary>
        /// 将字符串转换为short类型，如果传入的字符串为空，返回缺省值
        /// </summary>
        /// <param name="obj">待转换字符串</param>
        /// <param name="defaultValue">short缺省值</param>
        /// <returns>返回short类型值，如果传入的字符串为空，返回缺省值</returns>
        public static short ToInt16(object obj, short defaultValue)
        {
            if (obj != null)
                return ToInt16(obj.ToString(), defaultValue);

            return defaultValue;
        }


        /// <summary>
        /// 将字符串转换为ushort类型，如果传入的字符串不能转换为ushort，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">ushort缺省值</param>
        /// <returns>返回ushort类型值，如果传入的字符串不能转换为ushort，返回缺省值</returns>
        public static ushort ToUInt16(string s, ushort defaultValue)
        {
            ushort result;
            bool success = ushort.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为ushort类型，如果传入的字符串不能转换为ushort，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回ushort类型值，如果传入的字符串不能转换为ushort，返回0</returns>
        public static ushort ToUInt16(string s)
        {
            return ToUInt16(s, 0);
        }

        /// <summary>
        /// 将对象转换为ushort类型，如果传入的对象不能转换为ushort，返回0
        /// </summary>
        /// <param name="obj">待转换字符串</param>
        /// <returns>返回ushort类型值，如果传入的对象不能转换为ushort，返回0</returns>
        public static ushort ToUInt16(object obj)
        {
            return ToUInt16(obj, 0);
        }

        /// <summary>
        /// 将字符串转换为ushort类型，如果传入的字符串不能转换为ushort，返回0
        /// </summary>
        /// <param name="obj">待转换字符串</param>
        /// <param name="defaultValue">ushort缺省值</param>
        /// <returns>返回ushort类型值，如果传入的字符串不能转换为ushort，返回0</returns>
        public static ushort ToUInt16(object obj, ushort defaultValue)
        {
            if (obj != null)
                return ToUInt16(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将字符串转换为int类型，如果传入的字符串不能转换为int，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">int缺省值</param>
        /// <returns>返回int类型值，如果传入的字符串不能转换为int，返回缺省值</returns>
        public static int ToInt32(string s, int defaultValue)
        {
            int result;
            bool success = int.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为int类型，如果传入的字符串不能转换为int，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回int类型值，如果传入的字符串不能转换为int，返回0</returns>
        public static int ToInt32(string s)
        {
            return ToInt32(s, 0);
        }

        /// <summary>
        /// 将对象转换为int类型，如果传入的字符串不能转换为int，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回int类型值，如果传入的对象不能转换为int，返回0</returns>
        public static int ToInt32(object obj)
        {
            return ToInt32(obj, 0);
        }

        /// <summary>
        /// 将对象转换为int类型，如果传入的字符串不能转换为int，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">int缺省值</param>
        /// <returns>返回int类型值，如果传入的字符串不能转换为int，返回缺省值</returns>
        public static int ToInt32(object obj, int defaultValue)
        {
            if (obj != null)
                return ToInt32(obj.ToString(), defaultValue);

            return defaultValue;
        }


        /// <summary>
        /// 将字符串对象转换为uint类型，如果传入的字符串不能转换为uint，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串对象</param>
        /// <param name="defaultValue">uint缺省值</param>
        /// <returns>返回uint类型值，如果传入的字符串不能转换为uint，返回缺省值</returns>
        public static uint ToUInt32(string s, uint defaultValue)
        {
            uint result;
            bool success = uint.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串对象转换为uint类型，如果传入的字符串不能转换为uint，返回0
        /// </summary>
        /// <param name="s">待转换字符串对象</param>
        /// <returns>返回uint类型值，如果传入的字符串不能转换为uint，返回0</returns>
        public static uint ToUInt32(string s)
        {
            return ToUInt32(s, 0);
        }

        /// <summary>
        /// 将对象转换为uint类型，如果传入的对象不能转换为uint，返回0
        /// </summary>
        /// <param name="obj">待转换</param>
        /// <returns>返回uint类型值，如果传入的对象不能转换为uint，返回0</returns>
        public static uint ToUInt32(object obj)
        {
            return ToUInt32(obj, 0);
        }

        /// <summary>
        /// 将对象转换为uint类型，如果传入的对象不能转换为uint，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">uint缺省值</param>
        /// <returns>返回uint类型值，如果传入的对象不能转换为uint，返回缺省值</returns>
        public static uint ToUInt32(object obj, uint defaultValue)
        {
            if (obj != null)
                return ToUInt32(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将字符串转换为long类型，如果传入的字符串不能转换为long，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">long缺省值</param>
        /// <returns>返回long类型值，如果传入的字符串不能转换为long，返回缺省值</returns>
        public static long ToInt64(string s, long defaultValue)
        {
            long result;
            bool success = long.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为long类型，如果传入的字符串不能转换为long，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回long类型值，如果传入的字符串不能转换为long，返回0</returns>
        public static long ToInt64(string s)
        {
            return ToInt64(s, 0);
        }

        /// <summary>
        /// 将对象转换为long类型，如果传入的对象不能转换为long，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回long类型值，如果传入的对象不能转换为long，返回0</returns>
        public static long ToInt64(object obj)
        {
            return ToInt64(obj, 0);
        }

        /// <summary>
        /// 将对象转换为long类型，如果传入的对象不能转换为long，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">long缺省值</param>
        /// <returns>返回long类型值，如果传入的对象不能转换为long，返回缺省值</returns>
        public static long ToInt64(object obj, long defaultValue)
        {
            if (obj != null)
                return ToInt64(obj.ToString(), defaultValue);

            return defaultValue;
        }


        /// <summary>
        /// 将字符串转换为ulong类型，如果传入的字符串不能转换为ulong，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">ulong缺省值</param>
        /// <returns>返回ulong类型值，如果传入的字符串不能转换为ulong，返回缺省值</returns>
        public static ulong ToUInt64(string s, ulong defaultValue)
        {
            ulong result;
            bool success = ulong.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为ulong类型，如果传入的字符串不能转换为ulong，返回0
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回ulong类型值，如果传入的字符串不能转换为ulong，返回0</returns>
        public static ulong ToUInt64(string s)
        {
            return ToUInt64(s, 0);
        }

        /// <summary>
        /// 将对象转换为ulong类型，如果传入的对象不能转换为ulong，返回0
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回ulong类型值，如果传入的对象不能转换为ulong，返回0</returns>
        public static ulong ToUInt64(object obj)
        {
            return ToUInt64(obj, 0);
        }

        /// <summary>
        /// 将对象转换为ulong类型，如果传入的对象不能转换为ulong，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">ulong缺省值</param>
        /// <returns>返回ulong类型值，如果传入的对象不能转换为ulong，返回缺省值</returns>
        public static ulong ToUInt64(object obj, ulong defaultValue)
        {
            if (obj != null)
                return ToUInt64(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将字符串转换为DateTime类型，如果传入的字符串不能转换为DateTime，返回缺省值
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <param name="defaultValue">DateTime缺省值</param>
        /// <returns>返回DateTime类型值，如果传入的字符串不能转换为DateTime，返回缺省值</returns>
        public static DateTime ToDateTime(string s, DateTime defaultValue)
        {
            DateTime result;
            bool success = DateTime.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换为DateTime类型，如果传入的字符串不能转换为DateTime，返回DateTime.MinValue
        /// </summary>
        /// <param name="s">待转换字符串</param>
        /// <returns>返回DateTime类型值，如果传入的字符串不能转换为DateTime，返回DateTime.MinValue</returns>
        public static DateTime ToDateTime(string s)
        {
            return ToDateTime(s, DateTime.MinValue);
        }

        /// <summary>
        /// 将对象转换为DateTime类型，如果传入的对象不能转换为DateTime，返回DateTime.MinValue
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回DateTime类型值，如果传入的对象不能转换为DateTime，返回DateTime.MinValue</returns>
        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, DateTime.MinValue);
        }

        /// <summary>
        /// 将对象转换为DateTime类型，如果传入的对象不能转换为DateTime，返回DateTime.MinValue
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>返回DateTime类型值，如果传入的对象不能转换为DateTime，返回DateTime.MinValue</returns>
        public static DateTime ToDateTime2(DateTime obj, string format)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = format;
            return Convert.ToDateTime(obj, dtFormat);
        }

        /// <summary>
        /// 将对象转换为DateTime类型，如果传入的对象不能转换为DateTime，返回缺省值
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultValue">DateTime缺省值</param>
        /// <returns>返回DateTime类型值，如果传入的对象不能转换为DateTime，返回缺省值</returns>
        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            if (obj != null)
                return ToDateTime(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 将字符串类型转换成枚举类型，不忽略大小写
        /// </summary>
        /// <param name="enumType">型别</param>
        /// <param name="text">枚举值</param>
        /// <param name="defaultValue">缺省枚举值</param>
        /// <returns>返回一个enumType的对象，其值是text,如果转换失败，返回缺省值</returns>
        public static object ToEnum(Type enumType, string text, object defaultValue)
        {
            if (Enum.IsDefined(enumType, text))
            {
                return Enum.Parse(enumType, text, false);
            }

            return defaultValue;
        }

        /// <summary>
        /// 将对象转换成枚举类型，不忽略大小写
        /// </summary>
        /// <param name="enumType">型别</param>
        /// <param name="obj">枚举值</param>
        /// <param name="defaultValue">缺省枚举值</param>
        /// <returns>返回一个enumType的对象，其值是text,如果转换失败，返回缺省值</returns>
        public static object ToEnum(Type enumType, object obj, object defaultValue)
        {
            if (obj != null)
                return ToEnum(enumType, obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 创建一个枚举，其值为index
        /// </summary>
        /// <param name="enumType">型别</param>
        /// <param name="index">枚举值</param>
        /// <returns>返回一个enumType的枚举，值为index</returns>
        public static object ToEnum(Type enumType, int index)
        {
            return Enum.ToObject(enumType, index);
        }

        #endregion
    }
}