using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeKeSoftPlatform.Db
{
    internal static class GroupMemberUpdater
    {
        private static object _Signal = new object();

        //最小时间间隔，单位：分钟
        const int MIN_INTERVAL = 2;

        private static DateTime LastModifiedDate = DateTime.Now;
        private static bool Modified = false;

        public static void Notice()
        {
            lock (_Signal)
            {
                Modified = false;
                LastModifiedDate = DateTime.Now;
            }
        }

        static GroupMemberUpdater()
        {
            Run();
        }

        private static void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000 * 10);
                    lock (_Signal)
                    {
                        if (Modified == false)
                        {
                            return;
                        }

                        if ((DateTime.Now - LastModifiedDate).TotalMinutes <= MIN_INTERVAL)
                        {
                            return;
                        }
                        Modified = false;
                    }
                    
                    //更新工作组成员

                }
            });
        }
    }
}
