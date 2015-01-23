using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication14.Models;
using MvcApplication14.Models.ViewModels;
using MvcApplication14.Helpers;

namespace MvcApplication14.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user)
        {
            using (ChatContext ctx = new ChatContext())
            {
                User userFromDatabase;
                if ((userFromDatabase = ctx.Users.FirstOrDefault(m => m.UserLogin == user.UserLogin)) != null && userFromDatabase.UserPassword == user.UserPassword)
                {
                    ActiveListHelper.ActiveUsersList.Add(user.UserLogin, DateTime.Now.Ticks);
                    Session["login"] = user.UserLogin;
                    return RedirectToAction("MainWindow", "Chat");
                }
                ModelState.AddModelError("UserLogin", "Вы не зарегистрированы");
                return View();
                
                

            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ChatContext ctx = new ChatContext())
                {
                    if (ctx.Users.FirstOrDefault(m => m.UserLogin == model.Login) != null)
                    {
                        ModelState.AddModelError("Login", "Такой логин уже существует!");
                        return View();
                    }

                    User newuser = new User() { UserLogin = model.Login, UserPassword = model.Password, DateOfRegistration = DateTime.Now };
                    ctx.Users.Add(newuser);
                    ctx.SaveChanges();
                    //DatabaseHelper.UsersInMemory.Add(newuser);
                    return View("RegisterSuccess");
                }

            }
            return View();
        }

    }
}
