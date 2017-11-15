using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KeKeSoftPlatform.WebExtension
{
    public class BaseForm:MvcForm
    {
        protected HtmlHelper _HtmlHelper;
        public BaseForm(HtmlHelper htmlHelper)
            : base(htmlHelper.ViewContext)
        {
            this._HtmlHelper = htmlHelper;
        }

        protected override void Dispose(bool disposing)
        {
            var formToken = new TagBuilder("input");
            formToken.Attributes.Add("type", "hidden");
            formToken.Attributes.Add("value", new SessionPageTokenView().GetServerPageToken());
            formToken.Attributes.Add("name", PageTokenViewBase.ClientTokenName);
            
            _HtmlHelper.ViewContext.Writer.Write(formToken.ToString(TagRenderMode.SelfClosing));

            base.Dispose(disposing);
        }
    }
}
