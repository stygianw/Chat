using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MvcApplication14.Models
{
    public class ChatContext : DbContext
    {
        public ChatContext() : base("name=conn1")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(m => m.UserId);
            modelBuilder.Entity<User>().Property(m => m.UserLogin).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<User>().Property(m => m.UserPassword).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(m => m.DateOfRegistration).HasColumnType("datetime2");
            modelBuilder.Entity<User>().HasMany(m => m.Messages).WithRequired(m => m.RelatedUser);

            modelBuilder.Entity<Message>().HasKey(m => m.MessageId);
            modelBuilder.Entity<Message>().Property(m => m.Text).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<Message>().HasRequired(m => m.RelatedUser).WithMany(m => m.Messages);


            base.OnModelCreating(modelBuilder);
        }
    }
}