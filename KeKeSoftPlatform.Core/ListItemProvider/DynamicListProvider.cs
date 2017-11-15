using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;
using System.Data.Entity;

namespace KeKeSoftPlatform.Core
{
    public class DynamicListProvider : IListItemProvider
    {
        private Dictionary<string, Func<IEnumerable<ListItem>>> dynamicListCollection;

        public IEnumerable<ListItem> GetListItemCollection(string listName)
        {
            if (dynamicListCollection.ContainsKey(listName))
            {
                return dynamicListCollection[listName]();
            }
            else
            {
                throw new Exception("DynamicListProvider不包含" + listName + "列表");
            }
        }

        public DynamicListProvider()
        {
            dynamicListCollection = new Dictionary<string, Func<IEnumerable<ListItem>>>();

            this.dynamicListCollection.Add("ParentModule", () =>
            {
                using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                    return db.Module.Where(m => m.ParentId.HasValue == false).OrderBy(m => m.Sequence).Select(m => new ListItem { Text = m.Name, Value = m.Id.ToString() }).ToList();
                }
            });
        }


        public bool HasList(string listName)
        {
            return dynamicListCollection.ContainsKey(listName);
        }
    }
}
