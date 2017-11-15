using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KeKeSoftPlatform.WebExtension
{
    public static partial class HtmlHelperExtension
    {
        public static MvcHtmlString Image(this HtmlHelper html, string src, object htmlAttributes = null)
        {
            TagBuilder image = new TagBuilder("img");
            image.Attributes["src"] = src;
            image.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(image.ToString());
        }

        public static MvcHtmlString CheckBox(this HtmlHelper html, string name, string label, bool isChecked = false)
        {
            TagBuilder ck = new TagBuilder("input");
            ck.Attributes["type"] = "checkbox";
            ck.Attributes["name"] = name;
            ck.Attributes["id"] = name;
            ck.Attributes["value"] = "true";
            if (isChecked)
            {
                ck.Attributes["checked"] = "checked";
            }
            return new MvcHtmlString(ck.ToString() + label);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper html, string name, string listName, object value, bool autoInertAll = false,object htmlAtrributes=null)
        {
            var selectListItemCollection = new List<SelectListItem>();
            var listItemCollection = ListProviderBus.GetListItemCollection(listName);
            foreach (var item in listItemCollection)
            {
                selectListItemCollection.Add(new SelectListItem { Text = item.Text, Value = item.Value });
            }

            if (autoInertAll)
            {
                selectListItemCollection.Insert(0, new SelectListItem { Text = "==全部==", Value = string.Empty });
            }

            if (value != null)
            {
                var selectListItemSelected = selectListItemCollection.FirstOrDefault(m => m.Value == value.ToString());
                if (selectListItemSelected != null)
                {
                    selectListItemSelected.Selected = true;
                }
            }
            if (htmlAtrributes == null)
            {
                return html.DropDownList(name, selectListItemCollection, new { @class = "form-control" });
            }
            else
            {
                return html.DropDownList(name, selectListItemCollection, htmlAtrributes);
            }
        }

        public static MvcHtmlString Page<T>(this HtmlHelper html, Pager<T> pager)
        {
            return html.Partial("Pager", pager);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper html,string name, Enum type, bool autoInsertAll = false,bool clearSelected=false)
        {
            var selectListItemCollection = Enum.GetValues(type.GetType())
                                                     .Cast<Enum>()
                                                     .Select(m =>
                                                     {
                                                         string enumVal = Enum.GetName(type.GetType(), m);
                                                         return new SelectListItem()
                                                         {
                                                             Selected = (clearSelected ? false : (type.ToString() == enumVal)),
                                                             Text = m.EnumMetadataDisplay(),
                                                             Value = enumVal
                                                         };
                                                     }).ToList();
            if (autoInsertAll)
            {
                selectListItemCollection.Insert(0, new SelectListItem { Text = "==全部==", Value = string.Empty });
            }
            return html.DropDownList(name, selectListItemCollection, new { @class = "form-control" });
        }

        public static MvcHtmlString Tab(this HtmlHelper html, params ListItem[] tabItem)
        {
            return html.Partial("tab", tabItem);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper html, string text, object htmlAttributes = null)
        {
            var link = new TagBuilder("a");
            link.MergeAttribute("href", "#");
            link.SetInnerText(text);
            link.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(link.ToString());
        }


        #region VTree
        public static MvcHtmlString VTree<T>(this HtmlHelper html, VTreeNode<T> treeNode, Func<VTreeNode<T>, HelperResult> nodeContent)
        {
            var table = new TagBuilder("table");
            table.AddCssClass("jqOrgchart");
            table.Attributes["cellspacing"] = "0";
            table.Attributes["cellpadding"] = "0";

            var tr = new TagBuilder("tr");
            var td = new TagBuilder("td");
            if (treeNode.Children.Any())
            {
                td.Attributes["colspan"] = (treeNode.Children.Count * 2).ToString();
            }
            td.InnerHtml += nodeContent(treeNode);
            tr.InnerHtml += td;
            table.InnerHtml += tr;

            if (treeNode.Children.Any())
            {
                var downLineTr = new TagBuilder("tr");
                var downLineTd = new TagBuilder("td");
                downLineTd.Attributes["colspan"] = (treeNode.Children.Count * 2).ToString();
                var downLine = new TagBuilder("div");
                downLine.AddCssClass("down line");
                downLineTd.InnerHtml += downLine;
                downLineTr.InnerHtml += downLineTd;
                table.InnerHtml += downLineTr;

                var horizontalLineTr = new TagBuilder("tr");
                for (int i = 0; i < treeNode.Children.Count * 2; i++)
                {
                    var horizontalLineTd = new TagBuilder("td");
                    if (i % 2 == 0)
                    {
                        horizontalLineTd.AddCssClass("line right");
                    }
                    else
                    {
                        horizontalLineTd.AddCssClass("line left");
                    }

                    if (i > 0 && i < treeNode.Children.Count * 2 - 1)
                    {
                        horizontalLineTd.AddCssClass("top");
                    }
                    horizontalLineTr.InnerHtml += horizontalLineTd;
                }
                table.InnerHtml += horizontalLineTr;

                var childrenNodeTr = new TagBuilder("tr");
                foreach (var item in treeNode.Children)
                {
                    var childrenNodeTd = new TagBuilder("td");
                    childrenNodeTd.Attributes["colspan"] = "2";
                    childrenNodeTd.InnerHtml += BuildVTreeNode(item, nodeContent);
                    childrenNodeTr.InnerHtml += childrenNodeTd;
                }
                table.InnerHtml += childrenNodeTr;
            }
            return MvcHtmlString.Create(table.ToString());
        }

        private static MvcHtmlString BuildVTreeNode<T>(VTreeNode<T> node, Func<VTreeNode<T>, HelperResult> nodeContent)
        {
            var table = new TagBuilder("table");
            table.Attributes["cellspacing"] = "0";
            table.Attributes["cellpadding"] = "0";
            table.Attributes["border"] = "0";
            table.Attributes["style"] = "width:100%;";

            var tr = new TagBuilder("tr");
            var td = new TagBuilder("td");
            if (node.Children.Any())
            {
                td.Attributes["colspan"] = (node.Children.Count * 2).ToString();
            }
            td.InnerHtml += nodeContent(node);
            tr.InnerHtml += td;
            table.InnerHtml += tr;

            if (node.Children.Any())
            {
                var downLineTr = new TagBuilder("tr");
                var downLineTd = new TagBuilder("td");
                downLineTd.Attributes["colspan"] = (node.Children.Count * 2).ToString();
                var downLine = new TagBuilder("div");
                downLine.AddCssClass("down line");
                downLineTd.InnerHtml += downLine;
                downLineTr.InnerHtml += downLineTd;
                table.InnerHtml += downLineTr;

                var horizontalLineTr = new TagBuilder("tr");
                for (int i = 0; i < node.Children.Count * 2; i++)
                {
                    var horizontalLineTd = new TagBuilder("td");
                    if (i % 2 == 0)
                    {
                        horizontalLineTd.AddCssClass("line right");
                    }
                    else
                    {
                        horizontalLineTd.AddCssClass("line left");
                    }

                    if (i > 0 && i < node.Children.Count * 2 - 1)
                    {
                        horizontalLineTd.AddCssClass("top");
                    }
                    horizontalLineTr.InnerHtml += horizontalLineTd;
                }
                table.InnerHtml += horizontalLineTr;

                var childrenNodeTr = new TagBuilder("tr");
                foreach (var item in node.Children)
                {
                    var childrenNodeTd = new TagBuilder("td");
                    childrenNodeTd.Attributes["colspan"] = "2";
                    childrenNodeTd.InnerHtml += BuildVTreeNode(item, nodeContent);
                    childrenNodeTr.InnerHtml += childrenNodeTd;
                }
                table.InnerHtml += childrenNodeTr;
            }
            return new MvcHtmlString(table.ToString());
        }
        #endregion

        #region Html
        /// <summary>
        /// 生成链接地址
        /// </summary>
        /// <param name="html"></param>
        /// <param name="ignoreParams">忽略的参数</param>
        /// <returns></returns>
        public static string GenerateUrl(this HtmlHelper html, params string[] ignoreParams)
        {
            var routeValues = new RouteValueDictionary();
            UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
            var rq = html.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in rq.Keys)
            {
                if (ignoreParams != null && !ignoreParams.Any(m => m == key))
                {
                    routeValues[key] = rq[key];
                }
            }
            // Add action
            routeValues["action"] = (string)html.ViewContext.RouteData.Values["action"];

            // Add controller
            routeValues["controller"] = (string)html.ViewContext.RouteData.Values["controller"];

            return url.RouteUrl(routeValues);
        }

        public static MvcHtmlString Append(this MvcHtmlString source, string value)
        {
            return new MvcHtmlString(source.ToString() + value.ToString());
        }

        public static MvcHtmlString Append(this MvcHtmlString source, MvcHtmlString html)
        {
            return new MvcHtmlString(source.ToHtmlString() + html.ToHtmlString());
        }


        public static MvcHtmlString Append(this MvcHtmlString source, Func<object, HelperResult> html)
        {
            return new MvcHtmlString(source.ToHtmlString() + html(null).ToHtmlString());
        }
        #endregion
    }
}

