using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeKeSoftPlatform.Core
{
    public class _User:BaseUser
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OpenId { get; set; }
        public DateTime LoginDate { get; set; }
        public int? LevelId { get; set; }
        public List<Identity> OwnIdentities { get; set; }

        public _User()
        {
        }

        public string UserLoginTicketId
        {
            get
            {
                return EncryptUtils.MD5Encrypt(this.Id + this.LoginDate.ToOADate().ToString());
            }
        }

        public static bool HasValue
        {
            get
            {
                return !(HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request.IsAuthenticated == false);
            }
        }

        public static _User Value
        {
            get
            {
                if (HasValue==false)
                {
                    throw new Exception("未登录访问错误_User");
                }
                return (HttpContext.Current.User as FormsPrincipal<_User>).UserData;
            }
        }
    }
}
