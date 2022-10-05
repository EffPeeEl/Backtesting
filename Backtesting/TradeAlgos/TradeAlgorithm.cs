using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.TradeAlgos
{
    public abstract class TradeAlgorithm : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public  List<PriceData> Buys { get; set; }
        public  List<PriceData> Sells { get; set; }


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


        private Stock currentStock;

        public abstract double RunAlgorithm(Stock stock);
        public abstract double RunAlgorithm(Stock stock, DateTime startDate);
        public abstract double RunAlgorithm(Stock stock, DateTime startDate, DateTime endDate);

        private void Buy(int howMany, int dateIndex)
        {
            for (int i = 0; i < howMany; i++)
                Buys.Add(currentStock.PriceData[dateIndex]);
        }
        private void Sell(int howMany, int dateIndex)
        {
            for (int i = 0; i < howMany; i++)
                Sells.Add(currentStock.PriceData[dateIndex]);
        }

        private void SellAll(int dateIndex)
        {
            for(int i = 0; i < Buys.Count-Sells.Count; i ++)
                Sells.Add(currentStock.PriceData[dateIndex]);
            
        }


        private void Calculate()
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
            foreach (PriceData pd in Sells)
            {
                Score -= pd.ClosingPrice;
            }
            foreach (PriceData pd in Buys)
            {
                Score += pd.ClosingPrice;
            }

        }

    }
}
