using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mysqltest.Models;
using PagedList;

namespace mysqltest.Controllers
{
    public class ScheduleController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        RoleSearching rs = new RoleSearching();

        // GET: Schedule
        public ActionResult Index(int? page)
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var data = owldb.schedules.OrderBy(a => a.day).ToPagedList(pageNumber, pageSize);
            
            return View(data);
        }


        // GET: Schedule/Create
        public ActionResult Create()
        {
            ViewBag.day = ScheduleLists.Days();
            ViewBag.timePickerStart = ScheduleLists.StartTime();
            ViewBag.timePickerEnd = ScheduleLists.EndTime();

            return View();
        }

        // POST: Schedule/Create
        [HttpPost]
        public ActionResult Create(schedules sch)
        {
            try
            {
                var query = owldb.schedules.Where(a => a.day.Equals(sch.day) && a.star_time == sch.star_time && a.end_time == sch.end_time).FirstOrDefault();

                if (query != null)
                {
                    ViewBag.day = ScheduleLists.Days();
                    ViewBag.timePickerStart = ScheduleLists.StartTime();
                    ViewBag.timePickerEnd = ScheduleLists.EndTime();

                    ViewBag.error = "* Error: El horario elegido, ya se encuentra agregado.";

                    return View();
                }
                else
                {
                    if (sch.star_time > sch.end_time)
                    {
                        ViewBag.day = ScheduleLists.Days();
                        ViewBag.timePickerStart = ScheduleLists.StartTime();
                        ViewBag.timePickerEnd = ScheduleLists.EndTime();

                        ViewBag.error = "* Error: La hora de entrada debe ser menor que la hora de salida.";

                        return View();
                    }
                    else
                    {
                        //Agrega en base de datos el objeto de horario pasado por parámetro
                        owldb.schedules.Add(sch);
                        owldb.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: Schedule/Edit/5
        public ActionResult Edit(int id)
        {

            ViewBag.day = ScheduleLists.Days();
            ViewBag.timePickerStart = ScheduleLists.StartTime();
            ViewBag.timePickerEnd = ScheduleLists.EndTime();

            schedules sch = new schedules();
            sch.schedule_id = id;

            return View(sch);
        }

        // POST: Schedule/Edit/5
        [HttpPost]
        public ActionResult Edit(schedules sch)
        {
            try
            {
                schedules s = owldb.schedules.Single(q => q.schedule_id == sch.schedule_id);
                s.day = sch.day;
                s.star_time = sch.star_time;
                s.end_time = sch.end_time;

                owldb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(sch);
            }
        }

        // GET: Schedule/Delete/5
        public ActionResult Delete(int id, string x)
        {
            var query = owldb.schedules.Where(a => a.schedule_id == id).FirstOrDefault();
            schedules sch = new schedules();
            sch.day = query.day;
            sch.star_time = query.star_time;
            sch.end_time = query.end_time;

            return View(sch);
        }

        // POST: Schedule/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.schedules.Where(a => a.schedule_id == id).FirstOrDefault();
                owldb.schedules.Remove(query);
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
