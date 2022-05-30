﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.TradeAlgos
{
    public abstract class TradeAlgo : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public  Stock AlgoSelectedStock { get; set; }

        public  List<PriceData> Buys { get; set; }
        public  List<PriceData> HistoricBuys { get; set; }
        public  List<PriceData> Sells { get; set; }
        public  List<PriceData> HistoricSells { get; set; }

        public  string AlgoName { get; set; }

        private double _score;
        public double Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }




        public abstract void Buy(int index, int howMany);
        public abstract void Sell(int index, int howMany);
        public abstract void Run();
        public abstract string Run(int index);

        public void Calculate()
        {
            
            int over = 0;
            foreach (PriceData pd in Buys)
            {
                Score -= pd.ClosingPrice;

            }
            foreach (PriceData pd in Sells)
            {
                Score += pd.ClosingPrice;

            }

        }

        public string LastBuy()
        {
            return $"[{Buys[Buys.Count - 1].Date}] Bought for {Buys[Buys.Count - 1].ClosingPrice}";
        }


        public void WriteToFile()
        {
            Calculate();
            File.WriteAllText("result_of_algos.txt", $"{AlgoName}: {Score}");

        }

        public override string ToString()
        {
            return AlgoName;
        }

        public void CalculateFinalScore()
        {
            Score = 0;
            foreach (PriceData pd in HistoricBuys)
            {
                Score -= pd.ClosingPrice;
            }
            foreach (PriceData pd in HistoricSells)
            {
                Score += pd.ClosingPrice;
            }

        }

    }
}
