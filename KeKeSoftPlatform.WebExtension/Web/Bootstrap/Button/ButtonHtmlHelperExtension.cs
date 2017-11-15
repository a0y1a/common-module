using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace KeKeSoftPlatform.WebExtension
{
    public static partial class HtmlHelperExtension
    {
        public static MvcHtmlString Button(this HtmlHelper html, string text, string name, string iconType = null, string type = ButtonType.Button, string displayType = ButtonDisplayType.Default, string size = ButtonSize.Small, object htmlAttributes = null)
        {
            var button = new TagBuilder("button");
            button.Attributes["name"] = name;
            button.Attributes["id"] = name;
            button.AddCssClass("btn");
            button.AddCssClass(displayType);
            button.AddCssClass(size);

            if (iconType != null)
            {
                button.InnerHtml += html.Icon(iconType);
            }
            button.InnerHtml += " " + text;
            if (htmlAttributes != null)
            {
                button.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            button.Attributes.Add("type", type);

            return new MvcHtmlString(button.ToString());
        }

        

        public static MvcHtmlString ActionLinkButton(this HtmlHelper html, string text, string action, string controller = null, object routeValues = null, string name = null, string iconType = null, string type = ButtonDisplayType.Default, string size = ButtonSize.Small, object htmlAttributes = null)
        {
            var link = new TagBuilder("a");
            link.Attributes["name"] = name;
            link.Attributes["id"] = name;
            link.AddCssClass("btn");
            link.AddCssClass(type);
            link.AddCssClass(size);


            var routeValuesCollection = new RouteValueDictionary(routeValues);
            UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
            routeValuesCollection["action"] = action;
            routeValuesCollection["controller"] = controller;

            var htmlAttributesCollection = new RouteValueDictionary(htmlAttributes);
            htmlAttributesCollection["href"] = url.RouteUrl(routeValuesCollection);
            link.MergeAttributes(htmlAttributesCollection);

            if (iconType != null)
            {
                link.InnerHtml += html.Icon(iconType);
            }
            link.InnerHtml += " " + text;

            return new MvcHtmlString(link.ToString());
        }
    }
}
