using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.IO;
using KeKeSoftPlatform.Common;
using FluentValidation.Attributes;
using FluentValidation;
using System.Text.RegularExpressions;

namespace KeKeSoftPlatform.Core
{
    [Validator(typeof(ConfigParameterValidator))]
    public class ConfigParameter
    {
        [Display(Name = "商户默认密码")]
        public string DefaultPassword { get; set; }

        /// <summary>
        /// 管理员加密后密码
        /// </summary>
        [Display(Name = "管理员加密后密码")]
        public string MD5EncryptPassword { get; set; }
    }

    public class ConfigParameterValidator : AbstractValidator<ConfigParameter>
    {
        public ConfigParameterValidator()
        {
            RuleFor(m => m.DefaultPassword).NotEmpty().WithMessage("请输入默认密码").Matches(@"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,12}$").WithMessage("6到12位，必须字母和数字");
        }
    }

    public class Config
    {
        public const string SITE_NAME = "";
        public const string NAVIGATION_TOP_KEY = "_top_key_";
        public const string NAVIGATION_ITEM_KEY = "_item_key_";

        public static string PATH { get { return FilePath(); } }

        public static string FilePath()
        {
            string logFile = "App_Data/Config.xml";
            if (HttpContext.Current != null)
            {
                logFile = HttpContext.Current.Server.MapPath("~/" + logFile);
            }
            else
            {
                //多线程执行这里
                logFile = logFile.Replace("/", "\\");
                if (logFile.StartsWith("\\"))//确定 String 实例的开头是否与指定的字符串匹配。为下边的合并字符串做准备
                {
                    logFile = logFile.TrimStart('\\');//从此实例的开始位置移除数组中指定的一组字符的所有匹配项。为下边的合并字符串做准备
                }
                //AppDomain表示应用程序域，它是一个应用程序在其中执行的独立环境　　　　　　　
                //AppDomain.CurrentDomain 获取当前 Thread 的当前应用程序域。
                //BaseDirectory 获取基目录，它由程序集冲突解决程序用来探测程序集。
                //AppDomain.CurrentDomain.BaseDirectory综合起来就是返回此代码所在的路径
                //System.IO.Path.Combine合并两个路径字符串
                //Path.Combine(@"C:\11","aa.txt") 返回的字符串路径如后： C:\11\aa.txt
                logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFile);

            }
            return logFile;
        }

        public class KeyValueItem<T, K>
        {
            public T Key { get; set; }
            public K Value { get; set; }
        }

        public static Config Instance { get; set; }
        public ConfigParameter ConfigParameter { get; set; }
        public void Serialize()
        {
            Instance = this;
            XmlHelper.XmlSerializeToFile(this, PATH, Encoding.UTF8);
        }

        static Config()
        {
            Instance = new Config();
            if (File.Exists(PATH))
            {
                Instance = XmlHelper.XmlDeserializeFromFile<Config>(PATH, Encoding.UTF8);
            }
            else
            {
                Instance.ConfigParameter = new ConfigParameter
                {
                    DefaultPassword = "123qwe",
                    MD5EncryptPassword = "21232f297a57a5a743894a0e4a801fc3"
                };
            }
        }
    }
}
