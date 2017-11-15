using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    #region 推广二维码
    public enum QRCodeActionName
    {
        [EnumValue("QR_SCENE")]
        QR_SCENE,
        [EnumValue("QR_LIMIT_SCENE")]
        QR_LIMIT_SCENE,
        [EnumValue("QR_LIMIT_STR_SCENE")]
        QR_LIMIT_STR_SCENE
    }
    public class CreateQRCodeData
    {
        public class QRCodeActoinInfo
        {
            public class QRCodeSence
            {
                [JsonProperty("scene_id")]
                public int Id { get; set; }

                [JsonProperty("scene_str")]
                public string Str { get; set; }
            }
            [JsonProperty("scene")]
            public QRCodeSence Scene { get; set; }
        }

        [JsonProperty("expire_seconds")]
        public int? ExpireSeconds { get; set; }

        [JsonProperty("action_name")]
        [JsonConverter(typeof(EnumConverter))]
        public QRCodeActionName ActionName { get; set; }

        [JsonProperty("action_info")]
        public QRCodeActoinInfo ActoinInfo { get; set; }
    }

    public class QRCodeContainer
    {
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("expire_seconds")]
        public int ExpireSeconds { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    #endregion

    #region 微信菜单
    public class WeiXinMenu
    {
        [JsonProperty("button")]
        public List<WeiXinMenuButton> Button { get; set; }

        public WeiXinMenu()
        {
            this.Button = new List<WeiXinMenuButton>();
        }

        public WeiXinMenu Add(WeiXinMenuButton button)
        {
            this.Button.Add(button);
            return this;
        }
    }

    public enum WeiXinButtonType
    {
        [EnumValue("click")]
        Click,
        [EnumValue("view")]
        View,
        [EnumValue("scancode_push")]
        Scancode_push,
        [EnumValue("scancode_waitmsg")]
        Scancode_waitmsg,
        [EnumValue("pic_sysphoto")]
        Pic_sysphoto,
        [EnumValue("pic_photo_or_album")]
        Pic_photo_or_album,
        [EnumValue("pic_weixin")]
        Pic_weixin,
        [EnumValue("location_select")]
        Location_select,
        [EnumValue("media_id")]
        Media_id,
        [EnumValue("view_limited")]
        View_limited
    }
    public class WeiXinMenuButton
    {
        [JsonProperty("sub_button")]
        public List<WeiXinMenuButton> SubButton { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConverter))]
        private WeiXinButtonType _Type;

        [JsonProperty("name")]
        private string _Name;

        [JsonProperty("key")]
        private string _Key;

        [JsonProperty("url")]
        private string _Url;

        [JsonProperty("media_id ")]
        private string _MediaId;

        public WeiXinMenuButton()
        {
            this.SubButton = new List<WeiXinMenuButton>();
        }

        public WeiXinMenuButton Type(WeiXinButtonType type)
        {
            this._Type = type;
            return this;
        }

        public WeiXinMenuButton Name(string name)
        {
            this._Name = name;
            return this;
        }

        public WeiXinMenuButton Key(string key)
        {
            this._Key = key;
            return this;
        }

        public WeiXinMenuButton Url(string url)
        {
            this._Url = url;
            return this;
        }

        public WeiXinMenuButton MediaId(string mediaId)
        {
            this._MediaId = mediaId;
            return this;
        }

        public WeiXinMenuButton Sub(WeiXinMenuButton sub)
        {
            this.SubButton.Add(sub);
            return this;
        }
    }
    #endregion

    #region 网页授权
    public class OAuthTicketContainer
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("openid")]
        public string OpenId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("unionid")]
        public string UnionId { get; set; }
    }
    #endregion

    #region 用户信息
    public enum WeiXinUserSex
    {
        None,
        Male,
        Female
    }
    public class WeiXinUserInfo
    {
        [JsonProperty("subscribe")]
        public bool Subscribe { get; set; }

        [JsonProperty("openid")]
        public string OpenId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("sex")]
        public WeiXinUserSex Sex { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("headimgurl")]
        public string HeadimgUrl { get; set; }

        [JsonProperty("subscribe_time")]
        public int SubscribeTime { get; set; }

        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("groupid")]
        public string GroupId { get; set; }

    }
    #endregion

    #region jsapi-ticket
    public class JSAPI_Ticket
    {
        [JsonProperty("errcode")]
        public string ErrCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime Expire { get { return this.CreateDate.AddSeconds(this.ExpiresIn); } }

        public JSAPI_Ticket()
        {
            this.CreateDate = DateTime.Now;
        }
    }

    public class JSAPI_Config
    {
        public long Timestamp { get; set; }
        public string NonceStr { get; set; }
        public string Signature { get; set; }
    }
    #endregion

    public class WeiXinUserInfoData
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string WeiXinName { get; set; }
    }

    public class WeiXinUserPwdData
    {
        public string Pwd { get; set; }
        public string Confirm { get; set; }
    }
}