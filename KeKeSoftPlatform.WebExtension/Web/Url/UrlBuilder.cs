using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KeKeSoftPlatform.WebExtension
{
    public class UrlBuilder
    {
        private List<string> _IgnoreParams;
        private RouteData _RouteData;
        private HttpContextBase _HttpContext;

        public UrlBuilder Ignore(string key)
        {
            this._IgnoreParams.Add(key);
            return this;
        }

        public UrlBuilder Attach(string key, object value, bool replaceExisting = false)
        {
            if (this._RouteData.Values.ContainsKey(key))
            {
                if (replaceExisting)
                {
                    this._RouteData.Values[key] = value;
                    return this;
                }
                throw new Exception("生成url链接时，参数{0}已存在".FormatString(key));
            }
            else
            {
                this._RouteData.Values.Add(key, value);
                return this;
            }
        }

        public UrlBuilder Action(string actionName)
        {
            this._RouteData.Values.Add("action", actionName);
            return this;
        }

        public UrlBuilder Contorller(string controllerName)
        {
            this._RouteData.Values.Add("controller", controllerName);
            return this;
        }

        public UrlBuilder Area(string areaName)
        {
            this._RouteData.Values.Add("area", areaName);
            return this;
        }

        public string Build()
        {
            var routeValues = this._RouteData.Values;
            UrlHelper url = new UrlHelper(new RequestContext(this._HttpContext, this._RouteData));
            var rq = this._HttpContext.Request.QueryString;
            foreach (string key in rq.Keys)
            {
                if (this._IgnoreParams != null && !this._IgnoreParams.Any(m => string.Equals(m, key, StringComparison.CurrentCultureIgnoreCase)))
                {
                    routeValues[key] = rq[key];
                }
            }
            return url.RouteUrl(routeValues);
        }

        public UrlBuilder(HttpContextBase httpContext, RouteData routeData = null)
        {
            this._IgnoreParams = new List<string>();
            this._HttpContext = httpContext;
            this._RouteData = routeData;
            if (this._RouteData == null)
            {
                this._RouteData = new RouteData();
            }
        }

        public static UrlBuilder CreateInstance(HttpContextBase httpContext, RouteData routeData = null)
        {
            return new UrlBuilder(httpContext, routeData);
        }
    }
}
