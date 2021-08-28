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
    public class VirtualTestController : Controller
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
            var query = owldb.virtual_tests.OrderBy(a => a.name).ToPagedList(pageNumber, pageSize);

            return View(query);
        }

        public ActionResult GetInfo(int id)
        {
            var query = owldb.question_vt.Where(x => x.virtual_tests.virtual_test_id == id).ToList();

            var query2 = owldb.virtual_tests.Where(y => y.virtual_test_id == id).FirstOrDefault();

            ViewBag.prueba = query2.name;
            return View(query);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            ViewBag.testType = owldb.test_type.Select(x => x);
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        public ActionResult Create(virtual_tests vt)
        {
            var query = owldb.virtual_tests.Where(x => x.name == vt.name).FirstOrDefault();

            try
            {
                if (query==null)
                {
                    owldb.virtual_tests.Add(vt);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Prueba Virtual Existente.";
                    ViewBag.testType = owldb.test_type.Select(x => x);
                    return View();
                }
                
            }
            catch (DbEntityValidationException dbEx)
            {
                ViewBag.testType = owldb.test_type.Select(x => x);
                return View();
            }
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            virtual_tests vt = new virtual_tests();
            vt = owldb.virtual_tests.Where(x => x.virtual_test_id == id).FirstOrDefault();

            ViewBag.testType = owldb.test_type.Select(y => y);

            return View(vt);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public ActionResult Edit(virtual_tests vt)
        {
            var query = owldb.virtual_tests.Where(x => x.virtual_test_id != vt.virtual_test_id && x.name == vt.name).FirstOrDefault();

            try
            {
                if (query == null)
                {
                    virtual_tests virtualT = owldb.virtual_tests.Single(q => q.virtual_test_id == vt.virtual_test_id);
                    virtualT.name = vt.name;
                    virtualT.instructions = vt.instructions;
                    virtualT.duration = vt.duration;
                    virtualT.type = vt.type;

                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Nombre de Prueba Virtual Existente.";
                    ViewBag.testType = owldb.test_type.Select(y => y);
                    return View();
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                ViewBag.testType = owldb.test_type.Select(y => y);
                return View();
            }
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id, string q)
        {
            var query = owldb.virtual_tests.Where(a => a.virtual_test_id == id).FirstOrDefault();
            virtual_tests vt = new virtual_tests();
            vt.name = query.name;
            vt.duration = query.duration;
            vt.instructions = query.instructions;

            var query2 = owldb.test_type.Where(x => x.test_type_id == query.type).FirstOrDefault();

            ViewBag.testType = query2.name;

            return View(vt);
        }

        // POST: Module/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var q = owldb.module_vt.Where(a => a.virtual_test == id).FirstOrDefault();
                var query = owldb.virtual_tests.Where(a => a.virtual_test_id == id).FirstOrDefault();

                if (q != null)
                {
                    ViewBag.message = "No se puede Eliminar la prueba ya que se encuentra asignada a un Módulo";
                    return View(query);
                }
                else
                {
                    owldb.virtual_tests.Remove(query);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch(Exception e)
            {
                return View();
            }
        }
    }
}