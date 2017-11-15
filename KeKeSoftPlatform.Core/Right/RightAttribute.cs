using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.Core
{
    public enum Identity
    {
        User = 4,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 8,
        /// <summary>
        /// 超级管理员
        /// </summary>
        SuperAdmin = 16,
        /// <summary>
        /// 开发人员
        /// </summary>
        Developer = 32,
        /// <summary>
        /// 所有人员
        /// </summary>
        All =  Identity.User | Identity.Admin | Identity.SuperAdmin | Identity.Developer 
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RightAttribute : AuthorizeAttribute
    {
        public RightAttribute(Identity allowedIdentities) 
        {
            this._AllowedIdentities = allowedIdentities;
        }

        public RightAttribute()
        {

        }

        public bool AllowAnonymous { get; set; }

        private Identity? _AllowedIdentities;
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AllowAnonymous)
            {
                return;
            }

            if (this._AllowedIdentities.HasValue == false)
            {
                throw new Exception("未添加任何权限");
            }

            RightAuditor lastAuditor = null;
            var allowedIdentityItems = Enum.GetValues(typeof(Identity))
                .Cast<Identity>()
                .Where(m => this._AllowedIdentities.Has(m))
                .ToList();

            foreach (var item in allowedIdentityItems)
            {
                var auditor = RighAuditorFactory.GetRightAuditor(item);
                auditor.Audit(filterContext);
                if (auditor.Result.HasValue == false)
                {
                    throw new Exception(string.Format("{0}未设置审核结果（true或false）", auditor.GetType().FullName));
                }
                if (auditor.Result.Value)
                {
                    return;
                }
                lastAuditor = auditor;
            }

            if (lastAuditor == null)
            {
                filterContext.Result = new RedirectResult("/system/unauthorized?url=" + filterContext.HttpContext.Server.UrlEncode(UrlBuilder.CreateInstance(filterContext.HttpContext).Build()));
                return;
            }
            lastAuditor.TriggerFail();

            if (filterContext.Result == null)
            {
               base.OnAuthorization(filterContext);
            }
        }
    }
}
