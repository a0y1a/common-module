using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeKeSoftPlatform.WebExtension
{
    public class SessionPageTokenView : PageTokenViewBase
    {
        public override string GetServerPageToken()
        {
            if (HttpContext.Current.Session[ServerTokenName] != null)
            {
                return HttpContext.Current.Session[ServerTokenName].ToString();
            }
            else
            {
                var token = PageTokenViewBase.GenerateServerToken();
                HttpContext.Current.Session[ServerTokenName] = token;
                return token;
            }
        }

        public override string GetClientPageToken
        {
            get
            {
                return HttpContext.Current.Request.Params[ClientTokenName];
            }
        }

        public override bool TokensMatch
        {
            get
            {
                string formToken = GetClientPageToken;
                if (formToken != null)
                {
                    if (formToken.Equals(GetServerPageToken()))
                    {
                        HttpContext.Current.Session[ServerTokenName] = GenerateServerToken();
                        return true;
                    }
                }
                return false;
            }
        }

         
    }
}
