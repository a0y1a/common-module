using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace KeKeSoftPlatform.Db
{
    public class Service
    {
        public const string CAPTCHA = "_CAPTCHA_";
        public const string WX_DOMAIN = "localhost:18888";
        public const string DOMAIN = "118.89.232.41:52501";

        private KeKeSoftPlatformDbContext db;
        public Service(KeKeSoftPlatformDbContext db)
        {
            this.db = db;
        }

        #region 汉字转拼音
        /// <summary> 
        /// 汉字转拼音缩写 
        /// </summary> 
        /// <param name="str">要转换的汉字字符串</param> 
        /// <returns>拼音缩写</returns> 
        public static string GetPYString(string str)
        {
            string tempStr = "";
            if (!IsChina(str))
            {
                return str;
            }
            foreach (char c in str)
            {
                if ((int)c >= 33 && (int)c <= 126)
                {//字母和符号原样保留 
                    tempStr += c.ToString();
                }
                else
                {//累加拼音声母 
                    tempStr += GetPYChar(c.ToString());
                }
            }
            return tempStr;
        }

        public static bool IsChina(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    return BoolValue = true;
                }
            }
            return BoolValue;
        }


        /// <summary> 
        /// 取单个字符的拼音声母 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音声母</returns> 
        private static string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "g";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";

            return "*";
        }
        #endregion

        public static string Url(string path)
        {
            if (path.StartsWith("/")==false)
            {
                path = "/" + path;
            }
            return string.Format("http://{0}{1}",DOMAIN, path);
        }

        /// <summary>
        /// 验证二代身份证号正确性
        /// </summary>
        /// <param name="idNum"></param>
        /// <returns></returns>
        public static bool CheckIdNumber(string idNum)
        {
            try
            {
                DateTime birthDate = DateTime.Parse(idNum.Trim().Substring(6, 4) + "/" + idNum.Trim().Substring(10, 2) + "/" + idNum.Trim().Substring(12, 2));
            }
            catch (Exception e)
            {
                return false;
            }

            //判断是否是18位
            if (idNum.ToString().Trim().Replace(" ", "").Length != 18)
            {
                return false;
            }
            //判断是否含有全角字符
            if (Regex.IsMatch(idNum.ToString().Trim(), @"[^\x00-\xff]"))
            {
                return false;
            }
            //判断前17位是否是数字
            if (!Regex.IsMatch(idNum.ToString().Trim().Substring(0, 17), @"^\d{17}$"))
            {
                return false;
            }
            int[] iW = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            int iSum = 0;
            var v_card = idNum;
            for (var i = 0; i < 17; i++)
            {
                var iC = v_card.Substring(i, 1);
                var iVal = Convert.ToInt32(iC);
                iSum += iVal * iW[i];
            }
            var iJYM = iSum % 11;
            var sJYM = "";
            if (iJYM == 0) sJYM = "1";
            else if (iJYM == 1) sJYM = "0";
            else if (iJYM == 2) sJYM = "x";
            else if (iJYM == 3) sJYM = "9";
            else if (iJYM == 4) sJYM = "8";
            else if (iJYM == 5) sJYM = "7";
            else if (iJYM == 6) sJYM = "6";
            else if (iJYM == 7) sJYM = "5";
            else if (iJYM == 8) sJYM = "4";
            else if (iJYM == 9) sJYM = "3";
            else if (iJYM == 10) sJYM = "2";
            var cCheck = v_card.Substring(17, 1).ToLower();
            if (cCheck != sJYM)
            {
                return false;
            }
            return true;
        }
    }
}
