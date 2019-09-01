using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class SignInController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        public ActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public ActionResult LoggingIn(string Login, string Password)
        {
            try
            {
                Администраторы admin = context.Администраторы.Single(x => x.Login == Login);
                if (admin.Password != Password.Trim())
                {
                    ViewBag.Error = "Неверные логин или пароль. Пожалуйста, проверьте введенные данные и попробуйте снова.";
                }
                if (admin.Password == Password.Trim())
                {
                    Session.Add("CurrentAdmin", admin.Login);
                    Session["Address"] = admin.Имя + " " + admin.Отчество;
                    Session["Login"] = admin.Login;
                    ViewBag.Address = @"Здравствуйте, " + admin.Имя + " " + admin.Отчество.Trim() + "!";
                }
            }
            catch(Exception)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn"}));
            }

            return View();
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();

            return RedirectToAction("Index", new RouteValueDictionary(new { Controller = "Books", Action = "Index" }));
        }

        public ActionResult Register()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            return View(); 
        }

        [HttpPost]
        public ActionResult SaveNewAdministrator(string Name, string Surname, string Patronymic, string Login, string Password1, string Password2)
        {
            List<Администраторы> admins = context.Администраторы.ToList();
            try
            {
                for(int i = 0; i < admins.Count; i++)
                {
                    if (admins[i].Login == Login)
                    {
                        ViewBag.Error = "Такой логин уже занят. Пожалуйста, введите другой логин и попробуйте снова.";
                        return PartialView("Error");
                    }
                    if (admins[i].Login != Login)
                    {
                        if (Password1 != Password2)
                        {
                            ViewBag.Error = "Введенные пароли неверны. Пожалуйста, проверьте правильность введенных паролей и попробуйте снова.";
                            return PartialView("Error");
                        }
                        if (Password1 == Password2)
                        {
                            Администраторы admin = new Администраторы
                            {
                                Login = Login,
                                Password = Password1,
                                Имя = Name,
                                Отчество = Patronymic,
                                Фамилия = Surname
                            };
                            context.Администраторы.Add(admin);
                            context.SaveChanges();
                            ViewBag.Success = "Администратор был успешно добавлен.";
                            return PartialView("Success");
                        }
                     
                    }
                }
            }
            catch(Exception)
            {

            }
            return View();
        }
    }
}