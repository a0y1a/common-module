using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Mvc.Html;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.WebExtension
{
    public static partial class HtmlHelperExtension
    {
        public static MvcHtmlString Submit(this HtmlHelper html, string text = "提交", object htmlAttributes = null)
        {
            return html.Button(text, PV.SUBMIT_ID, BootstrapIcon.FLOPPY_DISK, ButtonType.Submit, ButtonDisplayType.Primary, ButtonSize.Default, htmlAttributes);
        }
        public static MvcHtmlString Row(this HtmlHelper html, string labelText, Func<object, object> valueHtml, Func<object, object> extensionHtml = null, object htmlAtrributes = null)
        {
            var row = new TagBuilder("div");
            row.AddCssClass("form-group");
            row.MergeAttributes(new RouteValueDictionary(htmlAtrributes));

            var label = new TagBuilder("label");
            label.AddCssClass("col-sm-2 control-label");
            label.SetInnerText(string.IsNullOrWhiteSpace(labelText) ? null : (labelText + "："));
            row.InnerHtml += label;

            var content = new TagBuilder("div");
            content.AddCssClass("col-sm-10");
            content.InnerHtml += valueHtml(null);

            row.InnerHtml += content;

            if (extensionHtml != null)
            {
                var extension = new TagBuilder("div");
                extension.InnerHtml += extensionHtml(null);
                content.InnerHtml += extension;
            }

            return new MvcHtmlString(row.ToString());
        }
        public static MvcHtmlString RowForEdit<T, V>(this HtmlHelper<T> html, Expression<Func<T, V>> propertyExpress, bool? required = null, Func<object, object> extensionHtml = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(propertyExpress, html.ViewData);

            var row = new TagBuilder("div");
            row.AddCssClass("form-group");

            var label = new TagBuilder("label");
            label.AddCssClass("col-md-2 control-label");
            label.InnerHtml += metadata.GetDisplayName() + "：";

            if (required == null)
            {
                var isRequiredAttribute = metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(IsRequiredAttribute), false);
                if (isRequiredAttribute == null || !isRequiredAttribute.Any())
                {
                    required = metadata.IsRequired;
                }
                else
                {
                    required = (isRequiredAttribute.First() as IsRequiredAttribute).IsRequired;
                }
            }

            if (required.Value)
            {
                TagBuilder homeRequiredHtml = new TagBuilder("span");
                homeRequiredHtml.AddCssClass("input-required hidden-xs");
                homeRequiredHtml.SetInnerText("*");
                label.InnerHtml = homeRequiredHtml.ToString() + " " + label.InnerHtml;

                TagBuilder endRequiredHtml = new TagBuilder("span");
                endRequiredHtml.AddCssClass("input-required hidden-lg");
                endRequiredHtml.SetInnerText("*");
                label.InnerHtml += "" + endRequiredHtml.ToString();


            }
            

            row.InnerHtml += label;

            var content = new TagBuilder("div");
            content.AddCssClass("col-md-10");
            content.InnerHtml += html.EditorFor(propertyExpress);
            content.InnerHtml += html.ValidationMessageFor(propertyExpress);

            var extension = new TagBuilder("div");
            extension.Attributes["id"] = "{0}_{1}".FormatString(metadata.PropertyName, PV.EXTENSION_ID);
            if (extensionHtml != null)
            {
                extension.InnerHtml += extensionHtml(null);
            }
            content.InnerHtml += extension;

            row.InnerHtml += content;

            

            return new MvcHtmlString(row.ToString());
        }
        public static MvcHtmlString RowForDisplay<T, V>(this HtmlHelper<T> html, Expression<Func<T, V>> propertyExpress, bool? hideValue = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(propertyExpress, html.ViewData);

            var row = new TagBuilder("div");
            row.AddCssClass("form-group");

            var label = new TagBuilder("label");
            label.AddCssClass("col-md-2 control-label");
            label.SetInnerText(metadata.GetDisplayName() + "：");
            row.InnerHtml += label;

            if (hideValue.HasValue)
            {
                if (hideValue == true)
                {
                    row.InnerHtml += html.HiddenFor(propertyExpress);
                }
                else
                {
                    var displayOnlyAttribute = metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(DisplayOnlyAttribute), false);
                    if (displayOnlyAttribute.Any() && (displayOnlyAttribute.First() as DisplayOnlyAttribute).HideValue == true)
                    {
                        row.InnerHtml += html.HiddenFor(propertyExpress).ToHtmlString();
                    }
                }
            }

            var valueHtmlContainer = new TagBuilder("div");
            valueHtmlContainer.AddCssClass("col-md-10");

            var valueHtml = new TagBuilder("p");
            valueHtml.Attributes["id"] = "__" + metadata.PropertyName;
            valueHtml.AddCssClass("form-control-static");
            //修改者：zjk,修改时间：2017-02-17，修改原因：SetInnerText方式会出现转义字符：&gt
            //valueHtml.SetInnerText(html.DisplayTextFor(propertyExpress).ToString());
            valueHtml.InnerHtml = html.DisplayTextFor(propertyExpress).ToString();
            valueHtmlContainer.InnerHtml += valueHtml;
            row.InnerHtml += valueHtmlContainer;

            return MvcHtmlString.Create(row.ToString());
        }

        #region 表单
        public static AdminLTEForm Form(this HtmlHelper htmlHelper)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, null, null, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, object routeValues)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, null, null,routeValues, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, null, null, routeValues, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, null, null, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, controllerName, null, method, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += "form-horizontal";
            }
            else
            {
                htmlAttributes.Add("class", "form-horizontal");
            }
            return new AdminLTEForm(htmlHelper, actionName, controllerName, null,method, htmlAttributes);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            var values = new RouteValueDictionary(htmlAttributes);
            if (values.ContainsKey("class"))
            {
                values["class"] += "form-horizontal";
            }
            else
            {
                values.Add("class", "form-horizontal");
            }

            return new AdminLTEForm(htmlHelper, actionName, controllerName, null, method, values);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, controllerName, routeValues, method);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            var htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", "form-horizontal");
            return new AdminLTEForm(htmlHelper, actionName, controllerName, routeValues, method);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {

            var values = new RouteValueDictionary(htmlAttributes);
            if (values.ContainsKey("class"))
            {
                values["class"] += "form-horizontal";
            }
            else
            {
                values.Add("class", "form-horizontal");
            }
            return new AdminLTEForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method,values);
        }
        public static AdminLTEForm Form(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += "form-horizontal";
            }
            else
            {
                htmlAttributes.Add("class", "form-horizontal");
            }
            return new AdminLTEForm(htmlHelper, actionName, controllerName, routeValues, method, htmlAttributes);
        }

        public class AdminLTEForm : BaseForm
        {
            public AdminLTEForm(HtmlHelper html)
                : base(html)
            {
                html.BeginForm();
            }
            public AdminLTEForm(HtmlHelper html, object routeValues)
                : base(html)
            {
                html.BeginForm(routeValues);
            }
            public AdminLTEForm(HtmlHelper html, RouteValueDictionary routeValues)
                : base(html)
            {
                html.BeginForm(routeValues);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName)
                : base(html)
            {
                html.BeginForm(actionName, controllerName);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, FormMethod method)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, method);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, object routeValues)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, RouteValueDictionary routeValues)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, method, htmlAttributes);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, FormMethod method, object htmlAttributes)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, method, htmlAttributes);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, object routeValues, FormMethod method)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues, method);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues, method);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues, method, htmlAttributes);
            }
            public AdminLTEForm(HtmlHelper html, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
                : base(html)
            {
                html.BeginForm(actionName, controllerName, routeValues, method, htmlAttributes);
            }
        }
        #endregion


        #region 查询
        public static SearchForm Search(this HtmlHelper htmlHelper, bool ShowSubmit = true)
        {
            return new SearchForm(htmlHelper, ShowSubmit);
        }
        public static FormGroup Group(this HtmlHelper htmlHelper, string labelName)
        {
            return new FormGroup(htmlHelper, labelName);
        }
        public static SearchGroup SearchBtn(this HtmlHelper htmlHelper)
        {
            return new SearchGroup(htmlHelper, "查询");
        }
        public static SearchGroup SearchBtn(this HtmlHelper htmlHelper, string labelName)
        {
            return new SearchGroup(htmlHelper, labelName);
        }
        public static void Group(this HtmlHelper htmlHelper, string labelName, MvcHtmlString searchContent)
        {
            using (FormGroup group = new FormGroup(htmlHelper, labelName))
            {
                htmlHelper.ViewContext.Writer.Write(searchContent.ToHtmlString());
            }
        }
        public class SearchGroup : IDisposable
        {
            protected HtmlHelper _HtmlHelper;

            protected TagBuilder group;
            public SearchGroup(HtmlHelper htmlHelper, string labelName)
            {
                this._HtmlHelper = htmlHelper;

                _HtmlHelper.ViewContext.Writer.Write(this._HtmlHelper.Button(labelName, "btnSearch", BootstrapIcon.SEARCH, ButtonType.Submit, htmlAttributes: new { style = "margin-left:20px;" }));

            }

            public void Dispose()
            {

            }
        }
        public class FormGroup : IDisposable
        {
            protected HtmlHelper _HtmlHelper;

            protected TagBuilder group;
            public FormGroup(HtmlHelper htmlHelper, string labelName)
            {
                this._HtmlHelper = htmlHelper;

                group = new TagBuilder("div");
                group.AddCssClass("form-group");

                var span = new TagBuilder("span");
                span.SetInnerText(labelName);

                this._HtmlHelper.ViewContext.Writer.Write(group.ToString(TagRenderMode.StartTag));
                this._HtmlHelper.ViewContext.Writer.Write(span.ToString(TagRenderMode.Normal));
            }

            public void Dispose()
            {
                this._HtmlHelper.ViewContext.Writer.Write(group.ToString(TagRenderMode.EndTag));
            }
        }
        public class SearchForm : IDisposable
        {
            protected HtmlHelper _HtmlHelper;

            protected TagBuilder panel;
            protected TagBuilder panelBody;
            protected TagBuilder form;
            protected bool IsSubmit;
            public SearchForm(HtmlHelper htmlHelper, bool ShowSubmit)
            {
                this._HtmlHelper = htmlHelper;
                this.IsSubmit = ShowSubmit;

                panel = new TagBuilder("div");
                panel.AddCssClass("panel panel-default");

                panelBody = new TagBuilder("div");
                panelBody.AddCssClass("panel-body");
                panelBody.Attributes.Add("id", "form_search");

                form = new TagBuilder("form");
                form.AddCssClass("form-inline");
                form.Attributes.Add("action", htmlHelper.GenerateUrl(Pager.PAGE_NUM_KEY));
                form.Attributes.Add("method", "get");
                form.Attributes.Add("role", "form");

                htmlHelper.ViewContext.Writer.Write(panel.ToString(TagRenderMode.StartTag));
                htmlHelper.ViewContext.Writer.Write(panelBody.ToString(TagRenderMode.StartTag));
                htmlHelper.ViewContext.Writer.Write(form.ToString(TagRenderMode.StartTag));

            }

            public void Dispose()
            {
                if (IsSubmit)
                {
                    var group = new TagBuilder("div");
                    group.AddCssClass("form-group");
                    group.InnerHtml += this._HtmlHelper.Button("查询", "btnSearch", BootstrapIcon.SEARCH, ButtonType.Submit, htmlAttributes: new { style = "margin-left:20px;" });
                    _HtmlHelper.ViewContext.Writer.Write(group);
                }
                _HtmlHelper.ViewContext.Writer.Write(form.ToString(TagRenderMode.EndTag));
                _HtmlHelper.ViewContext.Writer.Write(panelBody.ToString(TagRenderMode.EndTag));
                _HtmlHelper.ViewContext.Writer.Write(panel.ToString(TagRenderMode.EndTag));
            }
        }
        #endregion
    }
}
