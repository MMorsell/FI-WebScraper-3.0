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
                //try
                //{

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://192.168.1.35/dashboard/");

                //}
                //catch
                //{
                //    if (reportErrorMessagesNumber != 5)
                //    {
                //        reportErrorMessages.AppendLine($"Misslyckad uppdatering {DateTime.Now.ToString("HH:mm:ss")}");
                //        textBox3.Text = reportErrorMessages.ToString();
                //        reportErrorMessagesNumber++;

                //        int.TryParse(SecondsDelay.ToString(), out int timeout2);
                //        await Task.Delay(timeout2);
                //    }
                //    else
                //    {
                //        reportErrorMessages.Clear();
                //        reportErrorMessagesNumber = 0;
                //    }
                //}


                ControlAllCheckStates();


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




            }
////////////////////////////////////////////////////End Primary loop////////////////////////////////////////////////////////////////////////////////////

        }



        private void ControlAllCheckStates()
        {

            //Display options
            DisplayOnlySelectedData();


            //Notification options below
            //Warn only about purchases
            if (CheckBox2.IsChecked.GetValueOrDefault())
            {
                ReportOnlyPurchases = true;
            }
            else
            {
                ReportOnlyPurchases = false;
            }


            UpdateCellColors();
        }

        private void DisplayOnlySelectedData()
        {
            //source.SuspendBinding();
            //if (ShowOnlySalesRows)
            //{
            //    if (HideUHandelsplatsRows)
            //    {
            //        for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //        {
            //            if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase) && !dataGridView1.Rows[i].Cells[16].Value.ToString().Equals("Utanför handelsplats", StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                dataGridView1.Rows[i].Visible = true;
            //            }
            //            else
            //            {
            //                dataGridView1.Rows[i].Visible = false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //        {
            //            if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                dataGridView1.Rows[i].Visible = true;
            //            }
            //            else
            //            {
            //                dataGridView1.Rows[i].Visible = false;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (HideUHandelsplatsRows)
            //    {
            //        for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //        {
            //            if (!dataGridView1.Rows[i].Cells[16].Value.ToString().Equals("Utanför handelsplats", StringComparison.CurrentCultureIgnoreCase))
            //            {
            //                dataGridView1.Rows[i].Visible = true;
            //            }
            //            else
            //            {
            //                dataGridView1.Rows[i].Visible = false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //        {
            //            dataGridView1.Rows[i].Visible = true;
            //        }
            //    }
            //}
            //source.ResumeBinding();
        }


        private void UpdateCellColors()
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

        private void CheckBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckBox1.IsChecked.GetValueOrDefault())
            {
                ShowOnlySalesRows = true;
            }
            else
            {
                ShowOnlySalesRows = false;
            }
            DisplayOnlySelectedData();
        }

        private void CheckBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckBox2.IsChecked.GetValueOrDefault())
            {
                HideUHandelsplatsRows = true;
            }
            else
            {
                HideUHandelsplatsRows = false;
            }
            DisplayOnlySelectedData();
        }

        private void CheckBox3_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckBox3.IsChecked.GetValueOrDefault())
            {
                ReportOnlyPurchases = true;

            }
            else
            {
                ReportOnlyPurchases = true;

            }
        }

        private void CheckBox4_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckBox4.IsChecked.GetValueOrDefault())
            {
                SendPushNotice = false;
            }
            else
            {
                SendPushNotice = true;
            }
        }

        private void CheckBox5_CheckStateChanged_1(object sender, EventArgs e)
        {
            //if (CheckBox5.IsChecked.GetValueOrDefault())
            //{
            //    DisableColor = true;
            //}
            //else
            //{
            //    DisableColor = false;
            //}

            //ControlAllCheckStates();
        }

        private void WarningValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(warningValue.Text, out int input);
            if (input != 0)
            {
                MaxValueBeforeAResponse = input;
                MainWindow1.Title = "CoolFronWarning";
                UpdateCellColors();

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

        private void CombineMultipleSales_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow1.Title = "Cool";
            CombineMultipleSales = true;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.AddedSales;
        }

        private void CombineMultipleSales_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow1.Title = "Not Cool";
            CombineMultipleSales = false;
            dataGridView1.ItemsSource = null;
            dataGridView1.ItemsSource = scraper.Sales;
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
                dataGridView1.ItemsSource = scraper.AddedSales;
            }
            else
            {
                dataGridView1.ItemsSource = null;
                dataGridView1.ItemsSource = scraper.Sales;
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
    }
}
