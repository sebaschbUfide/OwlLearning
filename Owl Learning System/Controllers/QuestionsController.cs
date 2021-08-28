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
    public class QuestionsController : Controller
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
            var query = owldb.questions.OrderBy(a => a.question).ToPagedList(pageNumber, pageSize);

            return View(query);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            List<int> answers = new List<int>();
            answers.Add(1);
            answers.Add(2);
            answers.Add(3);
            answers.Add(4);

            ViewBag.ans = answers;
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        public ActionResult Create(questions q)
        {
            var query = owldb.questions.Where(x => x.question == q.question).FirstOrDefault();

            try
            {
                if (query == null)
                {
                    owldb.questions.Add(q);
                    owldb.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    List<int> answers = new List<int>
                    {
                        1,
                        2,
                        3,
                        4
                    };

                    ViewBag.Error = "* Pregunta Existente.";
                    ViewBag.ans = answers;
                    return View(q);
                }

                
            }
            catch (DbEntityValidationException dbEx)
            {

                List<int> answers = new List<int>
                {
                    1,
                    2,
                    3,
                    4
                };

                ViewBag.ans = answers;
                return View(q);
            }
        }


        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            List<int> answers = new List<int>();
            answers.Add(1);
            answers.Add(2);
            answers.Add(3);
            answers.Add(4);

            ViewBag.ans = answers;
            questions q = new questions();
            q = owldb.questions.Where(x => x.question_id == id).FirstOrDefault();

            return View(q);
        }

        // POST: Module/Edit/5
        [HttpPost]
        public ActionResult Edit(questions q)
        {
            var query = owldb.questions.Where(x => x.question_id != q.question_id && q.question == q.question).FirstOrDefault();

            try
            {
                if (query==null)
                {
                    questions qst = owldb.questions.Single(w => w.question_id == q.question_id);
                    qst.question = q.question;
                    qst.answer1 = q.answer1;
                    qst.answer2 = q.answer2;
                    qst.answer3 = q.answer3;
                    qst.answer4 = q.answer4;

                    owldb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    List<int> answers = new List<int>();
                    answers.Add(1);
                    answers.Add(2);
                    answers.Add(3);
                    answers.Add(4);

                    ViewBag.ans = answers;
                    ViewBag.Error = "* Pregunta Ya Existe.";
                    return View();
                }

                
            }
            catch (Exception e)
            {
                List<int> answers = new List<int>();
                answers.Add(1);
                answers.Add(2);
                answers.Add(3);
                answers.Add(4);

                ViewBag.ans = answers;
                return View();
            }
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id, string q)
        {
            var query = owldb.questions.Where(a => a.question_id == id).FirstOrDefault();
            questions qst = new questions();
            qst.question = query.question;
            qst.answer1 = query.answer1;
            qst.answer2 = query.answer2;
            qst.answer3 = query.answer3;
            qst.answer4 = query.answer4;

            return View(qst);
        }

        // POST: Module/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var query = owldb.questions.Where(a => a.question_id == id).FirstOrDefault();
                owldb.questions.Remove(query);
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