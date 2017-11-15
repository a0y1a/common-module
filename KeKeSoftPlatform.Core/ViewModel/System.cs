using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Attributes;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace KeKeSoftPlatform.Core
{
    #region 登录
    [Validator(typeof(AdminLoginDataValidator))]
    public class AdminLoginData
    {
        [Display(Name = "管理员编号")]
        public string Number { get; set; }

        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "验证码")]
        public string Captcha { get; set; }

        public string ReturnUrl { get; set; }
    }
    public class AdminLoginDataValidator : AbstractValidator<AdminLoginData>
    {
        public AdminLoginDataValidator()
        {
            RuleFor(m => m.Number).NotEmpty();
            RuleFor(m => m.Password).NotEmpty();
            RuleFor(m => m.Captcha).NotEmpty();
        }
    }
    #endregion

    #region 菜单分类、菜单、权限
    public enum MenuElementType
    {
        Category,
        Menu
    }
    public class MenuCategoryLoadData
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class MenuCategoryDetailData
    {
        public List<T_Menu> ChildrenMenus { get; set; }
        public List<T_Menu> NoneMenuCategoryMenus { get; set; }
        public List<T_MenuCategory> Children { get; set; }
        public class MenuElement
        {
            public Guid Id { get; set; }
            public MenuElementType Type { get; set; }
            public int SequenceNum { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
        }
        public List<MenuElement> MenuElements { get; set; }
    }

    public class MenuElement
    {
        public Guid Id { get; set; }
        public MenuElementType Type { get; set; }
    }

    public class MenuInitItem
    {
        public T_Menu Menu { get; set; }
        public List<T_MenuAction> MenuActionCollection { get; set; }
    }

    #endregion

    #region 商户管理

    [Validator(typeof(CreateUserDataValidator))]
    public class CreateUserData
    {
        [Display(Name = "商户号")]
        [IsRequired]
        public string Number { get; set; }

        [Display(Name = "商户名")]
        [IsRequired]
        public string Name { get; set; }

        [Display(Name = "操作员账号")]
        [IsRequired]
        public string AdminNumber { get; set; }

        [Display(Name = "公司注册名")]
        public string CompanyName { get; set; }

        [Display(Name = "公司注册号")]
        public string CompanyNumber { get; set; }

        [Display(Name = "法人代表")]
        public string RepresentativeName { get; set; }

        [Display(Name = "法人代表身份证号")]
        public string RepresentativeIDNumber { get; set; }

        [Display(Name = "联系人")]
        public string ContactName { get; set; }

        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }

        [Display(Name = "结算账户名")]
        public string BalanceName { get; set; }

        [Display(Name = "结算银行所属地区")]
        public string BalanceBankArea { get; set; }

        [Display(Name = "结算账号")]
        public string BalanceNumber { get; set; }

        [Display(Name = "结算银行")]
        public string BalanceBank { get; set; }

        [Display(Name = "结算类型")]
        [DropDownList("BalanceType")]
        public BalanceType BalanceType { get; set; }
    }

    public class CreateUserDataValidator : AbstractValidator<CreateUserData>
    {
        public CreateUserDataValidator()
        {
            RuleFor(m => m.Number).NotEmpty().WithMessage("请输入商户号").Must(m =>
            {
                using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                    return db.User.Any(n => n.Number.Trim() == m.Trim()) == false;
                }
            }).WithMessage("商户号已经存在，请重新输入");
            RuleFor(m => m.Name).NotEmpty().WithMessage("请输入商户名");
            RuleFor(m => m.AdminNumber).NotEmpty().WithMessage("请输入操作员账号").Must(m =>
            {
                using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                    return db.User.Any(n => n.AdminNumber.Trim() == m.Trim()) == false;
                }
            }).WithMessage("操作员账号已经存在，请重新输入");
            RuleFor(m => m.RepresentativeIDNumber)
                .Length(18).WithMessage("您输入的身份证号格式不对")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && Regex.IsMatch(k.ToString().Trim(), @"[^\x00-\xff]"))
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && !Regex.IsMatch(k.ToString().Trim().Substring(0, 17), @"\d{15}|\d{18}"))
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && k.ToString().Trim().Replace(" ", "").Length != 18)
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k))
                    {
                        int[] iW = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
                        int iSum = 0;
                        var v_card = k;
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
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!");
        }
    }

    [Validator(typeof(EditUserDataValidator))]
    public class EditUserData
    {
        [HiddenInput(DisplayValue = false)]
        public Guid UserId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        [Display(Name = "商户号")]
        [DisplayOnly(HideValue = true)]
        public string Number { get; set; }

        [Display(Name = "商户名")]
        [IsRequired]
        public string Name { get; set; }

        [Display(Name = "操作员账号")]
        [DisplayOnly(HideValue = true)]
        public string AdminNumber { get; set; }

        [Display(Name = "公司注册名")]
        public string CompanyName { get; set; }

        [Display(Name = "公司注册号")]
        public string CompanyNumber { get; set; }

        [Display(Name = "法人代表")]
        public string RepresentativeName { get; set; }

        [Display(Name = "法人代表身份证号")]
        public string RepresentativeIDNumber { get; set; }

        [Display(Name = "联系人")]
        public string ContactName { get; set; }

        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }

        [Display(Name = "结算账户名")]
        public string BalanceName { get; set; }

        [Display(Name = "结算银行所属地区")]
        public string BalanceBankArea { get; set; }

        [Display(Name = "结算账号")]
        public string BalanceNumber { get; set; }

        [Display(Name = "结算银行")]
        public string BalanceBank { get; set; }

        [Display(Name = "结算类型")]
        [DropDownList("BalanceType")]
        public BalanceType BalanceType { get; set; }
    }

    public class EditUserDataValidator : AbstractValidator<EditUserData>
    {
        public EditUserDataValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("请输入商户名");
            RuleFor(m => m.RepresentativeIDNumber)
                .Length(18).WithMessage("您输入的身份证号格式不对")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && Regex.IsMatch(k.ToString().Trim(), @"[^\x00-\xff]"))
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && !Regex.IsMatch(k.ToString().Trim().Substring(0, 17), @"\d{15}|\d{18}"))
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k) && k.ToString().Trim().Replace(" ", "").Length != 18)
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!")
                .Must(k =>
                {
                    if (!string.IsNullOrWhiteSpace(k))
                    {
                        int[] iW = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
                        int iSum = 0;
                        var v_card = k;
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
                    }
                    return true;
                }).WithMessage("您输入的身份证号格式不对!");
        }
    }

    #endregion

    #region 代扣管理

    public class StatisticsData
    {
        [Display(Name = "商户号")]
        public string Number { get; set; }

        [Display(Name = "商户名")]
        public string Name { get; set; }

        [Display(Name = "交易金额")]
        public decimal Amount { get; set; }

        [Display(Name = "代扣成功笔数")]
        public int SuccessCount { get; set; }
    }

    #endregion

    #region 参数配置

    [Validator(typeof(ConfigDataValidator))]
    public class ConfigData
    {
        public KeKeSoftPlatform.Core.Config Config { get; set; }

        /// <summary>
        /// 重置密码
        /// </summary>
        [Display(Name = "设置密码" )]
        [DataType(DataType.Password)]
        public string ResetPassword { get; set; }
    }

    public class ConfigDataValidator : AbstractValidator<ConfigData>
    {
        public ConfigDataValidator()
        {
            RuleFor(m => m.ResetPassword)
               .Length(6, 12).WithMessage("6到12位，必须字母和数字")
               .Matches(@"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,12}$").WithMessage("6到12位，必须字母和数字");
        }
    }

    #endregion

    #region 修改密码

    [Validator(typeof(ChangeAdminPwdDataValidator))]
    public class ChangeAdminPwdData
    {
        [Display(Name = "旧密码")]
        [IsRequired]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "新密码")]
        [IsRequired]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "确认新密码")]
        [IsRequired]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }

    public class ChangeAdminPwdDataValidator : AbstractValidator<ChangeAdminPwdData>
    {
        public ChangeAdminPwdDataValidator()
        {
            RuleFor(m => m.OldPassword).NotEmpty().WithMessage("请输入旧密码").Must(oldPassword =>
            {
                return EncryptUtils.MD5Encrypt(oldPassword) == KeKeSoftPlatform.Core.Config.Instance.ConfigParameter.MD5EncryptPassword;
            }).WithMessage("旧密码不匹配");

            RuleFor(m => m.NewPassword).NotEmpty().Matches("^(?![0-9]+$)[0-9A-Za-z]{6,16}$")
                .WithMessage("密码由字母和数字6-16位组成，不能为纯数字");

            RuleFor(m => m.ConfirmNewPassword).Equal(m => m.NewPassword).WithMessage("确认新密码必须与新密码相同");
        }
    }

    #endregion
}
