using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using mysqltest.Models;
using Owlapp.Tools;

namespace Owlapp.Controllers
{
    public class AccountController : Controller
    {
        owldbEntities01 owldb = new owldbEntities01();
        string secretKey = ConfigurationManager.AppSettings["SecretKey"];
        MailSender ms = new MailSender();
        public object EnableSsl { get; private set; }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        #region Registro

        public void registerMethod(users u)
        {
            //Agrega usuarios a la base de datos
            u.password = Seguridad.EncryptString(secretKey, u.password);
            owldb.users.Add(u);
            owldb.SaveChanges();

            //Asigna el rol de estudiante al usuario registrado desde el Sign Up
            role_user ru = new role_user
            {
                role_id = 3,
                user_id = u.user_id
            };

            owldb.role_user.Add(ru);
            owldb.SaveChanges();
        }

        [HttpPost]
        public ActionResult Registration(users u)
        {
            var secretKey = ConfigurationManager.AppSettings["SecretKey"];

            var query = owldb.users.Where(a => a.dni == u.dni || a.email == u.email).FirstOrDefault();

            if (query == null){
                registerMethod(u);
                return RedirectToAction("Login", "Account");
            }
            else if (query.dni == u.dni){
                ViewBag.Error = "* La identificación ingresada ya se encuentra previamente registrada.";
                return View();
            }
            else{
                ViewBag.Error = "* El correo ingresado ya se encuentra previamente registrado.";
                return View();
            }

        }
        #endregion

        public ActionResult Login()
        {
            return View();
        }

        #region login
        [HttpPost]
        public ActionResult Login(users u)
        {
            //CONSULTA A USUARIO CON LAS CREDENCIALES INGRESADAS
            var query = owldb.users.Where(w => w.email == u.email).FirstOrDefault();

            if (u.email != null)
            {
                if (query != null)
                {
                    //Usuario con contraseña creada por si mismo, no generada automaticamente
                    if (query.password.Length > 9)
                    {
                        var encryptedPassword = Seguridad.EncryptString(secretKey, u.password);

                        var lg = owldb.users.Where(a => a.email.Equals(u.email) && a.password.Equals(encryptedPassword)).FirstOrDefault();

                        if (lg != null)
                        {
                            var query2 = owldb.role_user.Where(w => w.user_id == query.user_id).FirstOrDefault();
                            Session["Identificacion"] = lg.dni.ToString();
                            Session["UserId"] = lg.user_id.ToString();
                            Session["Nombre"] = lg.first_name.ToString() + " " + lg.last_name;
                            Session["role"] = query2.role_id.ToString();

                            FormsAuthentication.SetAuthCookie(u.email, u.rememberme);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Error = "* Correo Electrónico o Contraseña Incorrectos.";
                            return View("Login");
                        }
                    }
                    else //Usuario Admin o Prof recién registrado con contraseña enviada por mail
                    {
                        var lg = owldb.users.Where(a => a.email.Equals(u.email) && a.password.Equals(u.password)).FirstOrDefault();

                        if (lg != null)
                        {
                            var query2 = owldb.role_user.Where(w => w.user_id == query.user_id).FirstOrDefault();
                            Session["Identificacion"] = lg.dni.ToString();
                            Session["UserId"] = lg.user_id.ToString();
                            Session["Nombre"] = lg.first_name.ToString() + " " + lg.last_name;
                            Session["role"] = query2.role_id.ToString();
                            FormsAuthentication.SetAuthCookie(u.email, u.rememberme);
                            return RedirectToAction("UpdatePassword", "User", new { id = lg.user_id });
                        }
                        else
                        {
                            ViewBag.Error = "* Correo Electrónico o Contraseña Incorrectos.";
                            return View("Login");
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "* Correo Electrónico o Contraseña Incorrectos.";
                    return View("Login");
                }

            }
            return View("Login");

        }
        #endregion



        #region Salir
        public ActionResult Salir()
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
        #endregion


        public ActionResult ValidationCode()
        {
            return View();
        }

        #region CodigoValidacion
        [HttpPost]
        public ActionResult ValidationCode(users u)
        {
            //query 1 consulta el id del usuario por medio del Email.
            //query 2 consulta codigo de verificacion en la tabla pass_verif_code en caso que haya uno existente
            //query 3 Consulta del codigo de verificacion del query 1


            //CONSULTA ID USUARIO POR EMAIL
            var query = owldb.users.Where(a => a.email.Equals(u.email)).FirstOrDefault();

            if (query != null)
            {
                //BUSCA CÓDIGOS DE VIRIFICACIÓN POR ID DE USUARIO
                var query2 = owldb.pass_verif_code.Where(a => a.user_id.Equals(query.user_id)).FirstOrDefault();

                if (query2 != null)
                {
                    owldb.pass_verif_code.Remove(query2);
                    owldb.SaveChanges();
                }

                //GENERA CÓDIGO DE VERIFICACIÓN
                owldb.generateVerificationCode(u.email);
                owldb.SaveChanges();

                var query3 = owldb.pass_verif_code.Where(a => a.user_id.Equals(query.user_id)).FirstOrDefault();

                ms.SendEmail(u.email, query.first_name+" "+query.last_name, "Owl Learnig: Verification Code", "Hola " + query.first_name + " " + query.last_name + ", tu código de verificación es: " + query3.verification_code);

                return RedirectToAction("VerifyValidationCode", "Account");

            }
            else
            {
                ViewBag.Respuesta = "Email No Encontrado";
                return View("ValidationCode");
            }

            
        }
        #endregion

        public ActionResult VerifyValidationCode()
        {
            return View();
        }

        #region VerificacodigodeValidacion
        [HttpPost]
        public ActionResult VerifyValidationCode(pass_verif_code pvc)
        {
            var query = owldb.pass_verif_code.Where(a => a.verification_code.Equals(pvc.verification_code)).FirstOrDefault();

            if (query != null)
            {
                return RedirectToAction("ChangePassword", "Account", new { id = query.user_id});
                
                ViewBag.Respuesta = "Ok: "+query.user_id;
            }
            else
            {
                ViewBag.Respuesta = "Bad: ";
            }

            return View();

        }
        #endregion


        public ActionResult ChangePassword()
        {
            return View();
        }
        #region CambiodeContraseña
        [HttpPost]
        public ActionResult ChangePassword(users u, String id)
        {
            
            var query = owldb.users.Where(a => a.user_id.ToString().Equals(id)).FirstOrDefault();

            var encryptedPassword = Seguridad.EncryptString(secretKey, u.password);

            if (encryptedPassword != query.password)
            {
                users us = owldb.users.Single(q => q.user_id.ToString().Equals(id));
                us.password = encryptedPassword;
                owldb.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View("ChangePassword");
            }

        }
        #endregion



        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
 }
