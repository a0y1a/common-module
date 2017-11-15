using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.Core
{
    //public class AccountExpiredAttribute : ActionFilterAttribute
    //{
    //    public bool Ignore { get; set; }
    //    public AccountExpiredAttribute()
    //    {
    //        this.Ignore = false;
    //    }
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        if (Ignore)
    //        {
    //            return;
    //        }
    //        Guid accountId;
    //        if ((filterContext.Controller as BaseController).KehuId.HasValue)
    //        {
    //            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
    //            {
    //                var employee = db.Employee.Find((filterContext.Controller as BaseController).KehuId);
    //                if (employee == null)
    //                {
    //                    return;
    //                }
    //                accountId = employee.AccountId;
    //            }
    //        }
    //        else
    //        {
    //            var filters= filterContext.ActionDescriptor.GetCustomAttributes(typeof(RightAttribute), true);
    //            if (filters != null && filters.Any())
    //            {
    //                if ((filters.First() as RightAttribute).AllowAnonymous)
    //                {
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                filters = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(RightAttribute), true);
    //                if (filters != null && filters.Any())
    //                {
    //                    if ((filters.First() as RightAttribute).AllowAnonymous)
    //                    {
    //                        return;
    //                    }
    //                }
    //            }
                
                
    //            accountId = _User.Value.AccountId;
    //        }
    //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
    //        {
    //            var account = db.Account.Find(accountId);
    //            if (account.Prefix == "00")
    //            {
    //                return;
    //            }
    //            if (account.Expired < DateTime.Now)
    //            {
    //                account.IsLock = true;
    //            }
    //            if (account.IsLock)
    //            {
    //                filterContext.Result = new RedirectResult("/system/expired");
    //                return;
    //            }
    //        }
    //    }
    //}
}
