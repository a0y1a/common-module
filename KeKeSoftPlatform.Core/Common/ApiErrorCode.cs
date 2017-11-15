using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Core
{
    public enum KehuApiErrorCode
    {
        /// <summary>
        /// 没有token
        /// </summary>
        LoseToken = 1001,
        /// <summary>
        /// 没有number
        /// </summary>
        LoseNumber,
        /// <summary>
        /// 没有number
        /// </summary>
        NotExistsKeHu,
        /// <summary>
        /// 密码不匹配
        /// </summary>
        NotMatchPassword,
        /// <summary>
        /// 没有number
        /// </summary>
        NotMatchToken
    }
}
