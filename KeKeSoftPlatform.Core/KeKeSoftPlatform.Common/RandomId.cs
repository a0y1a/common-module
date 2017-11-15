using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Common
{
    public sealed class RandomId : IFormattable
    {
        public const string DEFAULT_WORDS = "0123456789abcdefghigklmnopqrstuvwxyz~!@#$%^&*()";
        public const string NUMBER = "0123456789";

        #region 私有对象
        const string ONE = "{0}";
        static RandomId _Rid = new RandomId(0);
        readonly static Random _Rand = new Random();
        static string ToFormat(int length)
        {
            StringBuilder sb = new StringBuilder(length * 3);
            for (int i = 0; i < length; i++)
            {
                sb.Append(ONE);
            }
            return sb.ToString();
        }

        readonly string _Dict;
        readonly int _RMax;
        readonly string _Format;
        #endregion

        /// <summary> 构造函数
        /// </summary>
        /// <param name="length">生成Id长度</param>
        /// <param name="dict">随机字符字典,默认字典为0-9a-z~!@#$%^&*()</param>
        public RandomId(int length, string dict = DEFAULT_WORDS)
            : this(RandomId.ToFormat(length), dict)
        { }
        /// <summary> 构造函数
        /// </summary>
        /// <param name="format">生成Id格式</param>
        /// <param name="dict">随机字符字典,默认字典为0-9a-z~!@#$%^&*()</param>
        public RandomId(string format, string dict = DEFAULT_WORDS)
        {
            _Dict = dict;
            _Format = format;
            _RMax = dict.Length;
        }

        /// <summary> 生成Id
        /// </summary>
        public string Create()
        {
            return string.Format(_Format, this);
        }
        
        
        /// <summary>
        /// 生成Id
        /// </summary>
        /// <param name="length">生成Id长度</param>
        /// <param name="dict">随机字符字典,默认字典为0-9a-z~!@#$%^&*()</param>
        /// <returns></returns>
        public static string Create(int length, string dict = DEFAULT_WORDS)
        {
            return new RandomId(length, dict).Create();
        }
        /// <summary> 生成Id
        /// </summary>
        /// <param name="format">生成Id格式</param>
        /// <param name="dict">随机字符字典,默认字典为0-9a-z~!@#$%^&*()</param>
        public static string Create(string format, string dict = DEFAULT_WORDS)
        {
            return new RandomId(format, dict).Create();
        }

        #region IFormattable 成员

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return _Dict[_Rand.Next(0, _RMax)].ToString();
        }

        #endregion

        
    }

}
