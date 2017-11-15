using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace KeKeSoftPlatform.WebExtension
{
    public class Http
    {
        public static Http CreateInstance()
        {
            return new Http();
        }

        public Http()
        {
            this.parameters = new Dictionary<string, string>();
            this.encoding = System.Text.Encoding.UTF8;
        }

        private string url;
        private Encoding encoding;
        private Dictionary<string, string> parameters;
        private string body;

        public Http Url(string url)
        {
            this.url = url;
            return this;
        }

        public Http Encoding(Encoding encoding)
        {
            this.encoding = encoding;
            return this;
        }

        public Http Parameter(string key, string value)
        {
            this.parameters.Add(key, value);
            return this;
        }

        public Http Body(string body)
        {
            this.body = body;
            return this;
        }

        public string Post()
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            string postData = string.Empty ;
            if (string.IsNullOrWhiteSpace(body) == false)
            {
                postData += body;
            }
            // 将数据项转变成 name1=value1&name2=value2 的形式
            if (this.parameters != null && this.parameters.Count > 0)
            {
                postData = string.Join("&",
                        (from kvp in this.parameters
                         let item = kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)
                         select item
                         ).ToArray()
                     );
            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=" + encoding.WebName;

            if (string.IsNullOrWhiteSpace(postData))
            {
                throw new Exception("post请求内容不能为空");
            }
            byte[] buffer = encoding.GetBytes(postData);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public T Post<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Post());
        }

        public string Get()
        { 
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            string postData = string.Empty;
            // 将数据项转变成 name1=value1&name2=value2 的形式
            if (this.parameters != null && this.parameters.Count > 0)
            {
                postData = string.Join("&",
                        (from kvp in this.parameters
                         let item = kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)
                         select item
                         ).ToArray()
                     );
            }

            if (string.IsNullOrWhiteSpace(postData) == false)
            {
                if (url.Contains("?"))
                {
                    url += "&" + postData;
                }
                else
                {
                    url += "?" + postData;
                }
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded; charset=" + encoding.WebName;
            
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public T Get<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Get());
        }
    }
}
