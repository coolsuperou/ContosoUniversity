using ContosoUniversity.DAL;


using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContosoUniversity.Controllers
{
    public class UserController : Controller
    {
        private SchoolContext db = new SchoolContext();
        // GET: User
        public ActionResult Register()
        {
            return View();
        }

        //POST：注册用户信息，注册成功后自动登录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "uid,name,password,confirmPassword,address,tel,email")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                string userInfo = string.Format("uid:{0},name:{1},password:{2},address{3},tel:{4},email:{5}",
                    user.uid, user.name, user.password, user.address, user.tel, user.email);
                return Content(userInfo);
            }
            return View();
        }
    }
}
