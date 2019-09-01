using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class AccountController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        

        public ActionResult AccountSection(string Login)
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn"}));
            }
            Login = Login.Trim();
            Администраторы admin = context.Администраторы.Single(x => x.Login == Login);
            return View(admin);
        }

        public ActionResult SaveNewSNP(string Name, string Surname, string Patronymic)
        {
            string Login = Session["Login"].ToString();
            Name = Name.Trim();
            Surname = Surname.Trim();
            Patronymic = Patronymic.Trim();
            Администраторы admin = context.Администраторы.Single(x => x.Login == Login);
            admin.Имя = Name;
            admin.Фамилия = Surname;
            admin.Отчество = Patronymic;
            context.SaveChanges();
            Session.RemoveAll();
            Session.Add("CurrentAdmin", admin);
            Session["Address"] = admin.Имя + " " + admin.Отчество;
            Session["Login"] = admin.Login;
            ViewBag.Success = "ФИО были успешно обновлены.";
            return PartialView("Success");
        }
        public ActionResult ChangePassword (string OldPassword, string NewPassword1, string NewPassword2)
        {
            string Login = Session["Login"].ToString();
            OldPassword = OldPassword.Trim();
            NewPassword1 = NewPassword1.Trim();
            NewPassword2 = NewPassword2.Trim();
            try
            {
                Администраторы admin = context.Администраторы.Single(x => x.Login == Login);
                if (admin.Password != OldPassword)
                {
                    ViewBag.Error = "Ваш старый пароль был введен неверно. Пожалуйста, проверьте введенный пароль и попробуйте снова.";
                    return PartialView("Error");
                }
                if (admin.Password == OldPassword)
                {
                    if (NewPassword1 != NewPassword2)
                    {
                        ViewBag.Error = "Введенные новые пароли неверны. Пожалуйста, проверьте правильность новых введенных паролей и попробуйте снова.";
                        return PartialView("Error");
                    }

                    admin.Password = NewPassword1;
                    context.SaveChanges();

                    ViewBag.Success = "Пароль успешно обновлен.";
                    return PartialView("Success");
                }

            }
            catch(Exception)
            {

            }
            return View();
        }
    }
}