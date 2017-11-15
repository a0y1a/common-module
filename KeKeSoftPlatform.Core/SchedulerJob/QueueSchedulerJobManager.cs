using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Core
{
    internal class QueueSchedulerJobManager
    {
        private static Queue<QueueSchedulerJob> _Queue;
        public static object o = new object();

        static QueueSchedulerJobManager()
        {
            _Queue = new Queue<QueueSchedulerJob>();
        }

        public static void Enqueue(QueueSchedulerJob job)
        {
            lock (o)
            {
                _Queue.Enqueue(job);
            }
        }

        public static QueueSchedulerJob Dequeue()
        {
            lock (o)
            {
                if (_Queue.Any())
                {
                    return _Queue.Dequeue();
                }
                return null;
            }
        }
    }
}
