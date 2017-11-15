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
    /// 代扣状态
    /// </summary>
    public enum DaiKouStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [EnumValue("待审核")]
        Checking = 1,
        /// <summary>
        /// 已审核
        /// </summary>
        [EnumValue("已审核")]
        Checked,
        /// <summary>
        /// 银联处理中
        /// </summary>
        [EnumValue("银联处理中")]
        ApplyDaiKou,
        /// <summary>
        /// 代扣成功
        /// </summary>
        [EnumValue("代扣成功")]
        Success,
        /// <summary>
        /// 代扣失败
        /// </summary>
        [EnumValue("代扣失败")]
        Failure
    }

    /// <summary>
    /// 代扣表
    /// </summary>
    public class T_DaiKou
    {
        public T_DaiKou()
        {
            Id = PF.Key();
            Status = DaiKouStatus.Checking;
            CreateDate = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual T_User User { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(30)]
        public string IDNumber { get; set; }

        /// <summary>
        /// 持卡人姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        [MaxLength(200)]
        public string BankCardNumber { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [MaxLength(200)]
        public string BankName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [MaxLength(30)]
        public string Phone{ get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 开户行省份
        /// </summary>
        [MaxLength(30)]
        public string BankProvince { get; set; }

        /// <summary>
        /// 用途说明
        /// </summary>
        public string PurposeDescription { get; set; }

        /// <summary>
        /// 附言说明
        /// </summary>
        public string MemoDescription { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [MaxLength(50)]
        public string Version { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public DaiKouStatus Status { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 申请代扣日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }
    }
}
