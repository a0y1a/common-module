using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.Core
{
    public class DirectRightAuditor : RightAuditor
    {
        public DirectRightAuditor(Identity identity)
            : base(identity)
        {
        }


        public override void Audit(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated == false)
            {
                this.Result = false;
                return;
            }
            var returnUrl = UrlBuilder.CreateInstance(filterContext.HttpContext,filterContext.RequestContext.RouteData).Build();
            if (_User.Value.IsInvalid == false)
            {
                this.FailEventHanlder += (sender, e) =>
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                    filterContext.Result = new RedirectResult("/system/OnlineError?returnUrl=" + filterContext.HttpContext.Server.UrlEncode(returnUrl));
                };
                this.Result = false;
                return;
            }
            if (_User.Value.OwnIdentities.Contains(this.identity))
            {
                this.Result = true;
                return;
            }
            this.FailEventHanlder += (sender, e) =>
            {
                filterContext.Result = new RedirectResult("/system/unauthorized?url=" + filterContext.HttpContext.Server.UrlEncode(returnUrl));
            };
            this.Result = false;
            return;
        }
    }
}
