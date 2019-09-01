using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class ConstructorsController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        public ActionResult Index()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            List<string> Authors = new List<string>();
            List<string> Books = new List<string>();
            List<string> Buildings = new List<string>();
            List<string> Publishers = new List<string>();
            List<string> Genres = new List<string>();
            List<object> myModel = new List<object>();

            foreach (var item in context.ТаблицаКниг.ToList())
            {
                Books.Add(item.ISBN + ". " + item.НазваниеКниги + " " + item.ГодВыпуска);
            }
            myModel.Add(Books);

            foreach (var item in context.ТаблицаАвторов.ToList())
            {
                Authors.Add(item.КодАвтора + ". " + item.Имя + " " + item.Фамилия);
            }
            myModel.Add(Authors);

            foreach (var item in context.ТаблицаЗданий.ToList())
            {
                Buildings.Add(item.КодЗдания + ". " + item.НазваниеЗдания + ", г. " + item.Город + ", ул. " + item.Улица + ", д. " + item.НомерЗдания);
            }
            myModel.Add(Buildings);

            foreach (var item in context.ТаблицаИздательств.ToList())
            {
                Publishers.Add(item.КодИздательства + ". " + item.НазваниеИздательства + ", г. " + item.Город + ", ул. " + item.Улица + ", д. " + item.Дом);
            }
            myModel.Add(Publishers);

            foreach (var item in context.ЖанрыКниг.ToList())
            {
                Genres.Add(item.КодЖанра + ". " + item.ИмяЖанра);
            }
            myModel.Add(Genres);

            return View(myModel);
        }



        [HttpPost]
        public ActionResult HandleAuthorConnection(string BookLine, string AuthorLine, string submitButton)
        {
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(AuthorLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ТаблицаАвторов.Add(context.ТаблицаАвторов.Single(a => a.КодАвтора == Id));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(AuthorLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ТаблицаАвторов.Remove(context.ТаблицаАвторов.Single(a => a.КодАвтора == Id));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
            }
            return View();
        }


        [HttpPost]
        public ActionResult HandleBuildingConnection(string BookLine, string BuildingLine, decimal Price, string submitButton)
        {
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(BuildingLine.Split('.')[0]);
                            string price = Math.Round(Price, 2).ToString();
                            ЗданиеКнига instance = new ЗданиеКнига { ISBN = ISBN, КодЗдания = Id, ЦенаКниги = price };
                            context.ЗданиеКнига.Add(instance);
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(BuildingLine.Split('.')[0]);
                            context.ЗданиеКнига.Remove(context.ЗданиеКнига.Single(x => x.КодЗдания == Id && x.ISBN == ISBN));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
            }
            return View();
        }
        [HttpPost]
        public ActionResult HandlePublisherConnection(string BookLine, string PublisherLine, string submitButton)
        {
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(PublisherLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ТаблицаИздательств.Add(context.ТаблицаИздательств.Single(a => a.КодИздательства == Id));
                            context.SaveChanges();
                        }
                        catch(Exception)
                        {

                        }
                        return View();
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(PublisherLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ТаблицаИздательств.Remove(context.ТаблицаИздательств.Single(a => a.КодИздательства == Id));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
            }
            return View();
        }
        [HttpPost]
        public ActionResult HandleGenreConnection(string BookLine, string GenreLine, string submitButton)
        {
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(GenreLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ЖанрыКниг.Add(context.ЖанрыКниг.Single(a => a.КодЖанра == Id));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            string ISBN = BookLine.Split('.')[0];
                            int Id = Convert.ToInt32(GenreLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);

                            book.ЖанрыКниг.Remove(context.ЖанрыКниг.Single(a => a.КодЖанра == Id));
                            context.SaveChanges();
                        }
                        catch (Exception)
                        {

                        }
                        return View();
                    }
            }
            return View();
        }
    }
}