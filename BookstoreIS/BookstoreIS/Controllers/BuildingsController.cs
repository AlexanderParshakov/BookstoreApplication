using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class BuildingsController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        public ActionResult Index()
        {
            List<ТаблицаЗданий> Buildings = context.ТаблицаЗданий.ToList();

            List<object> myModel = new List<object>();
            myModel.Add(Buildings);
            return View(myModel);
        }

        public ActionResult ShowSearchedBuildings(string City)
        {
            List<ТаблицаЗданий> Buildings = context.ТаблицаЗданий.ToList();
            List<ТаблицаЗданий> FinalBuildings = new List<ТаблицаЗданий>();

            for(int i = 0; i < Buildings.Count; i++)
            {
                if (Buildings[i].Город != null)
                {
                    if (Buildings[i].Город.Contains(City.Trim()))
                    {
                        FinalBuildings.Add(Buildings[i]);
                    }
                }
            }
            List<object> myModel = new List<object>();
            myModel.Add(FinalBuildings);
            return View(myModel);
        }

        public ActionResult ShowBuildingDetails(int? Id)
        {

            int countRealAuthors = 0;
            List<ТаблицаЗданий> list = context.ТаблицаЗданий.ToList();
            List<ТаблицаКниг> BuildingBooks = new List<ТаблицаКниг>();
            List<string> BuildingBookGenres = new List<string>();
            int id = 0;
            List<string> BuildingBooksAuthors = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (id == 0)
                {
                    if (list[i].КодЗдания == Id)
                    {
                        id = i;
                    }

                }
            }
            List<ТаблицаЗданий> temp = new List<ТаблицаЗданий>();
            if (!Id.Equals(null))
            {
                temp.Add(list[id]);
            }

            if (Id.Equals(null))
            {
                id = 1;
                temp.Add(list[id]);
            }

            foreach (ТаблицаЗданий building in temp)
            {
                foreach (ТаблицаКниг book in GetBooks(building))
                {
                    string GenreName = null;
                    int countGenres = 0;
                    BuildingBooks.Add(book);


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
                    BuildingBookGenres.Add(GenreName);

                    context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                    foreach (ТаблицаАвторов author in book.ТаблицаАвторов)
                    {
                        if (book.ТаблицаАвторов.Count != 0)
                        {
                            BuildingBooksAuthors.Add(author.Имя + " " + author.Фамилия);
                            countRealAuthors++;
                        }
                    }

                    if (book.ТаблицаАвторов.Count == 0)
                    {
                        countRealAuthors++;
                    }
                    if (countRealAuthors != BuildingBooksAuthors.Count)
                    {
                        BuildingBooksAuthors.Add("Нет авторов");
                    }
                }
            }

            List<object> myModel = new List<object>();
            myModel.Add(temp);
            myModel.Add(BuildingBooks);
            myModel.Add(BuildingBookGenres);
            myModel.Add(BuildingBooksAuthors);
            return View(myModel);
        }

        public ActionResult ShowBookDetails(string ISBN)
        {
            List<ТаблицаКниг> list = context.ТаблицаКниг.ToList();

            int id = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (id == 0)
                {
                    if (list[i].ISBN == ISBN)
                    {
                        id = i;
                    }
                }
            }
            List<ТаблицаКниг> temp = new List<ТаблицаКниг>();
            temp.Add(list[id]);
            List<string> authors = new List<string>();
            List<string> genres = new List<string>();
            foreach (ТаблицаКниг book in temp)
            {
                context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                foreach (ТаблицаАвторов author in book.ТаблицаАвторов)
                {
                    authors.Add(author.Имя + " " + author.Фамилия);
                }
                context.Entry(book).Collection(x => x.ЖанрыКниг).Load();
                foreach (ЖанрыКниг genre in book.ЖанрыКниг)
                {
                    genres.Add(genre.ИмяЖанра);
                }
            }

            List<object> myModel = new List<object>();
            myModel.Add(temp);
            myModel.Add(authors);
            myModel.Add(genres);
            myModel.Add(GetBuildings(temp[0]));

            return View(myModel);
        }


        private List<ТаблицаКниг> GetBooks(ТаблицаЗданий building)
        {
            List<ТаблицаКниг> Books = context.ТаблицаКниг.ToList();
            List<string> NeededISBNs = new List<string>();
            List<ТаблицаКниг> FinalBooks = new List<ТаблицаКниг>();

            context.Entry(building).Collection(x => x.ЗданиеКнига).Load();
            foreach (ЗданиеКнига instance in building.ЗданиеКнига)
            {
                NeededISBNs.Add(instance.ISBN);
            } // получен список ISBNs книг в данном здании

            for (int i = 0; i < Books.Count; i++)
            {
                for (int j = 0; j < NeededISBNs.Count; j++)
                {
                    if (Books[i].ISBN == NeededISBNs[j])
                    {
                        FinalBooks.Add(Books[i]);
                    }
                }
            }

            return FinalBooks;
        }
        private List<ТаблицаЗданий> GetBuildings(ТаблицаКниг book)
        {
            List<ТаблицаЗданий> Buildings = context.ТаблицаЗданий.ToList();
            List<int> NeededCodes = new List<int>();
            List<ТаблицаЗданий> FinalBuildings = new List<ТаблицаЗданий>();

            context.Entry(book).Collection(x => x.ЗданиеКнига).Load();
            foreach (ЗданиеКнига instance in book.ЗданиеКнига)
            {
                NeededCodes.Add(instance.КодЗдания);
            } // получен список кодов магазинов с данной книгой

            for (int i = 0; i < Buildings.Count; i++)
            {
                for (int j = 0; j < NeededCodes.Count; j++)
                {
                    if (Buildings[i].КодЗдания == NeededCodes[j])
                    {
                        FinalBuildings.Add(Buildings[i]);
                    }
                }
            }

            return FinalBuildings;
        }

        public ActionResult Create()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            int incremented = context.ТаблицаЗданий.Count();
            return View(incremented);
        }

        public ActionResult SaveNewBuilding(int BuildingCode, string Name, string City, string Street, string Number, int Floor, string Usage)
        {
            try
            {
                ТаблицаЗданий Building = new ТаблицаЗданий
                {
                    КодЗдания = BuildingCode,
                    НазваниеЗдания = Name,
                    Город = City,
                    Улица = Street,
                    НомерЗдания = Number,
                    Этаж = Floor,
                    Назначение = Usage
                };

                context.ТаблицаЗданий.Add(Building);
                context.SaveChanges();
            }
            catch(Exception)
            {

            }
            return RedirectToAction("Create", new RouteValueDictionary(new { Controller = "Buildings", Action = "Create" }));
        }

        public ActionResult Edit(int Id)
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            ТаблицаЗданий building = new ТаблицаЗданий();
            try
            {
                building = context.ТаблицаЗданий.Single(x => x.КодЗдания == Id);
            }
            catch (Exception)
            {

            }
            return View(building);
        }

        public ActionResult SaveChanges(int BuildingCode, string Name, string City, string Street, string Number, int Floor, string Usage)
        {
            ТаблицаЗданий building = context.ТаблицаЗданий.Single(x => x.КодЗдания == BuildingCode);
            try
            {
                building.НазваниеЗдания = Name;
                building.Город = City;
                building.Улица = Street;
                building.НомерЗдания = Number;
                building.Этаж = Floor;
                building.Назначение = Usage;
                context.SaveChanges();
            }
            catch(Exception)
            {
                return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Buildings", Action = "Edit", Id = BuildingCode }));
            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Buildings", Action = "Edit", Id = BuildingCode }));
        }
    }
}