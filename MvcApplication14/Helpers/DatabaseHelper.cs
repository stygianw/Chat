using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Collections.Specialized;
using MvcApplication14.Models;
using System.Collections;


namespace MvcApplication14.Helpers
{
    public static class DatabaseHelper
    {
        
        private static int AddedMessagesCounter { get; set; }
        public static Queue<Message> MessagesInMemory { get; private set; }
        
        static DatabaseHelper()
        {
            MessagesInMemory = new Queue<Message>();
        }

        public static void LoadContentToMemory() 
        {
            if (MessagesInMemory.Count == 0)
            {
                using (ChatContext ctx = new ChatContext())
                {
                        var lst = ctx.Messages.Count() < 1000 ? ctx.Messages.Include("RelatedUser").ToList() : ctx.Messages.Include("RelatedUser").Take(1000).ToList();
                        foreach (var item in lst)
                        {
                            MessagesInMemory.Enqueue(item);
                        }
                    
                }    
            }
            
        }


        public static void SaveMessageToDatabase(Message message)
        {
            
        }

        public static void StartDumpingToDatabase(object cancellationToken)
        {
            
            while (true)
            {
                if (((CancellationToken)cancellationToken).IsCancellationRequested)
                {
                    return;
                }

                if (MessagesInMemory.Count > 0)
                {
                    using (ChatContext ctx = new ChatContext())
                    {
                        foreach (var item in MessagesInMemory.Skip(MessagesInMemory.Count - AddedMessagesCounter))
                        {
                            item.RelatedUser = ctx.Users.FirstOrDefault(m => m.UserLogin == item.RelatedUser.UserLogin);
                            ctx.Messages.Add(item);
                        }
                        ctx.SaveChanges();
                    }
                    AddedMessagesCounter = 0;
                }
                var i = new Action<object>(StartDumpingToDatabase);
                
                Thread.Sleep(15000);
                   
            }
        }

        public static void AddMessage(string username, string message)
        {

            Message ms = new Message();
            ms.RelatedUser = new User();
            ms.RelatedUser.UserLogin = username;
            ms.Text = message;
            MessagesInMemory.Enqueue(ms);
            
            if (MessagesInMemory.Count > 1000)
            {
                MessagesInMemory.Dequeue();    
            }

            new Action<Message>((msg) =>
            {
                using (ChatContext ctx = new ChatContext())
                {
                    msg.RelatedUser = ctx.Users.FirstOrDefault(m => m.UserLogin == msg.RelatedUser.UserLogin);
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                }

            }).BeginInvoke(ms, null, null);
            
            MessagesInMemory.OrderBy(m => m.)

            ActiveListHelper.StampTime(username);
           
            //AddedMessagesCounter++;
        }
     
        
    }

}