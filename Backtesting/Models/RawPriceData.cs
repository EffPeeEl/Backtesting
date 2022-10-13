using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Backtesting
{
    public class RawPriceData
    {
        // - IF YOU WANT TO USE INDEX Instead
        //[Index(0)]
        //public string _date { get; set; }
        //[Index(1)]
        //public string _bid { get; set; }
        //[Index(2)]
        //public string _ask { get; set; }
        //[Index(3)]
        //public string _openingPrice { get; set; }
        //[Index(4)]
        //public string _highPrice { get; set; }
        //[Index(5)]
        //public string _lowPrice { get; set; }
        //[Index(6)]
        //public string _closingPrice { get; set; }
        //[Index(7)]
        //public string _averagePrice { get; set; }
        //[Index(8)]
        //public string _volume { get; set; }
        //[Index(9)]
        //public string _turnover { get; set; }
        //[Index(10)]
        //public string _trades { get; set; }


        [Name("Date")]
        public string _date { get; set; }
        [Name("Bid")]
        public string _bid { get; set; }
        [Name("Ask")]
        public string _ask { get; set; }
        [Name("Opening price")]
        public string _openingPrice { get; set; }
        [Name("High price")]
        public string _highPrice { get; set; }
        [Name("Low price")]
        public string _lowPrice { get; set; }
        [Name("Closing price")]
        public string _closingPrice { get; set; }
        [Name("Average price")]
        public string _averagePrice { get; set; }
        [Name("Volume")]
        public string _volume { get; set; }
        [Name("Turnover")]
        public string _turnover { get; set; }
        [Name("Trades")]
        public string _trades { get; set; }

        public override string ToString()
        {
            return _date.ToString();
        }

    }

    public class RawStockdataMap : ClassMap<RawPriceData>
    {
        public RawStockdataMap()
        {
            Map(m => m._date).Name("Date");
            Map(m => m._ask).Name("Ask");
            Map(m => m._bid).Name("Bid");
            Map(m => m._openingPrice).Name("Opening price");
            Map(m => m._highPrice).Name("High price");
            Map(m => m._lowPrice).Name("Low price");
            Map(m => m._closingPrice).Name("Closing price");
            Map(m => m._averagePrice).Name("Average price");
            Map(m => m._volume).Name("Total volume");
            Map(m => m._turnover).Name("Turnover");
            Map(m => m._trades).Name("Trades");
        }
    }

}