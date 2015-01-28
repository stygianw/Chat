using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using MvcApplication14.Models;
using System.Threading;

namespace MvcApplication14.Helpers
{
    public static class ChatCache
    {
        public static List<Message> MemoryMessages { get; private set; }

        static ChatCache()
        {

           // MessagesInMemory = new List<Message>();

            using (ChatContext ctx = new ChatContext())
            {


                MemoryMessages = ctx.Messages
                    .Include("RelatedUser")
                    .OrderByDescending(m => m.Date)
                    .ToList();

                AsyncHelper.StartDroppingExpired();
                    

            }
        }

        public static void DropExpired()
        {
            if ((MemoryMessages.Last().Date - MemoryMessages.First().Date).Hours > 24)
            {
                
                int divider = MemoryMessages.Count / 2;
                int upperborder = MemoryMessages.Count, lowerborder = 0;
                int iterlimit = (int)Math.Round(Math.Log(MemoryMessages.Count, 2));

                for (int i = 0; i < iterlimit; i++)
                {
                    if (!MemoryMessages.ElementAt(divider).Date.IsOlderThan(24))
                    {
                        lowerborder = divider;
                    }

                    else
                    {
                        upperborder = divider;
                    }
                    divider = (lowerborder + upperborder) / 2;
                }
                MemoryMessages = MemoryMessages.GetRange(0, divider);
            }
            Thread.Sleep(20000);
            
        }

        private static bool IsOlderThan(this DateTime time, int hrs)
        {
            return (DateTime.Now - time).Hours < hrs;
            
        }

        public static void AddMessage(string login, string message)
        {
            Message ms = new Message();
            ms.RelatedUser = new User();
            ms.Date = new DateTime(DateTime.Now.Ticks);
            ms.Text = message;
            ms.RelatedUser.UserLogin = login;
            MemoryMessages.Add(ms);
            ActiveListHelper.StampTime(login);
            //DropExpired();

            using (ChatContext ctx = new ChatContext())
            {
                ms.RelatedUser = ctx.Users.First(m => m.UserLogin == login);
                ctx.Messages.Add(ms);
                ctx.SaveChanges();
            }
        }
    }
}