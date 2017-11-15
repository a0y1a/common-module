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
    public class RealTimeTextLog : LogProvider
    {
        private void WriteToFile(string text)
        {
            StreamWriter writer = null;
            try
            {
                var path = PF.GetPath("log/{0}.txt".FormatString(base.Group()));
                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                if (File.Exists(path))
                {
                    writer = File.AppendText(path);
                }
                else
                {
                    writer = File.CreateText(path);
                }
                writer.AutoFlush = true;
                writer.WriteLine(text);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                writer.Close();
                writer.Dispose();
            }

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
            if (autoAppendDate)
            {
                this.WriteToFile(DateTime.Now.ToString());
            }
            this.WriteToFile(text);
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
