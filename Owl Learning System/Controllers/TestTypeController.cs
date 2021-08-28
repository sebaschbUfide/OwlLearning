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
    public class TestTypeController : Controller
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
            var query = owldb.test_type.OrderBy(a => a.name).ToPagedList(pageNumber, pageSize);

            return View(query);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        public ActionResult Create(test_type tt)
        {
            var query = owldb.test_type.Where(x => x.name == tt.name).FirstOrDefault();

            try
            {
                if (query==null)
                {
                    owldb.test_type.Add(tt);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Tipo de Prueba Existente.";
                    return View();
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
            test_type ct = new test_type();
            ct = owldb.test_type.Where(x => x.test_type_id == id).FirstOrDefault();

            return View(ct);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public ActionResult Edit(test_type tt)
        {
            var query = owldb.test_type.Where(x => x.test_type_id != tt.test_type_id && x.name == tt.name).FirstOrDefault();

            try
            {
                if (query==null)
                {
                    test_type typeT = owldb.test_type.Single(q => q.test_type_id == tt.test_type_id);
                    typeT.name = tt.name;

                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "* Tipo de Prueba Existente.";
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
            var query = owldb.test_type.Where(a => a.test_type_id == id).FirstOrDefault();
            test_type ct = new test_type();
            ct.test_type_id = query.test_type_id;
            ct.name = query.name;

            return View(ct);
        }

        // POST: Module/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.test_type.Where(a => a.test_type_id == id).FirstOrDefault();
                owldb.test_type.Remove(query);
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