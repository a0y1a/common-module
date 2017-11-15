using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace KeKeSoftPlatform.WebExtension
{
    public class DropDownButtonGroup
    {
        public enum ItemType
        {
            Element,
            Divider,
            SubGroupHeader
        }

        public class Item
        {
            public ItemType Type { get; set; }
            public MvcHtmlString Content { get; set; }
        }

        private string _Title;
        public DropDownButtonGroup Title(string title)
        {
            this._Title = title;
            return this;
        }

        private string _Size;
        /// <summary>
        /// 引用ButtonSize中的常量，例如ButtonSize.Small
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public DropDownButtonGroup Size(string size)
        {
            this._Size = size;
            return this;
        }

        private List<Item> _Items;
        public DropDownButtonGroup Element(MvcHtmlString item)
        {
            var li = new TagBuilder("li");
            li.InnerHtml = item.ToHtmlString();
            if (string.IsNullOrWhiteSpace(li.InnerHtml))
            {
                return this;
            }
            _Items.Add(new Item
            {
                Type = ItemType.Element,
                Content = MvcHtmlString.Create(li.ToString())
            });
            return this;
        }

        public DropDownButtonGroup Element(Func<object, HelperResult> item)
        {
            var li = new TagBuilder("li");
            li.InnerHtml = item(null).ToHtmlString();
            if (string.IsNullOrWhiteSpace(li.InnerHtml))
            {
                return this;
            }
            _Items.Add(new Item
            {
                Type = ItemType.Element,
                Content = MvcHtmlString.Create(li.ToString())
            });
            return this;
        }

        public DropDownButtonGroup Divider()
        {
            var li = new TagBuilder("li");
            li.AddCssClass("divider");
            _Items.Add(new Item
            {
                Content = MvcHtmlString.Create(li.ToString()),
                Type = ItemType.Divider
            });
            return this;
        }

        public DropDownButtonGroup SubGroupHeader(string header)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("dropdown-header");
            li.SetInnerText(header);
            _Items.Add(new Item
            {
                Content = MvcHtmlString.Create(li.ToString()),
                Type = ItemType.SubGroupHeader
            });
            return this;
        }

        public MvcHtmlString Render()
        {
            var group = new TagBuilder("div");
            group.AddCssClass("btn-group");

            var button = new TagBuilder("button");
            button.AddCssClass(string.Format("btn dropdown-toggle {0} btn-default",this._Size));
            
            button.Attributes["type"] = "button";
            button.Attributes["data-toggle"] = "dropdown";
            button.InnerHtml += this._Title + " <span class=\"caret\"></span>";

            var itemBox = new TagBuilder("ul");
            itemBox.AddCssClass("dropdown-menu pull-right");
            itemBox.Attributes["role"] = "menu";

            Item lastItem = null;
            foreach (var item in this._Items)
            {
                if (string.IsNullOrWhiteSpace(item.Content.ToHtmlString()))
                {
                    continue;
                }
                if (item.Type == ItemType.Divider && (lastItem == null || lastItem.Type == ItemType.Divider))
                {
                    continue;
                }
                itemBox.InnerHtml += item.Content.ToHtmlString();
                lastItem = item;
            }

            group.InnerHtml += button.ToString() + itemBox.ToString();
            return new MvcHtmlString(group.ToString());
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DropDownButtonGroup()
        {
            this._Title = "更多";
            this._Items = new List<Item>();
        }

        public static DropDownButtonGroup Create()
        {
            return new DropDownButtonGroup();
        }
    }
}
