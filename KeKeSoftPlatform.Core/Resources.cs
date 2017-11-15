using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
namespace KeKeSoftPlatform.Core
{
    public class Resources
    {
        #region 原始错误信息
        public static string CreditCardError
        {
            get
            {
                return "'{PropertyName}' 不是有效的信用卡号";
            }
        }

        public static string email_error
        {
            get
            {
                return "'{PropertyName}' 不是有效的电子邮件地址";
            }
        }
        public static string equal_error
        {
            get
            {
                return "'{PropertyName}' 应该和 '{ComparisonValue}' 相等";
            }
        }
        public static string exact_length_error
        {
            get
            {
                return "'{PropertyName}' 必须是 {MaxLength} 个字符，您已经输入了 {TotalLength} 字符";
            }
        }
        public static string exclusivebetween_error
        {
            get
            {
                return "'{PropertyName}' 必须在 {From} 和 {To} 之外， 您输入了 {Value}";
            }
        }
        public static string greaterthan_error
        {
            get
            {
                return "'{PropertyName}' 必须大于 '{ComparisonValue}'";
            }
        }
        public static string greaterthanorequal_error
        {
            get
            {
                return "'{PropertyName}' 必须大于或等于 '{ComparisonValue}'";
            }
        }
        public static string inclusivebetween_error
        {
            get
            {
                return "'{PropertyName}' 必须在 {From} 和 {To} 之间， 您输入了 {Value}";
            }
        }
        public static string length_error
        {
            get
            {
                return "'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符，您已经输入了 {TotalLength} 字符。";
            }
        }
        public static string lessthan_error
        {
            get
            {
                return "'{PropertyName}' 必须小于 '{ComparisonValue}'";
            }
        }
        public static string lessthanorequal_error
        {
            get
            {
                return "'{PropertyName}' 必须小于或等于 '{ComparisonValue}'";
            }
        }
        public static string notempty_error
        {
            get
            {
                return "请填写 '{PropertyName}'";
            }
        }
        public static string notequal_error
        {
            get
            {
                return "'{PropertyName}' 不能和 '{PropertyValue}' 相等";
            }
        }
        public static string notnull_error
        {
            get
            {
                return "请填写 '{PropertyName}'";
            }
        }
        public static string predicate_error
        {
            get
            {
                return "指定的条件不符合 '{PropertyName}'";
            }
        }
        public static string regex_error
        {
            get
            {
                return "'{PropertyName}' 的格式不正确";
            }
        }
        public static string scale_precision_error
        {
            get
            {
                return "'{PropertyName}' 总位数不能超过 {expectedPrecision} 位，其中整数部分 {expectedScale} 位。您填写了 {digits} 位小数和 {actualScale} 位整数";
            }
        }
        #endregion
    }
}
