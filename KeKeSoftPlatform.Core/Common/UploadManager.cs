using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KeKeSoftPlatform.Core
{
    public class UploadManager
    {
        private static Func<string> FileNameWithoutExtensionRule = () =>
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + RandomId.Create(6, RandomId.NUMBER);
        };

        #region 系统可以用到的所有上传路径都在这里维护
        public const string Import_Excel = "/upload/Import";
        #endregion

        /// <summary>
        /// 根据虚拟路径（只是目录，不包含文件名称）以及文件名称获重命名之后的完整虚拟路径（如果虚拟目录不存在，则先创建后使用）
        /// </summary>
        /// <param name="virtualDirectory">虚拟路径（只是目录，不包含文件名称）</param>
        /// <param name="fileName">文件名称（包含扩展名称）</param>
        /// <example> 例如获取立面图的完整虚拟路径：
        /// <code>
        /// UploadManager.GetFullVirtualPath(UploadManager.Jiao_Dian_Tu,"立面图.png")
        /// </code>
        /// </example>
        /// <returns>完整虚拟路径</returns>
        public static string GetFullVirtualPath(string virtualDirectory, string fileName)
        {
            if (Directory.Exists(PF.GetPath(virtualDirectory)) == false)
            {
                Directory.CreateDirectory(PF.GetPath(virtualDirectory));
            }
            return $"{Import_Excel}/{FileNameWithoutExtensionRule() + Path.GetExtension(fileName)}";
        }
    }
}
