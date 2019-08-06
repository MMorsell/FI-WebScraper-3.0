//using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        PushNotice pushNotice;
        static int textData = 0;
        public decimal SecondsDelay { get; set; } = 7000;
        public static int ValueToWarnOver { get; set; } = 0;

        public static StringBuilder ReportErrorMessages = new StringBuilder();

        public static bool NewErrorMessage;
        public bool CombineMultipleSales { get; set; } = false;
        public List<Sale> ListOfSales { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            scraper = new Scraper();
            pushNotice = new PushNotice();
            ListOfSales = new List<Sale>();
            MainWindow1.Title = "Insynshandelsavläsare";

        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {

            CheckTextData();
            NewErrorMessage = false;

////////////////////////////////////////////////////Primary loop////////////////////////////////////////////////////////////////////////////////////
            while (textData % 2 != 0)
            {
                UpdateDataGrid();


                    //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                    ListOfSales = scraper.ScrapeData(@"http://192.168.1.35/dashboard/");



                if (!NewErrorMessage)
                {
                    ReportErrorMessages.Insert(0, $"Uppdaterades kl. {DateTime.Now.ToString("HH:mm:ss")}\n");
                }
                else
                {
                    NewErrorMessage = false;
                }

                ErrorTextBox.Text = ReportErrorMessages.ToString();

                //int numberOfNewErrorMessages = reportErrorMessages.Length - reportErrorMessagesNumber;
                //if (reportErrorMessages.Length + numberOfNewErrorMessages > reportErrorMessages.Length)
                //{
                //    //for (int i = reportErrorMessages.Length; i < reportErrorMessagesNumber; i++)
                //    //{
                //    //    ErrorTextBox.Text = reportErrorMessages[i].ToString();

                //    //}
                //}


                pushNotice.CheckForNewMessages(ListOfSales);


                int.TryParse(SecondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);




            }
////////////////////////////////////////////////////End Primary loop////////////////////////////////////////////////////////////////////////////////////

        }
      

        private void CombineMultipleSales_Checked(object sender, RoutedEventArgs e)
        {
            CombineMultipleSales = true;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.AllEntries;
        }
        private void CombineMultipleSales_Unchecked(object sender, RoutedEventArgs e)
        {
            CombineMultipleSales = false;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.CombinedSales;
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
        private void PreviewTextInputSecondsDelay(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void WarningValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^0-9\.\-\,]+").IsMatch(e.Text);
        }
        private void WarningValueInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(warningValue.Text, out int input);
            if (input != 0)
            {
                ValueToWarnOver = input;

            }
        }
        private void SelcondsDelayInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal.TryParse(selcondsDelayInput.Text, out decimal input);
            if (input != 0)
            {
                SecondsDelay = 1000 * input;

            }
        }
    }
}
