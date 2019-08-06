using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using FIWebScraper_netcore3._0;

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
                    var notice = new NotificationContent();
                    notice.Title = "Ny affär hittad!";
                    notice.Message = message;
                    notice.Type = NotificationType.Information;

                    notificationManager.Show(notice);
                    //notificationManager.Show(notice, onClick: () => this.WindowState = WindowState.Maximized);
                    //notificationManager.Show("hello", expirationTime);
                    ////notificationManager.Show(notice, "IsThisThingOn", expirationTime, onClick: () => this.WindowState = WindowState.Maximized);
                }
                ListOfPopupMessages.Clear();
                excel.WriteToExcel(AllSales);
            }
        }
        public static void AddNotice(string notice)
        {
            ListOfPopupMessages.Add(notice);
        }
    }
}
