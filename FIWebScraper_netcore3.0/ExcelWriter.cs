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
                    foreach (var entry in AllSales)
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
                    workSheet.Cells[$"K2:K{AllSales.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"N2:N{AllSales.Count + 2}"].Style.Numberformat.Format = "#,##0.00";
                    workSheet.Cells[$"A2:A{AllSales.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Cells[$"J2:J{AllSales.Count + 2}"].Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Cells.AutoFitColumns(0);

                    var filePath = new FileInfo(@$"{path}\InsynsAffärer\{DateTime.Now.ToShortDateString()}.xlsx");
                    package.SaveAs(filePath);

                }
            }
            catch
            {
                FIWebScraper_netcore3._0.MainWindow.ErrorMessages.Insert(0, $"Misslyckade att skriva til excel {DateTime.Now.ToString("HH:mm:ss")}\n");
                FIWebScraper_netcore3._0.MainWindow.NewErrorMessage = true;
            }



        }
    }
}
