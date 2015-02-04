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
        public static List<Message> MemoryMessages { get; set; }

        static ChatCache()
        {

           // MessagesInMemory = new List<Message>();

            using (ChatContext ctx = new ChatContext())
            {


                //MemoryMessages = ctx.Messages
                //    .Include("RelatedUser")
                //    .OrderByDescending(m => m.Date)
                //    .ToList();

                //AsyncHelper.StartDroppingExpired();
                    

            }
        }

        public static void DropExpiredBinary()
        {
            if ((DateTime.Now - MemoryMessages.First().Date).TotalHours > 24)
            {
                
                int divider = MemoryMessages.Count / 2;
                int upperborder = MemoryMessages.Count, lowerborder = 0;
                //int iterlimit = (int)Math.Round(Math.Log(MemoryMessages.Count, 2));

                while (upperborder - lowerborder > 1)
                {
                    if (!MemoryMessages.ElementAt(divider).Date.IsOlderThan(24))
                    {
                        lowerborder = divider;
                    }

                    else
                    {
                        upperborder = divider;
                    }
                    divider = lowerborder + ((upperborder - lowerborder) / 2);
                }
                MemoryMessages = MemoryMessages.GetRange(0, divider);
            }
            Thread.Sleep(20000);
            
        }

        public static void DropExpiredInterpolate()
        {
            if ((DateTime.Now - MemoryMessages.First().Date).TotalHours > 24)
            {
                int lowerborder = 0;
                int upperborder = MemoryMessages.Count - 1;
                TimeSpan key = new TimeSpan(24, 0, 0);
                

                while (MemoryMessages.ElementAt(lowerborder).GetLifetime() < key && MemoryMessages.ElementAt(upperborder).GetLifetime() > key)
                {
                    int divider = (int)Math.Round(((key - MemoryMessages.ElementAt(lowerborder).GetLifetime()).TotalSeconds / (MemoryMessages.ElementAt(upperborder).Date - MemoryMessages.ElementAt(lowerborder).Date).TotalSeconds * (upperborder - lowerborder)));

                    if (MemoryMessages.ElementAt(divider - 1).GetLifetime() < key && MemoryMessages.ElementAt(divider + 1).GetLifetime() > key)
                    {
                        MemoryMessages = MemoryMessages.GetRange(0, divider);
                        break;
                    }

                    if ((key - MemoryMessages.ElementAt(divider).GetLifetime()).TotalSeconds >= 0)
                    {
                        lowerborder = divider;
                    }
                    else
                    {
                        upperborder = divider;
                    }

                    
                }
                
            }
            Thread.Sleep(20000);

        }

        private static bool IsOlderThan(this DateTime time, int hrs)
        {
            return (DateTime.Now - time).Hours < hrs;
            
        }

        public static TimeSpan GetLifetime(this Message ms)
        {
            return new TimeSpan(ms.Date.Ticks);
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