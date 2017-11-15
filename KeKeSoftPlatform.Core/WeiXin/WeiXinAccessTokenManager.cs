using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.Core
{
    public sealed class WeiXinAccessTokenManager
    {
        public class WeiXinTicket
        {
            [JsonProperty("access_token")]
            public string Token { get; set; }

            [JsonProperty("expires_in")]
            public int Expire { get; set; }
        }

        public class WeiXinJsapi_Ticket
        {
            [JsonProperty("errcode")]
            public string ErrCode { get; set; }
            [JsonProperty("errmsg")]
            public string ErrMsg { get; set; }
            [JsonProperty("ticket")]
            public string Token { get; set; }

            [JsonProperty("expires_in")]
            public int Expire { get; set; }
        }

        public const string APP_ID = "wx63d5960a3bb18d1a";//wx7cdee159611d6371
        public const string APP_SECRET = "3da829accbda56d3b88e4b906b76de91";//23fa20f1ca62fc13c3e35a06d80f9ea0
        public const string WX_CODE = "MUSOU-SHOJI";//DDCPT_QLX

        private string _AccessToken;
        public string AccessToken
        {
            get
            {
                if (DateTime.Now > this._ExpiresIn)
                {
                    this.InitOrRefresh();
                }
                return _AccessToken;
            }
        }

        private DateTime _ExpiresIn;

        private string _Jsapi_Ticket;
        public string Jsapi_Ticket
        {
            get
            {
                if (DateTime.Now > this._Jsapi_ExpiresIn)
                {
                    this.Jsapi_InitOrRefresh();
                }
                return _Jsapi_Ticket;
            }
        }

        private DateTime _Jsapi_ExpiresIn;

        /// <summary>
        /// 获取或刷新access_token
        /// </summary>
        private void InitOrRefresh()
        {
            var ticket = Http.CreateInstance()
                .Url("https://api.weixin.qq.com/cgi-bin/token")
                .Parameter("grant_type", "client_credential")
                .Parameter("appid", APP_ID)
                .Parameter("secret", APP_SECRET)
                .Get<WeiXinTicket>();
            this._AccessToken = ticket.Token;
            this._ExpiresIn = DateTime.Now.AddSeconds(ticket.Expire);
        }

        /// <summary>
        /// 获取或刷新Jsapi_Ticket
        /// </summary>
        private void Jsapi_InitOrRefresh()
        {
            using (var log = LogProvider.Instance().Group("jsapi"))
            {
                log.Write("this._AccessToken:" + this.AccessToken);

                var ticket = Http.CreateInstance()
                .Url("https://api.weixin.qq.com/cgi-bin/ticket/getticket")
                .Parameter("grant_type", "client_credential")
                .Parameter("access_token", this.AccessToken)
                .Parameter("type", "jsapi")
                .Get<WeiXinJsapi_Ticket>();
                this._Jsapi_Ticket = ticket.Token;
                this._Jsapi_ExpiresIn = DateTime.Now.AddSeconds(ticket.Expire);

                log.Write("ticket:" + Http.CreateInstance()
                .Url("https://api.weixin.qq.com/cgi-bin/ticket/getticket")
                .Parameter("grant_type", "client_credential")
                .Parameter("access_token", this.AccessToken)
                .Parameter("type", "jsapi")
                .Post());
            }
        }

        #region 单例模式
        private static WeiXinAccessTokenManager _Instance;
        public static WeiXinAccessTokenManager Instance { get { return _Instance; } }
        static WeiXinAccessTokenManager()
        {
            _Instance = new WeiXinAccessTokenManager();
        }
        #endregion
    }
}
