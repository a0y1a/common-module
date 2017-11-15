using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    public class MenuActionAttribute:Attribute
    {
        /// <summary>
        /// 所属菜单地址
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 编号（用来在前台验证是否显示菜单操作）
        /// </summary>
        public string MenuActionCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string MenuActionName { get; set; }

        /// <summary>
        /// 构造函数，创建一个菜单操作权限项目
        /// </summary>
        /// <param name="menuUrl">所属菜单地址</param>
        /// <param name="menuActionCode">编号（用来在前台验证是否显示菜单操作）</param>
        /// <param name="menuActionName">名称</param>
        public MenuActionAttribute(string menuUrl, string menuActionCode, string menuActionName)
        {
            this.MenuUrl = menuUrl;
            this.MenuActionCode = menuActionCode;
            this.MenuActionName = menuActionName;
        }
    }
}
