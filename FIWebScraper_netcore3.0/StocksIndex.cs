using System;
using System.Collections.Generic;
using System.Text;

namespace FIWebScraper_netcore3._0
{
    public class StocksIndex
    {
        public string LinkToPage { get; set; }
        public string CompanyName { get; set; }
        public string KortnamnVarde { get; set; }
        public string ISINVarde { get; set; }
        public string Antal_Aktier_Varde { get; set; }
    }
}
