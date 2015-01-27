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
                int border = MemoryMessages.Count;
                
                while (divider > 0 && divider < (MemoryMessages.Count - 1))
                {
                    
                    if (!MemoryMessages.ElementAt(divider).Date.IsOlderThan(24))
                    {
                        if (MemoryMessages.ElementAt(divider + 1).Date.IsOlderThan(24))
                        {
                            break;
                        }

                        divider = (divider + border) / 2;
                    }

                    else
                    {
                        if (!MemoryMessages.ElementAt(divider - 1).Date.IsOlderThan(24))
                        {
                            break;
                        }
                        border = divider;
                        divider = border / 2;
                    }
                    
                }
                MemoryMessages = MemoryMessages.GetRange(0, divider);
            }
            
        }

        private static bool IsOlderThan(this DateTime time, int hrs)
        {
            if ((DateTime.Now - time).Hours < hrs)
            {
                return false;
            }
            else return true;
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