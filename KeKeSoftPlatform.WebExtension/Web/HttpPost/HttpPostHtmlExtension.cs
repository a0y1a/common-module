using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.WebPages;

namespace KeKeSoftPlatform.WebExtension
{
    public static class HttpPostHtmlExtension
    {
        public static MvcHtmlString HttpRePostSign(this HtmlHelper html)
        {
            return html.Hidden(PageTokenViewBase.ClientTokenName, new SessionPageTokenView().GetServerPageToken());
        }
    }
}
