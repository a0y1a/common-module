using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeKeSoftPlatform.Db
{
    /// <summary>
    /// 代扣导入记录表
    /// </summary>
    public class T_UserImportRecord
    {
        public T_UserImportRecord()
        {
            Id = PF.Key();
            CreateDate = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual T_User User { get; set; }

        /// <summary>
        /// 文件位置
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 成功的条数
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 有问题的条数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 导入时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
