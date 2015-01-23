using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication14.Helpers;

namespace MvcApplication14.Filters
{
    public class OnlyAuthorizedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            object login = filterContext.HttpContext.Session["login"];
            if (login == null || !ActiveListHelper.ActiveUsersList.Keys.Contains(login.ToString()))
            {
                filterContext.Result = new RedirectResult("/");
            }
            
            
        }
    }
}