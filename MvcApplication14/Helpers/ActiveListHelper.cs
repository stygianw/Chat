using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace MvcApplication14.Helpers
{
    public static class ActiveListHelper
    {
        public static Dictionary<string, long> ActiveUsersList { get; private set; }

        static ActiveListHelper()
        {
            ActiveUsersList = new Dictionary<string, long>();
        }

        public static void StampTime(string username)
        {
            string existingName;
            if ((existingName = ActiveUsersList.Keys.FirstOrDefault(m => m == username)) == null)
            {
                ActiveUsersList.Add(username, DateTime.Now.Ticks);
            }
            else
            {
                ActiveUsersList[username] = DateTime.Now.Ticks;
            }
        }

        public static void MonitorUsers(object cancellationToken)
        {
            CancellationToken token = (CancellationToken)cancellationToken;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                foreach (KeyValuePair<string, long> item in ActiveUsersList)
                {
                    if ((DateTime.Now - new DateTime(item.Value)).Minutes > 10)
                    {
                        ActiveUsersList.Remove(item.Key);
                    }
                }
                
                Thread.Sleep(25000);
            }
        }
    }
}