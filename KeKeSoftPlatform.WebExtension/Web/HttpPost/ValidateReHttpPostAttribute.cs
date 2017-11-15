using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    public class ValidateReHttpPostAttribute : ActionFilterAttribute
    {
        private PageTokenViewBase _PageTokenView;
        public ValidateReHttpPostAttribute()
        {
            this._PageTokenView = new SessionPageTokenView();
        }

        public Type Redirect { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.HttpMethod.Equals("POST") && filterContext.RequestContext.HttpContext.Request.IsAjaxRequest() == false)
            {
                if (this._PageTokenView.TokensMatch == false)
                {
                    var customAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ValidateReHttpPostAttribute), false);
                    if (customAttributes != null && customAttributes.Any())
                    {
                        var redirectHandler = (customAttributes.First() as ValidateReHttpPostAttribute);
                        if (redirectHandler.Redirect != null)
                        {
                            if (typeof(IValidateReHttpRedirect).IsAssignableFrom(redirectHandler.Redirect))
                            {
                                var handler = Activator.CreateInstance(redirectHandler.Redirect);
                                filterContext.Result = (redirectHandler.Redirect.InvokeMember("Redirect", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.InvokeMethod, null, handler, new object[] { filterContext }) as ActionResult);
                                base.OnActionExecuting(filterContext);
                                return;
                            }
                            else
                            {
                                throw new Exception("防重复提交类【{0}】未实现IValidateReHttpRedirect接口".FormatString(redirectHandler.Redirect.FullName));
                            }
                        }
                    }
                    //throw new ArgumentNullException("Invaild Http Post!");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
