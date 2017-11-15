using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Security.Cryptography;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.Core
{
    public class WeiXinService
    {
        public static QRCodeContainer CreateQRCodeContainer(CreateQRCodeData model)
        {
            return Http.CreateInstance()
                .Url("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}".FormatString(WeiXinAccessTokenManager.Instance.AccessToken))
                .Body(JsonConvert.SerializeObject(model))
                .Post<QRCodeContainer>();
        }
        public static ReturnValue Subscribe(XElement xml)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                //                var weiXinUserInfo = Http.CreateInstance()
                //                                            .Url(@"https://api.weixin.qq.com/cgi-bin/user/info")
                //                                            .Parameter("access_token", WeiXinAccessTokenManager.Instance.AccessToken)
                //                                            .Parameter("openid", xml.Element("FromUserName").Value)
                //                                            .Parameter("lang", "zh_CN")
                //                                            .Post<WeiXinUserInfo>();
                //                var replyMessage = @"<xml>
                //                                <ToUserName><![CDATA[{0}]]></ToUserName>
                //                                <FromUserName><![CDATA[{1}]]></FromUserName>
                //                                <CreateTime>{2}</CreateTime>
                //                                <MsgType><![CDATA[news]]></MsgType>
                //                                <ArticleCount>1</ArticleCount>
                //                                <Articles>
                //                                <item>
                //                                <Title><![CDATA[{3}]]></Title> 
                //                                <Description><![CDATA[{4}]]></Description>
                //                                <PicUrl><![CDATA[{5}]]></PicUrl>
                //                                <Url><![CDATA[{6}]]></Url>
                //                                </item>
                //                                </Articles>
                //                            </xml>".FormatString(weiXinUserInfo.OpenId, WeiXinAccessTokenManager.WX_CODE, DateTime.Now.ToFileTime(), Config.Instance.WeiXinHuiFu_Title, Config.Instance.WeiXinHuiFu_Content, "", "http://{0}/system/viphome".FormatString(Config.Instance.DomainHost));
                //                if (db.User.Any(m => m.OpenId == weiXinUserInfo.OpenId))
                //                {
                //                    return new ReturnValue
                //                    {
                //                        IsSuccess = false,
                //                        Data = replyMessage
                //                    };
                //                }
                //                T_User user = new T_User
                //                {
                //                    Id = PF.Key(),
                //                    Amount = 0m,
                //                    OpenId = weiXinUserInfo.OpenId,
                //                    CreateDate = DateTime.Now,
                //                    Name = weiXinUserInfo.Nickname,
                //                    Type = UserType.NewPeople,
                //                    Password = "123"
                //                };
                //                db.User.Add(user);
                //                db.SaveChanges();
                return new ReturnValue
                {
                    IsSuccess = true,
                    //Data = replyMessage
                };
            }
        }

        // <summary>
        /// 获取jsapi_ticket
        /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
        /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
        /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。
        ///本代码来自开源微信SDK项目：https://github.com/night-king/weixinSDK
        /// </summary>
        /// <param name="access_token">BasicAPI获取的access_token,也可以通过TokenHelper获取</param>
        /// <returns></returns>
        private static JSAPI_Ticket GetTickect(string access_token)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token);
            return Http.CreateInstance().Url(url).Get<JSAPI_Ticket>();
        }

        private static JSAPI_Ticket _JSAPI_Ticket;
        public static JSAPI_Ticket JSAPI_Ticket
        {
            get
            {
                if (_JSAPI_Ticket == null || _JSAPI_Ticket.Expire < DateTime.Now)
                {
                    _JSAPI_Ticket = GetTickect(WeiXinAccessTokenManager.Instance.AccessToken);
                }

                return _JSAPI_Ticket;
            }
        }


        public static JSAPI_Config JSAPI_Config(string url)
        {
            var jsapi_Config = new JSAPI_Config
            {
                NonceStr = CreatenNonce_str(),
                Timestamp = CreatenTimestamp()
            };
            var str = string.Empty;
            jsapi_Config.Signature = GetSignature(JSAPI_Ticket.Ticket, jsapi_Config.NonceStr, jsapi_Config.Timestamp, url, out str);
            return jsapi_Config;
        }

        private static string[] strs = new string[]
                                 {
                                  "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                  "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
                                 };
        /// <summary>
        /// 创建随机字符串
        ///本代码来自开源微信SDK项目：https://github.com/night-king/weixinSDK
        /// </summary>
        /// <returns></returns>
        public static string CreatenNonce_str()
        {
            Random r = new Random();
            var sb = new StringBuilder();
            var length = strs.Length;
            for (int i = 0; i < 15; i++)
            {
                sb.Append(strs[r.Next(length - 1)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 创建时间戳
        /// </summary>
        /// <returns></returns>
        public static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 签名算法
        ///本代码来自开源微信SDK项目：https://github.com/night-king/weixinSDK
        /// </summary>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        public static string GetSignature(string jsapi_ticket, string noncestr, long timestamp, string url, out string string1)
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            string1 = string1Builder.ToString();
            return Sha1(string1);
        }

        public static string Sha1(string orgStr, string encode = "UTF-8")
        {
            var sha1 = new SHA1Managed();
            var sha1bytes = System.Text.Encoding.GetEncoding(encode).GetBytes(orgStr);
            byte[] resultHash = sha1.ComputeHash(sha1bytes);
            string sha1String = BitConverter.ToString(resultHash).ToLower();
            sha1String = sha1String.Replace("-", "");
            return sha1String;
        }
    }
}
