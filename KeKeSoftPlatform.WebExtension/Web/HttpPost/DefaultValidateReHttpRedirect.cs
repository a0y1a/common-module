using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    public class DefaultValidateReHttpRedirect : IValidateReHttpRedirect
    {
        private string _Message;
        private string _Url;
        public DefaultValidateReHttpRedirect(string message,string url)
        {
            this._Message = message;
            this._Url = url;
        }
        public virtual ActionResult Redirect(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData[AlertEntity.ALERT_ENTITY] = new AlertEntity(this._Message, AlertType.Danger);
            return new RedirectResult(this._Url);
        }
    }
}
