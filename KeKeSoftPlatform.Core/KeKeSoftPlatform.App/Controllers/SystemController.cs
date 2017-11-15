using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using System.Data.SqlTypes;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Entity;

namespace KeKeSoftPlatform.App.Controllers
{
    public class SystemController : BaseController
    {
        #region  帐套管理
        public ActionResult Account(int pageNum = 1)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                return View(db.Account.OrderBy(m => m.Id).Page(pageNum));
            }
        }

        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(CreateAccountData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                //保存帐套信息
                var account = new T_Account()
                {
                    Id = PF.Key(),
                    IsLock = false,
                    Description = model.Description,
                    Name = model.Name,
                    Prefix = model.Prefix,
                    Expired=DateTime.Now.AddYears(1)
                };

                db.Account.Add(account);

                //保存根部门信息
                var department = new T_Department()
                {
                    AccountId = account.Id,
                    Id = PF.Key(),
                    Name = "新公司",
                    SequenceNum = 1,
                    Path=UtilEncoder.Instance.NextToString()
                };

                db.Department.Add(department);

                //保存根部门职务信息
                var job = new T_Job()
                {
                    Id = PF.Key(),
                    AccountId = account.Id,
                    DepartmentId = department.Id,
                    IsGroup = false,
                    Name = "董事长",
                    SequenceNum = 1,
                    Path=UtilEncoder.Instance.NextToString()
                };

                db.Job.Add(job);

                //保存根部门人员信息
                var employee = new T_Employee()
                {
                    Id = PF.Key(),
                    AccountId = account.Id,
                    Name = "xxx",
                    Number = account.Prefix + "0001",
                    Password = EncryptUtils.MD5Encrypt(KeKeSoftPlatform.Core.Config.Instance.InitPassword)
                };
                db.Employee.Add(employee);

                //保存职务人员关系
                db.EmployeeJobLink.Add(new T_EmployeeJobLink
                {
                    Id = PF.Key(),
                    EmployeeId = employee.Id,
                    JobId = job.Id
                });

                db.SaveChanges();

                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return RedirectToAction("account");
            }
        }

        public PartialViewResult EditAccount(Guid accountId)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var account = db.Account.Find(accountId);
                return PartialView(new EditAccountData
                {
                    IsLock = account.IsLock,
                    AccountId = account.Id,
                    Description = account.Description,
                    Expired = account.Expired,
                    Name = account.Name
                });
            }
        }

        [HttpPost]
        public ActionResult EditAccount(EditAccountData model)
        {
            if (ModelState.IsValid == false)
            {
                return PartialView(model);
            }
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var account = db.Account.Find(model.AccountId);
                account.Name = model.Name;
                account.Expired = model.Expired;
                account.IsLock = model.IsLock;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }
        #endregion

        #region 登录
        [Right(AllowAnonymous =true)]
        public ActionResult Login(string returnUrl)
        {
            return View(new EmployeeLoginData { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Right(AllowAnonymous =true)]
        public ActionResult Login(EmployeeLoginData model)
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

            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var md5Pwd = EncryptUtils.MD5Encrypt(model.Password);
                var employee = db.Employee.SingleOrDefault(m => m.Number == model.Number && m.Password == md5Pwd);
                if (employee == null)
                {
                    ModelState.AddModelError("Number", "账号或密码错误");
                    return View(model);
                }
                if (employee.EmployeeJobLinks.Any()==false)
                {

                    ModelState.AddModelError("Number", "该账号已停职，无法登录，请联系管理员");
                    return View(model);
                }

                var ownIdentities = new List<Identity>();
                ownIdentities.Add(Identity.Employee);
                foreach (var item in db.Job.Where(m => m.EmployeeJobLinks.Any(n => n.EmployeeId == employee.Id)).ToList())
                {
                    if (item.ParentId.HasValue == false && ownIdentities.Any(m => m == Identity.Admin) == false)
                    {
                        ownIdentities.Add(Identity.Admin);
                    }
                    if (item.ParentId.HasValue == false && employee.Number.StartsWith("00") && ownIdentities.Any(m => m == Identity.SuperAdmin) == false)
                    {
                        ownIdentities.Add(Identity.SuperAdmin);
                        ownIdentities.Add(Identity.Developer);
                    }
                }
                FormsPrincipal<_User>.SignIn(employee.Id.ToString(), new _User
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Code = employee.Number,
                    IsPersistent = true,
                    EnableSingleOnline = false,
                    OwnIdentities = ownIdentities,
                    AccountId = employee.AccountId
                });

                if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                {
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return Redirect(model.ReturnUrl);
                }
            }

        }

        #endregion

        #region 获取验证码
        [Right(AllowAnonymous =true)]
        public ActionResult Captcha()
        {
            Captcha captcha = new Captcha();
            var value = captcha.CreateCaptcha(4);
            Session[Service.CAPTCHA] = value;
            return File(captcha.CreateCaptchaGraphic(value), @"image/jpeg");
        }
        #endregion

        #region 菜单分类、菜单、权限
        [Right(Identity.Developer)]
        public ActionResult MenuCategory()
        {
            return View();
        }

        [Right(Identity.Developer)]
        public ActionResult LoadMenuCategory()
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var menuCategories = db.MenuCategory.ToList();
                var data = new List<ZTreeNode<MenuCategoryLoadData>>();

                foreach (var item in menuCategories.Where(m => m.ParentId.HasValue == false).OrderBy(m=>m.SequenceNum).ToList())
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
        [Right(Identity.Developer)]
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
        [Right(Identity.Developer)]
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
        [Right(Identity.Developer)]
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
        [Right(Identity.Developer)]
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
                    if (targetMenuCategory == null && oldParentMenuCategory == null || targetMenuCategory!=null && oldParentMenuCategory!=null && targetMenuCategory.ParentId == oldParentMenuCategory.Id)
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
                    if (targetMenuCategory == null && oldParentMenuCategory == null || targetMenuCategory != null && oldParentMenuCategory != null&& targetMenuCategory.ParentId == oldParentMenuCategory.Id)
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

        [Right(Identity.Developer)]
        public PartialViewResult MenuCategoryDetail(Guid? menuCategoryId)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var model = new MenuCategoryDetailData
                {
                    ChildrenMenus = db.MenuCategory.Find(menuCategoryId).Menus.OrderBy(m => m.SequenceNum).ToList(),
                    NoneMenuCategoryMenus = db.Menu.Where(m => m.MenuCategoryId.HasValue == false).OrderBy(m => m.SequenceNum).ToList()                    
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
        [Right(Identity.Developer)]
        public JsonNetResult AddToMenuCategory(Guid menuCategoryId, [JsonModelBinder("menuIds")]List<Guid> menuIds)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
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
        [Right(Identity.Developer)]
        public JsonNetResult SaveMenuSequenceNum(Guid? menuCategoryId, [JsonModelBinder("elements")]List<MenuElement> elements)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
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
                    if (elements[i-1].Type == MenuElementType.Menu)
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
        [Right(Identity.Developer)]
        public JsonNetResult RemoveMenu(Guid menuId)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var menu = db.Menu.Find(menuId);
                foreach (var item in db.Menu.Where(m=>m.SequenceNum>menu.SequenceNum).ToList())
                {
                    item.SequenceNum -= 1;
                }
                db.Menu.Find(menuId).MenuCategoryId = null;
                db.SaveChanges();

                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [HttpPost]
        [Right(Identity.Developer)]
        public JsonNetResult EditMenu(Guid menuId,string name)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.Menu.Find(menuId).Name = name;
                db.SaveChanges();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        [Right(Identity.Developer)]
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
                            Url = m.Value.Menu.Url
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

        [Right(Identity.Developer)]
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

        [Right(Identity.Employee)]
        public PartialViewResult TableAuthorize(Guid groupId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var authorize = db.Authorize.ToList();
                var menuActions = db.MenuAction.ToList();
                var menus = db.Menu.ToList();
                var menuCategories = db.MenuCategory.ToDictionary(m => m.Id);

                var sortedMenuCategories = new List<T_MenuCategory>();
                AppendMenuCategory(sortedMenuCategories, null, menuCategories.Values);

                var model = new List<TableAuthorizeData>();
                foreach (var item in sortedMenuCategories)
                {
                    foreach (var menu in menus.Where(m => m.MenuCategoryId == item.Id).OrderBy(m => m.SequenceNum).ToList())
                    {
                        model.Add(new TableAuthorizeData
                        {
                            MenuFullName = menu.Name,
                            AuthorizeItems = menuActions.Where(m => m.MenuId == menu.Id).Select(m => new TableAuthorizeData.AuthorizeItem
                            {
                                Enable = authorize.Any(n => n.GroupId == groupId && n.MenuActionId == m.Id),
                                MenuActionId = m.Id,
                                MenuMenuActionCode = m.MenuActionCode,
                                MenuMenuActionName = m.MenuActionName
                            }).ToList()
                        });
                    }
                }


                return PartialView(model);
            }
        }

        [HttpPost]
        [Right(Identity.Employee)]
        public JsonNetResult TableAuthorize(Guid groupId, [JsonModelBinder("data")]List<Guid> data)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.Authorize.RemoveRange(db.Authorize.Where(m => m.GroupId == groupId).ToList());
                db.Authorize.AddRange(data.Select(m => new T_Authorize
                {
                    Id = PF.Key(),
                    GroupId = groupId,
                    MenuActionId = m
                }).ToList());

                db.SaveChanges();
                AuthorizeProvider.NoticeChange();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }

        public ActionResult GridColumnAuthorize(Guid groupId)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var authorize = db.AuthorizeGridColumn.ToList();
                var gridColumns = db.GridColumn.ToList();
                var grids = db.Grid.ToList();

                var model = new List<GridColumnAuthorizeData>();
                foreach (var grid in grids)
                {
                    model.Add(new GridColumnAuthorizeData
                    {
                        GridName = grid.Name,
                        AuthorizeItems = gridColumns.Where(m => m.GridId == grid.Id).OrderBy(m=>m.SequenceNum).Select(m => new GridColumnAuthorizeData.AuthorizeItem
                        {
                            Enable = authorize.Any(n => n.GroupId == groupId && n.GridColumnId == m.Id),
                            GridColumnId = m.Id,
                            GridColumnName = m.Name
                        }).ToList()
                    });
                }


                return PartialView(model);
            }
        }

        [HttpPost]
        public ActionResult GridColumnAuthorize(Guid groupId, [JsonModelBinder("data")]List<Guid> data)
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.AuthorizeGridColumn.RemoveRange(db.AuthorizeGridColumn.Where(m => m.GroupId == groupId).ToList());
                db.AuthorizeGridColumn.AddRange(data.Select(m => new T_AuthorizeGridColumn
                {
                    Id = PF.Key(),
                    GroupId = groupId,
                    GridColumnId = m
                }).ToList());

                db.SaveChanges();
                AuthorizeProvider.NoticeChange();
                return JsonNet(new ReturnValue { IsSuccess = true });
            }
        }
        #endregion

        #region 系统参数
        [Menu("系统参数设置")]
        [Right(Identity.Employee)]
        public ActionResult Config()
        {
            return View(KeKeSoftPlatform.Core.Config.Instance);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Right(Identity.Employee)]
        public ActionResult Config(KeKeSoftPlatform.Core.Config model)
        {
            model.Serialize();
            TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
            return RedirectToAction("Config");
        }

        public ActionResult ClearMenuCache()
        {
            //清除缓存
            CacheProvider.Instance.Remove("_Layout");
            return RedirectToAction("MenuCategory");
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

        #region 修改密码
        [Menu("修改密码")]
        [AuditEmployeePassword(Ignore =true)]
        [Right(Identity.Employee)]
        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        [AuditEmployeePassword(Ignore =true)]
        [Right(Identity.Employee)]
        public ActionResult ChangePwd(ChangePwdData model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                var employee = db.Employee.Find(_User.Value.Id);
                employee.Password = EncryptUtils.MD5Encrypt(model.NewPassword);
                db.SaveChanges();

                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("密码已修改");
                return RedirectToAction("ChangePwd");
            }
        }

        [Right(Identity.Employee)]
        public ActionResult ResetPwd(Guid employeeId,string returnUrl="/orgchart/employee/index")
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                db.Employee.Find(employeeId).Password = EncryptUtils.MD5Encrypt(KeKeSoftPlatform.Core.Config.Instance.InitPassword);
                db.SaveChanges();
                TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("操作成功");
                return Redirect(returnUrl);
            }
        }
        #endregion


        public ActionResult Dashboard()
        {
            return RedirectToAction("bridge","business",null);
        }




        [AllowAnonymous]
        public ActionResult Test()
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                Guid? guid = Guid.NewGuid();
                var sql = db.Employee.Where(m => m.Id.CompareTo(guid.Value) > 0).ToString();

                return Content(sql);
            }
        }

        public JsonNetResult Query(string number, int pageNum = 1)
        {
            var data = new List<dynamic>();
            for (int i = 0; i < 103; i++)
            {
                data.Add(new
                {
                    name = "杜冠梦" + i.ToString() + "号",
                    number = i.ToString().PadLeft(6, '0'),
                    money = i,
                    status = ProcessStatus.Alive,
                    islock = (i % 2 == 0 ? true : false)
                });
            }
            var query = data.Where(m => true);
            if (string.IsNullOrWhiteSpace(number) == false)
            {
                query = query.Where(m => m.number.Contains(number));
            }
            return JsonNet(new ReturnValue
            {
                IsSuccess = true,
                Data = new Pager<object>
                {
                    Data = query.Skip((pageNum - 1) * 20).Take(20).ToList(),
                    ItemCount = query.Count(),
                    PageNum = pageNum,
                    PageSize = 20
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [Right(AllowAnonymous =true)]
        public ActionResult RepairEmployee()
        {
            using (KeKeSoftPlatformDbContext db=new KeKeSoftPlatformDbContext())
            {
                var igonre = new List<Guid>();
                igonre.Add(Guid.Parse("3C4AA1A8-9613-4C1F-8B30-A7331959D829"));
                igonre.Add(Guid.Parse("5F8A40EB-C523-4CD1-BAFD-A70E169AEC41"));
                igonre.Add(Guid.Parse("B7919E88-3E70-43CE-85F6-A70E16ADD2A3"));
                igonre.Add(Guid.Parse("1F670FCB-435D-4C2A-8604-A71B162060DF"));
                foreach (var item in db.Employee.Where(m=> !igonre.Any(n=>n==m.Id)).ToList())
                {
                    db.Database.ExecuteSqlCommand("update t_employee set id=@p0 where id=@p1", PF.Key().Value, item.Id);
                }

                db.SaveChanges();
                return Content("操作成功");
            }
        }

        /// <summary>
        /// 帐套过期
        /// </summary>
        /// <returns></returns>
        [AccountExpired(Ignore =true)]
        [Right(AllowAnonymous =true)]
        public ActionResult Expired()
        {
            return View();
        }
    }
}
