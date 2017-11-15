using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.Routing;

namespace KeKeSoftPlatform.WebExtension
{
    public static partial class BootstrapHtmlHelperExtension
    {
        public static MvcHtmlString Icon(this HtmlHelper html, string iconType)
        {
            var span = new TagBuilder("span");
            span.AddCssClass(iconType);
            return new MvcHtmlString(span.ToString());
        }

        public static MvcHtmlString Divider(this HtmlHelper html)
        {
            var li=new TagBuilder("li");
            li.AddCssClass("divider");
            return new MvcHtmlString(li.ToString());
        }

        public static MvcHtmlString DropDownButtonGroup(this HtmlHelper html,string title, params MvcHtmlString[] item)
        {
            var group = new TagBuilder("div");
            group.AddCssClass("btn-group");

            var button = new TagBuilder("button");
            button.AddCssClass("btn btn-default btn-xs dropdown-toggle");
            button.Attributes["type"] = "button";
            button.Attributes["data-toggle"] = "dropdown";
            button.InnerHtml += title + " <span class=\"caret\"></span>";

            var itemBox = new TagBuilder("ul");
            itemBox.AddCssClass("dropdown-menu pull-right");
            itemBox.Attributes["role"] = "menu";

            if (item != null && item.Any())
            {
                foreach (var m in item)
                {
                    if (m == null)
                    {
                        continue;
                    }
                    if (item.ToString().StartsWith("<li"))
                    {
                        itemBox.InnerHtml += m;
                    }
                    else
                    {
                        var itemWrapper = new TagBuilder("li");
                        itemWrapper.InnerHtml += m.ToHtmlString();
                        itemBox.InnerHtml += itemWrapper.ToString();
                    }
                }
            }

            group.InnerHtml += button.ToString() + itemBox.ToString();
            return new MvcHtmlString(group.ToString());
        }

        public static MvcHtmlString ExportToExcel(this HtmlHelper html, string actionName, string controllerName, string linkText = "导出Excel", object htmlAttributes = null)
        {
            var routeValues = new RouteValueDictionary();
            routeValues["action"] = actionName;
            routeValues["controller"] = controllerName;
            var rq = html.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in rq.Keys)
            {
                routeValues[key] = rq[key];
            }

            UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);

            TagBuilder link = new TagBuilder("a");
            link.Attributes["target"] = "_self";
            link.Attributes["href"] = url.RouteUrl(routeValues);
            link.AddCssClass("btn btn-info btn-sm");
            link.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            var icon = new TagBuilder("span");
            icon.AddCssClass(BootstrapIcon.EXPORT);
            link.InnerHtml += icon;

            link.InnerHtml += linkText;

            return new MvcHtmlString(link.ToString());
        }   
    }
}
