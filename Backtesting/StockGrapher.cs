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

namespace Backtesting
{
    public class StockGrapher
    {
        
        private Stock _stock;
        ChartValues<double> _CV;
        ChartValues<double> _CVI;
        public Axis axe;

        private double _bollingerStDevs;
        private int _bollingerAvgDayCount;

        string[] _Labels;

 
        public StockGrapher(string fileName)
        {
            _stock = new Stock(fileName);
        }
        public StockGrapher()
        {

        }

        public SeriesCollection CreateCartesian()
        {

            //CartesianChart C = new CartesianChart();

            //ChartValues<double> Cv = new ChartValues<double>();

            //foreach (var item in testList)
            //{
            //    Cv.Add(Convert.ToDouble(item._closingPrice));
            //}

            //LineSeries Ls = new LineSeries()
            //{
            //    Title = fileName,
            //    Values = Cv
            //};

            //SeriesCollection sc = new SeriesCollection
            //{
            //    Ls

            //};

            //C.Series = sc;


            //return C.Series;
            return new SeriesCollection();
        }

        public SeriesCollection CreateCandle(int candleSize)
        {
            List<PriceData> priceList = _stock._priceData;
            double highest;
            double lowest;
            double starting;
            double closing;

            ChartValues<OhlcPoint> ohlcP = new ChartValues<OhlcPoint>();

            //Days used in candle
            SeriesCollection PTEST = CreateBollinger(candleSize);

            for (int i = 0; i < priceList.Count; i++)
            {
                highest = 0;
                lowest = 150000;
                starting = priceList[i].ClosingPrice;

                //Checks if on last round, if so, finishes the loop
                if (i >= priceList.Count - candleSize)
                    candleSize = priceList.Count - i;

                for (int j = 0; j < candleSize ; j++)
                {

                    if (priceList[i].ClosingPrice > highest)
                        highest = priceList[i].ClosingPrice;
                    if (priceList[i].ClosingPrice < lowest)
                        lowest = priceList[i].ClosingPrice;
                    i++;
                }
                i--;

                closing = priceList[i-1].ClosingPrice;

                

                ohlcP.Add(new OhlcPoint(starting, highest, lowest, closing));
            }

            
               

            SeriesCollection SC = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = ohlcP,
                    PointGeometry = null

                },                
                new LineSeries()
                {
                    Values = _CV,
                    PointGeometry = null

                },
                new LineSeries()
                {
                    Values = _CVI,
                    PointGeometry = null
                }

            };

            CartesianChart CC = new CartesianChart
            {
                Series = SC,

            };

            return SC;
            
        }

        private SeriesCollection CreateBollinger(int candleSize)
        {
            _stock.CreateBollingerValues(_bollingerStDevs, _bollingerAvgDayCount);

            List<PriceData> priceList = _stock._priceData;
            ChartValues<double> BollingerLowerSeries = new ChartValues<double>();
            ChartValues<double> BollingerUpperSeries = new ChartValues<double>();

            int remainder = priceList.Count % candleSize;

            List<string>_Labels = new List<string>();


            
            // NOTE TO SELF; MIGHT BE BETTEER TO WRITE ALGO THAT WRITES BOLLINGER IN CSV FLE INSTEAD
            for (int i = 0; i < priceList.Count; i += candleSize)
            {

                BollingerLowerSeries.Add(_stock._priceData[i].LowerBollinger);
                BollingerUpperSeries.Add(_stock._priceData[i].UpperBollinger);
                _Labels.Add(_stock._priceData[i].Date.ToShortDateString());
            }

            axe = new Axis()
            {
                Labels = _Labels.ToArray()
            };
            

            _CV = BollingerLowerSeries; 
            _CVI = BollingerUpperSeries;

            SeriesCollection sc = new SeriesCollection()
            {
                
                new LineSeries()
                {
                    Values = BollingerLowerSeries,
                    PointGeometry = null

                },
                new LineSeries()
                {
                    Values = BollingerUpperSeries,
                    PointGeometry = null
                }
            };

            return sc;

        }

        internal SeriesCollection CreateGraph(string senderUID, string typeOfChart, int daysInCandle, double bollingerStDev, int bollingerDays)
        {
            _bollingerStDevs = bollingerStDev;
            _bollingerAvgDayCount = bollingerDays;
            _stock = new Stock(senderUID);

            switch (typeOfChart)
            {
                case "Candle":
                    return CreateCandle(daysInCandle);
                case "Line":
                    return CreateCartesian();
                default:
                    return new SeriesCollection();
                   
            }  


        }
    }


}