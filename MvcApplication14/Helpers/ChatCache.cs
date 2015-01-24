using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using MvcApplication14.Models;

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
                
                DropExpired();
                    

            }
        }

        static void DropExpired()
        {
            foreach (var item in MemoryMessages)
            {
                if ((DateTime.Now - item.Date).Hours > 24)
                {
                    MemoryMessages.Remove(item);
                }
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
            DropExpired();

            using (ChatContext ctx = new ChatContext())
            {
                ms.RelatedUser = ctx.Users.First(m => m.UserLogin == login);
                ctx.Messages.Add(ms);
                ctx.SaveChanges();
            }
        }
    }
}