using mysqltest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace mysqltest.Controllers
{
    public class CoursesController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        RoleSearching rs = new RoleSearching();

        
        public ActionResult Index(int? page)
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var data = owldb.courses.OrderBy(a => a.name).ToPagedList(pageNumber, pageSize);


            return View(data);
        }

        public ActionResult IndexPrincipiante()
        {
            var data = owldb.courses.Where(z => z.name.Contains("Principiante"));
            return View(data);
        }

        public ActionResult IndexIntermedio()
        {
            var data = owldb.courses.Where(z => z.name.Contains("Intermedio"));
            return View(data);
        }

        public ActionResult IndexAvanzado()
        {
            var data = owldb.courses.Where(z => z.name.Contains("Avanzado"));
            return View(data);
        }


        // GET: Schedule/Create
        public ActionResult Create()
        {
            ViewBag.type = owldb.course_type.Select(x => x);

            return View();
        }

        // POST: Schedule/Create
        [HttpPost]
        public ActionResult Create(courses c)
        {
            var query = owldb.courses.Where(x => x.name == c.name || x.description == c.description).FirstOrDefault();

            try
            {
                if (query == null)
                {
                    owldb.courses.Add(c);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.type = owldb.course_type.Select(x => x);
                    ViewBag.Error = "* Nombre de Curso Existente.";
                    return View();
                }
                
            }
            catch(Exception a)
            {
                Console.WriteLine(a.StackTrace);
                ViewBag.type = owldb.course_type.Select(x => x);
                return View(c);
            }
        }


        // GET: Schedule/Edit/5
        public ActionResult Edit(int id)
        {
            courses c = new courses();
            c = owldb.courses.Where(x => x.course_id == id).FirstOrDefault();
            ViewBag.type = owldb.course_type.Select(x => x);

            return View(c);
        }

        // POST: Schedule/Edit/5
        [HttpPost]
        public ActionResult Edit(courses c)
        {
            var query = owldb.courses.Where(x => x.course_id != c.course_id && x.name == c.name || x.description == c.description).FirstOrDefault();

            try
            {
                if (query == null)
                {
                    courses cs = owldb.courses.Single(q => q.course_id == c.course_id);
                    cs.name = c.name;
                    cs.description = c.description;
                    cs.duration = c.duration;
                    cs.cost = c.cost;
                    cs.type = c.type;
                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Curso Existente.";
                    ViewBag.type = owldb.course_type.Select(x => x);
                    return View(c);
                }
                
            }
            catch
            {
                ViewBag.type = owldb.course_type.Select(x => x);
                return View(c);
            }
        }

        // GET: Schedule/Delete/5
        public ActionResult Delete(int id, string x)
        {
            var query = owldb.courses.Where(a => a.course_id == id).FirstOrDefault();
            courses cs = new courses();
            cs.name = query.name;
            cs.description = query.description;
            cs.duration = query.duration;
            cs.cost = query.cost;
            cs.type = query.type;

            return View(cs);
        }

        // POST: Schedule/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.courses.Where(a => a.course_id == id).FirstOrDefault();
                owldb.courses.Remove(query);
                owldb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
