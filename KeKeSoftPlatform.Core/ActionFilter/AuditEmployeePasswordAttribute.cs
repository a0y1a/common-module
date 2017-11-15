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
    public class AuditEmployeePasswordAttribute : ActionFilterAttribute
    {
        //public bool Ignore { get; set; }
        //public AuditEmployeePasswordAttribute()
        //{
        //    this.Ignore = false;
        //}
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (Ignore==false && filterContext.HttpContext.Request.IsAuthenticated)
        //    {
        //        using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
        //        {
        //            var employee = db.Employee.Find(_User.Value.Id);
        //            if (employee.Password == EncryptUtils.MD5Encrypt(Config.Instance.InitPassword))
        //            {
        //                filterContext.Controller.TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("请先修改密码");
        //                filterContext.Result = new RedirectResult("/system/ChangePwd");
        //                return;
        //            }
        //        }
        //    }
        //}
    }
}
