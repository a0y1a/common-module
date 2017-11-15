using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using KeKeSoftPlatform.Db;
using KeKeSoftPlatform.WebExtension;

namespace KeKeSoftPlatform.App.Areas.weixin.Controllers
{
    [Right(AllowAnonymous = true)]
    public class GatewayController : BaseController
    {
        public const string WX_LOG_NAME = "weixin";
        public ActionResult Handle()
        {
            using (LogProvider log = LogProvider.Instance().Group(WX_LOG_NAME))
            {
                using (var stream = Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    var postString = Encoding.UTF8.GetString(postBytes);
                    var xml = XElement.Parse(postString);
                    log.Write("事件：");
                    log.Write(postString, false);
                    try
                    {
                        if (xml.Element("MsgType").Value == "event" && xml.Element("Event").Value == "subscribe")
                        {
                            var fromUser = xml.Element("FromUserName").Value;
                            var toUser = xml.Element("ToUserName").Value;
                            var result_Img = @"<xml>
                                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                                <CreateTime>{2}</CreateTime>
                                                <MsgType><![CDATA[text]]></MsgType>
                                                <Content><![CDATA[{3}]]></Content>
                                               </xml>".FormatString(fromUser, toUser, DateTime.Now.Ticks.ToString(), "您好！");
                            return Content(result_Img);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Write(ex.ToString());
                    }
                    return Content(string.Empty);
                }
            }
        }

        #region 微信提交地址验证
        public ActionResult CheckSignature(string signature, string timestamp, string nonce, string echostr)
        {
            return Content(echostr);
        }
        #endregion

        #region 菜单管理
        public ActionResult CreateMenu()
        {
            WeiXinMenu menu = new WeiXinMenu().Add(new WeiXinMenuButton()
                                                        .Type(WeiXinButtonType.View)
                                                        .Name("商城")
                                                        .Url("http://{0}/weixin/mall/Index".FormatString(Service.WX_DOMAIN)))
                                              .Add(new WeiXinMenuButton()
                                                        .Type(WeiXinButtonType.View)
                                                        .Name("查询系统")
                                                        .Url("http://{0}/weixin/query/ProductList".FormatString(Service.WX_DOMAIN)));

            var result = Http.CreateInstance()
                .Url("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}".FormatString(WeiXinAccessTokenManager.Instance.AccessToken))
                .Body(JsonConvert.SerializeObject(menu))
                .Post();

            return Content(result);
        }
        #endregion

        #region 网页授权
        public ActionResult Login()
        {
            string returnUrl = "/weixin/mall/Index";
            return Redirect(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect"
            .FormatString(WeiXinAccessTokenManager.APP_ID, Url.Encode("http://{0}/weixin/Gateway/FetchToken".FormatString(Service.WX_DOMAIN)), Url.Encode(returnUrl)));
        }

        //public ActionResult FetchToken(string code, string state)
        //{
        //    using (LogProvider log = LogProvider.Instance().Group(WX_LOG_NAME))
        //    {
        //        try
        //        {
        //            log.Write("FetchToken");
        //            var result = Http.CreateInstance()
        //                            .Url(@"https://api.weixin.qq.com/sns/oauth2/access_token")
        //                            .Parameter("appid", WeiXinAccessTokenManager.APP_ID)
        //                            .Parameter("secret", WeiXinAccessTokenManager.APP_SECRET)
        //                            .Parameter("code", code)
        //                            .Parameter("grant_type", "authorization_code")
        //                            .Post<OAuthTicketContainer>();
        //            log.Write("OpenId：" + result.OpenId);
        //            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
        //            {
        //                T_User user = null;
        //                var model = new _User();
        //                if (db.User.Any(m => m.OpenId == result.OpenId))
        //                {
        //                    user = db.User.FirstOrDefault(m => m.OpenId == result.OpenId);
        //                    model.Id = user.Id;
        //                    model.OpenId = user.OpenId;
        //                    model.Name = user.Name;
        //                    model.LevelId = user.LevelId;
        //                    model.OwnIdentities = new List<Identity> { Identity.User };
        //                    FormsPrincipal<_User>.SignIn(user.Id.ToString(), model);
        //                }
        //                else
        //                {
        //                    var weixinInfo = this.GetWeiXinUserInfo(result.OpenId);
        //                    user = new T_User()
        //                    {
        //                        OpenId = result.OpenId,
        //                        Name = weixinInfo.NickName,
        //                        CreateDate = DateTime.Now
        //                    };
        //                    db.User.Add(user);
        //                    db.SaveChanges();
        //                    model.Id = user.Id;
        //                    model.OpenId = user.OpenId;
        //                    model.Name = user.Name;
        //                    model.LevelId = user.LevelId;
        //                    model.OwnIdentities = new List<Identity> { Identity.User };
        //                    FormsPrincipal<_User>.SignIn(user.Id.ToString(), model);
        //                }
        //            }
        //            return Redirect(state);
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Write(ex.ToString());
        //            throw ex;
        //        }
        //    }
        //}

        #endregion

        #region 获取OpenId

        public ActionResult GetWeiXinOpenId()
        {
            string returnUrl = "/weixin/WeiXinCustomer/BindCustomerPwd";
            return Redirect(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect"
            .FormatString(WeiXinAccessTokenManager.APP_ID, Url.Encode("http://{0}/weixin/CustomerGateway/GetWeiXinOpenIdFetchToken".FormatString(Service.WX_DOMAIN)), Url.Encode(returnUrl)));
        }

        public ActionResult GetWeiXinOpenIdFetchToken(string code, string state)
        {
            var result = Http.CreateInstance()
                            .Url(@"https://api.weixin.qq.com/sns/oauth2/access_token")
                            .Parameter("appid", WeiXinAccessTokenManager.APP_ID)
                            .Parameter("secret", WeiXinAccessTokenManager.APP_SECRET)
                            .Parameter("code", code)
                            .Parameter("grant_type", "authorization_code")
                            .Post<OAuthTicketContainer>();
            return Redirect(state + "?openId={0}&nickName={1}".FormatString(result.OpenId, ""));
        }


        #endregion

        #region 获取个人信息

        //public UserInfoListItem GetWeiXinUserInfo(string openId)
        //{
        //    var result = Http.CreateInstance()
        //                   .Url(@"https://api.weixin.qq.com/cgi-bin/user/info")
        //                   .Parameter("access_token", WeiXinAccessTokenManager.Instance.AccessToken)
        //                   .Parameter("openid", openId)
        //                   .Parameter("lang", "zh_CN")
        //                   .Get();

        //    if (result.Contains("errcode"))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoListItem>(result);
        //    }
        //}

        //public ActionResult UpdateAllUserNickName()
        //{
        //    using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
        //    {
        //        var user = db.User.ToList();
        //        foreach (var item in user)
        //        {
        //            var weixinInfo = this.GetWeiXinUserInfo(item.OpenId);
        //            item.Name = weixinInfo.NickName;
        //        }
        //        db.SaveChanges();
        //        return Content(user.Count + "");
        //    }
        //}

        #endregion


        public ActionResult T()
        {
            WeiXinService.Subscribe(XElement.Parse(@"<xml><ToUserName><![CDATA[gh_f6d4adf275f7]]></ToUserName>
                                                    <FromUserName><![CDATA[oruuIt6giXCa_hOH6zjXEzG6Qw3U]]></FromUserName>
                                                    <CreateTime>1440145588</CreateTime>
                                                    <MsgType><![CDATA[event]]></MsgType>
                                                    <Event><![CDATA[subscribe]]></Event>
                                                    <EventKey><![CDATA[]]></EventKey>
                                                    </xml>"));
            return Content("123");
        }

        public JsonResult JSAPI_Config(string url)
        {
            return Json(WeiXinService.JSAPI_Config(url), JsonRequestBehavior.AllowGet);
        }
    }
}