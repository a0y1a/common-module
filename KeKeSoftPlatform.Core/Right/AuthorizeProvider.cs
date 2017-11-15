//using KeKeSoftPlatform.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using KeKeSoftPlatform.Db;
//using System.Web.WebPages;
//using System.Web.Mvc;

//namespace KeKeSoftPlatform.Core
//{
//    public class AuthorizeProvider
//    {
//        //public static int AuthorizeVersion { get; private set; }
        
//        //public static void NoticeChange()
//        //{
//        //    AuthorizeVersion += 1;
//        //}
//        //public static bool HasPermission(string menuUrl, string menuActionCode)
//        //{
//        //    if (_User.HasValue == false)
//        //    {
//        //        return false;
//        //    }

//        //    if (_User.Value.OwnIdentities.Contains(Identity.Admin))
//        //    {
//        //        return true;
//        //    }

//        //    var authorize = CacheProvider.Instance.Get<List<T_Authorize>>("AuthorizeProvider.HasPermission.Authorize" + AuthorizeVersion, () =>
//        //    {
//        //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
//        //        {
//        //            return db.Authorize.ToList();
//        //        }
//        //    });

//        //    var groupEmployee = CacheProvider.Instance.Get<List<T_GroupEmployee>>("AuthorizeProvider.HasPermission.AdminRoleLink" + AuthorizeVersion, () =>
//        //    {
//        //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
//        //        {
//        //            return db.GroupEmployee.ToList();
//        //        }
//        //    });

//        //    var menu = CacheProvider.Instance.Get<List<T_Menu>>("AuthorizeProvider.HasPermission.Menu" + AuthorizeVersion, () =>
//        //    {
//        //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
//        //        {
//        //            return db.Menu.ToList();
//        //        }
//        //    });

//        //    var menuAction = CacheProvider.Instance.Get<List<T_MenuAction>>("AuthorizeProvider.HasPermission.MenuAction" + AuthorizeVersion, () =>
//        //    {
//        //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
//        //        {
//        //            return db.MenuAction.ToList();
//        //        }
//        //    });

//        //    var _groupEmployee = groupEmployee.Where(m => m.EmployeeId == _User.Value.Id).ToList();
//        //    if (_groupEmployee.Any() == false)
//        //    {
//        //        return false;
//        //    }


//        //    var _menu = menu.SingleOrDefault(m => string.Equals(menuUrl, m.Url, StringComparison.CurrentCultureIgnoreCase));
//        //    if (_menu == null)
//        //    {
//        //        return false;
//        //    }

//        //    var _menuAction = menuAction.SingleOrDefault(m => m.MenuId == _menu.Id && string.Equals(menuActionCode, m.MenuActionCode, StringComparison.CurrentCultureIgnoreCase));
//        //    if (_menuAction == null)
//        //    {
//        //        return false;
//        //    }

//        //    var _authorize = authorize.Where(m => _groupEmployee.Any(n => n.GroupId == m.GroupId && m.MenuActionId == _menuAction.Id)).ToList();
//        //    if (_authorize.Any() == false)
//        //    {
//        //        return false;
//        //    }

//        //    return true;
//        //}

//        //public static HelperResult HasPermission(string menuUrl, string menuActionCode, Func<object, HelperResult> content)
//        //{
//        //    if (HasPermission(menuUrl, menuActionCode))
//        //    {
//        //        return content(null);
//        //    }
//        //    else
//        //    {
//        //        return new HelperResult(writer => { });
//        //    }
//        //}

//        //public static MvcHtmlString HasPermission(string menuUrl, string menuActionCode, MvcHtmlString content)
//        //{
//        //    if (HasPermission(menuUrl, menuActionCode))
//        //    {
//        //        return content;
//        //    }
//        //    else
//        //    {
//        //        return new MvcHtmlString("");
//        //    }
//        //}

//        //public static HelperResult HasPermission(Func<object, HelperResult> content, params Identity[] identities)
//        //{
//        //    if (_User.HasValue == false)
//        //    {
//        //        return new HelperResult(writer => { });
//        //    }
//        //    if (identities.Any(m => _User.Value.OwnIdentities.Any(n => n == m)) == false)
//        //    {
//        //        return new HelperResult(writer => { });
//        //    }
//        //    return content(null);

//        //}

//        //public static MvcHtmlString HasPermission(MvcHtmlString content, params Identity[] identities)
//        //{
//        //    if (_User.HasValue == false)
//        //    {
//        //        return new MvcHtmlString(string.Empty);
//        //    }
//        //    if (identities.Any(m => _User.Value.OwnIdentities.Any(n => n == m)) == false)
//        //    {
//        //        return new MvcHtmlString(string.Empty);
//        //    }
//        //    return content;
//        //}
//    }
//}
