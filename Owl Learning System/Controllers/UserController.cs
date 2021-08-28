using mysqltest.Models;
using Owlapp.Tools;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace mysqltest.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        RandomPassword rp = new RandomPassword();
        MailSender ms = new MailSender();
        string secretKey = ConfigurationManager.AppSettings["SecretKey"];


        // GET: User
        public ActionResult IndexAdmin(int? page)
        {

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.role_user.Where(a => a.role_id == 1).OrderBy(a => a.user_id).ToPagedList(pageNumber, pageSize);


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

        public ActionResult IndexProf(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.role_user.Where(a => a.role_id == 2).OrderBy(a => a.user_id).ToPagedList(pageNumber, pageSize);


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

        public ActionResult IndexStudent(int? page) {

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var query = owldb.role_user.Where(a=>a.role_id == 3).OrderBy(a => a.user_id).ToPagedList(pageNumber, pageSize);

            
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

        // GET: User/Create
        public ActionResult CreateAdmin()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult CreateAdmin(users u)
        {
            var query = owldb.users.Where(x => x.email == u.email).FirstOrDefault();
            var query2 = owldb.users.Where(x => x.dni == u.dni).FirstOrDefault();

            try
            {
                if (query == null && query2 == null)
                {
                    var secretKey = ConfigurationManager.AppSettings["SecretKey"];

                    String pass = rp.generateRandomPassword(9);

                    //Se crea una contraseña aleatoria para el administrador
                    u.password = pass;

                    //AGREGA USUARIO
                    owldb.users.Add(u);
                    owldb.SaveChanges();

                    role_user ru = new role_user();
                    ru.role_id = 1;
                    int uid = u.user_id;
                    ru.user_id = uid;

                    //ASIGNA EL ROL DE ADMINISTRADOR
                    owldb.role_user.Add(ru);
                    owldb.SaveChanges();

                    ms.SendMail(u.email,
                                "¡Bienvenido a Owl Learning! Ahora eres un Administrador.",
                                "Hola " + u.first_name + " " + u.last_name + ", has sido registrado como un Administrador." + "/n" +
                                "Tu usuario es: " + u.email + " y tu contraseña temporal es: " + pass);

                    return RedirectToAction("IndexAdmin", "User");
                }
                else if (query != null && query2 != null)
                {
                    ViewBag.Error = "* Datos de usuario ingreados ya se encuentran registrados.";
                    return View(u);
                }
                else if (query != null)
                {
                    ViewBag.Error = "* El correo ingresado ya se encuentra registrado.";
                    return View(u);
                }
                else
                {
                    ViewBag.Error = "* La identificación ingreasada ya se encuentra registrada.";
                    return View(u);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }
        }

        public ActionResult DeleteUser(int id, String x)
        {
            var query = owldb.role_user.Where(a => a.user_id == id).FirstOrDefault();

            return View(query);
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var query = owldb.role_user.Where(a => a.user_id == id).FirstOrDefault();
                owldb.role_user.Remove(query);

                var query2 = owldb.users.Where(a => a.user_id == id).FirstOrDefault();
                owldb.users.Remove(query2);

                owldb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult CreateProf()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult CreateProf(users u)
        {
            var query = owldb.users.Where(x => x.email == u.email).FirstOrDefault();
            var query2 = owldb.users.Where(x => x.dni == u.dni).FirstOrDefault();

            try
            {
                if (query==null && query2==null)
                {
                    var secretKey = ConfigurationManager.AppSettings["SecretKey"];

                    String pass = rp.generateRandomPassword(9);

                    //Se crea una contraseña aleatoria para el administrador
                    u.password = Seguridad.EncryptString(secretKey, pass);

                    //AGREGA USUARIO
                    owldb.users.Add(u);
                    owldb.SaveChanges();

                    role_user ru = new role_user();
                    ru.role_id = 2;
                    int uid = u.user_id;
                    ru.user_id = uid;

                    //ASIGNA EL ROL DE ADMINISTRADOR
                    owldb.role_user.Add(ru);
                    owldb.SaveChanges();

                    ms.SendMail(u.email,
                                "¡Bienvenido a Owl Learning! Ahora eres un Profesor de nuestra academia.",
                                "Hola " + u.first_name + " " + u.last_name + ", has sido registrado como un Profesor." + "\n" +
                                "Tu usuario es: " + u.email + " y tu contraseña temporal es: " + pass + "\n" +
                                "Al iniciar sesión por primera vez, deberás cambiar tu contraseña.");

                    return RedirectToAction("IndexAdmin", "User");
                }else if (query != null && query2 != null)
                {
                    ViewBag.Error = "* Datos de usuario ingreados ya se encuentran registrados.";
                    return View(u);
                }
                else if (query != null)
                {
                    ViewBag.Error = "* El correo ingresado ya se encuentra registrado.";
                    return View(u);
                }
                else
                {
                    ViewBag.Error = "* La identificación ingreasada ya se encuentra registrada.";
                    return View(u);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }
        }


        #region CambiodeContraseña
        //CAMBIAR CONTRASEÑA POR DEFECTO ENVIADA EL CORREO
        public ActionResult UpdatePassword()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult UpdatePassword(users u, int id)
        {
            var query = owldb.users.Where(l => l.user_id == id).FirstOrDefault();

            if (u.first_name.Equals(query.password))
            {
                var encryptedPassword = Seguridad.EncryptString(secretKey, u.password);
                users us = owldb.users.Single(q => q.user_id == id);
                us.password = encryptedPassword;
                owldb.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "*Contraseña Actual Incorrecta";
                return View(u);
            }

        }
        #endregion


        #region EdicionPerilesUsuario
        //PARA CONFIGURACIÓN DE PERFILES

        // EDICIÓN DE ADMINISTRADOR - GET
        public ActionResult EditAdmin(int id)
        {
            var query = owldb.users.Where(o => o.user_id == id).FirstOrDefault();
            return View(query);
        }

        // EDICIÓN DE ADMINISTRADOR - POST
        [HttpPost]
        public ActionResult EditAdmin(users u)
        {
            try
            {
                users us = owldb.users.Single(p => p.user_id == u.user_id);
                us.first_name = u.first_name;
                us.last_name = u.last_name;
                us.phone_number = u.phone_number;
                us.email = u.email;
                owldb.SaveChanges();
                return RedirectToAction("IndexAdmin","Profile");
            }
            catch
            {
                return View();
            }
        }


        // EDICIÓN DE PROFESOR - GET
        public ActionResult EditProf(int id)
        {
            var query = owldb.users.Where(o => o.user_id == id).FirstOrDefault();
            return View(query);
        }

        // EDICIÓN DE PROFESOR - POST
        [HttpPost]
        public ActionResult EditProf(users u)
        {
            try
            {
                users us = owldb.users.Single(p => p.user_id == u.user_id);
                us.first_name = u.first_name;
                us.last_name = u.last_name;
                us.phone_number = u.phone_number;
                us.email = u.email;
                owldb.SaveChanges();
                return RedirectToAction("IndexProf", "Profile");
            }
            catch
            {
                return View();
            }
        }


        // EDICIÓN DE ESTUDIANTE - GET
        public ActionResult EditStudent(int id)
        {
            var query = owldb.users.Where(o => o.user_id == id).FirstOrDefault();
            return View(query);
        }

        // EDICIÓN DE ESTUDIANTE - POST
        [HttpPost]
        public ActionResult EditStudent(users u)
        {
            try
            {
                users us = owldb.users.Single(p => p.user_id == u.user_id);
                us.first_name = u.first_name;
                us.last_name = u.last_name;
                us.phone_number = u.phone_number;
                us.email = u.email;
                owldb.SaveChanges();
                return RedirectToAction("IndexStudent", "Profile");
            }
            catch
            {
                return View();
            }
        }
        #endregion



        public ActionResult Disable(int id, int id2)
        {
            var query = owldb.users.Where(w => w.user_id == id).FirstOrDefault();
            ViewBag.role = id2;
            return View(query);
        }

        [HttpPost]
        public ActionResult Disable(users u)
        {
            try
            {
                users us = owldb.users.Single(q => q.user_id == u.user_id);
                us.status = false;
                owldb.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                return View();
            }
        }




        public ActionResult Enable(int id, int id2)
        {
            var query = owldb.users.Where(w => w.user_id == id).FirstOrDefault();
            ViewBag.role = id2;
            return View(query);
        }

        [HttpPost]
        public ActionResult Enable(users u)
        {
            try
            {
                users us = owldb.users.Single(q => q.user_id == u.user_id);
                us.status = true;
                owldb.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                return View();
            }
        }
    }
}
