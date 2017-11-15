using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KeKeSoftPlatform.Core
{
    public class CheckWeiXinAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            ReturnValue<RedirectResult> result = new ReturnValue<RedirectResult> { IsSuccess = true };

            if (!filterContext.RequestContext.HttpContext.Request.IsAuthenticated && filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                result.IsSuccess = false;
                result.Data = new RedirectResult("/weixin/Gateway/Login");
            }
            else
            {
                using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                   
                }
            }

            if (result.IsSuccess == false)
            {
                filterContext.Result = result.Data;
                //base.OnAuthorization(filterContext);
            }
        }
    }
}
