using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchedulerJobAttribute : Attribute
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; }
        public SchedulerJobAttribute(string name, bool enable, int interval = 1000*10)
        {
            this.Enable = enable;
            this.Name = name;
            this.Interval = interval;
        }
    }
}
