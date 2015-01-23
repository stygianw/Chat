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
        public static CancellationTokenSource DatabaseDumpingCancellation{ get; private set; }

        static AsyncHelper()
	    {
            UserMonitorCancellation = new CancellationTokenSource();
            DatabaseDumpingCancellation = new CancellationTokenSource();
	    }

        public static void StartDumping()
        {
            DatabaseHelper.LoadContentToMemory();
            Task startDumping = Task.Factory.StartNew(DatabaseHelper.StartDumpingToDatabase, DatabaseDumpingCancellation.Token, DatabaseDumpingCancellation.Token);
        }

        public static void StartUsersMonitoring()
        {
            Task startMonitoring = Task.Factory.StartNew(ActiveListHelper.MonitorUsers, UserMonitorCancellation.Token, UserMonitorCancellation.Token);
        }

    }
}