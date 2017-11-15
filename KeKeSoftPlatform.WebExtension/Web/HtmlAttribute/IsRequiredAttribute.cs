using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public class IsRequiredAttribute:Attribute
    {
        public bool IsRequired { get; set; }
        public IsRequiredAttribute(bool isRequired=true)
        {
            IsRequired = isRequired;
        }
    }
}
