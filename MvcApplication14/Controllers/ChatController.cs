using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication14.Helpers;
using MvcApplication14.Models;
using MvcApplication14.Filters;

namespace MvcApplication14.Controllers
{
    [OnlyAuthorized]
    public class ChatController : Controller
    {
        //
        // GET: /Chat/

        

        public ActionResult MainWindow()
        {
            return View();
        }

        public ActionResult GetMessages(int messno)
        {
            
            var messagesToSend = ChatCache.MemoryMessages.Skip(messno).Select(m => new { message = m.Text, user = m.RelatedUser.UserLogin }).ToList();
            return Json(new { messages = messagesToSend, messagesNumber = ChatCache.MemoryMessages.Count}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RecordMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                ChatCache.AddMessage(Session["login"].ToString(), message);
                return new HttpStatusCodeResult(200);
            }
            return Content("no message was sent", "text/plain");
        }

        public ActionResult GetUsersList()
        {
            return Json(new { users = ActiveListHelper.ActiveUsersList.Keys.ToList() }, JsonRequestBehavior.AllowGet);
        }

    }
}
