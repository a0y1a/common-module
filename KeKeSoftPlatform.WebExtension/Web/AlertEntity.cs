using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
namespace KeKeSoftPlatform.WebExtension
{
    public enum AlertType
    {
        Success = 1,
        Danger
    }
    public class AlertEntity
    {
        public const string ALERT_ENTITY = "_AlertEntity_";

        public string Message { get; set; }
        public bool CanClose { get; set; }
        public AlertType Type { get; set; }

        public AlertEntity(string message, AlertType type = AlertType.Success, bool canClose = true)
        {
            this.Message = message;
            this.Type = type;
            this.CanClose = canClose;
        }
    }
}
