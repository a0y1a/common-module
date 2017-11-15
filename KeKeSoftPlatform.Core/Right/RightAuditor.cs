using KeKeSoftPlatform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.Core
{
    public abstract class RightAuditor
    {
        protected Identity identity;
        public RightAuditor(Identity identity)
        {
            this.identity = identity;
        }
        public bool? Result { get; protected set; }

        public abstract void Audit(AuthorizationContext filterContext);

        public event EventHandler<EventArgs> SuccessEventHanlder;
        public event EventHandler<EventArgs> FailEventHanlder;

        public void TriggerSuccess()
        {
            if (this.SuccessEventHanlder != null)
            {
                this.SuccessEventHanlder(null, null);
            }
        }

        public void TriggerFail()
        {
            if (this.FailEventHanlder != null)
            {
                this.FailEventHanlder(null, null);
            }
        }
    }
}
