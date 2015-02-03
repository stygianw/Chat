using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

namespace MvcApplication14.Helpers
{
    public static class AsyncHelper
    {
        public static CancellationTokenSource UserMonitorCancellation { get; private set; }
        public static CancellationTokenSource DroppingExpiredCancellation { get; private set; }

        static AsyncHelper()
	    {
            UserMonitorCancellation = new CancellationTokenSource();
            DroppingExpiredCancellation = new CancellationTokenSource();
	    }

        public static void StartDroppingExpired()
        {
            Task dropExpired = Task.Factory.StartNew(ChatCache.DropExpiredInterpolate);
        }

        public static void StartUsersMonitoring()
        {
            Task startMonitoring = Task.Factory.StartNew(ActiveListHelper.MonitorUsers, UserMonitorCancellation.Token, UserMonitorCancellation.Token);
        }

    }
}