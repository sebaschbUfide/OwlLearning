
using mysqltest.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mysqltest.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        RoleSearching rs = new RoleSearching();

        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            return View();
        }


        //MUESTRA LA LISTA DE HORARIOS ASIGNADO A CURSOS
        public ActionResult SchedulesAssigned()
        {
            var data = owldb.course_schedule.Select(z => z).ToList();

            return View(data);
        }

        public ActionResult SchedulesAssignedInd(int id, int ? page)
        {
            var role = rs.GetRole(this.HttpContext.User.Identity.Name); //ROL DEL USUARIO LOGEADO

            List<course_schedule> c_sch_ids = new List<course_schedule>();//LISTA DE IDs DE HORARIOS DEL CURSO PASADO POR PARÁMETRO

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.course_schedule.Where(x => x.courses.course_id == id).OrderBy(x => x.course_id == id).ToPagedList(pageNumber, pageSize); //HORARIOS ASIGNADOS AL CURSO PASADO POR PARÁMETRO

            //foreach (var item in query) //RECORRE LOS HORARIOS Y GUARDA LOS IDs EN LA LISTA
            //{
            //    c_sch_ids.Add(item);
            //}

            var query2 = owldb.courses.Where(y => y.course_id == id).FirstOrDefault(); //DEVUELVE EL OBJETO DEL CURSO

            var loggedUser = rs.GetUser(this.HttpContext.User.Identity.Name); //USUARIO LOGEADO

            //BUSCA SI HAY UNA ASIGNACIÓN PREVIA ENTRE ESTE CURSO Y ESTE USUARIO
            var query3 = owldb.course_assignment.Where(a => a.user_id == loggedUser && a.course_id == id).FirstOrDefault();
            var query4 = owldb.course_assignment.Where(a => a.user_id == loggedUser).FirstOrDefault();

            if (query3 == null || role == 2) {
                if (query4!=null)
                {
                    ViewBag.sch2 = query4.schedule_id;
                    ViewBag.preenroll = 2;
                }
                else
                {
                    ViewBag.preenroll = 0;
                }
            } 
            else {
                ViewBag.sch = query3.schedule_id;
                if (query3.status.Equals("1")) { //El estado en 1 significa que ya está matriculado
                    ViewBag.preenroll = 2;
                }
                else { //El estado en 01 significa que está en espera de aprobación de matricula (PRE-MATRICULADO)
                    ViewBag.preenroll = 1;
                }
            }

            if (Request.IsAuthenticated)
            {
                ViewBag.role = role;
                ViewBag.user = loggedUser;
            }
            else
            {
                ViewBag.role = 4;
            }


            ViewBag.course = query2.name;

            if (query2.name.Contains("Principiante"))
            {
                ViewBag.type = 1;
            } 
            else if (query2.name.Contains("Intermedio"))
            {
                ViewBag.type = 2;
            }
            else
            {
                ViewBag.type = 3;
            }

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            
            return View(query);
        }

        //Asigna un horario a un curso
        public ActionResult AsignScheduleToCourse(int id)
        {
            ViewBag.course = owldb.courses.Where(x => x.course_id == id);

            var schedules = owldb.schedules.Select(x => x);

            List<schedules> list = new List<schedules>();
            foreach (var item in schedules)
            {
                //Se crean variables para representar las horas de la forma 00:00
                var st = item.star_time.ToString();
                var newST = st.Remove(5, 3);

                var et = item.end_time.ToString();
                var newET = et.Remove(5, 3);

                //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                schedules sch = new schedules();
                sch.schedule_id = item.schedule_id;
                sch.day = item.day + " (" + newST + " - " + newET + ")";
                list.Add(sch);
            }

            ViewBag.schedule = list;

            return View();
        }

        [HttpPost]
        public ActionResult AsignScheduleToCourse(course_schedule a)
        {
            try
            {
                var query = owldb.course_schedule.Where(w => w.course_id == a.course_id && w.schedule_id == a.schedule_id).FirstOrDefault();

                if (query == null)
                {
                    owldb.course_schedule.Add(a);
                    owldb.SaveChanges();

                    return RedirectToAction("Index", "Courses");
                }
                else
                {
                    ViewBag.course = owldb.courses.Where(x => x.course_id == a.course_id);

                    var schedules = owldb.schedules.Select(x => x);

                    List<schedules> list = new List<schedules>();
                    foreach (var item in schedules)
                    {
                        //Se crean variables para representar las horas de la forma 00:00
                        var st = item.star_time.ToString();
                        var newST = st.Remove(5, 3);

                        var et = item.end_time.ToString();
                        var newET = et.Remove(5, 3);

                        //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                        schedules sch = new schedules();
                        sch.schedule_id = item.schedule_id;
                        sch.day = item.day + " (" + newST + " - " + newET + ")";
                        list.Add(sch);
                    }

                    ViewBag.schedule = list;

                    ViewBag.Error = "* El horario elegido ya se encuentra asignado a este curso.";

                    return View(a);
                }
                
            }
            catch (Exception e)
            {
                ViewBag.course = owldb.courses.Where(x => x.course_id == a.course_id);

                var schedules = owldb.schedules.Select(x => x);

                List<schedules> list = new List<schedules>();
                foreach (var item in schedules)
                {
                    //Se crean variables para representar las horas de la forma 00:00
                    var st = item.star_time.ToString();
                    var newST = st.Remove(5, 3);

                    var et = item.end_time.ToString();
                    var newET = et.Remove(5, 3);

                    //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                    schedules sch = new schedules();
                    sch.schedule_id = item.schedule_id;
                    sch.day = item.day + " (" + newST + " - " + newET + ")";
                    list.Add(sch);
                }

                ViewBag.schedule = list;

                return View(a);
            }

        }

        //Edita asignación de un horario a un curso
        public ActionResult EditSchAssignment(int id)
        {
            ViewBag.course = owldb.courses.Select(x => x);

            var schedules = owldb.schedules.Select(x => x);

            List<schedules> list = new List<schedules>();
            foreach (var item in schedules)
            {
                //Se crean variables para representar las horas de la forma 00:00
                var st = item.star_time.ToString();
                var newST = st.Remove(5, 3);

                var et = item.end_time.ToString();
                var newET = et.Remove(5, 3);

                //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                schedules sch = new schedules();
                sch.schedule_id = item.schedule_id;
                sch.day = item.day + " (" + newST + " - " + newET + ")";
                list.Add(sch);
            }

            ViewBag.schedule = list;

            course_schedule csh = new course_schedule();
            csh.course_sh_id = id;

            return View(csh);
        }

        [HttpPost]
        public ActionResult EditSchAssignment(course_schedule csh)
        {
            try
            {
                course_schedule cs = owldb.course_schedule.Single(q => q.course_sh_id == csh.course_sh_id);
                cs.schedule_id = csh.schedule_id;
                owldb.SaveChanges();

                return RedirectToAction("SchedulesAssigned");
            }
            catch (Exception ex)
            {
                return View(csh);
                throw;
            }
        }

        //Elinmina asignación de un horario a un curso
        public ActionResult DeleteSchAssignment(int id, string y)
        {
            var query = owldb.course_schedule.Where(x => x.course_sh_id == id).FirstOrDefault();

            var query2 = owldb.courses.Where(x => x.course_id == query.course_id).FirstOrDefault();

            var query3 = owldb.schedules.Where(x => x.schedule_id == query.schedule_id).FirstOrDefault();

            ViewBag.course = query2.name;
            ViewBag.id = query.course_id;
            ViewBag.schedule = query3.day + " (" + query3.star_time.ToString().Remove(5, 3) + " - " + query3.end_time.ToString().Remove(5, 3) + ")";

            return View();
        }

        [HttpPost]
        public ActionResult DeleteSchAssignment(int id)
        {
            var query = owldb.course_schedule.Where(a => a.course_sh_id == id).FirstOrDefault();
            owldb.course_schedule.Remove(query);
            owldb.SaveChanges();

            return RedirectToAction("Index","Courses");
        }





        //MUESTRA LA LISTA DE PRUEBAS ASIGNADAS A MODULOS
        public ActionResult TestsAssigned()
        {
            var data = owldb.module_vt.Select(z => z).ToList();

            return View(data);
        }

        public ActionResult TestsAssignedInd(int id, int idc)
        {
            var user = rs.GetUser(this.HttpContext.User.Identity.Name); //OBTIENE ID DEL USUARIO REGISTRADO
            var completedTest = 0;

            var query = owldb.module_vt.Where(x => x.modules.module_id == id).ToList();

            var query2 = owldb.modules.Where(y => y.module_id == id).FirstOrDefault();


            List<int> vtDoneIDs = new List<int>(); //LISTA PARA LOS IDs DE LAS PRUEBAS REALIZADAS POR EL ESTUDIANTE
            List<decimal> gotScore = new List<decimal>();

            foreach (var item in query)
            {
                var query3 = owldb.vt_scored_record.Where(a => a.user_id == user && a.vt_id == item.virtual_test).FirstOrDefault();
                if (query3 != null && query3.status != 3)
                {
                    vtDoneIDs.Add(query3.vt_id);
                    gotScore.Add(query3.score);
                }
            }

            
            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.idc = idc;
            ViewBag.id = id;

            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);
            ViewBag.ct = completedTest;

            ViewBag.vts = vtDoneIDs; //Se envían los ids de la VT realizadas
            ViewBag.vtScores = gotScore;

            ViewBag.module = query2.name;
            return View(query);
        }

        //Asigna una prueba a un módulo
        public ActionResult AsignTestToModule(int id)
        {
            ViewBag.module = owldb.modules.Where(x => x.module_id == id);

            ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1 || x.type == 3);

            return View();
        }

        [HttpPost]
        public ActionResult AsignTestToModule(module_vt mvt)
        {
            try
            {
                if (mvt.star_time.Hour > mvt.end_time.Hour ||
                    mvt.star_time.Hour == mvt.end_time.Hour && mvt.star_time.Minute > mvt.end_time.Minute){
                    ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                    ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                    ViewBag.Error = "* La Hora de Inicio debe ser Menor que la Hora de Finalización.";
                    return View(mvt);
                }
                else if (mvt.star_time== mvt.end_time)
                {
                    ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                    ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                    ViewBag.Error = "* La Hora de Inicio y de Finalización debe ser diferente.";
                    return View(mvt);
                }
                else
                {
                    var query = owldb.module_vt.Where(w => w.module == mvt.module && w.virtual_test == mvt.virtual_test).FirstOrDefault();

                    if (query == null || query != null && query.star_time != mvt.star_time && query.end_time != mvt.end_time)
                    {
                        owldb.module_vt.Add(mvt);
                        owldb.SaveChanges();
                        return RedirectToAction("Index", "Module");
                    }
                    else
                    {
                        ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                        ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                        ViewBag.Error = "* La Prueba seleccionada ya ha sido asignada a este módulo.";
                        return View(mvt);
                    }
                }

 
            }
            catch (Exception e)
            {
                ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                return View(mvt);
            }

        }

        //Edita asignación de una prueba a un módulo
        public ActionResult EditTestAssignment(int id)
        {
            var query = owldb.module_vt.Where(x => x.module_VT_id == id).FirstOrDefault();
            
            ViewBag.module = owldb.modules.Where(x => x.module_id == query.module);

            ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);

            ViewBag.moduleId = id;

            module_vt mvt = new module_vt();
            mvt.module_VT_id = id;
            mvt.module = query.module;
            mvt.virtual_test = query.virtual_test;
            mvt.min_grade = query.min_grade;
            mvt.star_time = query.star_time;
            mvt.end_time = query.end_time;

            return View(mvt);
        }

        [HttpPost]
        public ActionResult EditTestAssignment(module_vt mvt)
        {
            try
            {

                if (mvt.star_time.Hour > mvt.end_time.Hour ||
                    mvt.star_time.Hour == mvt.end_time.Hour && mvt.star_time.Minute > mvt.end_time.Minute)
                {
                    ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                    ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                    ViewBag.Error = "* La Hora de Inicio debe ser Menor que la Hora de Finalización.";
                    return View(mvt);
                }
                else if (mvt.star_time == mvt.end_time)
                {
                    ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                    ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                    ViewBag.Error = "* La Hora de Inicio y de Finalización debe ser diferente.";
                    return View(mvt);
                }
                else
                {
                    var query = owldb.module_vt.Where(w => w.module == mvt.module && w.virtual_test == mvt.virtual_test).FirstOrDefault();

                    if (query == null || (query.star_time != mvt.star_time || query.end_time != mvt.end_time || query.min_grade != mvt.min_grade))
                    {
                        module_vt mvTest = owldb.module_vt.Single(q => q.module_VT_id == mvt.module_VT_id);
                        mvTest.module = mvt.module;
                        mvTest.virtual_test = mvt.virtual_test;
                        mvTest.star_time = mvt.star_time;
                        mvTest.end_time = mvt.end_time;
                        mvTest.min_grade = mvt.min_grade;
                        owldb.SaveChanges();

                        return RedirectToAction("TestsAssignedInd", "Admin", new {id = mvt.module, idc = 0});
                    }
                    else
                    {
                        ViewBag.module = owldb.modules.Where(x => x.module_id == mvt.module);
                        ViewBag.test = owldb.virtual_tests.Where(x => x.type == 1);
                        ViewBag.Error = "* La Prueba ya fue Asignada o NO se ha Editado ningún Campo.";
                        return View(mvt);
                    }
                }

            }
            catch (Exception ex)
            {
                return View(mvt);
                throw;
            }
        }

        //Elimina asignación de un horario a un curso
        public ActionResult DeleteTestAssignment(int id, string y)
        {
            var query = owldb.module_vt.Where(x => x.module_VT_id == id).FirstOrDefault();

            var query2 = owldb.modules.Where(x => x.module_id == query.module).FirstOrDefault();

            var query3 = owldb.virtual_tests.Where(x => x.virtual_test_id == query.virtual_test).FirstOrDefault();

            ViewBag.module = query2.name;
            ViewBag.vt = query3.name;

            module_vt mvt = new module_vt();
            mvt.star_time = query.star_time;
            mvt.end_time = query.end_time;
            mvt.min_grade = query.min_grade;

            return View(mvt);
        }

        [HttpPost]
        public ActionResult DeleteTestAssignment(int id)
        {
            var query = owldb.module_vt.Where(a => a.module_VT_id == id).FirstOrDefault();
            owldb.module_vt.Remove(query);
            owldb.SaveChanges();

            return RedirectToAction("TestsAssignedInd","Admin",new { id = query.module, idc = 0});
        }







        //MUESTRA LA LISTA DE PREGUNTAS ASIGNADAS A PRUEBAS
        public ActionResult QuestionsAssigned()
        {
            var data = owldb.question_vt.Select(z => z).ToList();

            return View(data);
        }

        public ActionResult QuestionsAssignedInd(int id)
        {
            var query = owldb.question_vt.Where(x => x.virtual_tests.virtual_test_id == id).ToList();
            var query2 = owldb.virtual_tests.Where(y => y.virtual_test_id == id).FirstOrDefault();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);

            ViewBag.prueba = query2.name;
            return View(query);
        }


        //PRUEBAS PARA REALIZAR EXAMEN
        public ActionResult Solved(int id, int qvt_id)
        {
            int user = rs.GetUser(this.HttpContext.User.Identity.Name);
            var query = owldb.question_vt.Where(x => x.virtual_test == id).ToList();

            List<int> lista = new List<int>(); //LISTA DE LOS IDs DE LAS PREGUNTAS ASIGNADAS A LA PRUEBA
            foreach (var item in query)
            {
                lista.Add(item.question);
            }


            List<questions> lq = new List<questions>(); //Lista de preguntas de acuerdo a las asignadas a la prueba
            foreach (var item in lista)
            {
                var query2 = owldb.questions.Where(a => a.question_id == item).FirstOrDefault();
                var query3 = owldb.answer_vt.Where(a => a.user_id == user && a.question_id == query2.question_id).FirstOrDefault();

                string marked_ans = "";

                if (query3 != null)
                {
                    var query4 = owldb.questions.Where(x => x.question_id == query3.question_id).FirstOrDefault();

                    if (query3.got_answer == 1)
                    {
                        marked_ans = query4.answer1;
                    }else if (query3.got_answer == 2)
                    {
                        marked_ans = query4.answer2;
                    }else if (query3.got_answer == 3)
                    {
                        marked_ans = query4.answer3;
                    }
                    else
                    {
                        marked_ans = query4.answer4;
                    }

                    query2.answer1 = marked_ans;
                }
                else
                {
                    query2.answer1 = "N/A";
                }

                lq.Add(query2);
            }

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            
            ViewBag.vt = id;
            ViewBag.user = user;
            ViewBag.qvt = qvt_id;
            ViewBag.qty_q = query.Count();

            
            return View(lq);
        }

        public ActionResult AnswerQuestion(int id, int vt)
        {
            var query = owldb.questions.Where(a => a.question_id == id).FirstOrDefault();

            ViewBag.q = query.question;
            ViewBag.a1 = query.answer1;
            ViewBag.a2 = query.answer2;
            ViewBag.a3 = query.answer3;
            ViewBag.a4 = query.answer4;

            var query_qvt = owldb.question_vt.Where(a => a.virtual_test == vt && a.question == id).FirstOrDefault();

            ViewBag.qvt = query_qvt.question_vt_id;
            ViewBag.qid = id;
            ViewBag.vt = vt;

            List<int> ans = new List<int>();
            ans.Add(1); ans.Add(2); ans.Add(3); ans.Add(4);

            ViewBag.ans = ans;

            return View();
        }

        [HttpPost]
        public ActionResult AnswerQuestion(answer_vt avt)
        {
            var query = owldb.question_vt.Where(a => a.question_vt_id == avt.question_vt_id).FirstOrDefault();
            avt.user_id = rs.GetUser(this.HttpContext.User.Identity.Name);

            var query2 = owldb.answer_vt.Where(a => a.user_id == avt.user_id && a.question_vt_id == avt.question_vt_id
             && a.question_id == avt.question_id).FirstOrDefault();

            if (query2 == null)
            {
                owldb.answer_vt.Add(avt);
                owldb.SaveChanges();
            }
            else
            {
                answer_vt a_v_t = owldb.answer_vt.Single(a => a.answer_vt_id == query2.answer_vt_id);
                a_v_t.user_id = avt.user_id;
                a_v_t.question_vt_id = avt.question_vt_id;
                a_v_t.question_id = avt.question_id;
                a_v_t.got_answer = avt.got_answer;
                owldb.SaveChanges();
            }
            
            return RedirectToAction("Solved", "Admin", new { id = query.virtual_test, qvt_id  = query.question_vt_id});
        }

        public ActionResult CheckVirtualTest(int user, int vt)
        {
            var role = rs.GetRole(this.HttpContext.User.Identity.Name);

            var query = owldb.answer_vt.Where(a => a.user_id == user); //BUSCA LAS RESPUESTAS DEL USUARIO REGISTRADO
            var query2 = owldb.question_vt.Where(y => y.virtual_test == vt); //BUSCA LAS Q-VT DE ACUERDO A LA VT
            var query4 = owldb.virtual_tests.Where(w => w.virtual_test_id == vt).FirstOrDefault();



            int q_answered = query.Count(); //RESPUESTAS CONTESTADAS
            decimal q_total = query2.Count();   //TOTAL DE PREGUNTAS DE LA PRUEBA
            
            

            List<int> qvt_ids = new List<int>(); //LISTA DE Q-VT
            List<int> q_ids = new List<int>();   //LISTA DE PREGUNTAS
            List<int> got_answered = new List<int>(); //LISTA DE RESPUESTAS OBTENIDAS
            
            foreach (var item in query)
            {
                qvt_ids.Add(item.question_vt_id);
                q_ids.Add(item.question_id);
                got_answered.Add(item.got_answer);
            }

            List<int> correct_answers = new List<int>(); // LISTA DE RESPUESTAS CORRECTAS
            foreach (var item in q_ids) //RECORRE LOS IDs DE PREGUNTAS PARA CONSULTAR LAS RESPUESTAS CORRECTAS
            {
                var query3 = owldb.questions.Where(z => z.question_id == item).FirstOrDefault();
                correct_answers.Add(query3.correct_answer);
            }

            decimal correct_got_answer = 0; //PARA SUMAR CANTIDAD DE RESPUESTAS CORRECTAS OBTENIDAS

            for (int i = 0; i < correct_answers.Count; i++)
            {
                if (correct_answers[i] == got_answered[i])
                {
                    correct_got_answer++;
                }
            }

            decimal score = (correct_got_answer / q_total) * 100;
            
            ViewBag.score = Math.Round(score, 2);
            ViewBag.total_q = q_total;
            ViewBag.cga = correct_got_answer;
            ViewBag.unanswered = q_total - q_answered;
            ViewBag.vt = query4.name;
            ViewBag.role = role;


            //REMUEVE LOS REGISTROS DE LAS RESPUESTAS TEMPORALES
            foreach (var item in query)
            {
                owldb.answer_vt.Remove(item);
            }
            owldb.SaveChanges();

            //GUARDA EL REGISTRO DE LA PRUEBA PARA EL USUARIO
            vt_scored_record vtsr = new vt_scored_record();
            vtsr.user_id = user;
            vtsr.vt_id = vt;
            vtsr.total_questions = (int)q_total;
            vtsr.correct_got_answer = (int)correct_got_answer;
            vtsr.score = score;
            vtsr.record_date = DateTime.Now;

            owldb.vt_scored_record.Add(vtsr);
            owldb.SaveChanges();



            return View();
        }


        //Asigna una pregunta a una prueba
        public ActionResult AsignQuestionToTest(int id)
        {
            ViewBag.test = owldb.virtual_tests.Where(x => x.virtual_test_id == id);

            var query = owldb.question_vt.Where(a => a.virtual_test == id); //LAS PREGUNTAS QUE UNA PRUEBA TIENE ASIGNADAS

            var query2 = owldb.questions.Select(x => x); //TODAS LAS PREGUNTAS


            List<int> list = new List<int>();
            foreach (var item in query)
            {
                list.Add(item.question);
            }

            List<int> list2 = new List<int>();
            foreach (var item in query2)
            {
                list2.Add(item.question_id);
            }

            //PREGUNTAS RESULTANTES
            List<questions> l = new List<questions>();


            //ELIMINA LAS PREGUNTAS YA ASIGNADAS A LA PRUEBA
            foreach (var item in list)
            {
                foreach (var item2 in list2)
                {
                    if (item == item2)
                    {
                        list2.Remove(item2);
                        break;
                    }
                }
            }

            //DEVUELVE LA LISTA DE LAS PREGUNTAS DISPONIBLES
            foreach (var item in list2)
            {
                var query3 = owldb.questions.Where(p => p.question_id == item).FirstOrDefault();
                l.Add(query3);
            }

            if (l.Count==0)
            {
                ViewBag.list = 0;
            }
            else
            {
                ViewBag.list = 1;
                ViewBag.question = l;
            }

            


            return View();
        }

        [HttpPost]
        public ActionResult AsignQuestionToTest(question_vt qvt)
        {
            try
            {
                owldb.question_vt.Add(qvt);
                owldb.SaveChanges();
                return RedirectToAction("Index", "VirtualTest");
            }
            catch (Exception e)
            {
                return View(qvt);
            }

        }

        //Edita asignación de una prueba a un módulo
        public ActionResult EditQstAssignment(int id)
        {
            ViewBag.question = owldb.questions.Select(x => x);

            var query = owldb.question_vt.Where(x => x.question_vt_id == id).FirstOrDefault();

            question_vt qvt = new question_vt();
            qvt.question_vt_id = query.question_vt_id;
            qvt.question = query.question;
            return View(qvt);
        }

        [HttpPost]
        public ActionResult EditQstAssignment(question_vt qvt)
        {
            try
            {
                question_vt qvTest = owldb.question_vt.Single(q => q.question_vt_id == qvt.question_vt_id);
                qvTest.question = qvt.question;
                owldb.SaveChanges();

                return RedirectToAction("QuestionsAssigned");
            }
            catch (Exception ex)
            {
                return View(qvt);
                throw;
            }
        }

        //Elinmina asignación de un horario a un curso
        public ActionResult DeleteQstAssignment(int id, string y)
        {
            var query = owldb.question_vt.Where(x => x.question_vt_id == id).FirstOrDefault();

            var query2 = owldb.questions.Where(x => x.question_id == query.question).FirstOrDefault();

            var query3 = owldb.virtual_tests.Where(x => x.virtual_test_id == query.virtual_test).FirstOrDefault();

            ViewBag.question = query2.question;
            ViewBag.vt = query3.name;
            ViewBag.vtid = query3.virtual_test_id;

            question_vt qvt = new question_vt();
            qvt.question = query.question;
            qvt.virtual_test = query.virtual_test;

            return View(qvt);
        }

        [HttpPost]
        public ActionResult DeleteQstAssignment(int id)
        {
            var query = owldb.question_vt.Where(a => a.question_vt_id == id).FirstOrDefault();
            owldb.question_vt.Remove(query);
            owldb.SaveChanges();

            return RedirectToAction("QuestionsAssignedInd","Admin", new { id = id});
        }


        //RESULTADOS DE EXÁMENES
        public ActionResult TestResults(int ? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.vt_scored_record.Select(a => a).OrderBy(a => a.vt_scored_record_id).ToPagedList(pageNumber, pageSize);

            return View(query);
        }

        public ActionResult TestResultInd(int id, int m, int c)
        {
            var user = rs.GetUser(this.HttpContext.User.Identity.Name);
            var query = owldb.vt_scored_record.Where(a=> a.user_id == user && a.vt_id == id).ToList();
            var query2 = owldb.virtual_tests.Where(a => a.virtual_test_id == id).FirstOrDefault();
            var query3 = owldb.module_vt.Where(a => a.module == m && a.virtual_test == id).FirstOrDefault();

            if (query != null)
            {
                ViewBag.check = 1;
            }
            else
            {
                ViewBag.check = 0;
            }

            ViewBag.vt = query2.name;
            ViewBag.module = m;
            ViewBag.course = c;
            ViewBag.minScore = query3.min_grade;
            return View(query);
        }


        //CURSOS ASIGNADOS A UN PROFESOR
        public ActionResult CoursesAssignedInd(int id)
        {
            var query = owldb.course_assignment.Where(x => x.users.user_id == id).ToList();
            var query2 = owldb.users.Where(y => y.user_id == id).FirstOrDefault();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.Usuario = query2.first_name + " " + query2.last_name;
            return View(query);
        }

        //ESTUDIANTES ASIGNADOS A UN CURSO
        public ActionResult CoursesAssignedTC(int id, int schedule)
        {
            var query = owldb.course_assignment.Where(x => x.course_id == id && x.schedule_id == schedule && x.enrollTime != null).ToList();
            var query2 = owldb.courses.Where(w => w.course_id == id).FirstOrDefault();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.studentQty = query.Count;
                ViewBag.check = 1;
            }

            ViewBag.course = query2.name;
            ViewBag.courseId = query2.course_id;
            return View(query);
        }

        //-----------------------------------------------------------------------------------------------------------------

        //ASIGNACIÓN DE CURSOS A PROFESORES
        public ActionResult AsignCourseToUser(int id, string err)
        {
            ViewBag.user = owldb.users.Where(a => a.user_id == id);
            

            ViewBag.course = owldb.courses.Select(a => a);//TODOS LOS CURSOS;

            ViewBag.error = err;

            return View();
        }

        [HttpPost]
        public ActionResult AsignCourseToUser(course_assignment ca)
        {
            return RedirectToAction("CheckSchAvailable", "Admin", new { user = ca.user_id, course = ca.course_id });
        }

        //HORARIOS DISPONIBLES DEL CURSO ELEGIDO 
        public ActionResult CheckSchAvailable(int user, int course)
        {

            //Revisar los cursos asignados a este usuario
            var query = owldb.course_assignment.Where(a => a.user_id == user);

            //Lista de IDs de los Cursos Asignados del Profesor
            List<int> c_ids = new List<int>();

            foreach (var item in query)
            {
                c_ids.Add(item.course_id);
            }

            //Lista de los IDs de horarios de los cursos asignados al profesor
            List<int> user_c_sch_ids = new List<int>();

            foreach (var item in c_ids)
            {
                var query2 = owldb.course_schedule.Where(a => a.course_id == item).FirstOrDefault();
                user_c_sch_ids.Add(query2.schedule_id);
            }


            //Lista de horarios del curso pasado por parámetro
            List<int> par_c_sch_ids = new List<int>();

            //Obtiene los IDs de los horarios del curso para por parámetro
            var query3 = owldb.course_schedule.Where(a => a.course_id == course);

            foreach (var item in query3)
            {
                par_c_sch_ids.Add(item.schedule_id);
            }

            //ELIMINA LOS HORARIOS QUE COINCIDEN Y DEJA LOS DISPONIBLES
            foreach (var item in user_c_sch_ids)
            {
                foreach (var item2 in par_c_sch_ids)
                {
                    if (item == item2)
                    {
                        par_c_sch_ids.Remove(item2);
                        break;
                    }
                }
            }

            List<schedules> l = new List<schedules>();
            //DEVUELTE LA LISTA DE LOS HORARIOS DISPONIBLES DEL CURSO
            foreach (var item in par_c_sch_ids)
            {
                var query4 = owldb.schedules.Where(p => p.schedule_id == item).FirstOrDefault();

                //Se crean variables para representar las horas de la forma 00:00
                var st = query4.star_time.ToString();
                var newST = st.Remove(5, 3);

                var et = query4.end_time.ToString();
                var newET = et.Remove(5, 3);

                //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                schedules sch = new schedules
                {
                    schedule_id = query4.schedule_id,
                    day = query4.day + " (" + newST + " - " + newET + ")"
                };

                l.Add(sch);

            }

            if (l.Count > 0)
            {
                ViewBag.sch = l;
                ViewBag.u_id = user;
                ViewBag.user = owldb.users.Where(a => a.user_id == user);
                ViewBag.course = owldb.courses.Where(a => a.course_id == course);

                return View();
            }
            else
            {
                return RedirectToAction("AsignCourseToUser","Admin", new { id = user, 
                    err= "Error: Los Horarios del Curso elegido interfieren con sus otros Cursos o No tienen Horarios Asignados." });
            }

        }

        //ASIGNA CURSO-HORARIO AL PROFESOR
        [HttpPost]
        public ActionResult CheckSchAvailable(course_assignment ca)
        {
            try
            {
                ca.status = "1";
                owldb.course_assignment.Add(ca);
                owldb.SaveChanges();
                return RedirectToAction("IndexProf", "User");
            }
            catch (Exception e)
            {
                ViewBag.user = owldb.users.Where(a => a.user_id == ca.user_id).FirstOrDefault();
                ViewBag.course = owldb.courses.Where(a => a.course_id == ca.course_id).FirstOrDefault();
                return View(ca);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------


        //Edita asignación de una prueba a un módulo
        public ActionResult EditCourseAssignment(int id)
        {
            List<String> status = new List<String>
            {
                "0",
                "1"
            };

            ViewBag.status = status;

            var query = owldb.course_assignment.Where(x => x.user_id == id).FirstOrDefault();

            return View(query);
        }

        [HttpPost]
        public ActionResult EditCourseAssignment(course_assignment c_asign)
        {
            try
            {
                course_assignment ca = owldb.course_assignment.Single(q => q.course_assign_id == c_asign.course_assign_id);
                ca.course_id = c_asign.course_id;
                ca.status = c_asign.status;
                owldb.SaveChanges();

                return RedirectToAction("QuestionsAssigned");
            }
            catch (Exception ex)
            {
                return View(c_asign);
                throw;
            }
        }

        //Elimina asignación de un horario a un curso
        public ActionResult DeleteCourseAssignment(int id, string y)
        {
            var query = owldb.course_assignment.Where(x => x.user_id == id).FirstOrDefault();

            var query2 = owldb.users.Where(x => x.user_id == query.user_id).FirstOrDefault();

            var query3 = owldb.courses.Where(x => x.course_id == query.course_id).FirstOrDefault();

            ViewBag.user = query2.first_name + " " + query2.last_name;
            ViewBag.course = query3.name;

            course_assignment ca = new course_assignment
            {
                course_id = query.course_id,
                user_id = query.user_id,
                status = query.status
            };

            return View(ca);
        }

        [HttpPost]
        public ActionResult DeleteCourseAssignment(int id)
        {
            var query = owldb.course_assignment.Where(a => a.user_id == id).FirstOrDefault();
            owldb.course_assignment.Remove(query);
            owldb.SaveChanges();

            return RedirectToAction("Index","CoursesAssignedInd", new { id = id});
        }




        //MATRICULAR CURSO
        public ActionResult EnrollCourse(int course, int schedule, int user)
        {
            ViewBag.user = owldb.users.Where(x => x.user_id == user);
            ViewBag.course = owldb.courses.Where(x => x.course_id == course);
            ViewBag.u = course;

            var schedules = owldb.schedules.Where(x => x.schedule_id == schedule);

            List<schedules> list = new List<schedules>();
            foreach (var item in schedules)
            {
                //Se crean variables para representar las horas de la forma 00:00
                var st = item.star_time.ToString();
                var newST = st.Remove(5, 3);

                var et = item.end_time.ToString();
                var newET = et.Remove(5, 3);

                //Se crea un objeto para setear las variables de las horas y pasarlas a la vista
                schedules sch = new schedules();
                sch.schedule_id = item.schedule_id;
                sch.day = item.day + " (" + newST + " - " + newET + ")";
                list.Add(sch);
            }

            ViewBag.schedule = list;

            return View();
        }

        [HttpPost]
        public ActionResult EnrollCourse(course_assignment ca)
        {
            ca.status = "0";
            ca.enrollTime = DateTime.Now;
            owldb.course_assignment.Add(ca);
            owldb.SaveChanges();
            return RedirectToAction("Index", "Home");
        }



        //VER ESTUDIANTES EN ESPERA DE CONFIRMACIÓN DE MATRICULA
        public ActionResult PreEnrolledStudents()
        {
            var query = owldb.course_assignment.Where(s => s.status.Equals("0") && s.schedule_id != null).ToList();

            if (query.Count != 0)
            {
                ViewBag.check = 1;
            }
            else
            {
                ViewBag.check = 0;
            }

            return View(query);
        }


        //ACEPTAR MATRICULA DE ESTUDIANTE
        public ActionResult ConfirmEnrollment(int id)
        {
            var query = owldb.course_assignment.Where(s => s.course_assign_id == id).FirstOrDefault();

            return View(query);
        }

        [HttpPost]
        public ActionResult ConfirmEnrollment(course_assignment csca)
        {
            course_assignment ca = owldb.course_assignment.Where(s => s.course_assign_id == csca.course_assign_id).FirstOrDefault();
            ca.status = "1";
            owldb.SaveChanges();
            return RedirectToAction("IndexEnrollStudents", "User");
        }

        //Vista Create de video
        public ActionResult AddMedia()
        {
            return View();
        }

        //post create/video

        [HttpPost]
        public ActionResult AddMedia(multimedia model) {

            model.Estado = 1;
            owldb.multimedia.Add(model);
            owldb.SaveChanges();
            return RedirectToAction("CheckMedia","Admin");
        }

        // Devuelve lista de Multimedia
        public ActionResult CheckMedia(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.multimedia.OrderBy(a => a.Description).ToPagedList(pageNumber, pageSize);

            if (query.Count!=0)
            {
                ViewBag.check = 1;
            }
            else
            {
                ViewBag.check = 0;
            }

            return View(query);

        }

        public ActionResult CheckMediaInd(int id, int idc)
        {
            var query = owldb.multimedia.Where(x => x.idMultimedia == id).ToList();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.c = idc;

            return View(query);
        }

        public ActionResult CheckMediaModule(int id, int idc)
        {
            var query = owldb.Multi_Modulo.Where(x => x.module_id == id).ToList();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            List<multimedia> mids = new List<multimedia>();
            foreach (var item in query)
            {
                var query1 = owldb.multimedia.Where(a => a.idMultimedia == item.idMultimedia).FirstOrDefault();
                if (query1!=null)
                {
                    mids.Add(query1);
                }
            }

            ViewBag.c = idc;

            return View(mids);
        }


        public ActionResult deleteMedia(int id, string s)
        {
            var query = owldb.multimedia.Where(a => a.idMultimedia == id).FirstOrDefault();

            if (query.Estado==1)
            {
                ViewBag.status = "Deshabilitar";
            }
            else
            {
                ViewBag.status = "Habilitar";
            }

            List<int> status = new List<int>();
            status.Add(0);
            status.Add(1);

            ViewBag.status2 = status;

            return View(query);
        }

            //Delete de video de lista sin confirmacion
        [HttpPost]
        public ActionResult deleteMedia (multimedia m) {

            multimedia mtm = owldb.multimedia.Where(a => a.idMultimedia == m.idMultimedia).FirstOrDefault();
            mtm.Estado = m.Estado;
            owldb.SaveChanges();

            return RedirectToAction("CheckMedia");

        }



//----------------------------------------------------Control de Archivos-----------------------//
        //Vista de Archivos de material
       [HttpGet]
        public ActionResult FilesView() {


            return View();
        }

        //Crea Archivo de material en bd y lo guarda en Downloads_modules
        [HttpPost]
        public ActionResult FilesView(FileControl fc)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    
                    owldb.FileControl.Add(fc);
                }
                catch (Exception e)
                {

                    ViewBag.Message = "Error al Conectar con el servidor";
                }

            }
            owldb.SaveChanges();
            return RedirectToAction("FilesList");
        }

        public ActionResult FilesList(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.FileControl.OrderBy(a => a.FileName).ToPagedList(pageNumber, pageSize);

            ViewBag.role = rs.GetRole(this.HttpContext.User.Identity.Name);

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            return View(query);
        }

        public ActionResult FileListInd(int id, int idc) {

            var query = owldb.FileControl.Where(x => x.idFile == id).ToList();
            var q2 = owldb.FC_Module.Where(x => x.module_id == x.module_id).FirstOrDefault();
            
            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }

            ViewBag.moduleName = q2.modules.name;
            
            ViewBag.c = idc;

            return View(query);

            
        }

        public ActionResult FileListModule(int id, int idc)
        {

            var query = owldb.FC_Module.Where(x => x.module_id == id).ToList();

            if (query.Count == 0)
            {
                ViewBag.check = 0;
            }
            else
            {
                ViewBag.check = 1;
            }


            ViewBag.c = idc;

            return View(query);


        }


        //Vista Create de video
        public ActionResult AddFiles()
        {
            return View();
        }

        //post create/video

        [HttpPost]
        public ActionResult AddFiles(FileControl fc)
        {
            owldb.FileControl.Add(fc);
            owldb.SaveChanges();
            return RedirectToAction("FilesList", "Admin");
        }

        public ActionResult DeleteFile(int id, string l)
        {
            var query = owldb.FileControl.Where(a => a.idFile == id).FirstOrDefault();
            FileControl fc = new FileControl();
            fc.FileName = query.FileName;
            fc.FilePath = query.FilePath;
            fc.FileType = query.FileType;
            
            return View(fc);
        }

        // POST: Schedule/Delete/5
        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            try
            {
                var query = owldb.FileControl.Where(a => a.idFile == id).FirstOrDefault();
                owldb.FileControl.Remove(query);
                owldb.SaveChanges();

                return RedirectToAction("FilesList","Admin");
            }
            catch
            {
                return View();
            }
        }

    } 
}
    











