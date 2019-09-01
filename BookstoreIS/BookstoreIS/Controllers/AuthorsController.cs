using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class AuthorsController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        public ActionResult Index()
        {
            return View(context.ТаблицаАвторов.ToList());
        }

        public ActionResult ShowSearchedAuthors(string Surname)
        {
            List<ТаблицаАвторов> Authors = context.ТаблицаАвторов.ToList();
            List<ТаблицаАвторов> FinalAuthors = new List<ТаблицаАвторов>() ;

            for(int i = 0; i < Authors.Count; i++)
            {
                if (Authors[i].Фамилия.Contains(Surname.Trim()))
                {
                    FinalAuthors.Add(Authors[i]);
                }
            }
            return View(FinalAuthors);
        }

        public ActionResult ShowBuildingDetails(int Id)
        {
            return RedirectToAction("ShowBuildingDetails", new RouteValueDictionary(new { Controller = "Buildings", Action = "ShowBuildingsDetails", Id = Id }));
        }
        public ActionResult ShowAuthorDetails(int Id)
        {

            List<ТаблицаАвторов> list = context.ТаблицаАвторов.ToList();
            List<ТаблицаКниг> AuthorBooks = new List<ТаблицаКниг>();
            List<string> AuthorBookGenres = new List<string>();
            int id = 0;
            
            for (int i = 0; i < list.Count; i++)
            {
                if (id == 0)
                {
                    if (list[i].КодАвтора == Id)
                    {
                        id = i;
                    }

                }
            }
            List<ТаблицаАвторов> temp = new List<ТаблицаАвторов>();
            if (!id.Equals(null))
            {
                temp.Add(list[id]);
            }
            
            if (id.Equals(null))
            {
                id = 1;
                temp.Add(list[id]);
            }

            foreach (ТаблицаАвторов author in temp)
            {
                context.Entry(author).Collection(x => x.ТаблицаКниг).Load();
                foreach (ТаблицаКниг book in author.ТаблицаКниг)
                {
                    string GenreName = null;
                    int countGenres = 0;
                    AuthorBooks.Add(book);


                    context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                    foreach (ЖанрыКниг genre in book.ЖанрыКниг) // получаем жанры книги
                    {
                        if (book.ЖанрыКниг.Count != countGenres + 1)
                        {
                            GenreName += genre.ИмяЖанра + ", ";
                            countGenres++;
                        }
                        else
                        {
                            GenreName += genre.ИмяЖанра;
                        }
                    }
                    AuthorBookGenres.Add(GenreName);
                }
            }

            List<object> myModel = new List<object>();
            myModel.Add(temp);
            myModel.Add(AuthorBooks);
            myModel.Add(AuthorBookGenres);
            return View(myModel);
        }



        public ActionResult ShowBookDetails(string ISBN)
        {
            return RedirectToAction("ShowBookDetails", new RouteValueDictionary(new { Controller = "Books", Action = "ShowBookDetails", ISBN = ISBN }));
        }

        public ActionResult Create()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            int NumberOfAuthors = context.ТаблицаАвторов.Count();
            return View(NumberOfAuthors);
        }

        public ActionResult SaveNewAuthor(int AuthorCode, string Name, string Surname, string Patronymic, HttpPostedFileBase file)
        {
            try
            {
                byte[] picture = new byte[file.ContentLength];
                file.InputStream.Read(picture, 0, file.ContentLength);
                ТаблицаАвторов author = new ТаблицаАвторов
                {
                    КодАвтора = AuthorCode,
                    Имя = Name,
                    Фамилия = Surname,
                    Отчество = Patronymic,
                    Фотография = picture
                };

                context.ТаблицаАвторов.Add(author);
                context.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Create", new RouteValueDictionary(new { Controller = "Authors", Action = "Create" }));
        }

        public ActionResult Edit(int Id)
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            ТаблицаАвторов author = new ТаблицаАвторов();
            try
            {
                author = context.ТаблицаАвторов.Single(x => x.КодАвтора == Id);
            }
            catch (Exception)
            {

            }
            return View(author);
        }

        public ActionResult SaveChanges(int AuthorCode, string Name, string Patronymic, string Surname, HttpPostedFileBase file)
        {
            ТаблицаАвторов author = context.ТаблицаАвторов.Single(x => x.КодАвтора == AuthorCode);
            try
            {
                if (file != null)
                {
                    byte[] picture = new byte[file.ContentLength];
                    file.InputStream.Read(picture, 0, file.ContentLength);
                    author.Фотография = picture;
                }
                author.Имя = Name;
                author.Отчество = Patronymic;
                author.Фамилия = Surname;
                context.SaveChanges();
            }
            catch(Exception)
            {
                return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Authors", Action = "Edit", Id = AuthorCode }));
            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Authors", Action = "Edit", Id = AuthorCode }));
        }
    }
}