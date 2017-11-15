using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace KeKeSoftPlatform.WebExtension
{
    class JsonModelBinder : IModelBinder
    {
        private string _ParameterKey;
        private FormMethod _FormMethod;
        public JsonModelBinder(string parameterKey,FormMethod method)
        {
            if (string.IsNullOrWhiteSpace(parameterKey))
            {
                throw new ArgumentNullException("parameterKey", "参数名称不能为空");
            }

            this._ParameterKey = parameterKey.Trim();
            this._FormMethod = method;
        }
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = string.Empty;
            switch (this._FormMethod)
            {
                case FormMethod.Get:
                    value = controllerContext.HttpContext.Request.QueryString[this._ParameterKey];
                    break;
                case FormMethod.Post:
                    value = controllerContext.HttpContext.Request.Form[this._ParameterKey];
                    break;
                default:
                    throw new Exception(string.Format("异常的FormMethod：{0}", this._FormMethod));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("数据为空，无法创建json数据");
            }
            return JsonConvert.DeserializeObject(value, bindingContext.ModelType);
        }
    }
    public class JsonModelBinderAttribute : CustomModelBinderAttribute
    {
        public string ParameterKey { get; set; }
        public FormMethod FormMethod { get; set; }

        public JsonModelBinderAttribute(string parameterKey, FormMethod method = FormMethod.Post)
        {
            this.ParameterKey = parameterKey;
            this.FormMethod = method;
        }

        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder(this.ParameterKey, this.FormMethod);
        }
    }

}
