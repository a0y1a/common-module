using System.Web.Mvc;

namespace KeKeSoftPlatform.App.Areas.OrgChart
{
    public class OrgChartAreaRegistration : AreaRegistration
    {
        public const string AREA_NAME = "OrgChart";

        public override string AreaName
        {
            get
            {
                return "OrgChart";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OrgChart_default",
                "OrgChart/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "KeKeSoftPlatform.App.Areas.OrgChart.Controllers" }
            );
        }
    }
}
