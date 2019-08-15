using HtmlAgilityPack;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FIWebScraper_netcore3._0.PushNotice;

namespace FIWebScraper_netcore3._0
{
    public class Scraper
    {
        public int NumberOfSales { get; set; }
        public bool FirstDownload { get; set; } = true;
        public List<StocksIndex> AllStocksIndex { get; set; }
        public HtmlDocument CurrentDocument { get; set; }
        private LoadFromJson jsonContext;

        private ObservableCollection<Sale> _allEntries = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> AllEntries
        {
            get { return _allEntries; }
            set { _allEntries = value; }
        }


        public Scraper()
        {
            jsonContext = new LoadFromJson();
            AllStocksIndex = new List<StocksIndex>();
            AllStocksIndex = jsonContext.GetAllStocks();
        }


        public async Task ScrapeData(string page)
        {
            List<string> ListOfText = DownloadNewVersion(page);

            List<Sale> ListOfPossiblyNewSales = CombindeListOfTextToSales(ListOfText);

            foreach (var sale in ListOfPossiblyNewSales)
            {
                bool saleAlreadyExist = false;
                foreach (var alreadyAddedSale in AllEntries)
                {
                    if (
                        sale.Publiceringsdatum == alreadyAddedSale.Publiceringsdatum &&
                        sale.Utgivare == alreadyAddedSale.Utgivare &&
                        sale.Namn == alreadyAddedSale.Namn &&
                        sale.Befattning == alreadyAddedSale.Befattning &&
                        sale.Karaktär == alreadyAddedSale.Karaktär &&
                        sale.Transaktionsdatum == alreadyAddedSale.Transaktionsdatum &&
                        sale.Volym == alreadyAddedSale.Volym &&
                        sale.Volymsenhet == alreadyAddedSale.Volymsenhet &&
                        sale.Pris == alreadyAddedSale.Pris &&
                        sale.Valuta == alreadyAddedSale.Valuta &&
                        sale.Handelsplats == alreadyAddedSale.Handelsplats
                        )
                    {
                        saleAlreadyExist = true;
                        break;
                    }
                }



                if (!saleAlreadyExist)
                {
                    foreach (var item in jsonContext.ListOfStocksIndex)
                    {
                        if (sale.ISIN == item.ISINVarde)
                        {
                            sale.LinkToAvanza = item.LinkToPage;
                            int.TryParse(item.Antal_Aktier_Varde.ToString(), out int result);
                            sale.Antal_Affärer = result;
                            break;
                        }
                    } 
                }
                



                if (!saleAlreadyExist)
                {
                    AllEntries.Add(sale);
                    if (sale.Totalt >= MainWindow.ValueToWarnOver)
                    {
                        AddNotice($"{sale.Namn} har {sale.Karaktär} {sale.Volym} st \ntill kursen {sale.Pris} {sale.Valuta}");
                    }
                    FIWebScraper_netcore3._0.MainWindow.UpdateGrid = true;
                }
            }

            FIWebScraper_netcore3._0.MainWindow.ListOfSales = AllEntries.ToList();
           
            //    catch
            //    {
            //        tempBuilder.Insert(0,$"FEL! ej tydbar data {DateTime.Now.ToString("HH:mm:ss")}\n");
            //        FIWebScraper_netcore3._0.MainWindow.NewErrorMessage = true;
            //        tempBuilderCount++;
           

            //if (tempBuilderCount == 10)
            //{
            //    FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"FEL! ej tydbar data {DateTime.Now.ToString("HH:mm:ss")}\n");
            //}
            //else
            //{
            //    FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, tempBuilder);
            //}


        }

        private List<Sale> CombindeListOfTextToSales(List<string> ListOfText)
        {
            var returnList = new List<Sale>();
            int position = 0;
            for (int i = 0; i < 10; i++)
            {
                DateTime.TryParse(ListOfText[0 + position], out DateTime publisDate);
                if (publisDate == DateTime.Today)
                {
                    var sale = new Sale();
                    sale.Publiceringsdatum = publisDate;
                    sale.Tid = DateTime.Now.ToString("HH:mm:ss");
                    sale.Utgivare = ListOfText[1 + position];
                    sale.Namn = ListOfText[2 + position];
                    sale.Befattning = ListOfText[3 + position];
                    sale.Karaktär = ListOfText[5 + position];
                    DateTime.TryParse(ListOfText[8 + position], out DateTime tdate);
                    sale.Transaktionsdatum = tdate;
                    double.TryParse(ListOfText[9 + position], out double volym);
                    sale.Volym = volym;
                    sale.Volymsenhet = ListOfText[10 + position];
                    double.TryParse(ListOfText[11 + position], out double pris);
                    sale.Pris = pris;
                    sale.Totalt = volym * pris;
                    sale.Valuta = ListOfText[12 + position];
                    sale.Handelsplats = ListOfText[13 + position];
                    sale.ISIN = ListOfText[7 + position];
                    returnList.Add(sale);
                }
                
                position = position + 16;
            }
            return returnList;
        }

        private List<string> DownloadNewVersion(string page)
        {
            var webInterface = new HtmlWeb();
            List<string> ListOfText = new List<string>();

            var htmlDocument = webInterface.Load(page);

            //TODO, Add discard update if first sale is the same
            var items = htmlDocument.DocumentNode.SelectSingleNode("//tbody").SelectNodes("//td");

            foreach (var item in items)
            {
                ListOfText.Add(System.Net.WebUtility.HtmlDecode(item.InnerText.ToString().Trim()));
            }

            int validNumberOfSales = ListOfText.Count / 16;


            //catch
            //{
            //    FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"FEL! internet timeout {DateTime.Now.ToString("HH:mm:ss")}\n");
            //    FIWebScraper_netcore3._0.MainWindow.NewErrorMessage = true;
            //    return null;
            //}
            items = null;
            webInterface = null;
            htmlDocument = null;
            return ListOfText;
        }
    }
}
