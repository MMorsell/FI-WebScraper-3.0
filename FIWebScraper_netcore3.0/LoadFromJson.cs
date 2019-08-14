using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FIWebScraper_netcore3._0
{
    public class LoadFromJson
    {
        public List<StocksIndex> ListOfStocksIndex { get; set; }

        public LoadFromJson()
        {


            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var filePath = Path.Combine(path, "products.json");

            var productData = File.ReadAllText(filePath);

            ListOfStocksIndex = JsonConvert.DeserializeObject<List<StocksIndex>>(productData);
        }


        public List<StocksIndex> GetAllStocks()
        {
            return ListOfStocksIndex;
        }

        

    }
}
