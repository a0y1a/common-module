//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Mvc;
//using KeKeSoftPlatform.Common;
//using KeKeSoftPlatform.Db;
//using KeKeSoftPlatform.WebExtension;

//namespace KeKeSoftPlatform.Core
//{
//    public class EmployeeRightAuditor : RightAuditor
//    {
//        //public EmployeeRightAuditor()
//        //    : base(Identity.Admin)
//        //{
//        //}

//        //public override void Audit(AuthorizationContext filterContext)
//        //{
//        //    if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated == false)
//        //    {
//        //        this.Result = false;
//        //        return;
//        //    }
//        //    var returnUrl = UrlBuilder.CreateInstance(filterContext.HttpContext).Build();
//        //    if (_User.Value.IsInvalid == false)
//        //    {
//        //        this.FailEventHanlder += (sender, e) =>
//        //        {
//        //            System.Web.Security.FormsAuthentication.SignOut();
//        //            filterContext.Result = new RedirectResult("/system/OnlineError?returnUrl=" + filterContext.HttpContext.Server.UrlEncode(returnUrl));
//        //        };
//        //        this.Result = false;
//        //        return;
//        //    }
//        //    if (new Identity[] { Identity.Admin, Identity.SuperAdmin, Identity.Developer }.Any(m => _User.Value.OwnIdentities.Contains(m)))
//        //    {
//        //        this.Result = true;
//        //        return;
//        //    }
//        //    if (_User.Value.OwnIdentities.Contains(this.identity) == false)
//        //    {
//        //        this.FailEventHanlder += (sender, e) =>
//        //        {
//        //            filterContext.Controller.TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("不允许身份【{0}】访问".FormatString(Identity.Admin.ToString()), AlertType.Danger, false);
//        //            filterContext.Result = new RedirectResult("/system/unauthorized?url=" + filterContext.HttpContext.Server.UrlEncode(returnUrl));
//        //        };
//        //        this.Result = false;
//        //        return;
//        //    }

//        //    string menuActionCode = string.Empty;
//        //    string menuUrl = string.Empty;

//        //    var menuActionAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(MenuActionAttribute), false).FirstOrDefault();
//        //    if (menuActionAttribute != null)
//        //    {
//        //        menuActionCode = (menuActionAttribute as MenuActionAttribute).MenuActionCode;
//        //        menuUrl = (menuActionAttribute as MenuActionAttribute).MenuUrl;
//        //    }

//        //    var menuAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(MenuAttribute), false).FirstOrDefault();
//        //    if (menuAttribute != null)
//        //    {
//        //        menuActionCode = "view";
//        //        menuUrl = "/{0}/{1}".FormatString(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
//        //        var areaNameAttributeCollection = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AreaNameAttribute), false);
//        //        if (areaNameAttributeCollection != null && areaNameAttributeCollection.Any())
//        //        {
//        //            menuUrl = "/" + (areaNameAttributeCollection.Single() as AreaNameAttribute).AreaName + menuUrl;
//        //        }
//        //        menuUrl = menuUrl.ToLower();
//        //    }

//        //    if (string.Empty == menuActionCode || string.Empty == menuUrl)
//        //    {
//        //        this.Result = true;
//        //        return;
//        //    }

//        //    if (AuthorizeProvider.HasPermission(menuUrl, menuActionCode))
//        //    {
//        //        this.Result = true;
//        //        return;
//        //    }
//        //    this.FailEventHanlder += (sender, e) =>
//        //    {
//        //        filterContext.Result = new RedirectResult("/system/unauthorized?url=" + filterContext.HttpContext.Server.UrlEncode(returnUrl));
//        //    };
//        //    this.Result = false;
//        //}
//    }
//}
