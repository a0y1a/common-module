using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KeKeSoftPlatform.Core
{
    public class RighAuditorFactory
    {
        public static RightAuditor GetRightAuditor(Identity identity)
        {
            switch (identity)
            {
                case Identity.User:
                    return new DirectRightAuditor(identity);
                case Identity.Admin:
                    return new DirectRightAuditor(identity);
                case Identity.SuperAdmin:
                    return new DirectRightAuditor(identity);
                case Identity.Developer:
                    return new DirectRightAuditor(identity);
                default:
                    throw new Exception("未找到合适的授权审核");
            }
        }
    }
}
