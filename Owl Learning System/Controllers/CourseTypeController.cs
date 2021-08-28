using mysqltest.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mysqltest.Controllers
{
    public class CourseTypeController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        RoleSearching rs = new RoleSearching();
        // GET: Module
        public ActionResult Index(int? page)
        {

            if (Request.IsAuthenticated)
            {
                ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.course_type.OrderBy(a => a.name).ToPagedList(pageNumber, pageSize);

            return View(query);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        public ActionResult Create(course_type ct)
        {
            var query = owldb.course_type.Where(x => x.name == ct.name).FirstOrDefault();

            try
            {
                if (query==null)
                {
                    owldb.course_type.Add(ct);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Tipo de Curso Existente.";
                    return View(ct);
                }
                
            }
            catch (DbEntityValidationException dbEx)
            {
                return View();
            }
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            course_type ct = new course_type();
            ct = owldb.course_type.Where(x => x.course_type_id == id).FirstOrDefault();

            return View(ct);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public ActionResult Edit(course_type ct)
        {
            var query = owldb.course_type.Where(x => x.course_type_id != ct.course_type_id && x.name == ct.name).FirstOrDefault();

            try
            {
                if (query == null)
                {
                    course_type courseT = owldb.course_type.Single(q => q.course_type_id == ct.course_type_id);
                    courseT.name = ct.name;

                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Tipo de Curso Existente.";
                    return View();
                }
                
            }
            catch (DbEntityValidationException dbEx)
            {
                return View();
            }
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id, string q)
        {
            var query = owldb.course_type.Where(a => a.course_type_id == id).FirstOrDefault();
            course_type ct = new course_type();
            ct.course_type_id = query.course_type_id; 
            ct.name = query.name;

            return View(ct);
        }

        // POST: Module/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.course_type.Where(a => a.course_type_id == id).FirstOrDefault();
                owldb.course_type.Remove(query);
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