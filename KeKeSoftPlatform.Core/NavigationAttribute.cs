using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.Core
{
    public class NavigationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.QueryString[Config.NAVIGATION_TOP_KEY]))
            {
                filterContext.HttpContext.Session[Config.NAVIGATION_TOP_KEY] = Guid.Parse(filterContext.HttpContext.Request.QueryString[Config.NAVIGATION_TOP_KEY]);
            }

            if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.QueryString[Config.NAVIGATION_ITEM_KEY]))
            {
                filterContext.HttpContext.Session[Config.NAVIGATION_ITEM_KEY] = Guid.Parse(filterContext.HttpContext.Request.QueryString[Config.NAVIGATION_ITEM_KEY]);
            }
        }
    }
}
