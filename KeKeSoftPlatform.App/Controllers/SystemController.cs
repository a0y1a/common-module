using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using System.Reflection;
using System.Data.Entity;

namespace KeKeSoftPlatform.App.Controllers
{
    [Right(Identity.Admin)]
    public class SystemController : BaseController
    {
        #region 登录

        [Right(AllowAnonymous = true)]
        public ActionResult Login(string returnUrl)
        {
            return View(new AdminLoginData { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Right(AllowAnonymous = true)]
        public ActionResult Login(AdminLoginData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            #region 验证码
            if (Session[Service.CAPTCHA] == null)
            {
                ModelState.AddModelError("Captcha", "验证码已过期");
                return View(model);
            }
            if (Session[Service.CAPTCHA] as string != model.Captcha)
            {
                ModelState.AddModelError("Captcha", "验证码不匹配");
                return View(model);
            }
            #endregion

            if (string.Equals(model.Number, "admin", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                ModelState.AddModelError("Number", "账号不正确");
                return View(model);
            }

            if (EncryptUtils.MD5Encrypt(model.Password) != KeKeSoftPlatform.Core.Config.Instance.ConfigParameter.MD5EncryptPassword)
            {
                ModelState.AddModelError("Password", "密码不正确");
                return View(model);
            }

            FormsPrincipal<_User>.SignIn("admin", new _User
            {
                Id = Guid.NewGuid(),
                Name = "管理员",
                Code = "admin",
                IsPersistent = true,
                EnableSingleOnline = false,
                OwnIdentities = new List<Identity> { Identity.Admin }
            });

            if (string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                return RedirectToAction("DaiKous");
            }
            return Redirect(model.ReturnUrl);
        }
       
        #endregion

        #region 验证码
        [Right(AllowAnonymous =true)]
        public ActionResult Captcha()
        {
            Captcha captcha = new Captcha();
            var value = captcha.CreateCaptcha(4);
            Session[Service.CAPTCHA] = value;
            return File(captcha.CreateCaptchaGraphic(value), @"image/jpeg");
        }
        #endregion

        #region 商户管理

        [Menu("商户列表")]
        public ActionResult Users(string adminNumber, string contactPhone, BalanceType? banlanceType, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_User> query = db.User;

                if (string.IsNullOrWhiteSpace(adminNumber) == false)
                {
                    query = query.Where(m => m.AdminNumber.Contains(adminNumber));
                }
                if (string.IsNullOrWhiteSpace(contactPhone) == false)
                {
                    query = query.Where(m => m.ContactPhone.Contains(contactPhone));
                }
                if(banlanceType.HasValue)
                {
                    query = query.Where(m => m.BalanceType == banlanceType.Value);
                }

                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        public ActionResult CreateUser()
        {
            return View(new CreateUserData { Number = UserEncoder.Instance.NextToString(), BalanceType = BalanceType.Public });
        }

        [HttpPost]
        public ActionResult CreateUser(CreateUserData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.User.Add(new T_User
                {
                    Number = model.Number,
                    Name = model.Name,
                    Password = EncryptUtils.MD5Encrypt(KeKeSoftPlatform.Core.Config.Instance.ConfigParameter.DefaultPassword),
                    AdminNumber = model.AdminNumber,
                    CompanyName = model.CompanyName,
                    CompanyNumber = model.CompanyNumber,
                    RepresentativeName = model.RepresentativeName,
                    RepresentativeIDNumber = model.RepresentativeIDNumber,
                    ContactName = model.ContactName,
                    ContactPhone = model.ContactPhone,
                    BalanceName = model.BalanceName,
                    BalanceBankArea = model.BalanceBankArea,
                    BalanceNumber = model.BalanceNumber,
                    BalanceBank = model.BalanceBank,
                    BalanceType = model.BalanceType
                });
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return RedirectToAction("Users");
            }
        }

        public ActionResult EditUser(Guid userId, string returnUrl = "/system/Users")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var user = db.User.Find(userId);
                return View(new EditUserData
                {
                    ReturnUrl = returnUrl,
                    UserId = userId,
                    Number = user.Number,
                    Name = user.Name,
                    AdminNumber = user.AdminNumber,
                    CompanyName = user.CompanyName,
                    CompanyNumber = user.CompanyNumber,
                    RepresentativeName = user.RepresentativeName,
                    RepresentativeIDNumber = user.RepresentativeIDNumber,
                    ContactName = user.ContactName,
                    ContactPhone = user.ContactPhone,
                    BalanceName = user.BalanceName,
                    BalanceBankArea = user.BalanceBankArea,
                    BalanceNumber = user.BalanceNumber,
                    BalanceBank = user.BalanceBank,
                    BalanceType = user.BalanceType
                });
            }
        }

        [HttpPost]
        public ActionResult EditUser(EditUserData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var user = db.User.Find(model.UserId);

                user.Name = model.Name;
                user.CompanyName = model.CompanyName;
                user.CompanyNumber = model.CompanyNumber;
                user.RepresentativeName = model.RepresentativeName;
                user.RepresentativeIDNumber = model.RepresentativeIDNumber;
                user.ContactName = model.ContactName;
                user.ContactPhone = model.ContactPhone;
                user.BalanceName = model.BalanceName;
                user.BalanceBankArea = model.BalanceBankArea;
                user.BalanceNumber = model.BalanceNumber;
                user.BalanceBank = model.BalanceBank;
                user.BalanceType = model.BalanceType;

                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(model.ReturnUrl);
            }
        }

