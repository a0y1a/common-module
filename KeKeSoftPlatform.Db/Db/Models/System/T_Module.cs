using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Db
{
    /// <summary>
    /// 模块表
    /// </summary>
    public class T_Module
    {
        public T_Module()
        {
            Id = PF.Key();
            CreateDate = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public int Sequence { get; set; }

        public string Url { get; set; }

        public virtual T_Module ParentModule { get; set; }
        public Guid? ParentId { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual ICollection<T_Module> Modules { get; set; }
    }
}
