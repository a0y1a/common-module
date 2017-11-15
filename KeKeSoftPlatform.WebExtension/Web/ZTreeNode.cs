using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace KeKeSoftPlatform.WebExtension
{
    public class ZTreeNode<T>
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userData")]
        public T UserData { get; set; }

        [JsonProperty("children")]
        public List<ZTreeNode<T>> Children { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        public ZTreeNode()
        {
            this.Children =new List<ZTreeNode<T>>();
        }
    }
}
