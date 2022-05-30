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
        
        public Stock SelectedStock;

        public StockGrapher(string stockUID)
        {
            SelectedStock = new Stock(stockUID);
        }

        public OhlcDataSeries<DateTime, double> CreateCandleSciChart(int candleSize)
        {

            OhlcDataSeries<DateTime, double> ohlcSci = new OhlcDataSeries<DateTime, double>();

            for (int i = 0; i < Math.Ceiling((double)SelectedStock.PriceData.Count / candleSize); i++)
            {
                var candleData = GetNextDayDataCandle(i * candleSize, candleSize);

                ohlcSci.Append(candleData.Item1, candleData.Item2.Open, candleData.Item2.High, candleData.Item2.Low, candleData.Item2.Close);
                
            }

            return ohlcSci;

        }

        

        public XyDataSeries<DateTime, double> CreateBollingerSci(int candleSize, bool isUpper, double _bollingerStDevs, int _bollingerAvgDayCount)
        {
            SelectedStock.CreateBollingerValues(_bollingerStDevs, _bollingerAvgDayCount);

            
            XyDataSeries<DateTime, double> BollingerSeries = new XyDataSeries<DateTime, double>();
            double sum;
            double averageInCandle = 0;

            // NOTE TO SELF; MIGHT BE BETTEER TO WRITE ALGO THAT WRITES BOLLINGER IN CSV FLE INSTEAD
            if(isUpper)
                for (int i = candleSize; i < SelectedStock.PriceData.Count; i += candleSize)
                {
                    sum = 0;
                    for (int j = 0; j < candleSize; j++)
                    {
                        sum += SelectedStock.PriceData[i-j].UpperBollinger;
                    }
                    averageInCandle = sum / candleSize;
                    BollingerSeries.Append(SelectedStock.PriceData[i].Date, averageInCandle);
                }
            else
                for (int i = candleSize; i < SelectedStock.PriceData.Count; i += candleSize)
                {
                    sum = 0;
                    for (int j = 0; j < candleSize; j++)
                    {
                        sum += SelectedStock.PriceData[i - j].LowerBollinger;
                    }
                    averageInCandle = sum / candleSize;
                    BollingerSeries.Append(SelectedStock.PriceData[i].Date, averageInCandle);

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

        public (DateTime, double) GetNextDayDataBollinger(int dayIndex, bool isUpper)
        {
            (DateTime, double) data = (DateTime.Now, 0);
            if (isUpper)
                data = (SelectedStock.PriceData[dayIndex].Date, SelectedStock.PriceData[dayIndex].UpperBollinger);
            else
                data = (SelectedStock.PriceData[dayIndex].Date, SelectedStock.PriceData[dayIndex].LowerBollinger);

            return data;
        }

        public (DateTime, OhlcPoint) GetNextDayDataCandle(int dayIndex, int candleSize)
        {
            double highest;
            double lowest;
            double starting;
            double closing;
            (DateTime, OhlcPoint) data;

            //Case where last candle amount isnt divisible 
            if(dayIndex + candleSize >= SelectedStock.PriceData.Count)
                candleSize = SelectedStock.PriceData.Count - dayIndex;

            highest = 0;
            lowest = 1500000;
            starting = SelectedStock.PriceData[dayIndex].ClosingPrice;

            for (int i = 0; i < candleSize; i++)
            {

                if (SelectedStock.PriceData[dayIndex + i].ClosingPrice > highest)
                    highest = SelectedStock.PriceData[dayIndex + i].ClosingPrice;
                if (SelectedStock.PriceData[dayIndex + i].ClosingPrice < lowest)
                    lowest = SelectedStock.PriceData[dayIndex + i].ClosingPrice;

            }

            closing = SelectedStock.PriceData[dayIndex + candleSize - 1].ClosingPrice;
            data.Item1 = SelectedStock.PriceData[dayIndex + candleSize - 1].Date;
            data.Item2 = new OhlcPoint(starting, highest, lowest, closing);

            return data;

        }


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