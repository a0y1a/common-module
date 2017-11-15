using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace KeKeSoftPlatform.WebExtension
{
    public class Grid
    {
        private Grid()
        {

        }

        public static Grid<T> Create<T>()
        {
            return new Grid<T>();
        }
    }

    public class Grid<T>
    {
        public const string KEY = "Id";

        public Grid()
        {
            Columns = new List<GridColumn<T>>();
        }

        private object _HtmlAttributes;
        private bool _ReplaceExisting;
        public Grid<T> Attributes(object htmlAttributes, bool replaceExisting = false)
        {
            this._HtmlAttributes = htmlAttributes;
            this._ReplaceExisting = replaceExisting;
            return this;
        }

        #region 数据源
        private IEnumerable<T> _DataSource;
        public Grid<T> SetDataSource(IEnumerable<T> dataSource)
        {
            this._DataSource = dataSource;
            return this;
        }
        #endregion

        #region 合计数据源
        private T _SumData;
        public Grid<T> Sum(T sumData)
        {
            this._SumData = sumData;
            return this;
        }
        #endregion

        public List<GridColumn<T>> Columns { get; private set; }
        

        #region 行主键
        private Func<T, object> _KeySelector;
        public Func<T, object> KeySelector { get { return _KeySelector; } }
        public Grid<T> Key(Func<T, object> keySelector)
        {
            this._KeySelector = keySelector;
            return this;
        }
        #endregion

        #region 行数据
        private Func<T, object> _RowHiddenDataSelector;
        public Func<T, object> RowHiddenDataSelector { get { return _KeySelector; } }

        public Grid<T> RowHiddenData(Func<T, object> selector)
        {
            this._RowHiddenDataSelector = selector;
            return this;
        }
        #endregion

        #region 行属性
        private Func<T, IDictionary<string, object>> _RowAttributes;
        public Grid<T> RowAttributes(Func<T,IDictionary<string,object>> attributes)
        {
            this._RowAttributes = attributes;
            return this;
        }

        public Grid<T> RowAttributes(Func<T, object> attributes)
        {
            this._RowAttributes = m =>
            {
                return new RouteValueDictionary(attributes(m));
            };
            return this;
        }
        #endregion

        /// <summary>
        /// 生成DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ToTable()
        {
            var data = new DataTable();
            foreach (var item in Columns)
            {
                data.Columns.Add(item.ColumnName);
            }
            if (this._DataSource != null && this._DataSource.Any())
            {
                for (int i = 0; i < _DataSource.Count(); i++)
                {
                    var row = data.NewRow();
                    for (int j = 0; j < Columns.Count; j++)
                    {
                        var value = Columns[j].ColumnValueCalculator(_DataSource.ElementAt(i));
                        row[j] = value == null ? "" : value.ToString();
                    }
                    data.Rows.Add(row);
                }
            }
            return data;
        }

        /// <summary>
        /// 输出html
        /// </summary>
        /// <returns></returns>
        public MvcHtmlString Render()
        {
            var table = new TagBuilder("table");
            table.MergeAttributes(new RouteValueDictionary(this._HtmlAttributes), this._ReplaceExisting);
            table.AddCssClass("table table-hover");
            var thead = new TagBuilder("thead");
            var trForTitle = new TagBuilder("tr");
            foreach (var column in this.Columns)
            {
                var th = new TagBuilder("th");
                th.MergeAttributes(new RouteValueDictionary(column.HtmlAttributes));
                th.InnerHtml+= column.ColumnName;
                trForTitle.InnerHtml += th;
            }

            thead.InnerHtml += trForTitle;
            table.InnerHtml += thead;

            var tbody = new TagBuilder("tbody");
            if (this._DataSource != null && this._DataSource.Any())
            {
                foreach (var item in this._DataSource)
                {
                    var trForData = new TagBuilder("tr");
                    if (_KeySelector != null)
                    {
                        var key = this._KeySelector(item);
                        if (key == null)
                        {
                            throw new Exception("没有找到键");
                        }
                        trForData.Attributes.Add("data-id", key.ToString());
                    }
                    if (this._RowHiddenDataSelector != null)
                    {
                        var rowHiddenData = this._RowHiddenDataSelector(item);
                        trForData.Attributes.Add("data-val", Newtonsoft.Json.JsonConvert.SerializeObject(rowHiddenData));
                    }
                    if (this._RowAttributes != null)
                    {
                        trForData.MergeAttributes(this._RowAttributes(item));
                    }
                    foreach (var column in this.Columns)
                    {
                        var td = new TagBuilder("td");
                        td.MergeAttributes(new RouteValueDictionary(column.HtmlAttributes));
                        td.InnerHtml += column.ColumnValueCalculator(item);
                        trForData.InnerHtml += td;
                    }
                    tbody.InnerHtml += trForData;
                }
            }

            #region 合计
            if (this._SumData != null)
            {
                var trSum = new TagBuilder("tr");
                foreach (var column in this.Columns)
                {
                    var td = new TagBuilder("td");
                    if (this.Columns.IndexOf(column) == 0)
                    {
                        td.SetInnerText("合计");
                    }
                    else
                    {
                        td.MergeAttributes(new RouteValueDictionary(column.HtmlAttributes));
                        try
                        {
                            td.InnerHtml += column.ColumnValueCalculator(this._SumData);
                        }
                        catch (Exception)
                        { }
                    }
                    trSum.InnerHtml += td;
                }
                tbody.InnerHtml += trSum;
            }
            #endregion
            table.InnerHtml += tbody;
            return new MvcHtmlString(table.ToString());
        }

        public GridColumn<T> Column(Expression<Func<T, object>> selector, Func<T, object> valueCalculator = null, object htmlAttributes = null)
        {
            var propertyeName = "";
            if (selector.Body is MemberExpression)
            {
                propertyeName = ((selector.Body as MemberExpression).Member as PropertyInfo).Name;
            }
            else if (selector.Body is UnaryExpression)
            {
                propertyeName = (((selector.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo).Name;
            }
            var column = new GridColumn<T>(propertyeName, selector.Compile(), this).Attributes(htmlAttributes);
            Columns.Add(column);
            return column;
        }

        public GridColumn<T> Column(string columnName, Func<T, object> valueCalculator, object htmlAttributes = null)
        {
            var column = new GridColumn<T>(columnName, valueCalculator, this).Name(columnName).Attributes(htmlAttributes);
            this.Columns.Add(column);
            return column;
        }

        public GridColumn<T> ActionColumn(params Func<T, object>[] valueCalculator)
        {
            var column = new GridColumn<T>("操作", m =>
            {
                var contextBox = new TagBuilder("div");
                contextBox.AddCssClass("grid-action");
                if (valueCalculator != null && valueCalculator.Any())
                    foreach (var calculator in valueCalculator)
                    {
                        contextBox.InnerHtml += calculator(m).ToString();
                    }
                return contextBox;
            }, this).Attributes(new { @class = "grid-action-td" });
            this.Columns.Add(column);
            column.Alias("操作");
            return column;
        }

    }
}
