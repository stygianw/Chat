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
            while (true)
            {
                if (MemoryMessages.Count > 1000)
                {
                    TimeSpan diff = MemoryMessages.Last().Date - MemoryMessages.First().Date;
                    double messagesPerMinute = MemoryMessages.Count / diff.Minutes;
                    int messagesPer24hrs = (int)messagesPerMinute * 60 * 24;

                    MemoryMessages.RemoveRange(messagesPer24hrs, (MemoryMessages.Count - messagesPer24hrs));
                }
                Thread.Sleep(20000);
            }
            

            


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