using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KeKeSoftPlatform.Core
{
    public class SchedulerJobThread : IDisposable
    {
        public int Interval { get; set; }
        public ISchedulerJob Job { get; set; }
        private Timer _timer;
        private bool _disposed;

        public SchedulerJobThread(ISchedulerJob job, int interval)
        {
            this.Interval = interval;
            this.Job = job;
        }

        public void InitTimer()
        {
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.TimerHandler), null, this.Interval, this.Interval);
            }
        }

        private void TimerHandler(object state)
        {
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.Job.Execute();
            this._timer.Change(this.Interval, this.Interval);
        }

        public void Dispose()
        {
            if ((this._timer != null) && !this._disposed)
            {
                lock (this)
                {
                    this._timer.Dispose();
                    this._timer = null;
                    this._disposed = true;
                }
            }
        }
    }
}
