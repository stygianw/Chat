using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication14.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public virtual List<Message> Messages{ get; set; }
    }

    public class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public User RelatedUser { get; set; }
    }
}