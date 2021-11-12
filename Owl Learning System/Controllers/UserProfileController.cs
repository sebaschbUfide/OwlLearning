using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mysqltest.Models;
using System.Web.Security;
using System.Configuration;
using Owlapp.Tools;
using System.Dynamic;
using Newtonsoft.Json;

namespace mysqltest.Controllers
{
    [Authorize]

    public class UserProfileController : Controller
    {
       
        // GET: UserProfile

        owldbEntities01 owldb = new owldbEntities01();

        RoleSearching rs = new RoleSearching();
        string secretKey = ConfigurationManager.AppSettings["SecretKey"];
         
        
        public ActionResult Index()
        {
            //DEVUELVE EL MODELO USER DEL USUARIO LOGEADO
            var u = rs.GetUserInfo(this.HttpContext.User.Identity.Name);
            var r = rs.GetRole(this.HttpContext.User.Identity.Name);

            var query = owldb.course_assignment.Where(a => a.user_id == u.user_id).FirstOrDefault();
            var query_1 = owldb.course_assignment.Where(a => a.user_id == u.user_id);

            List<int> idca = new List<int>();//LISTA DE IDs DE CADA COURSE ASSIGNMENT
            foreach (var item in query_1)
            {
                idca.Add(item.course_assign_id);
            }
            

            List<courses> l_c = new List<courses>();
            foreach (var item in idca)
            {
                var q1 = owldb.course_assignment.Where(a => a.course_assign_id == item).FirstOrDefault();
                var q2 = owldb.courses.Where(a => a.course_id == q1.course_id).FirstOrDefault(); //ESTE ES EL QUE SE DEVUELVE
                var q3 = owldb.schedules.Where(a => a.schedule_id == q1.schedule_id).FirstOrDefault();

                courses c = new courses();
                c.course_id = q2.course_id;
                c.name = q2.name + " - " + q3.day + " (" + q3.star_time + " - " + q3.end_time + ")";

                l_c.Add(c);

            }

            ViewBag.courses = l_c;

            if (r!=1)
            {
                return View(query);
            }
            else
            {
                return RedirectToAction("IndexAdmin", "UserProfile", query);
            }
        }

        [HttpPost]
        public ActionResult Index(course_assignment ca)
        {
            return RedirectToAction("ModuleDisplay", "UserProfile", new { id = ca.course_id});
        }

        public ActionResult IndexAdmin()
        {
            var u = rs.GetUserInfo(this.HttpContext.User.Identity.Name);
            var a = Seguridad.DecryptString(secretKey, u.password);
            u.password = a;
            return View(u);
        }

        public ActionResult EditProfile(int id)
        {

            var query = owldb.users.Where(x => x.user_id == id).FirstOrDefault();

            query.password = Seguridad.DecryptString(secretKey, query.password);


            return View(query);
        }

        [HttpPost]
        public ActionResult EditProfile(users user)
        {

            var x = 0;
            try
            {
                users u = owldb.users.Single(a => a.user_id == user.user_id);

                u.first_name = user.first_name;
                u.last_name = user.last_name;
                u.phone_number = user.phone_number;
                //u.password = Seguridad.EncryptString(secretKey, user.password);

                if (user.first_name == null || user.last_name == null || user.phone_number == null)
                {
                    return View(user);
                }
                else
                {

                    if (u.email != user.email)
                    {
                        x = 1;
                        u.email = user.email;

                    }
                    else
                    {
                        x = 0;
                        u.email = u.email;
                    }

                    owldb.SaveChanges();

                    if (x == 0)
                    {
                        return RedirectToAction("IndexAdmin", "UserProfile");

                    }
                    else
                    {

                        Session.Remove("Identificacion");
                        Session.RemoveAll();
                        Response.Cache.SetCacheability(HttpCacheability.Private);
                        Session.Clear();
                        FormsAuthentication.SignOut();
                        Session.Abandon();
                        Response.Cache.SetNoServerCaching();
                        Request.Cookies.Clear();
                        return RedirectToAction("Login", "Account");
                    }
                }

            }
            catch (Exception)
            {
                ViewBag.Error = "El correo que especifico ya se encuentra registrado";

                return View(user);

                throw;
            }

        }

        public ActionResult ModuleDisplay(int id)
        {

            var q = owldb.modules.Where(a => a.course == id).ToList();

            if (q.Count != 0)
            {
                ViewBag.check = 1;
            }
            else
            {
                ViewBag.check = 0;
            }

            ViewBag.course = id;

            return View(q);
        }

        [HttpPost]
        public ActionResult ModuleDisplay(modules modules)
        {
            return View();
        }

    }
}