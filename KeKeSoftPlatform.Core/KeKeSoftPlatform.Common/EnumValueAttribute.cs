using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Common
{
    public class EnumValueAttribute:Attribute
    {
        private string _Value;
        public virtual string Value { get { return _Value; } }

        public EnumValueAttribute(string value)
        {
            _Value = value;
        }
    }
}
