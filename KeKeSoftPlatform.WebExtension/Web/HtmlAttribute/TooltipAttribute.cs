using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public class TooltipAttribute:Attribute
    {
        private string _Text;
        public virtual string Text { get { return _Text; } }
        public TooltipAttribute(string text)
        {
            this._Text = text;
        }
    }

    
}
