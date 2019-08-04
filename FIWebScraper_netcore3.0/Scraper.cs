using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                                FIWebScraper_netcore3._0.MainWindow.AddNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris}");

                            }
                        }
                    }






                    nextPost = nextPost + 16;
                }
                catch
                {
                    FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.AppendLine($"Misslyckade att tyda data {DateTime.Now.ToString("HH:mm:ss")}");
                }
            }

            if (firstDownload == 0)
            {
                firstDownload++;
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
                FIWebScraper_netcore3._0.MainWindow.reportErrorMessages.AppendLine($"Misslyckade att ladda ner {DateTime.Now.ToString("HH:mm:ss")}");
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
                            FIWebScraper_netcore3._0.MainWindow.AddNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris}");
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
