using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FIWebScraper_netcore3._0
{
    public class ExcelWriter
    {
        public void WriteToExcel(List<Sale> AllSales)
        {
            try
            {
                using (var package = new ExcelPackage())
                {


                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    System.IO.Directory.CreateDirectory(@$"{path}\Insynsaffärer Loggar\");

                    var workSheet = package.Workbook.Worksheets.Add(DateTime.Now.ToShortDateString());


                    //header
                    workSheet.Cells[1, 1].Value = "Datum";
                    workSheet.Cells[1, 2].Value = "Tid";
                    workSheet.Cells[1, 3].Value = "Utgivare";
                    workSheet.Cells[1, 4].Value = "Person i ledande ställning";
                    workSheet.Cells[1, 5].Value = "Befattning";
                    workSheet.Cells[1, 6].Value = "Karaktär";
                    workSheet.Cells[1, 7].Value = "Transaktionsdatum";
                    workSheet.Cells[1, 8].Value = "Volym";
                    workSheet.Cells[1, 9].Value = "Volymsenhet";
                    workSheet.Cells[1, 10].Value = "Pris";
                    workSheet.Cells[1, 11].Value = "Totalt";
                    workSheet.Cells[1, 12].Value = "Antal aktier";
                    workSheet.Cells[1, 13].Value = "Valuta";
                    workSheet.Cells[1, 14].Value = "Handelsplats";
                    workSheet.Cells[1, 15].Value = "Avanza";



                    //all entries
                    int rowIndex = 2;
                    foreach (var entry in AllSales)
                    {
                        workSheet.Cells[rowIndex, 1].Value = entry.Publiceringsdatum.ToString();
                        workSheet.Cells[rowIndex, 2].Value = entry.Tid;
                        workSheet.Cells[rowIndex, 3].Value = entry.Utgivare;
                        workSheet.Cells[rowIndex, 4].Value = entry.Namn;
                        workSheet.Cells[rowIndex, 5].Value = entry.Befattning;
                        workSheet.Cells[rowIndex, 6].Value = entry.Karaktär;
                        workSheet.Cells[rowIndex, 7].Value = entry.Transaktionsdatum.ToString();
                        workSheet.Cells[rowIndex, 8].Value = entry.Volym;
                        workSheet.Cells[rowIndex, 9].Value = entry.Volymsenhet;
                        workSheet.Cells[rowIndex, 10].Value = entry.Pris;
                        workSheet.Cells[rowIndex, 11].Value = entry.Totalt;
                        workSheet.Cells[rowIndex, 12].Value = entry.Antal_Aktier_Varde;
                        workSheet.Cells[rowIndex, 13].Value = entry.Valuta;
                        workSheet.Cells[rowIndex, 14].Value = entry.Handelsplats;
                        workSheet.Cells[rowIndex, 15].Value = entry.LinkToAvanza;
                        rowIndex++;
                    }




                    //fix formatting
                    workSheet.Cells[$"H2:H{AllSales.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"J2:J{AllSales.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"K2:K{AllSales.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    //workSheet.Cells[$"A2:A{AllSales.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    //workSheet.Cells[$"J2:J{AllSales.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    ////workSheet.Cells[$"P2:P{AllSales.Count + 2}"].Style.Numberformat.Format = "0:P2";
                    workSheet.Cells.AutoFitColumns(0);

                    var filePath = new FileInfo(@$"{path}\Insynsaffärer Loggar\{DateTime.Now.ToShortDateString()}.xlsx");
                    package.SaveAs(filePath);

                }
            }
            catch
            {
                FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"Misslyckade att skriva til excel {DateTime.Now.ToString("HH:mm:ss")}\n");
                FIWebScraper_netcore3._0.MainWindow.NewErrorMessage = true;
            }
        }
        public void ReadFromExcel()
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    var filePath = new FileInfo(@$"{path}\Insynsaffärer Loggar\{DateTime.Now.ToShortDateString()}.xlsx");

                    using (var stream = new FileStream(filePath.ToString(), FileMode.Open))
                    {
                        package.Load(stream);
                    }

                    var currentWorksheet = package.Workbook.Worksheets[0];
                    
                    //Gets number of rows from possibly already created file
                    int numberOfRows = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        var row = currentWorksheet.Cells[i + 1, 1].Value;
                        if (row == null)
                        {
                            break;
                        }
                        numberOfRows = i + 1;
                    }

                    var listOfEntries = new List<Sale>();
                    for (int i = 2; i < numberOfRows + 1; i++)
                    {
                        var entry = new Sale();
                        
                        var a = (string)currentWorksheet.Cells[i, 1].Value;
                        DateTime.TryParse(a, out DateTime publicerindsDateParsed);
                        entry.Publiceringsdatum = publicerindsDateParsed;
                        
                        entry.Tid = (string)currentWorksheet.Cells[i, 2].Value;
                        entry.Utgivare = (string)currentWorksheet.Cells[i, 3].Value;
                        entry.Namn = (string)currentWorksheet.Cells[i, 4].Value;
                        entry.Befattning = (string)currentWorksheet.Cells[i, 5].Value;
                        entry.Karaktär = (string)currentWorksheet.Cells[i, 6].Value;

                        var b = (string)currentWorksheet.Cells[i, 7].Value;
                        DateTime.TryParse(b, out DateTime transactionsDateParsed);
                        entry.Transaktionsdatum = transactionsDateParsed;
                        
                        entry.Volym = (double)currentWorksheet.Cells[i, 8].Value;
                        entry.Volymsenhet = (string)currentWorksheet.Cells[i, 9].Value;
                        entry.Pris = (double)currentWorksheet.Cells[i, 10].Value;
                        entry.Totalt = (double)currentWorksheet.Cells[i, 11].Value;
                        entry.Antal_Aktier_Varde = (string)currentWorksheet.Cells[i, 12].Value;
                        entry.Valuta = (string)currentWorksheet.Cells[i, 13].Value;
                        entry.Handelsplats = (string)currentWorksheet.Cells[i, 14].Value;
                        entry.LinkToAvanza = (string)currentWorksheet.Cells[i, 15].Value;
                        listOfEntries.Add(entry);
                    }


                    FIWebScraper_netcore3._0.MainWindow.ListOfSales = listOfEntries;


                }
            }
            catch
            {
                FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"Om inget har loggats idag så kan detta ignoreras");
                FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"Misslyckade att läsa från excel {DateTime.Now.ToString("HH:mm:ss")}\n");
                FIWebScraper_netcore3._0.MainWindow.NewErrorMessage = true;
            }
        }


    }
}
