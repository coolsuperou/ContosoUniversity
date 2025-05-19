using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using System.Web.Security;

namespace ContosoUniversity.Controllers
{
    public class AuthController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Auth/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            var user = db.Users.Where(s => s.name.Equals(userName, StringComparison.OrdinalIgnoreCase) && s.password.Equals(password)).FirstOrDefault();
            if (user == null)
            {
                ViewBag.ErrorMessage = "用户名或者密码错误。";
                return View();
            }
            FormsAuthentication.SetAuthCookie(user.name, true);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                user.name,
                DateTime.Now,
                DateTime.Now.AddMinutes(1000),
                false,
                "");
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
            Session["Username"] = user.name;
            Session["UserID"] = user.uid;
            string returnUrl = FormsAuthentication.GetRedirectUrl(user.name, false);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl == "/default.aspx")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public void Logoff()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            Response.Redirect("/Auth/Login");
        }
    }
}