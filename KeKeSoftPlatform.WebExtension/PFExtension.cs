using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.WebExtension
{
    public static class PFExtension
    {
        public static string EnumMetadataDisplay(this Enum value)
        {
            var attribute = value.GetType().GetField(Enum.GetName(value.GetType(), value)).GetCustomAttributes(
                 typeof(EnumValueAttribute), false)
                 .Cast<EnumValueAttribute>()
                 .FirstOrDefault();
            if (attribute != null)
            {
                return attribute.Value;
            }

            return Enum.GetName(value.GetType(), value);
        }

        public static IEnumerable<SelectListItem> ToSelectDataSource(this Enum type)
        {
            return Enum.GetValues(type.GetType())
                        .Cast<Enum>()
                        .Select(m =>
                        {
                            string enumVal = Enum.GetName(type.GetType(), m);
                            return new SelectListItem()
                            {
                                Selected = (type.ToString() == enumVal),
                                Text = m.EnumMetadataDisplay(),
                                Value = enumVal
                            };
                        });
        }
    }
}
