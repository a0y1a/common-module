using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Core
{
    public abstract class QueueSchedulerJob : ISchedulerJob
    {
        public void Execute()
        {
            QueueSchedulerJobManager.Enqueue(this);
        }
        public abstract void Run();
    }
}
