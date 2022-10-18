using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Backtesting

{
    public class Stock
    {
        public string _Name;
        public string _Ticker;
        public List<PriceData> PriceData;
        private string _filePath;

        public List<FinancialData> Financials;

        
        
        public Stock(string fileName)
        {
            _Name = fileName.Remove(fileName.IndexOf('-'), fileName.Length - fileName.IndexOf('-'));

            PriceData = new List<PriceData>();

            _filePath = fileName;



            //Config for CSV
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                Delimiter = ";"

            };

            var rawDataList = new List<RawPriceData>();
            //CSV-READER
            using (var reader = new StreamReader($@"{_filePath}"))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RawStockdataMap>();

                rawDataList = csv.GetRecords<RawPriceData>().ToList();
                var records = csv.GetRecords<RawPriceData>();

            }
            rawDataList.Reverse();

            //Converts from raw PriceData from CSV file String to mathable
            foreach (var item in rawDataList)
            {
                PriceData.Add(new PriceData(item));
            }

        }

        //Can make this take variables that changes how many stDev, days etc
        public void CreateBollingerValues(double howManyStDevsFromAvg, int howManyDays)
        {
            List<double> avgList = new List<double>();
            List<double> stDevList = new List<double>();


            double average = 0;
            double sum = PriceData[0].ClosingPrice;
            double stDev = 0;
            double stDevSum = 0;


            PriceData[0].SMAx = PriceData[0].ClosingPrice;
            PriceData[0].LowerBollinger = PriceData[0].ClosingPrice;
            PriceData[0].UpperBollinger = PriceData[0].ClosingPrice;

            //Average of first n before 20
            for (int i = 1; i < howManyDays; i++)
            {
                average = sum / i;
                sum += PriceData[i].ClosingPrice;
                

                stDevSum += Math.Pow(PriceData[i].ClosingPrice - average, 2);

                stDev = Math.Sqrt(stDevSum / i);

                PriceData[i].SMAx = average;
                PriceData[i].StDevXDays = stDev;
                PriceData[i].LowerBollinger = (average - (howManyStDevsFromAvg * stDev));
                PriceData[i].UpperBollinger = (average + (howManyStDevsFromAvg * stDev));

                
            }


            //Algo to not run average formula i times, just/20 times
            for (int i = howManyDays; i < PriceData.Count; i++)
            {
                average = sum / howManyDays;
                sum -= PriceData[i - howManyDays].ClosingPrice;
                sum += PriceData[i].ClosingPrice;

                

                stDevSum -= Math.Pow(PriceData[i - howManyDays].ClosingPrice - PriceData[i - howManyDays].SMAx, 2);
                stDevSum += Math.Pow(PriceData[i].ClosingPrice - average, 2);

                if (stDevSum < 0)
                    stDevSum = stDevSum * -1;

                stDev = Math.Sqrt(stDevSum / howManyDays);


                PriceData[i].SMAx = average;
                PriceData[i].StDevXDays = stDev;
                PriceData[i].LowerBollinger = (average - (howManyStDevsFromAvg * stDev));
                PriceData[i].UpperBollinger = (average + (howManyStDevsFromAvg * stDev));


            }


        }

        public static string GetTickerFromFileName(string fileName)
        {
            return fileName.Remove(fileName.IndexOf('-'), fileName.Length - fileName.IndexOf('-'));
        }

        public override string ToString()
        {
            return _Name;
        }

        
    }
}