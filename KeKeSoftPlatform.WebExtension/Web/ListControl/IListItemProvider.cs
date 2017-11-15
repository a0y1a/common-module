using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public interface IListItemProvider
    {
        IEnumerable<ListItem> GetListItemCollection(string listName);
        bool HasList(string listName);
    }
}