        public PartialViewResult UserDetail(Guid userId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return PartialView(db.User.Find(userId));
            }
        }

        public ActionResult DeleteUser(Guid userId, string returnUrl = "/system/Users")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.User.Remove(db.User.Find(userId));
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(returnUrl);
            }
        }

        public ActionResult ResetUserPwd(Guid userId, string returnUrl = "/system/Users")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.User.Find(userId).Password = EncryptUtils.MD5Encrypt(KeKeSoftPlatform.Core.Config.Instance.ConfigParameter.DefaultPassword);
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("重置密码成功");
                return Redirect(returnUrl);
            }
        }

        #endregion

        #region 代扣管理

        [Menu("代扣信息")]
        public ActionResult DaiKous(string userNumber, string iDNumber, string name, string phone, DaiKouStatus? status, DateTime? minDate, DateTime? maxDate, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_DaiKou> query = db.DaiKou.Include(m => m.User);

                if (string.IsNullOrWhiteSpace(userNumber) == false)
                {
                    query = query.Where(m => m.User.Number.Contains(userNumber));
                }
                if (string.IsNullOrWhiteSpace(iDNumber) == false)
                {
                    query = query.Where(m => m.IDNumber.Contains(iDNumber));
                }
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    query = query.Where(m => m.Name.Contains(name));
                }
                if (string.IsNullOrWhiteSpace(phone) == false)
                {
                    query = query.Where(m => m.Phone.Contains(phone));
                }
                if (status.HasValue)
                {
                    query = query.Where(m => m.Status == status.Value);
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate < maxDate.Value);
                }

                return View(query.OrderByDescending(m => m.Id).Page(pageNum));
            }
        }

        public ActionResult CheckDaiKou(Guid daiKouId, string returnUrl = "/system/DaiKous")
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var daiKou = db.DaiKou.Find(daiKouId);
                
                if(daiKou.Status != DaiKouStatus.Checking)
                {
                    TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("状态异常，审核失败", AlertType.Danger);
                    return Redirect(returnUrl);
                }

                daiKou.Status = DaiKouStatus.Checked;
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(returnUrl);
            }
        }

        [HttpPost]
        public ActionResult DealDaiKou(Guid daiKouId, DaiKouStatus status)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var daiKou = db.DaiKou.Find(daiKouId);

                if (daiKou.Status != DaiKouStatus.ApplyDaiKou)
                {
                    TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("状态异常，处理失败", AlertType.Danger);
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                }

                daiKou.Status = status;
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
        }
        public ActionResult DeleteDaiKou(Guid daiKouId, string returnUrl = "/system/DaiKous")
        {
            using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.DaiKou.Remove(db.DaiKou.Find(daiKouId));
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(returnUrl);
            }
        }

        public ActionResult ExportDaiKou(string userNumber, string iDNumber, string name, string phone, DaiKouStatus? status, DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_DaiKou> query = db.DaiKou.Include(m => m.User);

                if (string.IsNullOrWhiteSpace(userNumber) == false)
                {
                    query = query.Where(m => m.User.Number.Contains(userNumber));
                }
                if (string.IsNullOrWhiteSpace(iDNumber) == false)
                {
                    query = query.Where(m => m.IDNumber.Contains(iDNumber));
                }
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    query = query.Where(m => m.Name.Contains(name));
                }
                if (string.IsNullOrWhiteSpace(phone) == false)
                {
                    query = query.Where(m => m.Phone.Contains(phone));
                }
                if (status.HasValue)
                {
                    query = query.Where(m => m.Status == status.Value);
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate < maxDate.Value);
                }

                var data = query.OrderByDescending(m => m.Id).ToList();

                return ExportToExcel("代扣信息表", Grid.Create<T_DaiKou>().SetDataSource(data)
                                                    .Column(m => m.User.Number).Name("商户号").End()
                                                    .Column(m => m.IDNumber).Name("身份证号").End()
                                                    .Column(m => m.Name).Name("持卡人姓名").End()
                                                    .Column(m => m.BankCardNumber).Name("银行卡号").End()
                                                    .Column(m => m.BankName).Name("银行名称").End()
                                                    .Column(m => m.Phone).Name("电话号码").End()
                                                    .Column(m => m.Amount).Name("交易金额").End()
                                                    .Column(m => m.BankProvince).Name("开户行省份").End()
                                                    .Column(m => m.PurposeDescription).Name("用途说明").End()
                                                    .Column(m => m.MemoDescription).Name("附言说明").End()
                                                    .Column(m => m.Version).Name("版本").End()
                                                    .Column(m => m.Status).Name("状态").Value(typeof(DaiKouStatus)).End()
                                                    .Column(m => m.CreateDate).Name("创建日期").Value(m => m.CreateDate.DateFormat()).End()
                                                    .ToTable());
            }
        }

        /// <summary>
        /// 查询状态为 银联处理中 的代扣
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryDaiKou(DateTime? applyDate)
        {
            using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                if(applyDate.HasValue == false)
                {
                    var daiKouList = db.DaiKou.Where(m => m.Status == DaiKouStatus.ApplyDaiKou).OrderByDescending(m => m.ApplyDate).ToList();
                    if (daiKouList.Count == 0)
                    {
                        return Json(new ReturnValue { IsSuccess = false });
                    }
                    return Json(new ReturnValue { IsSuccess = true, Data = new { Count = daiKouList.Count, ApplyDate = daiKouList.FirstOrDefault().ApplyDate } });
                }
                else
                {
                    //查询最近一条的代扣记录
                    var daiKou = db.DaiKou.Where(m => m.Status == DaiKouStatus.ApplyDaiKou).OrderByDescending(m => m.ApplyDate).FirstOrDefault();

                    if(daiKou != null && daiKou.ApplyDate > applyDate.Value)
                    {
                        return Json(new ReturnValue { IsSuccess = true, Data = new { ApplyDate = daiKou.ApplyDate } });
                    }
                    return Json(new ReturnValue { IsSuccess = false });
                }
            }
        }

        #endregion

        #region 统计查询 

        [Menu("统计查询")]
        public ActionResult DaiKouStatistics(string userNumber, DateTime? minDate, DateTime? maxDate, int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_DaiKou> query = db.DaiKou.Include(m => m.User);

                if (string.IsNullOrWhiteSpace(userNumber) == false)
                {
                    query = query.Where(m => m.User.Number.Contains(userNumber));
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate < maxDate.Value);
                }

                return View(query.GroupBy(m => m.User.Number).Select(m => new StatisticsData
                                {
                                    Number = m.Key,
                                    Name = m.FirstOrDefault().User.Name,
                                    SuccessCount = m.Where(n => n.Status == DaiKouStatus.Success).Count(),
                                    Amount = m.Select(n => n.Amount).DefaultIfEmpty(0m).Sum()
                                }).OrderBy(m => m.Name).Page(pageNum));
            }
        }

        public ActionResult DaiKouStatisticsDetail(string userNumber, DateTime? minDate, DateTime? maxDate)
        {
            using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_DaiKou> query = db.DaiKou.Include(m => m.User);

                if (string.IsNullOrWhiteSpace(userNumber) == false)
                {
                    query = query.Where(m => m.User.Number.Contains(userNumber));
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate >= minDate.Value);
                    ViewBag.MinDate = minDate.Value.DateFormat();
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate < maxDate.Value);
                    ViewBag.MaxDate = maxDate.Value.DateFormat();
                }

                return View(query.OrderByDescending(m => m.Id).ToList());
            }
        }

        public ActionResult ExportDaiKouStatistics(string userNumber, DateTime? minDate, DateTime? maxDate)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                IQueryable<T_DaiKou> query = db.DaiKou.Include(m => m.User);

                if (string.IsNullOrWhiteSpace(userNumber) == false)
                {
                    query = query.Where(m => m.User.Number.Contains(userNumber));
                }
                if (minDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate >= minDate.Value);
                }
                if (maxDate.HasValue)
                {
                    query = query.Where(m => m.CreateDate < maxDate.Value);
                }

                var data = query.GroupBy(m => m.User.Number).Select(m => new StatisticsData
                {
                    Number = m.Key,
                    Name = m.FirstOrDefault().User.Name,
                    SuccessCount = m.Where(n => n.Status == DaiKouStatus.Success).Count(),
                    Amount = m.Select(n => n.Amount).DefaultIfEmpty(0m).Sum()
                }).OrderBy(m => m.Name).ToList();

                return ExportToExcel("统计信息", Grid.Create<StatisticsData>().SetDataSource(data)
                                    .Column(m => m.Number).Name("商户号").End()
                                    .Column(m => m.Name).Name("商户名").End()
                                    .Column(m => m.Amount).Name("交易金额").End()
                                    .Column(m => m.SuccessCount).Name("代扣成功笔数").End()
                                    .ToTable());
            }
        }

        #endregion

        #region 菜单权限
        public ActionResult MenuCategory()
        {
            return View();
        }

        public ActionResult LoadMenuCategory()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var menuCategories = db.MenuCategory.ToList();
                var data = new List<ZTreeNode<MenuCategoryLoadData>>();

                foreach (var item in menuCategories.Where(m => m.ParentId.HasValue == false).OrderBy(m => m.SequenceNum).ToList())
                {
                    var node = new ZTreeNode<MenuCategoryLoadData>()
                    {
                        Name = item.Name,
                        UserData = new MenuCategoryLoadData
                        {
                            Id = item.Id,
                            Name = item.Name
                        }
                    };
                    data.Add(node);
                    this.AppendMenuCategory(node, menuCategories);
                }

                return JsonNet(new ReturnValue { IsSuccess = true, Data = data });
            }
        }
        private void AppendMenuCategory(ZTreeNode<MenuCategoryLoadData> parentNode, List<T_MenuCategory> menuCategories)
        {
            foreach (var item in menuCategories.Where(m => m.ParentId == parentNode.UserData.Id).OrderBy(m => m.SequenceNum).ToList())
            {
                var node = new ZTreeNode<MenuCategoryLoadData>()
                {
                    Name = item.Name,
                    UserData = new MenuCategoryLoadData
                    {
                        Id = item.Id,
                        Name = item.Name
                    }
                };
                parentNode.Children.Add(node);
                this.AppendMenuCategory(node, menuCategories);
            }
        }

        [HttpPost]
        public JsonNetResult CreateMenuCategory(Guid? parentMenuCategoryId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var entity = new T_MenuCategory
                {
                    Id = PF.Key(),
                    Name = "新添加节点",
                    ParentId = parentMenuCategoryId
                };
                if (parentMenuCategoryId.HasValue)
                {
                    var parent = db.MenuCategory.Find(parentMenuCategoryId.Value);
                    entity.Path = parent.Path + UtilEncoder.Instance.NextToString();
                    entity.SequenceNum = parent.Children.Count() + 1;
                }
                else
                {
                    entity.Path = UtilEncoder.Instance.NextToString();
                    entity.SequenceNum = db.MenuCategory.Count(m => m.ParentId.HasValue == false) + 1;
                }
                db.MenuCategory.Add(entity);

                db.SaveChanges();
                return JsonNet(new ReturnValue
                {
                    IsSuccess = true,
                    Data = new ZTreeNode<MenuCategoryLoadData>()
                    {
                        Name = entity.Name,
                        UserData = new MenuCategoryLoadData
                        {
                            Id = entity.Id,
                            Name = entity.Name
                        }
                    }
                });
            }
        }

        [HttpPost]
        public JsonNetResult EditMenuCategory(Guid menuCategoryId, string menuCategoryName)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.MenuCategory.Find(menuCategoryId).Name = menuCategoryName;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        public JsonNetResult DeleteMenuCategory(Guid menuCategoryId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var entity = db.MenuCategory.Find(menuCategoryId);
                db.MenuCategory.RemoveRange(db.MenuCategory.Where(m => m.Path.Contains(entity.Path)).OrderByDescending(m => m.Path));
                db.MenuCategory.Remove(entity);
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        public JsonNetResult MoveMenuCategory([JsonModelBinder("menuCategoryIds")]List<Guid> menuCategoryIds, string moveType, Guid? targetNodeId)
        {
            var result = KeKeSoftPlatformDbContext.UseTransaction(db =>
            {
                var oldParentMenuCategory = db.MenuCategory.Find(db.MenuCategory.Find(menuCategoryIds.First()).ParentId);
                if (moveType == "inner")
                {
                    #region inner
                    T_MenuCategory newParentMenuCategory = null;
                    var newSequenceNum = 1;
                    if (targetNodeId.HasValue)
                    {
                        newParentMenuCategory = db.MenuCategory.Find(targetNodeId.Value);
                        newSequenceNum = newParentMenuCategory.Children.Count();
                    }
                    else
                    {
                        newSequenceNum = db.MenuCategory.Count(m => m.ParentId.HasValue == false);
                    }

                    //把老分类下的其他节点重新排序
                    List<T_MenuCategory> children = null;
                    if (oldParentMenuCategory == null)
                    {
                        children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                    }
                    else
                    {
                        children = db.MenuCategory.Where(m => m.ParentId == oldParentMenuCategory.Id).ToList();
                    }

                    var sequenceNum = 1;
                    foreach (var item in children.Where(m => menuCategoryIds.Contains(m.Id) == false).OrderBy(m => m.SequenceNum).ToList())
                    {
                        item.SequenceNum = sequenceNum++;
                    }

                    //转移分类并修改所有子元素的path

                    foreach (var menuCategoryId in menuCategoryIds.ToList())
                    {
                        var menuCategory = db.MenuCategory.Find(menuCategoryId);
                        menuCategory.ParentId = targetNodeId.Value;
                        menuCategory.SequenceNum = ++newSequenceNum;
                        var allChildren = db.MenuCategory.Where(m => m.Path.Contains(menuCategory.Path)).ToList();
                        if (oldParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = item.Path.Replace(oldParentMenuCategory.Path, "");
                            }
                        }

                        if (newParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = newParentMenuCategory.Path + item.Path;
                            }
                        }
                    }
                    #endregion

                    db.SaveChanges();
                    return new ReturnValue { IsSuccess = true };
                }

                if (moveType == "prev")
                {
                    T_MenuCategory targetMenuCategory = db.MenuCategory.Find(targetNodeId.Value);
                    List<T_MenuCategory> children = null;

                    //把老分类下的其他节点重新排序
                    if (oldParentMenuCategory == null)
                    {
                        children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                    }
                    else
                    {
                        children = db.MenuCategory.Where(m => m.ParentId == oldParentMenuCategory.Id).ToList();
                    }

                    var sequenceNum = 1;
                    foreach (var item in children.Where(m => menuCategoryIds.Contains(m.Id) == false).OrderBy(m => m.SequenceNum).ToList())
                    {
                        item.SequenceNum = sequenceNum++;
                    }
                    if (targetMenuCategory == null && oldParentMenuCategory == null || targetMenuCategory != null && oldParentMenuCategory != null && targetMenuCategory.ParentId == oldParentMenuCategory.Id)
                    {
                        //只排序就可以了
                        sequenceNum = targetMenuCategory.SequenceNum;
                        foreach (var menuCategoryId in menuCategoryIds.ToList())
                        {
                            var menuCategory = db.MenuCategory.Find(menuCategoryId);
                            menuCategory.SequenceNum = sequenceNum++;
                        }

                        foreach (var item in children.Where(m => menuCategoryIds.Contains(m.Id) == false && m.SequenceNum >= targetMenuCategory.SequenceNum).ToList())
                        {
                            item.SequenceNum = item.SequenceNum + menuCategoryIds.Count;
                        }

                        db.SaveChanges();
                        return new ReturnValue { IsSuccess = true };
                    }

                    //转移分类并修改所有子元素的path
                    var newParentMenuCategory = db.MenuCategory.Find(targetMenuCategory.ParentId);
                    foreach (var menuCategoryId in menuCategoryIds.ToList())
                    {
                        var menuCategory = db.MenuCategory.Find(menuCategoryId);
                        menuCategory.ParentId = targetMenuCategory.ParentId;
                        var allChildren = db.MenuCategory.Where(m => m.Path.Contains(menuCategory.Path)).ToList();
                        if (oldParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = item.Path.Replace(oldParentMenuCategory.Path, "");
                            }
                        }

                        if (newParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = newParentMenuCategory.Path + item.Path;
                            }
                        }
                    }

                    //新分类排序
                    sequenceNum = targetMenuCategory.SequenceNum;
                    foreach (var menuCategoryId in menuCategoryIds.ToList())
                    {
                        var menuCategory = db.MenuCategory.Find(menuCategoryId);
                        menuCategory.SequenceNum = sequenceNum++;
                    }

                    List<T_MenuCategory> newParentMenuCategory_Children = null;
                    if (targetMenuCategory.ParentId.HasValue)
                    {
                        newParentMenuCategory_Children = targetMenuCategory.Parent.Children.ToList();
                    }
                    else
                    {
                        newParentMenuCategory_Children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                    }

                    foreach (var item in newParentMenuCategory_Children.Where(m => menuCategoryIds.Contains(m.Id) == false && m.SequenceNum >= targetMenuCategory.SequenceNum).ToList())
                    {
                        item.SequenceNum = item.SequenceNum + menuCategoryIds.Count;
                    }

                    db.SaveChanges();
                    return new ReturnValue { IsSuccess = true };
                }

                if (moveType == "next")
                {
                    T_MenuCategory targetMenuCategory = db.MenuCategory.Find(targetNodeId.Value);
                    List<T_MenuCategory> children = null;

                    //把老分类下的其他节点重新排序
                    if (oldParentMenuCategory == null)
                    {
                        children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                    }
                    else
                    {
                        children = db.MenuCategory.Where(m => m.ParentId == oldParentMenuCategory.Id).ToList();
                    }

                    var sequenceNum = 1;
                    foreach (var item in children.Where(m => menuCategoryIds.Contains(m.Id) == false).OrderBy(m => m.SequenceNum).ToList())
                    {
                        item.SequenceNum = sequenceNum++;
                    }
                    if (targetMenuCategory == null && oldParentMenuCategory == null || targetMenuCategory != null && oldParentMenuCategory != null && targetMenuCategory.ParentId == oldParentMenuCategory.Id)
                    {
                        //只排序就可以了
                        sequenceNum = targetMenuCategory.SequenceNum;
                        foreach (var menuCategoryId in menuCategoryIds.ToList())
                        {
                            var menuCategory = db.MenuCategory.Find(menuCategoryId);
                            menuCategory.SequenceNum = sequenceNum++;
                        }

                        foreach (var item in children.Where(m => menuCategoryIds.Contains(m.Id) == false && m.SequenceNum > targetMenuCategory.SequenceNum).ToList())
                        {
                            item.SequenceNum = item.SequenceNum + menuCategoryIds.Count;
                        }

                        db.SaveChanges();
                        return new ReturnValue { IsSuccess = true };
                    }

                    //转移分类并修改所有子元素的path
                    var newParentMenuCategory = db.MenuCategory.Find(targetMenuCategory.ParentId);
                    foreach (var menuCategoryId in menuCategoryIds.ToList())
                    {
                        var menuCategory = db.MenuCategory.Find(menuCategoryId);
                        menuCategory.ParentId = targetMenuCategory.ParentId;
                        var allChildren = db.MenuCategory.Where(m => m.Path.Contains(menuCategory.Path)).ToList();
                        if (oldParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = item.Path.Replace(oldParentMenuCategory.Path, "");
                            }
                        }

                        if (newParentMenuCategory != null)
                        {
                            foreach (var item in allChildren)
                            {
                                item.Path = newParentMenuCategory.Path + item.Path;
                            }
                        }
                    }

                    //新分类排序
                    sequenceNum = targetMenuCategory.SequenceNum;
                    foreach (var menuCategoryId in menuCategoryIds.ToList())
                    {
                        var menuCategory = db.MenuCategory.Find(menuCategoryId);
                        menuCategory.SequenceNum = ++sequenceNum;
                    }

                    List<T_MenuCategory> newParentMenuCategory_Children = null;
                    if (targetMenuCategory.ParentId.HasValue)
                    {
                        newParentMenuCategory_Children = targetMenuCategory.Parent.Children.ToList();
                    }
                    else
                    {
                        newParentMenuCategory_Children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                    }
                    foreach (var item in newParentMenuCategory_Children.Where(m => menuCategoryIds.Contains(m.Id) == false && m.SequenceNum > targetMenuCategory.SequenceNum).ToList())
                    {
                        item.SequenceNum = item.SequenceNum + menuCategoryIds.Count;
                    }

                    db.SaveChanges();
                    return new ReturnValue { IsSuccess = true };
                }


                return new ReturnValue { IsSuccess = true };
            });

            return JsonNet(result);
        }

        public PartialViewResult MenuCategoryDetail(Guid? menuCategoryId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var model = new MenuCategoryDetailData
                {
                    ChildrenMenus = db.MenuCategory.Find(menuCategoryId).Menus.OrderBy(m => m.SequenceNum).ToList(),
                    NoneMenuCategoryMenus = db.Menu.Where(m => m.MenuCategoryId.HasValue == false && m.IsUser == false).OrderBy(m => m.SequenceNum).ToList()
                };
                if (menuCategoryId.HasValue)
                {
                    model.Children = db.MenuCategory.Where(m => m.ParentId == menuCategoryId).ToList();
                }
                else
                {
                    model.Children = db.MenuCategory.Where(m => m.ParentId.HasValue == false).ToList();
                }
                model.MenuElements = model.ChildrenMenus.Select(m => new MenuCategoryDetailData.MenuElement
                {
                    Id = m.Id,
                    SequenceNum = m.SequenceNum,
                    Type = MenuElementType.Menu,
                    Url = m.Url,
                    Name = m.Name
                }).Concat(model.Children.Select(m => new MenuCategoryDetailData.MenuElement
                {
                    Id = m.Id,
                    SequenceNum = m.SequenceNum,
                    Type = MenuElementType.Category,
                    Name = m.Name
                })).OrderBy(m => m.SequenceNum).ToList();
                return PartialView(model);
            }
        }

        [HttpPost]
        public JsonNetResult AddToMenuCategory(Guid menuCategoryId, [JsonModelBinder("menuIds")]List<Guid> menuIds)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var childrenCount = db.MenuCategory.Find(menuCategoryId).Menus.Count();
                foreach (var menuId in menuIds)
                {
                    var menu = db.Menu.Find(menuId);
                    menu.MenuCategoryId = menuCategoryId;
                    menu.SequenceNum = ++childrenCount;
                }

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        public JsonNetResult SaveMenuSequenceNum(Guid? menuCategoryId, [JsonModelBinder("elements")]List<MenuElement> elements)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var childrenCount = db.MenuCategory.Find(menuCategoryId).Menus.Count();
                var childrenMenuCategoryCount = 0;
                if (menuCategoryId.HasValue)
                {
                    childrenMenuCategoryCount = db.MenuCategory.Count(m => m.ParentId == menuCategoryId);
                }
                else
                {
                    childrenMenuCategoryCount = db.MenuCategory.Count(m => m.ParentId.HasValue == false);
                }
                if (elements.Count(m => m.Type == MenuElementType.Menu) != childrenCount || elements.Count(m => m.Type == MenuElementType.Category) != childrenMenuCategoryCount)
                {
                    return JsonNet(new ReturnValue { IsSuccess = false, Error = "服务器异常" });
                }

                for (int i = 1; i <= elements.Count; i++)
                {
                    if (elements[i - 1].Type == MenuElementType.Menu)
                    {
                        var menu = db.Menu.Find(elements[i - 1].Id);
                        menu.SequenceNum = i;
                    }
                    else
                    {
                        var menuCategory = db.MenuCategory.Find(elements[i - 1].Id);
                        menuCategory.SequenceNum = i;
                    }
                }

                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        [Right(Identity.Admin)]
        public JsonNetResult RemoveMenu(Guid menuId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var menu = db.Menu.Find(menuId);
                foreach (var item in db.Menu.Where(m => m.SequenceNum > menu.SequenceNum).ToList())
                {
                    item.SequenceNum -= 1;
                }
                db.Menu.Find(menuId).MenuCategoryId = null;
                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        public JsonNetResult EditMenu(Guid menuId, string name)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                db.Menu.Find(menuId).Name = name;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public ActionResult InitRightResource()
        {
            var allControllers = Assembly.GetAssembly(typeof(SystemController))
                                            .GetTypes()
                                            .Where(type => type.IsSubclassOf(typeof(BaseController)))
                                            .ToList();

            var menuItemCollection = new Dictionary<string, MenuInitItem>();


            #region 创建所有的菜单
            foreach (var controller in allControllers)
            {
                var areaNameAttributeCollection = controller.GetCustomAttributes(typeof(AreaNameAttribute), false);
                var controllerActions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                if (controllerActions.Any())
                {
                    foreach (var action in controllerActions)
                    {
                        var menuAttributeCollection = action.GetCustomAttributes(typeof(MenuAttribute), false);

                        if (menuAttributeCollection != null && menuAttributeCollection.Any())
                        {
                            var menuAttribute = menuAttributeCollection.First() as MenuAttribute;
                            var menuUrl = string.Format("/{0}/{1}", controller.Name.Replace("Controller", ""), action.Name);
                            if (areaNameAttributeCollection != null && areaNameAttributeCollection.Any())
                            {
                                menuUrl = "/" + (areaNameAttributeCollection.Single() as AreaNameAttribute).AreaName + menuUrl;
                            }
                            menuUrl = menuUrl.ToLower();

                            var menuItem = new MenuInitItem
                            {
                                Menu = new T_Menu
                                {
                                    Name = menuAttribute.DefaultName,
                                    SequenceNum = 1,
                                    Url = menuUrl,
                                    Id = PF.Key()
                                },
                                MenuActionCollection = new List<T_MenuAction>()
                            };

                            menuItem.MenuActionCollection.Add(new T_MenuAction
                            {
                                MenuActionCode = "view",
                                MenuActionName = "查看",
                                MenuId = menuItem.Menu.Id
                            });

                            var tmp = controller.Name.ToLower();
                            //如果是商户的菜单，则设置 IsUser 为 true
                            if(controller.Name.ToLower() == "usercontroller")
                            {
                                menuItem.Menu.IsUser = true;
                            }

                            menuItemCollection[menuUrl] = menuItem;
                        }
                    }
                }
            }
            #endregion

            #region 添加所有的菜单操作
            foreach (var controller in allControllers)
            {
                var controllerActions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                if (controllerActions.Any())
                {
                    foreach (var action in controllerActions)
                    {
                        var menuActionAttributeCollection = action.GetCustomAttributes(typeof(MenuActionAttribute), false);
                        if (menuActionAttributeCollection != null && menuActionAttributeCollection.Any())
                        {
                            var menuActionAttribute = menuActionAttributeCollection.First() as MenuActionAttribute;
                            if (menuItemCollection.ContainsKey(menuActionAttribute.MenuUrl.ToLower()) == false)
                            {
                                throw new Exception("菜单地址【{0}】不存在".FormatString(menuActionAttribute.MenuUrl.ToLower()));
                            }
                            if (!menuItemCollection[menuActionAttribute.MenuUrl.ToLower()].MenuActionCollection.Any(m => m.MenuActionCode.Equals(menuActionAttribute.MenuActionCode)))
                            {

                                menuItemCollection[menuActionAttribute.MenuUrl.ToLower()].MenuActionCollection.Add(new T_MenuAction
                                {
                                    MenuActionCode = menuActionAttribute.MenuActionCode.ToLower(),
                                    MenuActionName = menuActionAttribute.MenuActionName,
                                    MenuId = menuItemCollection[menuActionAttribute.MenuUrl.ToLower()].Menu.Id
                                });
                            }
                        }
                    }
                }
            }
            #endregion

            #region 保存数据
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var menuCollection = db.Menu.ToList();
                var menuActionCollection = db.MenuAction.ToList();

                //删除被移除的菜单、菜单操作
                menuCollection.ForEach(m =>
                {
                    if (!menuItemCollection.ContainsKey(m.Url.ToLower()))
                    {
                        db.Menu.Remove(db.Menu.Find(m.Id));
                    }
                    else
                    {

                        foreach (var item in menuActionCollection.Where(n => n.MenuId == m.Id))
                        {
                            var menuItem = menuItemCollection[m.Url.ToLower()];
                            if (!menuItem.MenuActionCollection.Any(n => n.MenuActionCode == item.MenuActionCode))
                            {
                                db.MenuAction.Remove(db.MenuAction.Find(item.Id));
                            }
                        }
                    }
                });

                //重新获取原有的菜单、菜单操作
                menuCollection = db.Menu.ToList();
                menuActionCollection = db.MenuAction.ToList();

                //添加新菜单或者修改已有的菜单
                menuItemCollection.ToList().ForEach(m =>
                {
                    var menu = menuCollection.FirstOrDefault(n => n.Url.Equals(m.Key));
                    if (menu == null)
                    {
                        db.Menu.Add(new T_Menu
                        {
                            Id = m.Value.Menu.Id,
                            Name = m.Value.Menu.Name,
                            SequenceNum = m.Value.Menu.SequenceNum,
                            Url = m.Value.Menu.Url,
                            IsUser = m.Value.Menu.IsUser
                        });
                        foreach (var item in m.Value.MenuActionCollection)
                        {
                            db.MenuAction.Add(new T_MenuAction
                            {
                                Id = PF.Key(),
                                MenuId = item.MenuId,
                                MenuActionCode = item.MenuActionCode,
                                MenuActionName = item.MenuActionName
                            });
                        }
                    }
                    else
                    {
                        foreach (var item in m.Value.MenuActionCollection)
                        {
                            var menuAction = menuActionCollection.FirstOrDefault(n => n.MenuId == menu.Id && n.MenuActionCode == item.MenuActionCode);
                            if (menuAction == null)
                            {
                                item.MenuId = menu.Id;
                                db.MenuAction.Add(new T_MenuAction
                                {
                                    Id = PF.Key(),
                                    MenuId = menu.Id,
                                    MenuActionCode = item.MenuActionCode,
                                    MenuActionName = item.MenuActionName
                                });
                            }
                            else
                            {
                                var updateMenuAction = db.MenuAction.Find(menuAction.Id);
                                updateMenuAction.MenuActionCode = item.MenuActionCode;
                                updateMenuAction.MenuActionName = item.MenuActionName;
                            }
                        }
                    }
                });
                db.SaveChanges();
            }
            #endregion

            //清除缓存
            CacheProvider.Instance.Remove("_Layout");
            CacheProvider.Instance.Clear();

            TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("初始化菜单成功！");
            return RedirectToAction("MenuCategory");
        }

        private void AppendMenuCategory(List<T_MenuCategory> sortedMenuCategories, T_MenuCategory parentMenuCategory, IEnumerable<T_MenuCategory> menuCategories)
        {
            List<T_MenuCategory> children = null;
            if (parentMenuCategory == null)
            {
                children = menuCategories.Where(m => m.ParentId.HasValue == false).OrderBy(m => m.SequenceNum).ToList();
            }
            else
            {
                children = menuCategories.Where(m => m.ParentId == parentMenuCategory.Id).OrderBy(m => m.SequenceNum).ToList();
            }

            foreach (var item in children)
            {
                sortedMenuCategories.Add(item);
                AppendMenuCategory(sortedMenuCategories, item, menuCategories);
            }
        }
        
        #endregion

        #region 系统参数

        [Menu("参数设置")]
        [Right(Identity.Admin)]
        public ActionResult Config()
        {
            return View(KeKeSoftPlatform.Core.Config.Instance);
        }

        [HttpPost]
        public ActionResult Config(KeKeSoftPlatform.Core.Config model)
        {
            model.Serialize();
            TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
            return RedirectToAction("Config");
        }

        #endregion

        #region 修改密码

        [Menu("修改密码")]
        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePwd(ChangeAdminPwdData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var config = KeKeSoftPlatform.Core.Config.Instance;
            config.ConfigParameter.MD5EncryptPassword = EncryptUtils.MD5Encrypt(model.NewPassword);
            config.Serialize();

            TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("密码已修改");
            return RedirectToAction("ChangePwd");
        }

        #endregion

        #region 安全退出
        [Menu("安全退出")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion
    }
}
