using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Attributes;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace KeKeSoftPlatform.Core
{
    public class CreateModuleData
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称是必填项")]
        public string Name { get; set; }

        [Display(Name = "链接")]
        public string Url { get; set; }

        [Display(Name = "父级")]
        [DropDownList("ParentModule")]
        public Guid? ParentId { get; set; }
    }
}
