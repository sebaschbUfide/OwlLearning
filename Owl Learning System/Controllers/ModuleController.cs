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
    public class ModuleController : Controller
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
            var query = owldb.modules.OrderBy(a => a.module_id).ToPagedList(pageNumber, pageSize);

            if (query == null)
            {
                ViewBag.Check = 0;
            }
            else
            {
                ViewBag.Check = 1;
            }

            return View(query);
        }

        public ActionResult IndexPlus()
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            }

            var query = owldb.modules.Select(x => x).ToList();
            return View(query);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            ViewBag.courses = owldb.courses.Select(x => x);
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        public ActionResult Create(modules m)
        {
            try
            {
                var query = owldb.modules.Where(x => x.name == m.name).FirstOrDefault();

                if (query==null)
                {
                    owldb.modules.Add(m);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Módulo Existente.";
                    ViewBag.courses = owldb.courses.Select(x => x);
                    return View();
                }

            }
            catch (Exception e)
            {
                ViewBag.courses = owldb.courses.Select(x => x);
                return View();
            }
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            modules m = new modules();
            m = owldb.modules.Where(x => x.module_id == id).FirstOrDefault();

            ViewBag.courses = owldb.courses.Select(x => x);

            return View(m);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public ActionResult Edit(modules m)
        {
            var query = owldb.modules.Where(x => x.module_id != m.module_id && x.name == m.name);

            try
            {
                if (query==null)
                {
                    modules md = owldb.modules.Single(q => q.module_id == m.module_id);
                    md.name = m.name;
                    md.description = m.description;
                    md.course = m.course;

                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Módulo Existente.";
                    ViewBag.courses = owldb.courses.Select(x => x);
                    return View();
                }
                
            }
            catch (DbEntityValidationException dbEx)
            {
                ViewBag.courses = owldb.courses.Select(x => x);
                return View();
            }
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id, string q)
        {
            var query = owldb.modules.Where(a => a.module_id == id).FirstOrDefault();
            modules m = new modules();
            m.name = query.name;
            m.description = query.description;

            var query2 = owldb.courses.Where(x => x.course_id == query.course).FirstOrDefault();

            ViewBag.course = query2.name;

            return View(m);
        }

        // POST: Module/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.modules.Where(a => a.module_id == id).FirstOrDefault();
                owldb.modules.Remove(query);
                owldb.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult MultiAssignInd(int id)
        {
            var query = owldb.Multi_Modulo.Where(a => a.modules.module_id == id).ToList();

            var q2 = owldb.modules.Where(q => q.module_id == id).FirstOrDefault();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);

            ViewBag.module = q2.name;

            List<multimedia> mids = new List<multimedia>();
            foreach (var item in query)
            {
                var query1 = owldb.multimedia.Where(a => a.idMultimedia == item.idMultimedia).FirstOrDefault();
                if (query1 != null)
                {
                    mids.Add(query1);
                }
            }

            return View(mids);

        }

        public ActionResult FileAssignInd(int id)
        {
            var q1 = owldb.FC_Module.Where(a => a.modules.module_id == id).ToList();

            var q2 = owldb.modules.Where(q => q.module_id == id).FirstOrDefault();

            if (q1.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);

            ViewBag.module = q2.name;


            return View(q1);

        }


        public ActionResult AssignMultiToModule(int id)
        {
            ViewBag.module = owldb.modules.Where(x => x.module_id == id);

            ViewBag.multi = owldb.multimedia.Select(x => x);

            return View();
        }

        [HttpPost]
        public ActionResult AssignMultiToModule(Multi_Modulo mm)
        {
            try
            {
                var query = owldb.Multi_Modulo.Where(w => w.module_id == mm.module_id && w.idMultimedia == mm.idMultimedia).FirstOrDefault();

                if (query == null)
                {
                    owldb.Multi_Modulo.Add(mm);
                    owldb.SaveChanges();
                }

                return RedirectToAction("IndexPlus", "Module");
            }
            catch (Exception e)
            {
                ViewBag.module = owldb.modules.Where(x => x.module_id == mm.module_id);

                ViewBag.multi = owldb.multimedia.Where(x => x.Estado == 1);
                return View(mm);
            }

        }



        public ActionResult AssignFileToModule(int id)
        {
            ViewBag.module = owldb.modules.Where(x => x.module_id == id);

            ViewBag.file = owldb.FileControl.Select(x => x);

            return View();
        }

        [HttpPost]
        public ActionResult AssignFileToModule(FC_Module fcm)
        {
            try
            {
                var query = owldb.FC_Module.Where(w => w.module_id == fcm.module_id && w.idFile == fcm.idFile).FirstOrDefault();

                if (query == null)
                {
                    owldb.FC_Module.Add(fcm);
                    owldb.SaveChanges();
                }

                return RedirectToAction("IndexPlus", "Module");
            }
            catch (Exception e)
            {
                ViewBag.module = owldb.modules.Where(x => x.module_id == fcm.module_id);
                ViewBag.file = owldb.FileControl.Select(x => x);
                ViewBag.multi = owldb.FileControl.Select(x => x);
                return View(fcm);
            }

        }


        public ActionResult DeleteFileAssignment(int id, string y)
        {
            var query = owldb.FC_Module.Where(x => x.idFC_Module == id).FirstOrDefault();

            var query2 = owldb.modules.Where(x => x.module_id == query.module_id).FirstOrDefault();

            var query3 = owldb.FileControl.Where(x => x.idFile == query.idFile).FirstOrDefault();

            ViewBag.module = query2.name;
            ViewBag.file = query3.FileName;
            ViewBag.fileid = query2.module_id;


            return View();
        }

        [HttpPost]
        public ActionResult DeleteFileAssignment(int id)
        {
            var query = owldb.FC_Module.Where(a => a.idFC_Module == id).FirstOrDefault();
            owldb.FC_Module.Remove(query);
            owldb.SaveChanges();

            return RedirectToAction("FileAssignedInd", "Module", new { id = query.module_id });
        }

    }
}
