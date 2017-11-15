using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.WebPages;
using Newtonsoft.Json;

namespace KeKeSoftPlatform.WebExtension
{
    public class Pager
    {
        public const string PAGE_NUM_KEY = "pageNum";
        public const int DEFAULT_PAGE_SIZE = 20;

        /// <summary>
        /// 数据总条数
        /// </summary>
        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }
        /// <summary>
        /// 每页多少条
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页页码
        /// </summary>
        [JsonProperty("pageNum")]
        public int PageNum { get; set; }
        /// <summary>
        /// 一共多少页
        /// </summary>
        [JsonProperty("pageCount")]
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)ItemCount / (double)PageSize);
            }
        }

        public Pager(int pageSize = Pager.DEFAULT_PAGE_SIZE)
        {
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 生成链接地址
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string GeneratePagerItem(HtmlHelper html, int pageNum)
        {
            return UrlBuilder.CreateInstance(html.ViewContext.RequestContext.HttpContext, html.ViewContext.RequestContext.RouteData)
                .Ignore(Pager.PAGE_NUM_KEY)
                .Attach(Pager.PAGE_NUM_KEY, pageNum, true)
                .Build();
        }
    }
    public class Pager<T> : Pager
    {
        /// <summary>
        /// 合计数据
        /// </summary>
        [JsonProperty("sumData")]
        public T SumData { get;set;}
        public Pager<T> SetSumData(T sumData)
        {
            this.SumData = sumData;
            return this;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }

    }
}
