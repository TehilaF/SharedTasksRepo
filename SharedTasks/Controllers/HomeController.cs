using SharedTasks.Data;
using SharedTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SharedTasks.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.GetByEmail(User.Identity.Name);
            return View(new IndexViewModel { UserId = user.Id });
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.LogIn(email, password);
            if (user == null)
            {
                return Redirect("/home/login");
            }

            FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
            return Redirect("/home/index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            repo.AddUser(user, password);
            return Redirect("/home/index");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/home/login");
        }
    }
}