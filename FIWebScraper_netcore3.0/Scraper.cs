using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FIWebScraper_netcore3._0
{
    public class Scraper
    {
        public int numberOfSales { get; set; }
        int firstDownload = 0;

        private ObservableCollection<Sale> _combinedSales = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> CombinedSales
        {
            get { return _combinedSales; }
            set { _combinedSales = value; }
        }


        private ObservableCollection<Sale> _allEntries = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> AllEntries
        {
            get { return _allEntries; }
            set { _allEntries = value; }
        }

        public void ScrapeData(string page)
        {
            var tempBuilder = new StringBuilder();
            int tempBuilderCount = 0;
            List<string> listOfText = DownloadNewVersion(page);

            int nextPost = 0;
            for (int i = 0; i < 10; i++)
            {
                //Creates a new sale
                try
                {
                    DateTime.TryParse(listOfText[0 + nextPost], out DateTime publishDateParsed);
                    var timeNow = DateTime.Now.ToString("HH:mm:ss");
                    DateTime.TryParse(listOfText[8 + nextPost], out DateTime transactionDateParsed);
                    double.TryParse(listOfText[9 + nextPost], out double volymParsed);
                    double.TryParse(listOfText[11 + nextPost], out double prisParsed);

                    var sale = new Sale { saleNumber = numberOfSales + 1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed };

                    //checks if record already exists with person and total cost
                    bool recordExistInSaleList = EntryAlreadyExistsInCombinedSalesList(sale);

                    //Checks if entry is already combined to one row
                    bool entryAlreadyExistsInAddedList = EntryAlreadyExistsInAllEntriesList(sale);


                    //checks if person has bought many and combines the ammount to one row. statusrow updates with number of sales, total volume and total cost is correct
                    bool isSecondPurchaseOfSameStock = EntryHasBeenAddedToOneRow(sale, recordExistInSaleList, entryAlreadyExistsInAddedList);




                    //if it doesnt exist, add it to the main interface
                    if (!recordExistInSaleList && !isSecondPurchaseOfSameStock && !entryAlreadyExistsInAddedList && sale.Publiceringsdatum == DateTime.Today)
                    {
                        if (firstDownload == 0)
                        {
                            //Sales.Insert(Sales.Count,sale);
                            AllEntries.Insert(AllEntries.Count, new Sale { saleNumber = numberOfSales + 1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed });
                            numberOfSales++;
                        }
                        else
                        {
                            //Sales.Insert(0,sale);
                            AllEntries.Insert(0, new Sale { saleNumber = numberOfSales + 1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed });
                            numberOfSales++;
                            if (sale.Totalt > MainWindow.MaxValueBeforeAResponse)
                            {

                                FIWebScraper_netcore3._0.MainWindow.AddNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris} {sale.Valuta}");

                            }
                        }
                    }






                    nextPost = nextPost + 16;
                }
                catch
                {
                    tempBuilder.Insert(0,$"FEL! ej tydbar data {DateTime.Now.ToString("HH:mm:ss")}\n");
                    FIWebScraper_netcore3._0.MainWindow.newErrorMessage = true;
                    tempBuilderCount++;
                }
            }

            if (firstDownload == 0)
            {
                firstDownload++;
            }


            if (tempBuilderCount == 10)
            {
                FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.Insert(0, $"FEL! ej tydbar data {DateTime.Now.ToString("HH:mm:ss")}\n");
            }
            else
            {
                FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.Insert(0, tempBuilder);
            }


        }

        public void WriteToExcel()
        {
            try
            {
                using (var package = new ExcelPackage())
                {


                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    System.IO.Directory.CreateDirectory(@$"{path}\InsynsAffärer\");

                    var workSheet = package.Workbook.Worksheets.Add(DateTime.Now.ToShortDateString());


                    //header
                    workSheet.Cells[1, 1].Value = "Datum";
                    workSheet.Cells[1, 2].Value = "Tid";
                    workSheet.Cells[1, 3].Value = "Utgivare";
                    workSheet.Cells[1, 4].Value = "Person i ledande ställning";
                    workSheet.Cells[1, 5].Value = "Befattning";
                    workSheet.Cells[1, 6].Value = "Närstående";
                    workSheet.Cells[1, 7].Value = "Karaktär";
                    workSheet.Cells[1, 8].Value = "Instrumentnamn";
                    workSheet.Cells[1, 9].Value = "ISIN";
                    workSheet.Cells[1, 10].Value = "Transaktionsdatum";
                    workSheet.Cells[1, 11].Value = "Volym";
                    workSheet.Cells[1, 12].Value = "Volymsenhet";
                    workSheet.Cells[1, 13].Value = "Pris";
                    workSheet.Cells[1, 14].Value = "Totalt";
                    workSheet.Cells[1, 15].Value = "Valuta";
                    workSheet.Cells[1, 16].Value = "Handelsplats";



                    //all entries
                    int rowIndex = 2;
                    foreach (var entry in AllEntries)
                    {
                        workSheet.Cells[rowIndex, 1].Value = entry.Publiceringsdatum;
                        workSheet.Cells[rowIndex, 2].Value = entry.Tid;
                        workSheet.Cells[rowIndex, 3].Value = entry.Utgivare;
                        workSheet.Cells[rowIndex, 4].Value = entry.Namn;
                        workSheet.Cells[rowIndex, 5].Value = entry.Befattning;
                        workSheet.Cells[rowIndex, 6].Value = entry.Närstående;
                        workSheet.Cells[rowIndex, 7].Value = entry.Karaktär;
                        workSheet.Cells[rowIndex, 8].Value = entry.Instrumentnamn;
                        workSheet.Cells[rowIndex, 9].Value = entry.ISIN;
                        workSheet.Cells[rowIndex, 10].Value = entry.Transaktionsdatum;
                        workSheet.Cells[rowIndex, 11].Value = entry.Volym;
                        workSheet.Cells[rowIndex, 12].Value = entry.Volymsenhet;
                        workSheet.Cells[rowIndex, 13].Value = entry.Pris;
                        workSheet.Cells[rowIndex, 14].Value = entry.Totalt;
                        workSheet.Cells[rowIndex, 15].Value = entry.Valuta;
                        workSheet.Cells[rowIndex, 16].Value = entry.Handelsplats;
                        rowIndex++;
                    }




                    //fix formatting
                    workSheet.Cells[$"K2:K{AllEntries.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"N2:N{AllEntries.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"A2:A{AllEntries.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Cells[$"J2:J{AllEntries.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Cells.AutoFitColumns(0);

                    var filePath = new FileInfo(@$"{path}\InsynsAffärer\{DateTime.Now.ToShortDateString()}.xlsx");
                    package.SaveAs(filePath);

                }
            }
            catch 
            {
                FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.Insert(0, $"Misslyckade att skriva til excel {DateTime.Now.ToString("HH:mm:ss")}\n");
                FIWebScraper_netcore3._0.MainWindow.newErrorMessage = true;
            }



        }
    

        private List<string> DownloadNewVersion(string page)
        {
            var webInterface = new HtmlWeb();
            List<string> listOfText = new List<string>();
            try
            {
                var htmlDocument = webInterface.Load(page);




                var outerDiv = htmlDocument.DocumentNode.SelectSingleNode("//*[@class = 'table table-bordered table-hover table-striped zero-margin-top']");

                var outerDivText = outerDiv.InnerText;

                listOfText = outerDivText.Split('\n').ToList();

                listOfText.RemoveRange(0, 24);
                listOfText.RemoveRange(listOfText.Count - 1, 1);

                for (int i = 0; i < listOfText.Count; i++)
                {
                    listOfText[i] = listOfText[i].Replace('"', '!');
                    listOfText[i] = listOfText[i].Trim();
                }

                List<int> intPositionsOfDetaljer = new List<int>();
                for (int i = 0; i < listOfText.Count; i++)
                {
                    if (listOfText[i].Equals("Detaljer"))
                    {
                        intPositionsOfDetaljer.Add(i);
                    }
                }


                intPositionsOfDetaljer.Reverse();

                foreach (var intPosition in intPositionsOfDetaljer)
                {
                    listOfText.RemoveRange(intPosition + 1, 4);
                    listOfText.RemoveRange(intPosition - 4, 4);

                }

                for (int i = 0; i < listOfText.Count; i++)
                {
                    listOfText[i] = System.Net.WebUtility.HtmlDecode(listOfText[i]);
                }

            }
            catch
            {
                FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.Insert(0,$"FEL! internet timeout {DateTime.Now.ToString("HH:mm:ss")}\n");
                FIWebScraper_netcore3._0.MainWindow.newErrorMessage = true;
                return listOfText;
            }
            return listOfText;
        }

        private bool EntryHasBeenAddedToOneRow(Sale sale, bool recordExistInSaleList, bool entryAlreadyExistsInAddedList)
        {
            bool result = false;



            if (!recordExistInSaleList && !entryAlreadyExistsInAddedList)
            {
                foreach (var record in CombinedSales)
                {
                    if (sale.Utgivare == record.Utgivare && sale.Namn == record.Namn && sale.Befattning == record.Befattning && sale.Karaktär == record.Karaktär && sale.Instrumentnamn == record.Instrumentnamn)
                    {
                        //Add made another sale respond here, only new sales from the same company gets here
                        record.Volym = record.Volym + sale.Volym;
                        record.Totalt = record.Totalt + sale.Totalt;
                        result = true;
                        record.Antal_Affärer++;
                        //AddedSales.Add(sale);
                        //AddedSales.Insert(0, sale);
                        AllEntries.Insert(0, sale);
                        numberOfSales++;

                        if (firstDownload != 0 && sale.Totalt > MainWindow.MaxValueBeforeAResponse)
                        {
                            FIWebScraper_netcore3._0.MainWindow.AddNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris} {sale.Valuta}");
                        }


                    }

                }
            }

            return result;
        }

        private bool EntryAlreadyExistsInCombinedSalesList(Sale newEntry)
        {
            bool result = false;

            foreach (var entry in CombinedSales)
            {
                if (newEntry.Publiceringsdatum == entry.Publiceringsdatum && newEntry.Utgivare == entry.Utgivare && newEntry.Namn == entry.Namn && newEntry.Befattning == entry.Befattning && newEntry.Närstående == entry.Närstående && newEntry.Karaktär == entry.Karaktär && newEntry.Instrumentnamn == entry.Instrumentnamn && newEntry.ISIN == entry.ISIN && newEntry.Transaktionsdatum == entry.Transaktionsdatum && newEntry.Volym == entry.Volym && newEntry.Volymsenhet == entry.Volymsenhet && newEntry.Pris == entry.Pris &&  newEntry.Totalt == entry.Totalt && newEntry.Valuta == entry.Valuta && newEntry.Handelsplats == entry.Handelsplats)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool EntryAlreadyExistsInAllEntriesList(Sale newEntry)
        {

            bool result = false;

            foreach (var entry in AllEntries)
            {
                //if (newEntry.Publiceringsdatum == entry.Publiceringsdatum && newEntry.Utgivare == entry.Utgivare && newEntry.Namn == entry.Namn && newEntry.Transaktionsdatum == entry.Transaktionsdatum && newEntry.Pris == entry.Pris && newEntry.Volym == entry.Volym && newEntry.Totalt == entry.Totalt)
                if (newEntry.Publiceringsdatum == entry.Publiceringsdatum && newEntry.Utgivare == entry.Utgivare && newEntry.Namn == entry.Namn && newEntry.Befattning == entry.Befattning && newEntry.Närstående == entry.Närstående && newEntry.Karaktär == entry.Karaktär && newEntry.Instrumentnamn == entry.Instrumentnamn && newEntry.ISIN == entry.ISIN && newEntry.Transaktionsdatum == entry.Transaktionsdatum && newEntry.Volym == entry.Volym && newEntry.Volymsenhet == entry.Volymsenhet && newEntry.Pris == entry.Pris && newEntry.Totalt == entry.Totalt && newEntry.Valuta == entry.Valuta && newEntry.Handelsplats == entry.Handelsplats)
                {
                    result = true;
                }
            }


            return result;
        }
    }
}
