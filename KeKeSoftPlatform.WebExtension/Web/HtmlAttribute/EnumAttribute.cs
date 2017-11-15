using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property)]
    public class EnumAttribute : Attribute, IMetadataAware
    {
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.TemplateHint = "Enum";
        }
    }
}
