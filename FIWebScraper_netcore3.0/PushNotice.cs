using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using FIWebScraper_netcore3._0;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace FIWebScraper_netcore3._0
{
    public class PushNotice
    {
        public static List<string> ListOfPopupMessages { get; set; }
        NotificationManager notificationManager;
        ExcelWriter excel;
        public PushNotice()
        {
            ListOfPopupMessages = new List<string>();
            notificationManager = new NotificationManager();
            excel = new ExcelWriter();
        }


        public void CheckForNewMessages(List<Sale> AllSales)
        {
            if (ListOfPopupMessages.Count != 0)
            {
                foreach (var message in ListOfPopupMessages)
                {

                    using (TaskbarIcon tbi = new TaskbarIcon())
                    {
                        tbi.TrayBalloonTipClicked += new RoutedEventHandler(BalloonTip_Clicked);
                        string title = "Ny affär hittad!";
                        string bla = message;

                        //show balloon with custom icon
                        tbi.ShowBalloonTip(title, bla, BalloonIcon.Info);
                    }



                    //var notice = new NotificationContent();
                    //notice.Title = "Ny affär hittad!";
                    //notice.Message = message;
                    //notice.Type = NotificationType.Information;

                    //notificationManager.Show(notice);
                    ////notificationManager.Show(notice, onClick: () => this.WindowState = WindowState.Maximized);
                    ////notificationManager.Show("hello", expirationTime);
                    ////notificationManager.Show(notice, "IsThisThingOn", expirationTime, onClick: () => this.WindowState = WindowState.Maximized);
                }
                ListOfPopupMessages.Clear();
                excel.WriteToExcel(AllSales);
            }
        }

        private void BalloonTip_Clicked(object sender, RoutedEventArgs e)
        {
            Console.Read();
            Console.Read();
            Console.Read();
        }

        public static void AddNotice(string notice)
        {
            ListOfPopupMessages.Add(notice);
        }
    }
}
