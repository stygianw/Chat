using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MvcApplication14;
using System.Linq;
using MvcApplication14.Models;
using MvcApplication14.Helpers;

namespace UnitTestProject1
{
    [TestClass]
    public class SearchTest
    {
        [TestMethod]
        public void TestDropExpiredInterpolate()
        {
            List<Message> TestMessages = new List<Message>();
            for (int i = 0; i < 40; i++)
            {
                TestMessages.Add(new Message() { Date = new DateTime(new TimeSpan(i, i / 2, 0).Ticks) });
            }

            MvcApplication14.Helpers.ChatCache.MemoryMessages = TestMessages;
            MvcApplication14.Helpers.ChatCache.DropExpiredInterpolate();

            foreach (var item in ChatCache.MemoryMessages)
            {
                Assert.IsTrue(new TimeSpan(item.Date.Ticks) <= new TimeSpan(24, 0, 0));
            }
        }
    }
}
