﻿using HtmlAgilityPack;
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

        private ObservableCollection<Sale> _sales = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> Sales
        {
            get { return _sales; }
            set { _sales = value; }
        }


        private ObservableCollection<Sale> _addedSales = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> AddedSales
        {
            get { return _addedSales; }
            set { _addedSales = value; }
        }

        public void ScrapeData(string page)
        {

            List<string> listOfText = DownloadNewVersion(page);

            int nextPost = 0;
            for (int i = 0; i < 10; i++)
            {
                DateTime publishDateParsed = new DateTime();
                //Creates a new sale
                try
                {

                    DateTime.TryParse(listOfText[0 + nextPost], out publishDateParsed);

                }
                catch
                {
                    Console.Read();
                }
                var timeNow = DateTime.Now.ToString("HH:mm:ss");
                DateTime.TryParse(listOfText[8 + nextPost], out DateTime transactionDateParsed);
                double.TryParse(listOfText[9 + nextPost], out double volymParsed);
                double.TryParse(listOfText[11 + nextPost], out double prisParsed);

                var sale = new Sale { saleNumber=numberOfSales + 1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed };


                //checks if record already exists with person and total cost
                bool recordExistInSaleList = EntryAlreadyExistsInSaleList(sale);

                //Checks if entry is already combined to one row
                bool entryAlreadyExistsInAddedList = EntryAlreadyExistsInAlreadyAddedList(sale);


                //checks if person has bought many and combines the ammount to one row. statusrow updates with number of sales, total volume and total cost is correct
                bool isSecondPurchaseOfSameStock = EntryHasBeenAddedToOneRow(sale, recordExistInSaleList, entryAlreadyExistsInAddedList);


                Console.Read();
                //if it doesnt exist, add it to the main interface
                //if (!recordExistInSaleList && sale.Publiceringsdatum == DateTime.Today)
                if (!recordExistInSaleList && !isSecondPurchaseOfSameStock && !entryAlreadyExistsInAddedList && sale.Publiceringsdatum == DateTime.Today)
                {
                    if (firstDownload == 0)
                    {
                        Sales.Insert(Sales.Count,sale);
                        AddedSales.Insert(AddedSales.Count,new Sale { saleNumber = numberOfSales+1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed });
                        numberOfSales++;
                    }
                    else
                    {
                        Sales.Insert(0,sale);
                        AddedSales.Insert(0,new Sale { saleNumber = numberOfSales + 1, Publiceringsdatum = publishDateParsed, Tid = timeNow, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed });
                        numberOfSales++;

                        //Somehow this adds it to the first row
                        AddedSales[0].saleNumber = sale.saleNumber;
                        AddedSales[0].Publiceringsdatum = sale.Publiceringsdatum;
                        AddedSales[0].Tid = sale.Tid;
                        AddedSales[0].Utgivare = sale.Utgivare;
                        AddedSales[0].Namn = sale.Namn;
                        AddedSales[0].Befattning = sale.Befattning;
                        AddedSales[0].Närstående = sale.Närstående;
                        AddedSales[0].Karaktär = sale.Karaktär;
                        AddedSales[0].Instrumentnamn = sale.Instrumentnamn;
                        AddedSales[0].ISIN = sale.ISIN;
                        AddedSales[0].Transaktionsdatum = sale.Transaktionsdatum;
                        AddedSales[0].Volym = sale.Volym;
                        AddedSales[0].Volymsenhet = sale.Volymsenhet;
                        AddedSales[0].Pris = sale.Pris;
                        AddedSales[0].Totalt = sale.Totalt;
                        AddedSales[0].Valuta = sale.Valuta;
                        AddedSales[0].Antal_Affärer = sale.Antal_Affärer;
                        AddedSales[0].Handelsplats = sale.Handelsplats;
                        AddedSales[0].Status = sale.Status;
                        AddedSales[0].Detaljer = sale.Detaljer;

                        Sales[0].saleNumber = sale.saleNumber;
                        Sales[0].Publiceringsdatum = sale.Publiceringsdatum;
                        Sales[0].Tid = sale.Tid;
                        Sales[0].Utgivare = sale.Utgivare;
                        Sales[0].Namn = sale.Namn;
                        Sales[0].Befattning = sale.Befattning;
                        Sales[0].Närstående = sale.Närstående;
                        Sales[0].Karaktär = sale.Karaktär;
                        Sales[0].Instrumentnamn = sale.Instrumentnamn;
                        Sales[0].ISIN = sale.ISIN;
                        Sales[0].Transaktionsdatum = sale.Transaktionsdatum;
                        Sales[0].Volym = sale.Volym;
                        Sales[0].Volymsenhet = sale.Volymsenhet;
                        Sales[0].Pris = sale.Pris;
                        Sales[0].Totalt = sale.Totalt;
                        Sales[0].Valuta = sale.Valuta;
                        Sales[0].Antal_Affärer = sale.Antal_Affärer;
                        Sales[0].Handelsplats = sale.Handelsplats;
                        Sales[0].Status = sale.Status;
                        Sales[0].Detaljer = sale.Detaljer;



                        FIWebScraper_netcore3._0.MainWindow.PushNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris}");

                    }
                }


                nextPost = nextPost + 16;
            }

            if (firstDownload == 0)
            {
                firstDownload++;
            }

        }

        private void MoveAllRowsDown()
        {
            if (numberOfSales != 0)
            {
                for (int i = numberOfSales - 1; i >= 0; i--)
                {
                    // scraper.AddedSales[0].Namn = "Martin";
                    var number = i;
                }
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

            }
            return listOfText;
        }

        private bool EntryHasBeenAddedToOneRow(Sale sale, bool recordExistInSaleList, bool entryAlreadyExistsInAddedList)
        {
            bool result = false;



            if (!recordExistInSaleList && !entryAlreadyExistsInAddedList)
            {
                foreach (var record in Sales)
                {
                    if (sale.Utgivare == record.Utgivare && sale.Namn == record.Namn && sale.Befattning == record.Befattning && sale.Karaktär == record.Karaktär && sale.Instrumentnamn == record.Instrumentnamn)
                    {
                        //Add made another sale respond here, only new sales from the same company gets here
                        record.Volym = record.Volym + sale.Volym;
                        record.Totalt = record.Totalt + sale.Totalt;
                        result = true;
                        record.Antal_Affärer++;
                        //AddedSales.Add(sale);
                        AddedSales.Insert(0, sale);
                        numberOfSales++;

                        AddedSales[0].saleNumber = sale.saleNumber;
                        AddedSales[0].Publiceringsdatum = sale.Publiceringsdatum;
                        AddedSales[0].Tid = sale.Tid;
                        AddedSales[0].Utgivare = sale.Utgivare;
                        AddedSales[0].Namn = sale.Namn;
                        AddedSales[0].Befattning = sale.Befattning;
                        AddedSales[0].Närstående = sale.Närstående;
                        AddedSales[0].Karaktär = sale.Karaktär;
                        AddedSales[0].Instrumentnamn = sale.Instrumentnamn;
                        AddedSales[0].ISIN = sale.ISIN;
                        AddedSales[0].Transaktionsdatum = sale.Transaktionsdatum;
                        AddedSales[0].Volym = sale.Volym;
                        AddedSales[0].Volymsenhet = sale.Volymsenhet;
                        AddedSales[0].Pris = sale.Pris;
                        AddedSales[0].Totalt = sale.Totalt;
                        AddedSales[0].Valuta = sale.Valuta;
                        AddedSales[0].Antal_Affärer = sale.Antal_Affärer;
                        AddedSales[0].Handelsplats = sale.Handelsplats;
                        AddedSales[0].Status = sale.Status;
                        AddedSales[0].Detaljer = sale.Detaljer;

                        FIWebScraper_netcore3._0.MainWindow.PushNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris}");


                    }

                }
            }

            return result;
        }

        private bool EntryAlreadyExistsInSaleList(Sale newEntry)
        {
            bool result = false;

            foreach (var entry in Sales)
            {
                if (newEntry.Publiceringsdatum == entry.Publiceringsdatum && newEntry.Utgivare == entry.Utgivare && newEntry.Namn == entry.Namn && newEntry.Befattning == entry.Befattning && newEntry.Närstående == entry.Närstående && newEntry.Karaktär == entry.Karaktär && newEntry.Instrumentnamn == entry.Instrumentnamn && newEntry.ISIN == entry.ISIN && newEntry.Transaktionsdatum == entry.Transaktionsdatum && newEntry.Volym == entry.Volym && newEntry.Volymsenhet == entry.Volymsenhet && newEntry.Pris == entry.Pris &&  newEntry.Totalt == entry.Totalt && newEntry.Valuta == entry.Valuta && newEntry.Handelsplats == entry.Handelsplats)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool EntryAlreadyExistsInAlreadyAddedList(Sale newEntry)
        {

            bool result = false;

            foreach (var entry in AddedSales)
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
