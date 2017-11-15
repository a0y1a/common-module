using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeKeSoftPlatform.Common;
using KeKeSoftPlatform.Core;
using KeKeSoftPlatform.WebExtension;
using KeKeSoftPlatform.Db;

namespace KeKeSoftPlatform.Core
{
    public class BridgeMananger
    {
        private static Guid? _CurrentBridgeId;
        public static Guid? CurrentBridgeId
        {
            get
            {
                return _CurrentBridgeId;
            }
        }

        public static void ChangeBridge(Guid bridgeId)
        {
            _CurrentBridgeId = bridgeId;
        }

       
    }
}
