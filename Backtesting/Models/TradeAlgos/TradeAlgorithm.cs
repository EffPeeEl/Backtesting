using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


        public TradeAlgorithm()
        {
            Actions = new ObservableCollection<AlgoAction>();
            Buys = new ObservableCollection<PriceData>();
            Sells = new ObservableCollection<PriceData>();
        }

        public ObservableCollection<AlgoAction> Actions;


        public ObservableCollection<PriceData> Buys { get; set; }
        public ObservableCollection<PriceData> Sells { get; set; }


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

        public abstract AlgoAction RunAlgorithmSingleStep(Stock stock, int index);


        public void RunAlgorithm(Stock stock)
        {
            RunAlgorithm(stock, stock.PriceData[0].Date, stock.PriceData[stock.PriceData.Count - 1].Date);
        }
        public void RunAlgorithm(Stock stock, DateTime startDate)
        {
            RunAlgorithm(stock, startDate, stock.PriceData[stock.PriceData.Count - 1].Date);
        }
        public void RunAlgorithm(Stock stock, DateTime startDate, DateTime endDate)
        {

            //Kanske kan gå och ändra med en IndexOf() - metod men iom att det är en property av klassen som ska hittas gör jag såhär
            int indexToStart = 0;
            int indexToEnd = 0;
            for (int i = 0; i < stock.PriceData.Count; i++)
            {
                if (stock.PriceData[i].Date == startDate)
                {
                    indexToStart = i;
                }
                if (stock.PriceData[i].Date == endDate)
                {
                    indexToEnd = i;
                }
            }


            for (int i = indexToStart; i <= indexToEnd; i++)
            {
                RunAlgorithmSingleStep(stock, i);
            }


        }

        protected void Buy(Stock s, int howMany, int dateIndex)
        {
            for (int i = 0; i < howMany; i++)
            {
                Buys.Add(s.PriceData[dateIndex]);
            }

            Actions.Add(new AlgoAction(TradeAction.Buy, howMany, s.PriceData[dateIndex]));

        }
        protected void Sell(int howMany, int dateIndex)
        {
            for (int i = 0; i < howMany; i++)
                Sells.Add(currentStock.PriceData[dateIndex]);
        }
        protected void SellAll(int dateIndex)
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

    public class AlgoAction
    {
        public TradeAction Action { get; private set; }
        public int Amount { get; private set; }
        public PriceData StockPrice { get; private set; }


        public AlgoAction(TradeAction action, int amount, PriceData stockPrice)
        {
            this.Action = action;
            this.Amount = amount;
            this.StockPrice = stockPrice;
        }


        public double GetTotal()
        {
            return Amount * StockPrice.ClosingPrice;
        }

        public override string ToString()
        {
            return $"{Action} * [{Amount}] at {StockPrice.ClosingPrice}";
        }

    }
    public enum TradeAction
    {
        Buy,
        Sell
    }
}
