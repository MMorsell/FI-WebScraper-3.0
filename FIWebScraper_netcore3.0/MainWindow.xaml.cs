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
        public List<string> ListOfAlertMessagesSent { get; set; }

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
            ListOfAlertMessagesSent = new List<string>();


        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            CheckTextData();

            //Primary loop
            while (textData % 2 != 0)
            {


                //Updates the data

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

                //tries to download the new version
                //try
                //{

                var notificationManager = new NotificationManager();

                //var notis = new NotificationContent();
                //notis.Title = "sample";
                //notis.Message = "babababbba";
                //notis.Type = NotificationType.Information;

                //notificationManager.Show(notis);
                //notificationManager.Show(new NotificationContent
                //{
                //    Title = "Sample notification",
                //    Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",

                //    Type = NotificationType.Success
                //});

                // notificationManager.Show()



                //notificationManager.Show(
                //new NotificationContent { Title = "Notification", Message = "Notification in window!" },
                //areaName: "WindowArea");

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

                //Delay until next update
                int.TryParse(SecondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);
                notificationManager.Show("Nytt köp av Viktor myehsekk", onClick: () => this.WindowState = WindowState.Maximized,
               onClose: () => Console.WriteLine("Closed!"));

                //Måste "flytta" ner alla poster....
                scraper.AddedSales[0].Namn = "Martin";

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
            PushNotice();
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

        private void PushNotice()
        {
            //NotifyIcon notifyIcon = new NotifyIcon()
            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{
            //    double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

            //    if (totalt > MaxValueBeforeAResponse)
            //    {
            //        //Add numberformat here
            //        //string formattedTotal = $"{0:N}";
            //        //string msg = string.Format(formattedTotal, totalt);
            //        string message = $"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {totalt} på {dataGridView1.Rows[i].Cells[7].Value}";
            //        NotifyIcon notifyIcon = new NotifyIcon
            //        {
            //            Visible = true,
            //            BalloonTipTitle = $"Ny Affär av {dataGridView1.Rows[i].Cells[3].Value}",
            //            BalloonTipText = message,
            //            Icon = SystemIcons.Application
            //        };


            //        bool alreadySentAlert = CheckIfMessageIsAlreadySent(message);

            //        if (!alreadySentAlert && SendPushNotice)
            //        {


            //            if (ReportOnlyPurchases)
            //            {
            //                if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase))
            //                {
            //                    notifyIcon.ShowBalloonTip(30000);
            //                    ListOfAlertMessagesSent.Add(message);
            //                }

            //            }
            //            else
            //            {
            //                notifyIcon.ShowBalloonTip(30000);
            //                ListOfAlertMessagesSent.Add(message);
            //            }



            //        }

            //        notifyIcon.Dispose();
            //    }
            //}
        }

        private bool CheckIfMessageIsAlreadySent(string v)
        {
            bool result = false;
            foreach (var message in ListOfAlertMessagesSent)
            {
                if (message == v)
                {
                    result = true;
                }
            }

            return result;
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

            PushNotice();
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

            PushNotice();
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

        private void DataGridView1_Click(object sender, EventArgs e)
        {
            ControlAllCheckStates();
        }

        private void WarningValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(warningValue.Text, out int input);
            if (input != 0)
            {
                MaxValueBeforeAResponse = input;
                MainWindow1.Title = "CoolFronWarning";
                UpdateCellColors();
                PushNotice();

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
    }
}
