using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.WebExtension
{
    public class BaseUser
    {
        /// <summary>
        /// 是否持久化登录
        /// </summary>
        public bool IsPersistent { get; set; }
        /// <summary>
        /// 过期时间，单位：分钟
        /// </summary>
        public int? Expiration { get; set; }

        public bool EnableSingleOnline { get; set; }
        internal string CurrentOnlineToken { get; set; }

        public bool IsInvalid { get;internal set; }

        public BaseUser()
        {
            IsPersistent = true;
            IsInvalid = true;
        }
    }
    public class FormsPrincipal<TUserData> : IPrincipal
        where TUserData : BaseUser, new()
    {
        private IIdentity _identity;
        private TUserData _userData;

        public FormsPrincipal(FormsAuthenticationTicket ticket, TUserData userData)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
            if (userData == null)
                throw new ArgumentNullException("userData");

            _identity = new FormsIdentity(ticket);
            _userData = userData;
        }

        public TUserData UserData
        {
            get { return _userData; }
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            // 把判断用户组的操作留给UserData去实现。

            IPrincipal principal = _userData as IPrincipal;
            if (principal == null)
                throw new NotImplementedException();
            else
                return principal.IsInRole(role);
        }

        private static Dictionary<string, string> _Online = new Dictionary<string, string>();

        /// <summary>
        /// 执行用户登录操作
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="userData">与登录名相关的用户信息</param>
        /// <param name="expiration">登录Cookie的过期时间，单位：分钟。</param>
        public static void SignIn(string loginName, TUserData userData)
        {
            if (string.IsNullOrEmpty(loginName))
                throw new ArgumentNullException("loginName");
            if (userData == null)
                throw new ArgumentNullException("userData");

            if (userData.EnableSingleOnline)
            {
                userData.CurrentOnlineToken = PF.Key().ToString();
                if (_Online.ContainsKey(loginName))
                {
                    _Online[loginName] = userData.CurrentOnlineToken;
                }
                else
                {
                    _Online.Add(loginName, userData.CurrentOnlineToken);
                }
            }

            // 1. 把需要保存的用户数据转成一个字符串。
            string data = null;
            if (userData != null)
                data = JsonConvert.SerializeObject(userData);


            // 2. 创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                2, loginName, DateTime.Now, DateTime.Now.AddDays(1), userData.IsPersistent, data);


            // 3. 加密Ticket，变成一个加密的字符串。
            string cookieValue = FormsAuthentication.Encrypt(ticket);


            // 4. 根据加密结果创建登录Cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (userData.Expiration.HasValue)
                cookie.Expires = DateTime.Now.AddMinutes(userData.Expiration.Value);

            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();

            // 5. 写登录Cookie
            context.Response.Cookies.Remove(cookie.Name);
            context.Response.Cookies.Add(cookie);

            HttpContext.Current.User = new FormsPrincipal<TUserData>(ticket, userData);
        }


        /// <summary>
        /// 根据HttpContext对象设置用户标识对象
        /// </summary>
        /// <param name="context"></param>
        public static void RestoreUser(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // 1. 读登录Cookie
            HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return;

            try
            {
                TUserData userData = null;
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null && string.IsNullOrEmpty(ticket.UserData) == false)
                    // 3. 还原用户数据
                    userData = JsonConvert.DeserializeObject<TUserData>(ticket.UserData);

                if (ticket != null && userData != null)
                    // 4. 构造我们的MyFormsPrincipal实例，重新给context.User赋值。
                    context.User = new FormsPrincipal<TUserData>(ticket, userData);

                if (userData.EnableSingleOnline)
                {
                    if (_Online.ContainsKey(ticket.Name) == false)
                    {
                        userData.IsInvalid = false;
                    }
                    if (_Online[ticket.Name] != userData.CurrentOnlineToken)
                    {
                        userData.IsInvalid = false;
                    }
                }
                if (userData.Expiration.HasValue)
                {
                    cookie.Expires = DateTime.Now.AddMinutes(userData.Expiration.Value);
                }
                context.Response.Cookies.Remove(cookie.Name);
                cookie.Expires = DateTime.Now.AddMinutes(userData.Expiration.Value);
                context.Response.Cookies.Add(cookie);
                
            }
            catch { /* 有异常也不要抛出，防止攻击者试探。 */ }
        }
    }
}
