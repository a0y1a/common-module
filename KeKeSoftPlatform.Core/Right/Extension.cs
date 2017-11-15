//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Mvc;

//namespace KeKeSoftPlatform.Core
//{
//    public static partial class PFExtension
//    {
//        public static MvcHtmlString HasPermission(this MvcHtmlString content, string menuUrl, string menuActionCode)
//        {
//            return AuthorizeProvider.HasPermission(menuUrl, menuActionCode, content);
//        }

//        public static MvcHtmlString HasPermission(this MvcHtmlString content, params Identity[] identities)
//        {
//            return AuthorizeProvider.HasPermission(content, identities);
//        }
//    }
//}
