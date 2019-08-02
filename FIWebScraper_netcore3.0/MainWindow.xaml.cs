//using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Notifications.Wpf;
using System.Text.RegularExpressions;

namespace FIWebScraper_netcore3._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Scraper scraper;
        static int textData = 0;
        public decimal SecondsDelay { get; set; } = 5000;
        public int MaxValueBeforeAResponse { get; set; } = 300000;
        static List<string> ListOfPopupMessages = new List<string>();
        public bool ReportOnlyPurchases { get; set; } = false;
        public bool SendPushNotice { get; set; } = true;
        public bool ShowOnlySalesRows { get; set; } = false;
        public bool HideUHandelsplatsRows { get; set; } = false;
        public bool DisableColor { get; set; } = false;
        StringBuilder reportErrorMessages = new StringBuilder();
        public int reportErrorMessagesNumber { get; set; }
        public bool CombineMultipleSales { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();
            scraper = new Scraper();
            MainWindow1.Title = "Insynshandelsavläsare";
            ListOfPopupMessages = new List<string>();
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            var notificationManager = new NotificationManager();
            CheckTextData();

////////////////////////////////////////////////////Primary loop////////////////////////////////////////////////////////////////////////////////////
            while (textData % 2 != 0)
            {
                UpdateDataGrid();


                //tries to download the new version
                try
                {

                    //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                    scraper.ScrapeData(@"http://localhost/dashboard/");

                }
                catch
                {
                    if (reportErrorMessagesNumber != 5)
                    {
                        reportErrorMessages.AppendLine($"Misslyckad uppdatering {DateTime.Now.ToString("HH:mm:ss")}");
                        ErrorTextBox.Text = reportErrorMessages.ToString();
                        reportErrorMessagesNumber++;

                        int.TryParse(SecondsDelay.ToString(), out int timeout2);
                        await Task.Delay(timeout2);
                    }
                    else
                    {
                        reportErrorMessages.Clear();
                        reportErrorMessagesNumber = 0;
                    }
                }


                if (ListOfPopupMessages.Count != 0)
                {
                    foreach (var message in ListOfPopupMessages)
                    {

                    var notice = new NotificationContent();
                    notice.Title = "Ny affär hittad!";
                    notice.Message = message;
                    notice.Type = NotificationType.Information;
                  
                    notificationManager.Show(notice, onClick: () => this.WindowState = WindowState.Maximized);
                    }
                    ListOfPopupMessages.Clear();
                }



                int.TryParse(SecondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);

                ExportToExcelAndCsv();


            }
////////////////////////////////////////////////////End Primary loop////////////////////////////////////////////////////////////////////////////////////

        }
      

        private void CombineMultipleSales_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow1.Title = "Cool";
            CombineMultipleSales = true;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.AllEntries;
        }
        private void CombineMultipleSales_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow1.Title = "Not Cool";
            CombineMultipleSales = false;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.CombinedSales;
        }
        public static void AddNotice(string message)
        {
            ListOfPopupMessages.Add(message);
        }
        private void UpdateDataGrid()
        {
            if (!CombineMultipleSales)
            {
                dataGridView1.ItemsSource = null;
                dataGridView1.ItemsSource = scraper.AllEntries;
            }
            else
            {
                dataGridView1.ItemsSource = null;
                dataGridView1.ItemsSource = scraper.CombinedSales;
            }
        }
        private void CheckTextData()
        {
            textData++;
            if (textData % 2 != 0)
            {
                button1.Content = "Pause";
                MainWindow1.Title = "Programmet Körs";
            }
            else
            {
                button1.Content = "Start";
                MainWindow1.Title = "Insynshandelsavläsare";
            }
        }
        private void RedMarkNewRowsOverTheValue()
        {

            //source.SuspendBinding();
            //if (!DisableColor)
            //{
            //    for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //    {
            //        double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

            //        if (totalt > MaxValueBeforeAResponse)
            //        {
            //            dataGridView1.row[i].DefaultCellStyle.BackColor = Color.Red;
            //            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
            //        }
            //        else
            //        {
            //            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            //            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //    {
            //        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
            //        dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //}
            //source.ResumeBinding();
        }
        private void PreviewTextInputSecondsDelay(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void WarningValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void WarningValueInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(warningValue.Text, out int input);
            if (input != 0)
            {
                MaxValueBeforeAResponse = input;
                MainWindow1.Title = "CoolFronWarning";

            }
        }
        private void SelcondsDelayInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal.TryParse(selcondsDelayInput.Text, out decimal input);
            if (input != 0)
            {
                SecondsDelay = 1000 * input;
                MainWindow1.Title = "Cool";

            }
        }
        private void ExportToExcelAndCsv()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dataGridView1.SelectAllCells();
            dataGridView1.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGridView1);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dataGridView1.UnselectAllCells();
            System.IO.StreamWriter file1 = new System.IO.StreamWriter(path);
            file1.WriteLine(result.Replace(',', ' '));
            file1.Close();

            MessageBox.Show(" Exporting DataGrid data to Excel file created.xls");
        }
    }
}
