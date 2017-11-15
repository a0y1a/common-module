using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;

namespace KeKeSoftPlatform.Core
{
    public class StaticListProvider : IListItemProvider
    {
        private Dictionary<string, IEnumerable<ListItem>> listCollection;

        public IEnumerable<ListItem> GetListItemCollection(string listName)
        {
            if (listCollection.ContainsKey(listName))
            {
                return listCollection[listName];
            }
            else
            {
                throw new Exception("StaticListProvider不包含" + listName + "列表");
            }
        }

        public StaticListProvider()
        {
            this.listCollection = new Dictionary<string, IEnumerable<ListItem>>();
        }


        public bool HasList(string listName)
        {
            return listCollection.ContainsKey(listName);
        }
    }
}
