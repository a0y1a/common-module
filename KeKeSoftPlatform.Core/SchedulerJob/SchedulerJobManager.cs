using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    public class SchedulerJobManager
    {
        private static bool _Initialized;
        static SchedulerJobManager()
        {
            _Initialized = false;
        }
        public static void StartService()
        {
            if (_Initialized == false)
            {
                Assembly.GetAssembly(typeof(SchedulerJobManager))
                    .GetTypes()
                    .Where(m => m.IsClass && typeof(ISchedulerJob).IsAssignableFrom(m))
                    .ToList()
                    .ForEach(m =>
                    {
                        var schedulerJobAttribute = m.GetCustomAttributes(typeof(SchedulerJobAttribute), false).FirstOrDefault();
                        if (schedulerJobAttribute != null && (schedulerJobAttribute as SchedulerJobAttribute).Enable)
                        {
                            SchedulerJobThread schedulerJobThead = new SchedulerJobThread(Activator.CreateInstance(m) as ISchedulerJob, (schedulerJobAttribute as SchedulerJobAttribute).Interval);
                            schedulerJobThead.InitTimer();
                        }
                    });
            }

        }
    }
}
