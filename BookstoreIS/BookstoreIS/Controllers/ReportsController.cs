using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookstoreIS.Controllers
{
    public class ReportsController : Controller
    {
        BookstoreApplicationEntities context = new BookstoreApplicationEntities();
        public ActionResult Index()
        {
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
        

        public void ExportToExcel()
        {
            List<ТаблицаКниг> Books = Session["FoundBooks"] as List<ТаблицаКниг>;
            List<string> BookAuthors = Session["FoundAuthors"] as List<string>;
            List<string> BookGenres = Session["FoundGenres"] as List<string>;
            string FormedHeading = Session["FormedHeading"] as string;

            ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add("Отчет");

            ws.Cells["A1"].Value = "Параметры запроса: ";
            ws.Cells["B1"].Value = FormedHeading;

            ws.Cells["A2"].Value = "Дата построения отчета: ";
            ws.Cells["B2"].Value = DateTime.Now.ToLongDateString();
            ws.Cells["A3"].Value = "Время построения отчета: ";
            ws.Cells["B3"].Value = DateTime.Now.ToLongTimeString();
            ws.Cells["A1:A3"].Style.Font.Bold = true;


            ws.Cells["A5"].Value = "ISBN";
            ws.Cells["B5"].Value = "Название";
            ws.Cells["C5"].Value = "Автор(ы)";
            ws.Cells["D5"].Value = "Жанр";
            ws.Cells["E5"].Value = "Год выпуска";
            ws.Cells["F5"].Value = "Переплет";
            ws.Cells["A5:F5"].Style.Font.Bold = true;

            int RowStartBooks = 6;
            int RowStartAuthors = 6;
            int RowStartGenres = 6;
            foreach (var item in Books)
            {
                ws.Cells[string.Format("A{0}", RowStartBooks)].Value = item.ISBN;
                ws.Cells[string.Format("B{0}", RowStartBooks)].Value = item.НазваниеКниги;
                ws.Cells[string.Format("E{0}", RowStartBooks)].Value = item.ГодВыпуска;
                ws.Cells[string.Format("F{0}", RowStartBooks)].Value = item.Переплет;
                RowStartBooks++;
            }
            foreach(var item in BookAuthors)
            {
                ws.Cells[string.Format("C{0}", RowStartAuthors)].Value = item;
                RowStartAuthors++;
            }

            foreach(var item in BookGenres)
            {
                ws.Cells[string.Format("D{0}", RowStartGenres)].Value = item;
                RowStartGenres++;
            }

            ws.Cells["A1:A10000"].AutoFitColumns();
            ws.Cells["B2:I10000"].AutoFitColumns();
            ws.Cells["A:AZ"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename =" + "ExcelReport.xlsx");
            Response.BinaryWrite(pack.GetAsByteArray());
            Response.End();
        }

        public ActionResult GetBooksByParameters(string AuthorLine, string BuildingLine, string PublisherLine, string GenreLine)
        {
            List<ТаблицаКниг> Books = context.ТаблицаКниг.ToList();
            List<ТаблицаКниг> FinalBooksByAuthor = new List<ТаблицаКниг>();
            List<ТаблицаКниг> FinalBooksByBuilding = new List<ТаблицаКниг>();
            List<ТаблицаКниг> FinalBooksByPublisher = new List<ТаблицаКниг>();
            List<ТаблицаКниг> FinalBooksByGenre = new List<ТаблицаКниг>();

            List<string> BookAuthors = new List<string>();
            List<string> BookGenres = new List<string>();

            List<ТаблицаКниг> FinalList = new List<ТаблицаКниг>();

            ТаблицаАвторов author;
            ТаблицаЗданий building;
            ТаблицаИздательств publisher;
            ЖанрыКниг genre;

            string FormedHeading = "";
            int HeadingCount = 0;


            if (AuthorLine.Contains("Выберите") == false)
            {
                int AuthorCode = Convert.ToInt32(AuthorLine.Split('.')[0]);
                author = context.ТаблицаАвторов.Single(x => x.КодАвтора == AuthorCode);
                FormedHeading += "Автор: " + author.Имя + " " + author.Фамилия;
                for (int i = 0; i < Books.Count; i++)
                {
                    context.Entry(Books[i]).Collection(x => x.ТаблицаАвторов).Load();
                    for (int j = 0; j < Books[i].ТаблицаАвторов.Count; j++)
                    {
                        if (Books[i].ТаблицаАвторов.ToList()[j] == author)
                        {
                            FinalBooksByAuthor.Add(Books[i]);
                        }
                    }
                }
                HeadingCount++;
                FinalList = FinalBooksByAuthor;
            }

            if (PublisherLine.Contains("Выберите") == false)
            {
                int PublisherCode = Convert.ToInt32(PublisherLine.Split('.')[0]);
                publisher = context.ТаблицаИздательств.Single(x => x.КодИздательства == PublisherCode);
                if (FinalList.Count == 0 && AuthorLine.Contains("Выберите") == true)
                {

                    for (int i = 0; i < Books.Count; i++)
                    {
                        context.Entry(Books[i]).Collection(x => x.ТаблицаИздательств).Load();
                        for (int j = 0; j < Books[i].ТаблицаИздательств.Count; j++)
                        {
                            if (Books[i].ТаблицаИздательств.ToList()[j] == publisher)
                            {
                                FinalBooksByPublisher.Add(Books[i]);
                            }
                        }
                    }
                    FinalList = FinalBooksByPublisher;
                }
                if (FinalList.Count > 0)
                {
                    for (int i = 0; i < FinalBooksByAuthor.Count; i++)
                    {
                        context.Entry(FinalBooksByAuthor[i]).Collection(x => x.ТаблицаИздательств).Load();
                        for (int j = 0; j < FinalBooksByAuthor[i].ТаблицаИздательств.Count; j++)
                        {
                            if (FinalBooksByAuthor[i].ТаблицаИздательств.ToList()[j] == publisher)
                            {
                                FinalBooksByPublisher.Add(FinalBooksByAuthor[i]);
                            }
                        }
                    }
                    FinalList = FinalList.Intersect(FinalBooksByPublisher).ToList();
                }
                if (HeadingCount > 0)
                {
                    FormedHeading += ", Издательство: " + publisher.НазваниеИздательства;
                }
                if (HeadingCount == 0)
                {
                    FormedHeading += "Издательство: " + publisher.НазваниеИздательства;
                }
                HeadingCount++;
            }

            if (GenreLine.Contains("Выберите") == false)
            {
                int GenreCode = Convert.ToInt32(GenreLine.Split('.')[0]);
                genre = context.ЖанрыКниг.Single(x => x.КодЖанра == GenreCode);
                if (FinalList.Count == 0 && AuthorLine.Contains("Выберите") == true && PublisherLine.Contains("Выберите") == true)
                {
                    for (int i = 0; i < Books.Count; i++)
                    {
                        context.Entry(Books[i]).Collection(x => x.ЖанрыКниг).Load();
                        for (int j = 0; j < Books[i].ЖанрыКниг.Count; j++)
                        {
                            if (Books[i].ЖанрыКниг.ToList()[j] == genre)
                            {
                                FinalBooksByGenre.Add(Books[i]);
                            }
                        }
                    }
                    FinalList = FinalBooksByGenre;
                }
                if (FinalList.Count > 0)
                {
                    FinalBooksByGenre = new List<ТаблицаКниг>();
                    for (int i = 0; i < FinalList.Count; i++)
                    {
                        context.Entry(FinalList[i]).Collection(x => x.ЖанрыКниг).Load();
                        for (int j = 0; j < FinalList[i].ЖанрыКниг.Count; j++)
                        {
                            if (FinalList[i].ЖанрыКниг.ToList()[j] == genre)
                            {
                                FinalBooksByGenre.Add(FinalList[i]);
                            }
                        }
                    }
                    FinalList = FinalList.Intersect(FinalBooksByGenre).ToList();
                }
                if (HeadingCount > 0)
                {
                    FormedHeading += ", Жанр: " + genre.ИмяЖанра;
                }
                if (HeadingCount == 0)
                {
                    FormedHeading += "Жанр: " + genre.ИмяЖанра;
                }
                HeadingCount++;
            }

            if (BuildingLine.Contains("Выберите") == false)
            {
                bool HaveBeen = false;
                int BuildingCode = Convert.ToInt32(BuildingLine.Split('.')[0]);
                building = context.ТаблицаЗданий.Single(x => x.КодЗдания == BuildingCode);
                if (FinalList.Count == 0 && AuthorLine.Contains("Выберите") == true && PublisherLine.Contains("Выберите") == true && GenreLine.Contains("Выберите") == true)
                {
                    for (int i = 0; i < Books.Count; i++)
                    {
                        List<ТаблицаЗданий> BookBuildings = GetBuildings(Books[i]);
                        for (int j = 0; j < BookBuildings.Count; j++)
                        {
                            if (BookBuildings[j] == building)
                            {
                                FinalBooksByBuilding.Add(Books[i]);
                            }
                        }
                    }
                    FinalList = FinalBooksByBuilding;
                    HaveBeen = true;
                }
                if (FinalList.Count > 0 && HaveBeen == false)
                {

                    for (int i = 0; i < FinalList.Count; i++)
                    {
                        List<ТаблицаЗданий> BookBuildings = GetBuildings(FinalList[i]);
                        for (int j = 0; j < BookBuildings.Count; j++)
                        {
                            if (BookBuildings[j] == building)
                            {
                                FinalBooksByBuilding.Add(FinalList[i]);
                            }
                        }
                    }
                    FinalList = FinalList.Intersect(FinalBooksByBuilding).ToList();
                }
                if (HeadingCount > 0)
                {
                    FormedHeading += ", " + building.Назначение + ": " + building.НазваниеЗдания + ", г." + building.Город;
                }
                if (HeadingCount == 0)
                {
                    FormedHeading += building.Назначение + ": " + building.НазваниеЗдания + ", г." + building.Город;
                }
                HeadingCount++;
            }

            foreach (var book in FinalList)
            {
                string AuthorName = null;
                int countAuthors = 0;
                string GenreName = null;
                int countGenres = 0;


                context.Entry(book).Collection(x => x.ЖанрыКниг).Load();
                foreach (ЖанрыКниг item in book.ЖанрыКниг) // получаем жанры книги
                {
                    if (book.ЖанрыКниг.Count != countGenres + 1)
                    {
                        GenreName += item.ИмяЖанра + ", ";
                        countGenres++;
                    }
                    else
                    {
                        GenreName += item.ИмяЖанра;
                    }
                }
                BookGenres.Add(GenreName);

                context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                foreach (ТаблицаАвторов item in book.ТаблицаАвторов) // получаем имена авторов книги
                {
                    if (book.ТаблицаАвторов.Count != countAuthors + 1)
                    {
                        AuthorName += item.Имя + " " + item.Фамилия + ", ";

                        countAuthors++;
                    }
                    else
                    {
                        AuthorName += item.Имя + " " + item.Фамилия + " ";
                    }
                }
                BookAuthors.Add(AuthorName);
            }
            List<object> myModel = new List<object>();

            if (AuthorLine.Contains("Выберите") && BuildingLine.Contains("Выберите") && PublisherLine.Contains("Выберите") && GenreLine.Contains("Выберите") && BuildingLine.Contains("Выберите"))
            {
                BookAuthors = new List<string>();
                BookGenres = new List<string>();
                FinalList = context.ТаблицаКниг.ToList();
                foreach (var book in FinalList)
                {
                    string AuthorName = null;
                    int countAuthors = 0;
                    string GenreName = null;
                    int countGenres = 0;

                    context.Entry(book).Collection(x => x.ЖанрыКниг).Load();
                    foreach (ЖанрыКниг item in book.ЖанрыКниг) // получаем жанры книги
                    {
                        if (book.ЖанрыКниг.Count != countGenres + 1)
                        {
                            GenreName += item.ИмяЖанра + ", ";
                            countGenres++;
                        }
                        else
                        {
                            GenreName += item.ИмяЖанра;
                        }
                    }
                    BookGenres.Add(GenreName);

                    context.Entry(book).Collection(x => x.ТаблицаАвторов).Load();
                    foreach (ТаблицаАвторов item in book.ТаблицаАвторов) // получаем имена авторов книги
                    {
                        if (book.ТаблицаАвторов.Count != countAuthors + 1)
                        {
                            AuthorName += item.Имя + " " + item.Фамилия + ", ";

                            countAuthors++;
                        }
                        else
                        {
                            AuthorName += item.Имя + " " + item.Фамилия + " ";
                        }
                    }
                    BookAuthors.Add(AuthorName);
                }
            }
            if (FinalList.Count == 0)
            {
                ViewBag.Message = "По введенным Вами параметрам не было найдено ни одной книги.";
            }
            if (FinalList.Count == context.ТаблицаКниг.Count())
            {
                FormedHeading = "Сплошной список книг, без параметров";
            }
            if (FinalList.Count() != 0)
            {
                FormedHeading += ". Найдено книг: " + FinalList.Count.ToString();
            }
            myModel.Add(FinalList);
            myModel.Add(BookAuthors);
            myModel.Add(BookGenres);
            myModel.Add(FormedHeading);
            Session["FoundBooks"] = FinalList;
            Session["FoundAuthors"] = BookAuthors;
            Session["FoundGenres"] = BookAuthors;
            Session["FormedHeading"] = FormedHeading;
            return PartialView("ListOfBooks", myModel);
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
    }
}