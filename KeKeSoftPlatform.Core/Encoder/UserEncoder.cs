using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    public class UserEncoder : EncoderProvider
    {
        public UserEncoder(string prevFix)
            : base(prevFix)
        {

        }

        private static UserEncoder _Instance;
        public static UserEncoder Instance { get { return _Instance; } }

        static UserEncoder()
        {
            _Instance = new UserEncoder("");
        }
    }
}
