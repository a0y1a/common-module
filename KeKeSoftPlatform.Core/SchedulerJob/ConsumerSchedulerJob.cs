using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Core
{
    [SchedulerJob("消费者队列", true, 1000)]
    public class ConsumerSchedulerJob : ISchedulerJob
    {
        public void Execute()
        {
            var job = QueueSchedulerJobManager.Dequeue();
            if (job != null)
            {
                job.Run();
            }
        }
    }
}
