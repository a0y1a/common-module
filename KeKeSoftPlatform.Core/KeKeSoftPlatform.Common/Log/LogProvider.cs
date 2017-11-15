using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Common
{
    public abstract class LogProvider : IDisposable
    {
        public LogProvider()
        {
            this._Enable = true;
        }
        public static LogProvider Instance<T>() where T : LogProvider, new()
        {
            return default(T);
        }

        private static Func<LogProvider> _Builder;
        public static void RegisterDefalutProvider(Func<LogProvider> builder)
        {
            _Builder = builder;
        }

        public static LogProvider Instance()
        {
            if (_Builder == null)
            {
                throw new Exception("构造器尚未注册");
            }

            return _Builder();
        }

        private bool _Enable;
        public LogProvider Enable(bool enable)
        {
            this._Enable = enable;
            return this;
        }

        public bool Enable()
        {
            return this._Enable;
        }

        private string _GroupName;
        public virtual LogProvider Group(string groupName)
        {
            this._GroupName = groupName;
            return this;
        }

        public string Group()
        {
            if (string.IsNullOrWhiteSpace(this._GroupName))
            {
                throw new Exception("未设置Group");
            }
            return this._GroupName;
        }

        public virtual LogProvider AutoGroup(string prevFix = null)
        {
            this._GroupName = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(prevFix) == false)
            {
                this._GroupName = prevFix + this._GroupName;
            }
            return this;
        }

        public virtual LogProvider Group(Func<string> groupNameCalc)
        {
            this._GroupName = groupNameCalc();
            return this;
        }

        public abstract void Progress(int sum, int currentIndex);

        public abstract void Write(string text, bool autoAppendDate = true);

        public abstract void Dispose();
    }
}
