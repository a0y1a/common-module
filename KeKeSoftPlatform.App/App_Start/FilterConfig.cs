using System.Web;
using System.Web.Mvc;
using KeKeSoftPlatform.Core;

namespace KeKeSoftPlatform.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NavigationAttribute());
        }
    }
}
