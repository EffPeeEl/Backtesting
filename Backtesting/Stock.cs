using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Backtesting

{
    public class Stock
    {
        public string _Name;
        public string _Ticker;
        public List<PriceData> _priceData;
        private string _filePath;
        
        public Stock(string fileName)
        {
            _Name = fileName.Remove(fileName.IndexOf('-'), fileName.Length - fileName.IndexOf('-'));

            _priceData = new List<PriceData>();
            _filePath = @"D:\Finance\OMX Stonks" + "\\" + fileName;

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
                _priceData.Add(new PriceData(item));
            }

        }

        //Can make this take variables that changes how many stDev, days etc
        public void CreateBollingerValues(double howManyStDevsFromAvg, int howManyDays)
        {
            List<double> avgList = new List<double>();
            List<double> stDevList = new List<double>();


            double average = 0;
            double sum = 0;
            double stDev = 0;
            double stDevSum = 0;

            _priceData[0].LowerBollinger = _priceData[1].ClosingPrice;
            _priceData[0].UpperBollinger = _priceData[1].ClosingPrice;

            //Average of first n before 20
            for (int i = 1; i < howManyDays; i++)
            {
                sum += _priceData[i].ClosingPrice;
                average = sum / i;
                avgList.Add(average);

                stDevSum += (_priceData[i].ClosingPrice - average) * (_priceData[i].ClosingPrice - average);

                stDev = Math.Sqrt(stDevSum / i);

                stDevList.Add(stDev);

                _priceData[i].LowerBollinger = (average - (howManyStDevsFromAvg * stDev));
                _priceData[i].UpperBollinger = (average + (howManyStDevsFromAvg * stDev));
            }


            //Algo to not run average formula i times, just/20 times
            for (int i = howManyDays; i < _priceData.Count; i++)
            {
                sum -= _priceData[i - howManyDays].ClosingPrice;
                sum += _priceData[i].ClosingPrice;
                average = sum / howManyDays;

                stDevSum -= (_priceData[i - howManyDays].ClosingPrice - average) * (_priceData[i - howManyDays].ClosingPrice - average);
                stDevSum += (_priceData[i].ClosingPrice - average) * (_priceData[i].ClosingPrice - average);

                stDev = Math.Sqrt(stDevSum / howManyDays);

                avgList.Add(average);
                stDevList.Add(stDev);

                _priceData[i].LowerBollinger = (average - (howManyStDevsFromAvg * stDev));
                _priceData[i].UpperBollinger = (average + (howManyStDevsFromAvg * stDev));
            }


        }

        public override string ToString()
        {
            return _Name;
        }

        
    }
}