using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public class GridColumn<T>
    {
        private Grid<T> gridInstance;


        private Func<T, object> _CurrentCellValueCalculator;

        public GridColumn(string name, Func<T, object> valueCalculator, Grid<T> gridInstance)
        {
            _CurrentCellValueCalculator = valueCalculator;
            this.gridInstance = gridInstance;
            _ColumnValueCalculator = _CurrentCellValueCalculator;
            _ColumnName = name;
            _Alias = name;
        }

        #region  列名
        private string _ColumnName;
        public string ColumnName { get { return _ColumnName; } }
        public GridColumn<T> Name(string name)
        {
            this._ColumnName = name;
            this._Alias = name;
            return this;
        }
        #endregion

        #region 别名
        private string _Alias;
        public string Alias()
        {
            return this._Alias;
        }
        public GridColumn<T> Alias(string alias)
        {
            this._Alias = alias;
            return this;
        }

        #endregion

        public GridColumn<T> Name(Func<object, object> nameCalculator)
        {
            this._ColumnName = nameCalculator(null).ToString();
            this._Alias = this._ColumnName;
            return this;
        }

        private object _HtmlAttributes;
        public object HtmlAttributes { get { return _HtmlAttributes; } }
        public GridColumn<T> Attributes(object htmlAttributes)
        {
            this._HtmlAttributes = htmlAttributes;
            return this;
        }



        private Func<T, object> _ColumnValueCalculator;
        public Func<T, object> ColumnValueCalculator { get { return _ColumnValueCalculator; } }
        public GridColumn<T> Value(Func<T, object> calculator)
        {
            _ColumnValueCalculator = calculator;
            return this;
        }

        public GridColumn<T> Value(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("enumType参数必须为枚举类型");
            }
            var items = Enum.GetValues(enumType)
                         .Cast<Enum>()
                         .Select(m =>
                         {
                             string enumVal = Convert.ToInt32(m).ToString();
                             return new ListItem()
                             {
                                 Text = m.EnumMetadataDisplay(),
                                 Value = enumVal
                             };
                         });
            _ColumnValueCalculator = m =>
            {
                var item = items.FirstOrDefault(n => n.Value == Convert.ToInt32(_CurrentCellValueCalculator(m)).ToString());
                if (item != null)
                {
                    return item.Text;
                }
                else
                {
                    return "";
                }
            };
            return this;
        }

        public GridColumn<T> Value(object trueValue, object falseValue)
        {
            _ColumnValueCalculator = m =>
            {
                if (Convert.ToBoolean(_CurrentCellValueCalculator(m)))
                {
                    return trueValue;
                }
                else
                {
                    return falseValue;
                }
            };

            return this;
        }

        public Grid<T> End()
        {
            if (this.gridInstance.Columns.Where(m => m.Alias() == this.Alias()).Count() > 1)
            {
                throw new Exception("别名出现重复：" + this.Alias());
            }
            return this.gridInstance;
        }
    }
}
