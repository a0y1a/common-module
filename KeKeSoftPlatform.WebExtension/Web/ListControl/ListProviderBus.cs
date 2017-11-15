using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public static class ListProviderBus
    {
        public static readonly string ListItemKey = "zmz.list_item_key";

        private static List<IListItemProvider> providerCollection;
        public static List<IListItemProvider> GetProviderCollection()
        {
            if (providerCollection == null)
            {
                throw new Exception("尚未初始化ListItemProviderBus.providerCollection");
            }
            else
            {
                return providerCollection;
            }
        }

        public static IEnumerable<ListItem> GetListItemCollection(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName))
            {
                throw new ArgumentNullException("列表名称不能为空");
            }
            if (providerCollection == null)
            {
                throw new Exception("尚未初始化ListItemProviderBus.providerCollection");
            }
            else
            {
                foreach (var provider in providerCollection)
                {
                    if (provider.HasList(listName))
                    {
                        return provider.GetListItemCollection(listName);
                    }
                }
                throw new Exception("列表【{0}】不存在".FormatString(listName));
            }
        }

        public static void Initialization(IListItemProvider defaultProvider, params IListItemProvider[] provider)
        {            
            if (defaultProvider == null)
            {
                throw new ArgumentNullException("defaultProvider");
            }
            if (providerCollection != null)
            {
                throw new Exception("ListItemProviderBus.providerCollection已经初始化，不能重复初始化");
            }
            else
            {
                providerCollection = new List<IListItemProvider>();
                providerCollection.Add(defaultProvider);
                if (provider != null)
                {
                    foreach (var item in provider)
                    {
                        providerCollection.Add(item);
                    }
                }
            }
        }
    }
}
