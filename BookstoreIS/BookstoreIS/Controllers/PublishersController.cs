using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class PublishersController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();

        public ActionResult Index()
        {
            List<ТаблицаИздательств> Publishers = context.ТаблицаИздательств.ToList();

            List<object> myModel = new List<object>();
            myModel.Add(Publishers);

            return View(myModel);
        }

        public ActionResult ShowSearchedPublishers(string Name)
        {
            List<ТаблицаИздательств> Publishers = context.ТаблицаИздательств.ToList();
            List<ТаблицаИздательств> FinalPublishers = new List<ТаблицаИздательств>();

            for (int i = 0; i < Publishers.Count; i++)
            {
                if (Publishers[i].НазваниеИздательства != null)
                {
                    if (Publishers[i].НазваниеИздательства.Contains(Name.Trim()))
                    {
                        FinalPublishers.Add(Publishers[i]);
                    }
                }
            }
                List<object> myModel = new List<object>();
            myModel.Add(FinalPublishers);

            return View(myModel);
        }
        public ActionResult ShowBookDetails(string ISBN)
        {
            return RedirectToAction("ShowBookDetails", new RouteValueDictionary(new { Controller = "Books", Action = "ShowBookDetails", ISBN = ISBN }));
        }

        public ActionResult ShowBuildingDetails(int Id)
        {
            return RedirectToAction("ShowBuildingDetails", new RouteValueDictionary(new { Controller = "Buildings", Action = "ShowBuildingsDetails", Id = Id }));
        }

        public ActionResult ShowPublisherDetails(int? Id)
        {
            List<ТаблицаИздательств> list = context.ТаблицаИздательств.ToList();
            List<ТаблицаКниг> PublisherBooks = new List<ТаблицаКниг>();
            List<string> PublisherBooksGenres = new List<string>();
            int id = 0;
            List<string> PublisherBooksAuthors = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (id == 0)
                {
                    if (list[i].КодИздательства == Id)
                    {
                        id = i;
                    }

                }
            }
            List<ТаблицаИздательств> temp = new List<ТаблицаИздательств>();
            if (!Id.Equals(null))
            {
                temp.Add(list[id]);
            }

            if (Id.Equals(null))
            {
                id = 1;
                temp.Add(list[id]);
            }

            foreach (ТаблицаИздательств publisher in temp)
            {
                foreach (ТаблицаКниг book in GetBooks(publisher))
                {
                    string AuthorName = null;
                    int countAuthors = 0;
                    string GenreName = null;
                    int countGenres = 0;
                    PublisherBooks.Add(book);


                    context.Entry(book).Collection(x => x.ЖанрыКниг).Load();
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
                    PublisherBooksGenres.Add(GenreName);

                    context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                    foreach (ТаблицаАвторов author in book.ТаблицаАвторов) // получаем имена авторов книги
                    {
                        if (book.ТаблицаАвторов.Count != countAuthors + 1)
                        {
                            AuthorName += author.Имя + " " + author.Фамилия + ", ";

                            countAuthors++;
                        }
                        else
                        {
                            AuthorName += author.Имя + " " + author.Фамилия + " ";
                        }
                    }
                    PublisherBooksAuthors.Add(AuthorName);
                }
            }

            List<object> myModel = new List<object>();
            myModel.Add(temp);
            myModel.Add(PublisherBooks);
            myModel.Add(PublisherBooksGenres);
            myModel.Add(PublisherBooksAuthors);
            return View(myModel);
        }


        private List<ТаблицаКниг> GetBooks(ТаблицаИздательств publisher)
        {
            List<ТаблицаКниг> Books = context.ТаблицаКниг.ToList();
            List<string> NeededISBNs = new List<string>();
            List<ТаблицаКниг> FinalBooks = new List<ТаблицаКниг>();

            context.Entry(publisher).Collection(x => x.ТаблицаКниг).Load();
            foreach (ТаблицаКниг instance in publisher.ТаблицаКниг)
            {
                FinalBooks.Add(instance);
            } // получен список книг для данного издательстваа


            return FinalBooks;
        }

        public ActionResult Create()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            int incremented = context.ТаблицаИздательств.Count();
            return View(incremented);
        }

        public ActionResult SaveNewPublisher(int PublisherCode, string Name, string City, string Street, string Number)
        {
            try
            {
                ТаблицаИздательств Publisher = new ТаблицаИздательств
                {
                    КодИздательства = PublisherCode,
                    НазваниеИздательства = Name,
                    Город = City,
                    Улица = Street,
                    Дом = Number
                };

                context.ТаблицаИздательств.Add(Publisher);
                context.SaveChanges();
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Create", new RouteValueDictionary(new { Controller = "Publishers", Action = "Create" }));
        }

        public ActionResult Edit(int Id)
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            ТаблицаИздательств publisher = context.ТаблицаИздательств.Single(x => x.КодИздательства == Id);
            return View(publisher);
        }

        public ActionResult SaveChanges(int PublisherCode, string Name, string City, string Street, string Number)
        {
            ТаблицаИздательств publisher = context.ТаблицаИздательств.Single(x => x.КодИздательства == PublisherCode);
            {
                try
                {
                    publisher.НазваниеИздательства = Name;
                    publisher.Город = City;
                    publisher.Улица = Street;
                    publisher.Дом = Number;
                    context.SaveChanges();
                }
                catch(Exception)
                {
                    return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Publishers", Action = "Edit", Id = PublisherCode }));
                }
            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Publishers", Action = "Edit", Id = PublisherCode }));
        }
    }
}