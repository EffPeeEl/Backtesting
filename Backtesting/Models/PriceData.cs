using System;
using System.Collections.Generic;

namespace Backtesting
{
    public class PriceData
    {

        public DateTime Date { get; set; }

        public double Bid { get; set; }
        public double Ask { get; set; }
        public double OpeningPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosingPrice { get; set; }
        public double AveragePrice { get; set; }
        public double Volume { get; set; }
        public double Turnover { get; set; }               
        public double Trades { get; set; }

        //Calculated data
        public double SMAx { get; set; }
        public double LowerBollinger { get; set; }
        public double UpperBollinger { get; set; }
        public double StDevXDays { get; set; }

        public override string ToString()
        {
            return Date.ToString();
        }

        public PriceData(RawPriceData data)
        {
            DateTime.TryParse(data._date, out DateTime _date);
            Date = _date;
            Double.TryParse(data._bid, out double _bid);
            Bid = _bid;
            Double.TryParse(data._ask, out double _ask);
            Ask = _ask;
            Double.TryParse(data._openingPrice, out double _openingPrice);
            OpeningPrice = _openingPrice;
            Double.TryParse(data._highPrice, out double _highPrice);
            HighPrice = _highPrice;
            Double.TryParse(data._lowPrice, out double _lowPrice);
            LowPrice = _lowPrice;
            Double.TryParse(data._closingPrice, out double _closingPrice);
            ClosingPrice = _closingPrice;
            Double.TryParse(data._averagePrice, out double _averagePrice);
            AveragePrice = _averagePrice;
            Double.TryParse(data._volume, out double _volume);
            Volume = _volume;
            Double.TryParse(data._turnover, out double _turnover);
            Turnover = _turnover;
            Double.TryParse(data._trades, out double _trades);
            Trades = _trades;
        }

    }
}