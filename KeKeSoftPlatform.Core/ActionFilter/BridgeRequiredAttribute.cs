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
    public class BridgeRequiredAttribute : ActionFilterAttribute
    {
        public bool Ignore { get; set; }
        public BridgeRequiredAttribute()
        {
            this.Ignore = false;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Ignore == false && filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (BridgeMananger.CurrentBridgeId.HasValue == false)
                {
                    filterContext.Controller.TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity("请先在左上角选择桥梁，然后再继续此操作");
                    filterContext.Result = new RedirectResult("/business/bridgemap");
                }
            }
        }
    }
}
