using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Web;

using System.Data;
using System.IO;
using KeKeSoftPlatform.Common;
using System.Xml.Serialization;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;

namespace KeKeSoftPlatform.Core
{
    public class BridgeConfig
    {
      
        private static string Path(Guid bridgeId)
        {
            return PF.GetPath($"/app_data/{bridgeId}_config.xml");
        }

        private static Dictionary<Guid, BridgeConfig> _Collection = new Dictionary<Guid, BridgeConfig>();


        #region 预警参数
        [Display(Name = "绿色累计沉降")]
        public decimal GreenLeiJiChenJiang { get; set; }

        [Display(Name = "绿色沉降差值")]
        public decimal GreenChenJiangChaZhi { get; set; }

        [Display(Name = "绿色沉降速率")]
        public decimal GreenChenJiangSuLv { get; set; }

        [Display(Name = "黄色累计沉降上限值")]
        public decimal YellowLeiJiChenJiangShangXian { get; set; }

        [Display(Name = "黄色累计沉降下限值")]
        public decimal YellowLeiJiChenJiangXiaXian { get; set; }

        [Display(Name = "黄色沉降差值上限值")]
        public decimal YellowChenJiangChaZhiShangXian { get; set; }

        [Display(Name = "黄色沉降差值下限值")]
        public decimal YellowChenJiangChaZhiXiaXian { get; set; }

        [Display(Name = "黄色沉降速率")]
        public decimal YellowChenJiangSuLv { get; set; }

        [Display(Name = "红色累计沉降")]
        public decimal RedLeiJiChenJiang { get; set; }

        [Display(Name = "红色沉降差值")]
        public decimal RedChenJiangChaZhi { get; set; }

        [Display(Name = "红色沉降速率")]
        public decimal RedChenJiangSuLv { get; set; }
        #endregion

        #region 预警电话管理
        [Display(Name = "接收预错电话1")]
        public string JieShouYuCuoDianHua_1 { get; set; }

        [Display(Name = "接收预错电话2")]
        public string JieShouYuCuoDianHua_2 { get; set; }

        [Display(Name = "接收预错电话3")]
        public string JieShouYuCuoDianHua_3 { get; set; }

        [Display(Name = "接收预错电话4")]
        public string JieShouYuCuoDianHua_4 { get; set; }

        [Display(Name = "接收预错电话5")]
        public string JieShouYuCuoDianHua_5 { get; set; }

        [Display(Name = "黄色预警电话1")]
        public string YellowYuJingDianHua_1 { get; set; }

        [Display(Name = "黄色预警电话2")]
        public string YellowYuJingDianHua_2 { get; set; }

        [Display(Name = "黄色预警电话3")]
        public string YellowYuJingDianHua_3 { get; set; }

        [Display(Name = "黄色预警电话4")]
        public string YellowYuJingDianHua_4 { get; set; }

        [Display(Name = "黄色预警电话5")]
        public string YellowYuJingDianHua_5 { get; set; }

        [Display(Name = "红色预警电话1")]
        public string RedYuJingDianHua_1 { get; set; }

        [Display(Name = "红色预警电话2")]
        public string RedYuJingDianHua_2 { get; set; }

        [Display(Name = "红色预警电话3")]
        public string RedYuJingDianHua_3 { get; set; }

        [Display(Name = "红色预警电话4")]
        public string RedYuJingDianHua_4 { get; set; }

        [Display(Name = "红色预警电话5")]
        public string RedYuJingDianHua_5 { get; set; }
        #endregion

        public static BridgeConfig Instance(Guid bridgeId)
        {
            if (System.IO.File.Exists(Path(bridgeId)) == false)
            {
                var instance = new BridgeConfig()
                {

                };
                Serialize(instance, bridgeId);
                return instance;
            }
            if (_Collection.ContainsKey(bridgeId) == false)
            {
                var instance = XmlHelper.XmlDeserializeFromFile<BridgeConfig>(Path(bridgeId), Encoding.UTF8);
                return instance;
            }

            return _Collection[bridgeId];
        }

        public static void Serialize(BridgeConfig config, Guid bridgeId)
        {
            _Collection[bridgeId] = config;
            XmlHelper.XmlSerializeToFile(config, Path(bridgeId), Encoding.UTF8);
        }
    }
}
