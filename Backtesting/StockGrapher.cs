using System;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.VisualBasic;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Defaults;
using SciChart.Charting.Model.DataSeries;

namespace Backtesting
{
    public class StockGrapher
    {
        
        public Stock _stock;
        public StockGrapher(string stockUID)
        {
            _stock = new Stock(stockUID);
        }


        public OhlcDataSeries<DateTime, double> CreateCandleSciChart(int candleSize)
        {
            List<PriceData> priceList = _stock._priceData;
            double highest;
            double lowest;
            double starting;
            double closing;

            OhlcDataSeries<DateTime, double> ohlcSci = new OhlcDataSeries<DateTime, double>();


            for (int i = 0; i < priceList.Count; i++)
            {
                highest = 0;
                lowest = 150000;
                starting = priceList[i].ClosingPrice;

                //Checks if on last round, if so, finishes the loop
                if (i >= priceList.Count - candleSize)
                    candleSize = priceList.Count - i;

                for (int j = 0; j < candleSize; j++)
                {

                    if (priceList[i].ClosingPrice > highest)
                        highest = priceList[i].ClosingPrice;
                    if (priceList[i].ClosingPrice < lowest)
                        lowest = priceList[i].ClosingPrice;
                    i++;
                }
                i--;

                closing = priceList[i].ClosingPrice;

                ohlcSci.Append(priceList[i].Date, starting, highest, lowest, closing);



                //highest = 0;
                //lowest = 150000;
                //starting = priceList[i].ClosingPrice;

                ////Checks if on last round, if so, finishes the loop
                //if (i >= priceList.Count - candleSize)
                //    candleSize = priceList.Count - i;

                //for (int j = 0; j < candleSize; j++)
                //{

                //    if (priceList[i].ClosingPrice > highest)
                //        highest = priceList[i].ClosingPrice;
                //    if (priceList[i].ClosingPrice < lowest)
                //        lowest = priceList[i].ClosingPrice;
                //    i++;
                //}
                //i--;

                //closing = priceList[i - 1].ClosingPrice;

                //ohlcSci.Append(priceList[i].Date, starting, highest, lowest, closing);
            }

            return ohlcSci;

        }

        

        public XyDataSeries<DateTime, double> CreateBollingerSci(int candleSize, bool isUpper, double _bollingerStDevs, int _bollingerAvgDayCount)
        {
            _stock.CreateBollingerValues(_bollingerStDevs, _bollingerAvgDayCount);

            
            XyDataSeries<DateTime, double> BollingerSeries = new XyDataSeries<DateTime, double>();
            double sum;
            double averageInCandle = 0;

            

            // NOTE TO SELF; MIGHT BE BETTEER TO WRITE ALGO THAT WRITES BOLLINGER IN CSV FLE INSTEAD
            if(isUpper)
                for (int i = candleSize; i < _stock._priceData.Count; i += candleSize)
                {
                    sum = 0;
                    for (int j = 0; j < candleSize; j++)
                    {
                        sum += _stock._priceData[i-j].UpperBollinger;
                    }
                    averageInCandle = sum / candleSize;
                    BollingerSeries.Append(_stock._priceData[i].Date, averageInCandle);
                }
            else
            {
                for (int i = candleSize; i < _stock._priceData.Count; i += candleSize)
                {
                    sum = 0;
                    for (int j = 0; j < candleSize; j++)
                    {
                        sum += _stock._priceData[i - j].LowerBollinger;
                    }
                    averageInCandle = sum / candleSize;
                    BollingerSeries.Append(_stock._priceData[i].Date, averageInCandle);

                }
            }


            return BollingerSeries;

        }

        
        internal OhlcDataSeries<DateTime, double> CreateGraphSci(string senderUID, string typeOfChart, int daysInCandle)
        {
            switch (typeOfChart)
            {
                case "Candle":
                    return CreateCandleSciChart(daysInCandle);
                default:
                    return new OhlcDataSeries<DateTime, double>();

            }
        }

        public (DateTime, double) GetNextDayData()
        {
            (DateTime, double) data = (DateTime.Now, 0);
            if (index < _stock._priceData.Count)
            {
            
                data = (_stock._priceData[index].Date, _stock._priceData[index].ClosingPrice);
                index++;

            }
            
            return data;
        }

        internal bool isIndexMax()
        {
            return index == _stock._priceData.Count;
        }

        private int index;
        #region Livechart Stuff
        //public SeriesCollection CreateCartesian()
        //{

