using System.Web.Mvc;

namespace KeKeSoftPlatform.App.Areas.weixin
{
    public class weixinAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "weixin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "weixin_default",
                "weixin/{controller}/{action}/{id}",
                new { controller = "Gateway", action = "Handle", id = UrlParameter.Optional }
            );
        }
    }
}