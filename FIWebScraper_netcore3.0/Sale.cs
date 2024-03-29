﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FIWebScraper_netcore3._0
{
    public class Sale
    {
        public int saleNumber { get; set; }
        public DateTime Publiceringsdatum { get; set; }
        public string Tid { get; set; }
        public string Utgivare { get; set; }
        public string Namn { get; set; }
        public string Befattning { get; set; }
        public string Närstående { get; set; }
        public string Karaktär { get; set; }
        public string Instrumentnamn { get; set; }
        public string ISIN { get; set; }
        public DateTime Transaktionsdatum { get; set; }
        public double Volym { get; set; }
        public string Volymsenhet { get; set; }
        public double Pris { get; set; }
        public string Valuta { get; set; }
        public double Totalt { get; set; }
        public int Antal_Affärer { get; set; } = 1;
        public string Handelsplats { get; set; }
        public string Status { get; set; } = "0";
        public string Detaljer { get; set; }
        public string LinkToAvanza { get; set; }
        public string CompanyName { get; set; }
        public string KortnamnVarde { get; set; }
        public string Antal_Aktier_Varde { get; set; }
        public double Antal_Aktier_Compared_To_Sale { get; set; }
    }
}
