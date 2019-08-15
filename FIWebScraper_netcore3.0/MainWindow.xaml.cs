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
using System.Windows.Documents;
using System.Diagnostics;

namespace FIWebScraper_netcore3._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Scraper scraper;
        PushNotice pushNotice;
        public bool ProgramIsRunning { get; set; } = false;
        public decimal SecondsDelay { get; set; } = 700;
        public static int ValueToWarnOver { get; set; } = 0;

        public static StringBuilder ErrorMessages = new StringBuilder();
        public static StringBuilder RegularMessages = new StringBuilder();

        public static bool NewErrorMessage;
        public bool CombineMultipleSales { get; set; } = false;
        public static List<Sale> ListOfSales { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            scraper = new Scraper();
            pushNotice = new PushNotice();
            ListOfSales = new List<Sale>();
            MainWindow1.Title = "Insynshandelsavläsare";
            NewErrorMessage = false;
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {


            StartStopProgram();
            UpdateDataGrid();
            ////////////////////////////////////////////////////Primary loop////////////////////////////////////////////////////////////////////////////////////
            while (ProgramIsRunning)
            {


                await scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                //ListOfSales = await Task.Run(() => scraper.ScrapeData(@"http://192.168.1.35/dashboard/"));
                //ListOfSales = scraper.ScrapeData(@"http://localhost/dashboard/");

                if (!NewErrorMessage)
                {
                    UpdateDataGrid();
                    RegularMessages.Insert(0, $"Uppdaterades kl. {DateTime.Now.ToString("HH:mm:ss")}\n");
                    Log.Text = RegularMessages.ToString();
                }
                else
                {
                    NewErrorMessage = false;
                    ErrorTextBox.Text = ErrorMessages.ToString();
                }


                if (ErrorMessages.Length > 10000)
                {
                    ErrorMessages.Clear();
                }



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
        private void StartStopProgram()
        {
            ProgramIsRunning = !ProgramIsRunning;
            if (ProgramIsRunning)
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
        void OpenLink(object sender, RoutedEventArgs e)
        {
            var destination = ((Hyperlink)e.OriginalSource).NavigateUri;
            Process.Start(@"C:\Program Files\internet explorer\iexplore.exe", destination.ToString());
            Console.Read();
        }
    }
}

