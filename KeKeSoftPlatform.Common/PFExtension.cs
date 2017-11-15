using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace KeKeSoftPlatform.Common
{
    public static class PFExtension
    {
        #region Enum
        

        /// <summary>
        /// int转换为enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToEnum<T>(this string obj) where T : struct
        {
            if (string.IsNullOrEmpty(obj))
            {
                return default(T);
            }
            try
            {
                return (T)Enum.Parse(typeof(T), obj, true);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        #endregion

        #region Enum Flag
        //是否存在权限
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool HasForAny<T>(this Enum type, T defaultValue, params T[] value)
        {
            try
            {
                var result = false;
                result = (((int)(object)type & (int)(object)defaultValue) == (int)(object)defaultValue);
                if (result)
                {
                    return result;
                }
                else
                {
                    if (value != null)
                    {
                        foreach (var item in value)
                        {
                            result = (((int)(object)type & (int)(object)item) == (int)(object)item);
                            if (result)
                            {
                                return result;
                            }
                        }
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        //判断权限
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }
        //添加权限
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "不能添加类型 '{0}'",
                        typeof(T).Name
                        ), ex);
            }
        }

        //移除权限
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "不能移除类型 '{0}'",
                        typeof(T).Name
                        ), ex);
            }
        }
        #endregion

        #region string 
        public static string NullOrValue(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                return value.Trim();
            }
        }

        public static string FormatString(this string value, params object[] p)
        {
            return string.Format(value, p);
        }

        /// <summary>
        /// 根据长度分割字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<string> Split(this string source, int length)
        {
            var result = new List<string>();
            for (int index = 0; index + length <= source.Length; index += length)
            {
                result.Add(source.Substring(index, length));
            }
            return result;
        }

        /// <summary>
        /// 去除字符串中的所有空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringWithoutSpace(this string str)
        {
            //替换空格
            byte[] spaceArr = new byte[] { 0xc2, 0xa0 };
            var replaceStr = Encoding.UTF8.GetString(spaceArr);
            return str.Replace(" ", "").Replace(replaceStr, "");
        }

        /// <summary>
        /// 根据附件重命名后的文件名，获取附件原名
        /// </summary>
        /// <param name="fuJianName"></param>
        /// <returns></returns>
        public static string GetOriginalName(this string fuJianName)
        {
            return fuJianName.Substring(0, fuJianName.Length - 19 - System.IO.Path.GetExtension(fuJianName).Length) + System.IO.Path.GetExtension(fuJianName);
        }

        #endregion

        #region IEnumerable

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
            return collection;
        }

        #endregion

        #region DateTime

        public static string DateFormat(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion
    }
}
