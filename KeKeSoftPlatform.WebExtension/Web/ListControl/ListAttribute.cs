using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    public abstract class ListAttribute : Attribute, IMetadataAware
    {
        public string ListName { get; private set; }
        public ListAttribute(string listName)
        {
            this.ListName = listName;
        }
        
        public virtual void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add(ListProviderBus.ListItemKey, this.ListName);
        }
    }
}