        //    //CartesianChart C = new CartesianChart();

        //    //ChartValues<double> Cv = new ChartValues<double>();

        //    //foreach (var item in testList)
        //    //{
        //    //    Cv.Add(Convert.ToDouble(item._closingPrice));
        //    //}

        //    //LineSeries Ls = new LineSeries()
        //    //{
        //    //    Title = fileName,
        //    //    Values = Cv
        //    //};

        //    //SeriesCollection sc = new SeriesCollection
        //    //{
        //    //    Ls

        //    //};

        //    //C.Series = sc;


        //    //return C.Series;
        //    return new SeriesCollection();
        //}
        //public SeriesCollection CreateCandle(int candleSize)
        //{
        //    List<PriceData> priceList = _stock._priceData;
        //    double highest;
        //    double lowest;
        //    double starting;
        //    double closing;

        //    ChartValues<OhlcPoint> ohlcP = new ChartValues<OhlcPoint>();

        //    //Days used in candle
        //    SeriesCollection PTEST = CreateBollinger(candleSize);

        //    for (int i = 0; i < priceList.Count; i++)
        //    {
        //        highest = 0;
        //        lowest = 150000;
        //        starting = priceList[i].ClosingPrice;

        //        //Checks if on last round, if so, finishes the loop
        //        if (i >= priceList.Count - candleSize)
        //            candleSize = priceList.Count - i;

        //        for (int j = 0; j < candleSize; j++)
        //        {

        //            if (priceList[i].ClosingPrice > highest)
        //                highest = priceList[i].ClosingPrice;
        //            if (priceList[i].ClosingPrice < lowest)
        //                lowest = priceList[i].ClosingPrice;
        //            i++;
        //        }
        //        i--;

        //        closing = priceList[i - 1].ClosingPrice;



        //        ohlcP.Add(new OhlcPoint(starting, highest, lowest, closing));
        //    }




        //    SeriesCollection SC = new SeriesCollection
        //    {
        //        new OhlcSeries()
        //        {
        //            Values = ohlcP,
        //            PointGeometry = null

        //        },
        //        new LineSeries()
        //        {
        //            Values = _CV,
        //            PointGeometry = null

        //        },
        //        new LineSeries()
        //        {
        //            Values = _CVI,
        //            PointGeometry = null
        //        }

        //    };

        //    CartesianChart CC = new CartesianChart
        //    {
        //        Series = SC,

        //    };

        //    return SC;

        //}
        //internal SeriesCollection CreateGraph(string senderUID, string typeOfChart, int daysInCandle, double bollingerStDev, int bollingerDays)
        //{

        //    _stock = new Stock(senderUID);

        //    switch (typeOfChart)
        //    {
        //        case "Candle":
        //            return CreateCandle(daysInCandle);
        //        case "Line":
        //            return CreateCartesian();
        //        default:
        //            return new SeriesCollection();

        //    }

        //}
        //private SeriesCollection CreateBollinger(int candleSize)
        //{

        //    List<PriceData> priceList = _stock._priceData;
        //    ChartValues<double> BollingerLowerSeries = new ChartValues<double>();
        //    ChartValues<double> BollingerUpperSeries = new ChartValues<double>();

        //    int remainder = priceList.Count % candleSize;

        //    List<string> _Labels = new List<string>();



        //    // NOTE TO SELF; MIGHT BE BETTEER TO WRITE ALGO THAT WRITES BOLLINGER IN CSV FLE INSTEAD
        //    for (int i = 0; i < priceList.Count; i += candleSize)
        //    {

        //        BollingerLowerSeries.Add(_stock._priceData[i].LowerBollinger);
        //        BollingerUpperSeries.Add(_stock._priceData[i].UpperBollinger);
        //        _Labels.Add(_stock._priceData[i].Date.ToShortDateString());
        //    }

        //    axe = new Axis()
        //    {
        //        Labels = _Labels.ToArray()
        //    };


        //    _CV = BollingerLowerSeries;
        //    _CVI = BollingerUpperSeries;

        //    SeriesCollection sc = new SeriesCollection()
        //    {

        //        new LineSeries()
        //        {
        //            Values = BollingerLowerSeries,
        //            PointGeometry = null

        //        },
        //        new LineSeries()
        //        {
        //            Values = BollingerUpperSeries,
        //            PointGeometry = null
        //        }
        //    };

        //    return sc;

        //}
        #endregion

    }


}