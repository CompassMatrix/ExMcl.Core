using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Microsoft;
using System;
using System.Collections.Generic;
using System.Net;
using WPFLauncher.Model;
using WPFLauncher.Network.Protocol;
using WPFLauncher.Util;

namespace Microsoft.Event
{
    internal class LogsInterception : IMethodHook
    {


        [HookMethod("WPFLauncher.Manager.Auth.aqf")]
        public void g(string lpt)
        { }
    }
}