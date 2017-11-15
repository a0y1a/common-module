using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KeKeSoftPlatform.Common;
using System.Web.SessionState;

namespace KeKeSoftPlatform.Core
{
    public class ProgressReportData
    {
        public int Speed { get; set; }
        public string Text { get; set;}
    }
    public class ProgressCache
    {
        private Dictionary<string, Dictionary<int, int>> _Data;
        private Dictionary<string, Dictionary<int, ProgressReportData>> _DataText;
        public ProgressCache()
        {
            this._Data = new Dictionary<string, Dictionary<int, int>>();
            this._DataText = new Dictionary<string, Dictionary<int, ProgressReportData>>();
        }

        public void Register(string userLoginTicketId, int itemId, int index)
        {
            if (_Data.ContainsKey(userLoginTicketId) == false)
            {
                _Data.Add(userLoginTicketId, new Dictionary<int, int>());
            }

            if (_Data[userLoginTicketId].ContainsKey(itemId) == false)
            {
                _Data[userLoginTicketId].Add(itemId, index);
            }
            else
            {
                _Data[userLoginTicketId][itemId] = index;
            }
        }

        public void RegisterText(string userLoginTicketId, int itemId, ProgressReportData item)
        {
            if (_DataText.ContainsKey(userLoginTicketId) == false)
            {
                _DataText.Add(userLoginTicketId, new Dictionary<int, ProgressReportData>());
            }

            if (_DataText[userLoginTicketId].ContainsKey(itemId) == false)
            {
                _DataText[userLoginTicketId].Add(itemId, item);
            }
            else
            {
                _DataText[userLoginTicketId][itemId] = item;
            }
        }

        public int Report(string userLoginTicketId, int itemId)
        {
            if (_Data.ContainsKey(userLoginTicketId) == false || _Data[userLoginTicketId].ContainsKey(itemId) == false)
            {
                return 0;
            }
            return _Data[userLoginTicketId][itemId];

        }

        public ProgressReportData ReportText(string userLoginTicketId, int itemId)
        {
            if (_DataText.ContainsKey(userLoginTicketId) == false || _DataText[userLoginTicketId].ContainsKey(itemId) == false)
            {
                return new ProgressReportData { Speed=0,Text=""};
            }
            return _DataText[userLoginTicketId][itemId];

        }

        public void Remove(string userLoginTicketId, int itemId)
        {
            if (_Data.ContainsKey(userLoginTicketId) == true && _Data[userLoginTicketId].ContainsKey(itemId) == true)
            {
                _Data[userLoginTicketId].Remove(itemId);
            }
        }

        public void RemoveText(string userLoginTicketId, int itemId)
        {
            if (_DataText.ContainsKey(userLoginTicketId) == true && _DataText[userLoginTicketId].ContainsKey(itemId) == true)
            {
                _DataText[userLoginTicketId].Remove(itemId);
            }
        }


        private static object o = new object();
        private static ProgressCache _Instance;
        public static ProgressCache Instance
        {
            get
            {
                lock (o)
                {
                    if (_Instance == null)
                    {
                        _Instance = new ProgressCache();
                    }

                    return _Instance;
                }
            }
        }
    }


    public class ProgressManager
    {
        public class Progress
        {
            public int Id { get; private set; }
            public const string CURRENT_INDEX = "__Progress__CurrentIndex";

            public Progress(int id)
            {
                this.Id = id;

                ProgressManager.Register(this);
            }


            public int Report(string userLoginTicketId)
            {
                return ProgressCache.Instance.Report(userLoginTicketId, this.Id);
            }

            public ProgressReportData ReportText(string userLoginTicketId)
            {
                return ProgressCache.Instance.ReportText(userLoginTicketId, this.Id);
            }

            public Progress Register(string userLoginTicketId, int currentIndex, int total)
            {
                ProgressCache.Instance.Register(userLoginTicketId, this.Id, currentIndex * 100 / total);
                return this;
            }

            public Progress RegisterText(string userLoginTicketId, int currentIndex, int total,string message)
            {
                ProgressCache.Instance.RegisterText(userLoginTicketId, this.Id, new ProgressReportData {Speed= currentIndex * 100 / total,Text= message } );
                return this;
            }
        }

        private static List<Progress> _Data = new List<Progress>();
        private static void Register(Progress item)
        {
            if (_Data.Any(m => m.Id == item.Id))
            {
                throw new Exception("进度条Id重复【{0}】".FormatString(item.Id));
            }
            _Data.Add(item);
        }

        public static Progress Find(int id)
        {
            var result = _Data.FirstOrDefault(m => m.Id == id);
            if (result == null)
            {
                throw new Exception("没有找到该进度条");
            }
            return result;
        }

        public static Progress UserImportTemplate = new Progress(1);
    }
}
