using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    public interface IValidateReHttpRedirect
    {
        ActionResult Redirect(ActionExecutingContext filterContext);
    }
}
