using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using mysqltest.Models;
using Owl_Learning_2022.Models;
using Owlapp.Tools;

namespace Owl_Learning_2022.Controllers
{
    public class AccountController : Controller
    {

        owldbEntities owldb = new owldbEntities();
        string secretKey = ConfigurationManager.AppSettings["SecretKey"];
        RoleSearching rs = new RoleSearching();
        MailSender ms = new MailSender();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        #region Registration

        public ActionResult Registration()
        {
            return View();
        }

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

            if (u.dni == null)
            {
                ViewBag.Error = "* Debe Completar todos los Espacios";
                return View();
            }
            else
            {
                var query = owldb.users.Where(a => a.dni == u.dni || a.email == u.email).FirstOrDefault();

                if (query == null)
                {
                    registerMethod(u);
                    return RedirectToAction("Login", "Account");
                }
                else if (query.dni == u.dni)
                {
                    ViewBag.Error = "* La identificación ingresada ya se encuentra previamente registrada.";
                    return View();
                }
                else
                {
                    ViewBag.Error = "* El correo ingresado ya se encuentra previamente registrado.";
                    return View();
                }

            }



        }
        #endregion

        #region LogIn
        public ActionResult Login()
        {
            return View();
        }

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
                            //Session["UserId"] = query2.role_id.ToString();
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

        #region SendValidationCode
        public ActionResult ValidationCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidationCode(users u)
        {

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

                ms.SendEmail(u.email, query.first_name + " " + query.last_name, "Owl Learnig: Verification Code", "Hola " + query.first_name + " " + query.last_name + ", tu código de verificación es: " + query3.verification_code);

                return RedirectToAction("VerifyValidationCode", "Account");

            }
            else
            {
                ViewBag.Respuesta = "Email No Encontrado";
                return View("ValidationCode");
            }


        }

        #endregion

        #region VerifyValidationCode
        public ActionResult VerifyValidationCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyValidationCode(pass_verif_code pvc)
        {
            var query = owldb.pass_verif_code.Where(a => a.verification_code.Equals(pvc.verification_code)).FirstOrDefault();

            if (query != null)
            {
                return RedirectToAction("ChangePassword", "Account", new { id = query.user_id });

            }
            else
            {
                ViewBag.Respuesta = "* Código de Verificación Incorrecto";
            }

            return View();

        }
        #endregion

        #region PasswordChange
        public ActionResult ChangePassword(int id)
        {
            users u = new users();
            u.user_id = id;
            return View(u);
        }

        [HttpPost]
        public ActionResult ChangePassword(users u)
        {
            var query = owldb.users.Where(l => l.user_id == u.user_id).FirstOrDefault();

            var decryptedPassword = Seguridad.DecryptString(secretKey, query.password);

            if (!decryptedPassword.Equals(u.password))
            {
                if (u.password.Equals(u.last_name))
                {
                    var encryptedPassword = Seguridad.EncryptString(secretKey, u.password);
                    users us = owldb.users.Single(q => q.user_id == u.user_id);
                    us.password = encryptedPassword;
                    owldb.SaveChanges();

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Error = "* Error de Confirmación de Contraseña";
                    return View(u);
                }
            }
            else
            {
                ViewBag.Error = "* Su nueva contraseña debe ser diferente a su anterior contraseña.";
                return View(u);
            }



        }

        #endregion

        #region LogOut
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

    }
}