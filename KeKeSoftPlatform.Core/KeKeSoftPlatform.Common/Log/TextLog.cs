using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Threading;

namespace KeKeSoftPlatform.Common
{
    public class LT_Log
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Group { get; set; }
    }

    public class TextLog : LogProvider
    {
        public static object o = new object();

        private static Dictionary<string, List<LT_Log>> _DataBuffer;
        private static Dictionary<string, List<LT_Log>> _DataPersistent;

        static TextLog()
        {
            _DataBuffer = new Dictionary<string, List<LT_Log>>();
            _DataPersistent = new Dictionary<string, List<LT_Log>>();

            Run();
        }

        static void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    lock (o)
                    {
                        _DataPersistent = _DataBuffer;
                        if (_DataPersistent.Count > 0)
                        {
                            _DataBuffer = new Dictionary<string, List<LT_Log>>();
                        }
                    }
                    if (_DataPersistent.Count > 0)
                    {
                        foreach (var item in _DataPersistent)
                        {
                            var path = PF.GetPath("log/{0}.txt".FormatString(item.Key));
                            if(Directory.Exists(Path.GetDirectoryName(path)) == false)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                            }
                            StreamWriter writer = null;
                            try
                            {
                                if (File.Exists(path))
                                {
                                    writer = File.AppendText(path);
                                }
                                else
                                {
                                    writer = File.CreateText(path);
                                }
                                writer.AutoFlush = true;

                                foreach (var log in item.Value)
                                {
                                    if (log.CreateDate.HasValue)
                                    {
                                        writer.WriteLine("{0}==================================================".FormatString(DateTime.Now.ToString()));
                                    }
                                    writer.WriteLine(log.Text);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                if (writer != null)
                                {
                                    writer.Close();
                                    writer.Dispose();
                                }
                            }
                        }
                        _DataPersistent.Clear();
                    }
                    if (_DataPersistent.Count <= 10)
                    {
                        Thread.Sleep(1000 * 5);
                    }
                }
            });
        }

        public override void Progress(int sum, int currentIndex)
        {
            Write("共{0}，当前{1}，剩余{2}".FormatString(sum.ToString().PadRight(10, ' '), currentIndex.ToString().PadRight(10, ' '), (sum - currentIndex).ToString().PadRight(10, ' ')), false);
        }

        public override void Write(string text, bool autoAppendDate = true)
        {
            if (this.Enable() == false)
            {
                return;
            }
            if (_DataBuffer.ContainsKey(this.Group()) == false)
            {
                _DataBuffer.Add(this.Group(), new List<LT_Log>());
            }

            _DataBuffer[this.Group()].Add(new LT_Log
            {
                CreateDate = DateTime.Now,
                Group = this.Group(),
                Text = text
            });
        }

        public override void Dispose()
        {
            if (this.Enable() == false)
            {
                return;
            }
        }
    }
}
