using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeKeSoftPlatform.WebExtension
{
    public abstract class PageTokenViewBase
    {
        public static readonly string ClientTokenName = "_ClientTokenName_";
        public static readonly string ServerTokenName = "_ServerTokenName_";

        public abstract string GetServerPageToken();
        public abstract string GetClientPageToken { get; }
        public abstract bool TokensMatch { get; }

        public static string GenerateServerToken()
        {
            return EncryptUtils.MD5Encrypt(HttpContext.Current.Session.SessionID + DateTime.Now.Ticks.ToString());
        }
    }
}
