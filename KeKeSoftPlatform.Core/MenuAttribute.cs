using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    public class MenuAttribute:Attribute
    {
        /// <summary>
        /// 默认名称
        /// </summary>
        public string DefaultName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defaultName">默认名称</param>
        public MenuAttribute(string defaultName = "未命名")
        {
            this.DefaultName = defaultName;
        }
    }
}
