using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.Core
{
    [AttributeUsage(AttributeTargets.Class ,AllowMultiple = false)]
    public class AreaNameAttribute:Attribute
    {
        public string AreaName { get;private set; }

        public AreaNameAttribute(string areaName)
        {
            this.AreaName = areaName;
        }
    }
}
