using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookstoreIS.Controllers
{
    public class BooksController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();

        public ActionResult ShowBuildingDetails(int Id)
        {
            return RedirectToAction("ShowBuildingDetails", new RouteValueDictionary(new { Controller = "Buildings", Action = "ShowBuildingsDetails", Id = Id }));
        }
        public ActionResult Index()
       {
            List<object> myModel = new List<object>();


            List<ТаблицаКниг> books = context.ТаблицаКниг.ToList();
            List<string> AuthorNames = new List<string>();
            List<double?> Grades = new List<double?>();
            List<string> Genres = new List<string>();
            List<ЖанрыКниг> AllGenres = context.ЖанрыКниг.ToList();

            for (int i = 0; i < books.Count; i++)
            {
                string AuthorName = null;
                string GenreName = null;
                int countAuthors = 0;
                int countGenres = 0;
                context.Entry(books[i]).Collection(x => x.ТаблицаАвторов).Load();
                foreach (ТаблицаАвторов author in books[i].ТаблицаАвторов) // получаем имена авторов книги
                {
                    if (books[i].ТаблицаАвторов.Count != countAuthors + 1)
                    {
                        AuthorName += author.Имя + " " + author.Фамилия + ", ";

                        countAuthors++;
                    }
                    else
                    {
                        AuthorName += author.Имя + " " + author.Фамилия + " ";
                    }
                }
                AuthorNames.Add(AuthorName);

                context.Entry(books[i]).Collection(x => x.ТаблицаАвторов).Load();
                foreach (ЖанрыКниг genre in books[i].ЖанрыКниг) // получаем жанры книги
                {
                    if (books[i].ЖанрыКниг.Count != countGenres + 1)
                    {
                        GenreName += genre.ИмяЖанра + ", ";
                        countGenres++;
                    }
                    else
                    {
                        GenreName += genre.ИмяЖанра;
                    }
                }
                Genres.Add(GenreName);

                double? grade = 0;
                context.Entry(books[i]).Collection(x => x.КнигаОценки).Load();
                foreach (КнигаОценки BookMark in books[i].КнигаОценки) // получаем оценки книги
                {
                    grade += BookMark.ОценкаПользователей;
                }

                grade = (grade / books[i].КнигаОценки.Count);
                Grades.Add(grade);
            }

            myModel.Add(context.ТаблицаКниг);
            myModel.Add(AuthorNames);
            myModel.Add(Grades);
            myModel.Add(Genres);
            myModel.Add(AllGenres);
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
            List<string> publishers = new List<string>();
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
                context.Entry(book).Collection(x => x.ТаблицаИздательств).Load();
                foreach (ТаблицаИздательств publisher in book.ТаблицаИздательств)
                {
                    publishers.Add(publisher.НазваниеИздательства);
                }
            }

            List<object> myModel = new List<object>();
            myModel.Add(temp);
            myModel.Add(authors);
            myModel.Add(genres);
            myModel.Add(GetBuildings(temp[0]));
            myModel.Add(publishers);

            return View(myModel);
        }

        public ActionResult ShowSearchedBooks(string BookName, string Genre)
        {
            List<ТаблицаКниг> books = context.ТаблицаКниг.ToList();
            List<string> bookNames = new List<string>();
            List<ТаблицаКниг> FinalBooks = new List<ТаблицаКниг>();
            List<ЖанрыКниг> AllGenres = context.ЖанрыКниг.ToList();
            List<string> AuthorNames = new List<string>();
            List<double?> Grades = new List<double?>();
            List<string> Genres = new List<string>();
            int GenreCode = 0;
            try
            {
                GenreCode = context.ЖанрыКниг.Single(x => x.ИмяЖанра == Genre).КодЖанра;
            }
            catch(Exception)
            {

            }

            if (BookName != "" && Genre != "Выберите жанр...")
            {
                for (int i = 0; i < books.Count; i++)
                {
                    context.Entry(books[i]).Collection(x => x.ЖанрыКниг).Load();
                    if (books[i].НазваниеКниги.Contains(BookName))
                    {
                        for (int j = 0; j < books[i].ЖанрыКниг.ToList().Count; j++)
                        {
                            if (books[i].ЖанрыКниг.ToList()[j].ИмяЖанра.Contains(Genre))
                            {
                                bookNames.Add(books[i].НазваниеКниги);
                            }
                        }
                    }
                }
            }

            if (BookName != "" && Genre == "Выберите жанр...")
            {
                for (int i = 0; i < books.Count; i++)
                {
                    if (books[i].НазваниеКниги.Contains(BookName))
                    {
                        bookNames.Add(books[i].НазваниеКниги);
                    }
                }
            }

            if (BookName == "" && Genre != "Выберите жанр...")
            {
                for (int i = 0; i < books.Count; i++)
                {
                    context.Entry(books[i]).Collection(x => x.ЖанрыКниг).Load();
                    for (int j = 0; j < books[i].ЖанрыКниг.ToList().Count; j++)
                    {
                        if (books[i].ЖанрыКниг.ToList()[j].ИмяЖанра.Contains(Genre))
                        {
                            bookNames.Add(books[i].НазваниеКниги);
                        }
                    }
                }
            }

            if (BookName == "" && Genre == "Выберите жанр...")
            {
                for (int i = 0; i < books.Count; i++)
                {
                    bookNames.Add(books[i].НазваниеКниги);
                }
            }

            for (int i = 0; i < books.Count; i++)
            {
                for (int j = 0; j < bookNames.Count; j++)
                {
                    if (books[i].НазваниеКниги == bookNames[j])
                    {
                        FinalBooks.Add(books[i]);
                    }
                }
            }

            for (int i = 0; i < FinalBooks.Count; i++)
            {
                string AuthorName = null;
                string GenreName = null;
                int countAuthors = 0;
                int countGenres = 0;
                context.Entry(FinalBooks[i]).Collection(x => x.ТаблицаАвторов).Load();
                foreach (ТаблицаАвторов author in FinalBooks[i].ТаблицаАвторов) // получаем имена авторов книги
                {
                    if (FinalBooks[i].ТаблицаАвторов.Count != countAuthors + 1)
                    {
                        AuthorName += author.Имя + " " + author.Фамилия + ", ";

                        countAuthors++;
                    }
                    else
                    {
                        AuthorName += author.Имя + " " + author.Фамилия + " ";
                    }
                }
                AuthorNames.Add(AuthorName);

                context.Entry(FinalBooks[i]).Collection(x => x.ЖанрыКниг).Load();
                foreach (ЖанрыКниг genre in FinalBooks[i].ЖанрыКниг) // получаем жанры книги
                {
                    if (FinalBooks[i].ЖанрыКниг.Count != countGenres + 1)
                    {
                        GenreName += genre.ИмяЖанра + ", ";
                        countGenres++;
                    }
                    else
                    {
                        GenreName += genre.ИмяЖанра;
                    }
                }
                Genres.Add(GenreName);

                double? grade = 0;
                context.Entry(FinalBooks[i]).Collection(x => x.КнигаОценки).Load();
                foreach (КнигаОценки BookMark in FinalBooks[i].КнигаОценки) // получаем оценки книги
                {
                    grade += BookMark.ОценкаПользователей;
                }

                grade = (grade / FinalBooks[i].КнигаОценки.Count);
                Grades.Add(grade);
            }
            List<object> myModel = new List<object>();
            if (GenreCode != 0)
            {
                Session["ChosenGenre"] = context.ЖанрыКниг.Single(x => x.КодЖанра == GenreCode);
            }
            Session["BookName"] = BookName;

            myModel.Add(FinalBooks);
            myModel.Add(AuthorNames);
            myModel.Add(Grades);
            myModel.Add(Genres);
            myModel.Add(AllGenres);
            return View(myModel);
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


        [HttpGet]
        public ActionResult Edit(string ISBN)
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);
            Session["EditableBook"] = book;

            List <ТаблицаКниг> Books = new List<ТаблицаКниг>();
            List<string> Authors = new List<string>();
            List<string> bookAuthors = new List<string>();
            List<string> Buildings = new List<string>();
            List<string> bookBuildings = new List<string>();
            List<string> Publishers = new List<string>();
            List<string> bookPublishers = new List<string>();
            List<string> Genres = new List<string>();
            List<string> bookGenres = new List<string>();

            List<object> myModel = new List<object>();

            foreach (var item in context.ТаблицаАвторов.ToList())
            {
                Authors.Add(item.КодАвтора + ". " + item.Имя + " " + item.Фамилия);
            }
            Books.Add(book);

            foreach (var item in context.ТаблицаЗданий.ToList())
            {
                Buildings.Add(item.КодЗдания + ". " + item.НазваниеЗдания + ", г. " + item.Город + ", ул. " + item.Улица + ", д. " + item.НомерЗдания);
            }

            foreach (var item in context.ТаблицаИздательств.ToList())
            {
                Publishers.Add(item.КодИздательства + ". " + item.НазваниеИздательства + ", г. " + item.Город + ", ул. " + item.Улица + ", д. " + item.Дом);
            }

            foreach (var item in context.ЖанрыКниг.ToList())
            {
                Genres.Add(item.КодЖанра + ". " + item.ИмяЖанра);
            }

            context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
            foreach (ТаблицаАвторов author in book.ТаблицаАвторов) // получаем имена авторов книги
            {
                bookAuthors.Add(author.КодАвтора + ". " + author.Имя + " " + author.Фамилия);
            }

            myModel.Add(Books);
            myModel.Add(Authors);
            myModel.Add(bookAuthors);
            myModel.Add(Buildings);
            myModel.Add(Publishers);
            myModel.Add(Genres);

            return View(myModel);
        }

        [HttpPost]
        public ActionResult HandleAuthorConnection(string AuthorLine, string submitButton)
        {
            ТаблицаКниг sampleBook = Session["EditableBook"] as ТаблицаКниг;
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(AuthorLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ТаблицаАвторов.Add(context.ТаблицаАвторов.Single(a => a.КодАвтора == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Авторство было успешно добавлено.";
                            return PartialView("AuthorSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Авторство не было добавлено в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                        
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(AuthorLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ТаблицаАвторов.Remove(context.ТаблицаАвторов.Single(a => a.КодАвтора == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Авторство было успешно удалено.";
                            return PartialView("AuthorSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Авторство не было удалено в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
            }
            return View();
        }

        [HttpPost]
        public ActionResult HandleBuildingConnection(string BuildingLine, decimal Price, string submitButton)
        {
            ТаблицаКниг sampleBook = Session["EditableBook"] as ТаблицаКниг;
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(BuildingLine.Split('.')[0]);
                            string price = Math.Round(Price, 2).ToString();
                            ЗданиеКнига instance = new ЗданиеКнига { ISBN = sampleBook.ISBN, КодЗдания = Id, ЦенаКниги = price };
                            context.ЗданиеКнига.Add(instance);
                            context.SaveChanges();
                            ViewBag.Success = "Книга была успешно добавлена в магазин или склад.";
                            return PartialView("BuildingSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Книга не была добавлена в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(BuildingLine.Split('.')[0]);
                            context.ЗданиеКнига.Remove(context.ЗданиеКнига.Single(x => x.КодЗдания == Id && x.ISBN == sampleBook.ISBN));
                            context.SaveChanges();
                            ViewBag.Success = "Книга была успешно удалена из магазина или склада.";
                            return PartialView("BuildingSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Книга не была удалена в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
            }
            return View();
        }

        [HttpPost]
        public ActionResult HandlePublisherConnection(string PublisherLine, string submitButton)
        {
            ТаблицаКниг sampleBook = Session["EditableBook"] as ТаблицаКниг;
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(PublisherLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ТаблицаИздательств.Add(context.ТаблицаИздательств.Single(a => a.КодИздательства == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Книга была успешно добавлена в издательство.";
                            return PartialView("PublisherSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Книга не была добавлена издательство в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(PublisherLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ТаблицаИздательств.Remove(context.ТаблицаИздательств.Single(a => a.КодИздательства == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Книга была успешно удалена из издательства.";
                            return PartialView("PublisherSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Книга не была удалена из издательства в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
            }
            return View();
        }

        [HttpPost]
        public ActionResult HandleGenreConnection(string GenreLine, string submitButton)
        {
            ТаблицаКниг sampleBook = Session["EditableBook"] as ТаблицаКниг;
            switch (submitButton)
            {
                case "Добавить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(GenreLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ЖанрыКниг.Add(context.ЖанрыКниг.Single(a => a.КодЖанра == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Жанр был успешно добавлен для данной книги.";
                            return PartialView("GenreSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Жанр для книги не был добавлен в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
                case "Удалить связь":
                    {
                        try
                        {
                            int Id = Convert.ToInt32(GenreLine.Split('.')[0]);

                            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == sampleBook.ISBN);

                            book.ЖанрыКниг.Remove(context.ЖанрыКниг.Single(a => a.КодЖанра == Id));
                            context.SaveChanges();
                            ViewBag.Success = "Жанр для данной был успешно удален.";
                            return PartialView("GenreSuccess");
                        }
                        catch (Exception)
                        {
                            ViewBag.Success = "Жанр для данной книги не был удален в силу возникновения исключения.";
                            return PartialView("Error");
                        }
                    }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveChanges(string ISBN, string title, string ReleaseYear, int? pages, string binding, string format, string authors, HttpPostedFileBase file)
        {
            List<ТаблицаКниг> books = context.ТаблицаКниг.ToList();
            ISBN = ISBN.Trim();
            ТаблицаКниг book = context.ТаблицаКниг.Single(b => b.ISBN == ISBN);
            if (pages.Equals(null) == false)
            {
                try
                {
                    if (file != null)
                    {
                        byte[] picture = new byte[file.ContentLength];
                        file.InputStream.Read(picture, 0, file.ContentLength);
                        book.Обложка = picture;
                    }
                    book.НазваниеКниги = title;
                    book.ГодВыпуска = ReleaseYear;
                    book.КоличествоСтраниц = pages;
                    book.Переплет = binding;
                    book.ФорматКниги = format;
                    context.SaveChanges();
                }
                catch (Exception)
                {

                }
                return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Books", Action = "Edit", ISBN = ISBN }));
            }
            if (pages.Equals(null) == true)
            {
                View("WrongPagesInfo").ViewBag.Error = "Введенные данные не соответствуют формату. Количество страниц должно быть числом и не должно содержать символов, отличных от цифр.";
                return PartialView("WrongPagesInfo");
            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Books", Action = "Edit", ISBN = ISBN }));
        }


        public ActionResult Create()
        {
            if (Session["CurrentAdmin"] == null)
            {
                return RedirectToAction("SignIn", new RouteValueDictionary(new { Controller = "SignIn", Action = "SignIn" }));
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveNewBook(string ISBN, string Title, string ReleaseYear, int? Pages, string Binding, string Format, HttpPostedFileBase file)
        {
            ТаблицаКниг book = new ТаблицаКниг();
            try
            {
                byte[] picture = new byte[file.ContentLength];
                file.InputStream.Read(picture, 0, file.ContentLength);
                book.ISBN = ISBN;
                book.НазваниеКниги = Title;
                book.ГодВыпуска = ReleaseYear;
                book.КоличествоСтраниц = Pages;
                book.Переплет = Binding;
                book.ФорматКниги = Format;
                book.Обложка = picture;
                context.ТаблицаКниг.Add(book);
                context.SaveChanges();
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Edit", new RouteValueDictionary(new { Controller = "Books", Action = "Edit", ISBN = ISBN }));
        }
    }
}