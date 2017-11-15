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

            dynamicListCollection.Add("SelectUser", () =>
            {
                using(KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
                {
                    List<ListItem> userList = new List<ListItem>() { new ListItem { Text = "==全部==", Value = "" } };
                    userList.AddRange(db.User.ToList().Select(m => new ListItem { Text = m.Number + " —— " + m.Name, Value = m.Number.ToString() }));
                    return userList;
                }
            });
        }


        public bool HasList(string listName)
        {
            return dynamicListCollection.ContainsKey(listName);
        }
    }
}
