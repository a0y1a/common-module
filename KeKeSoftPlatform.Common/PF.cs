using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace KeKeSoftPlatform.Common
{
    
    public partial class PF
    {
        /// <summary>
        /// 创建Guid主键
        /// </summary>
        /// <returns></returns>
        public static CombGuid Key()
        {
            return CombGuid.NewComb();
        }
        

        #region 根据虚拟路径获取物理路径
        /// <summary>
        /// 根据虚拟路径获取物理路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径，例如：App_Data/a.txt</param>
        /// <returns></returns>
        public static string GetPath(string virtualPath)
        {
            //多线程执行这里
            virtualPath = virtualPath.Replace("/", "\\");
            if (virtualPath.StartsWith("\\"))//确定 String 实例的开头是否与指定的字符串匹配。为下边的合并字符串做准备
            {
                virtualPath = virtualPath.TrimStart('\\');//从此实例的开始位置移除数组中指定的一组字符的所有匹配项。为下边的合并字符串做准备
            }
            //AppDomain表示应用程序域，它是一个应用程序在其中执行的独立环境　　　　　　　
            //AppDomain.CurrentDomain 获取当前 Thread 的当前应用程序域。
            //BaseDirectory 获取基目录，它由程序集冲突解决程序用来探测程序集。
            //AppDomain.CurrentDomain.BaseDirectory综合起来就是返回此代码所在的路径
            //System.IO.Path.Combine合并两个路径字符串
            //Path.Combine(@"C:\11","aa.txt") 返回的字符串路径如后： C:\11\aa.txt
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
        }
        #endregion

        

    }
}
