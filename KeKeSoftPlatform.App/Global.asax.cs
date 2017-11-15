using FluentValidation;
using FluentValidation.Mvc;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using KeKeSoftPlatform.Common;
using System.Globalization;

namespace KeKeSoftPlatform.App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthenticateRequest += MvcApplication_AuthenticateRequest;
        }
        void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            FormsPrincipal<_User>.RestoreUser(app.Context);
        }
        protected void Application_Start()
        {
            //CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            //CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //数据校验
            FluentValidationModelValidatorProvider.Configure();
            ValidatorOptions.ResourceProviderType = typeof(Resources);

            ListProviderBus.Initialization(new DynamicListProvider(), new StaticListProvider());

            CacheExtensions.Debug = true;
        }
    }
}
