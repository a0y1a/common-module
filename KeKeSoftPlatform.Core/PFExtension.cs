using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;

namespace KeKeSoftPlatform.Core
{
    public static partial class PFExtension
    {
        public static string GetReturnUrl(this HtmlHelper html, string defaultReturnUrl)
        {
            if (string.IsNullOrWhiteSpace(defaultReturnUrl))
            {
                throw new ArgumentNullException("默认返回地址不能为空");
            }
            if (string.IsNullOrWhiteSpace(html.ViewContext.HttpContext.Request.QueryString["returnUrl"]))
            {
                return defaultReturnUrl;
            }
            return html.ViewContext.HttpContext.Request.QueryString["returnUrl"];
        }
    }
}
