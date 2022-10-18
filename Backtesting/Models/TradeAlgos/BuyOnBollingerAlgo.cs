using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.TradeAlgos
{
    class BuyOnBollingerAlgo : TradeAlgorithm
    {

        private int SimpleMovingAverageDays;
        private double StDevsFromAverage;



        public BuyOnBollingerAlgo(double _StDevs, int _SMAdays)
        {
            AlgoName = $"Buy/Sell on Bollinger [{_StDevs} - {_SMAdays}]";

            
            SimpleMovingAverageDays = _SMAdays;
            StDevsFromAverage = _StDevs;

            Buys = new ObservableCollection<PriceData>();
            Sells = new ObservableCollection<PriceData>();

            Score = 0;

            
             
        }


       

        public override double RunAlgorithmSingleStep(Stock stock, int index)
        {
            int howManyStocksToBuy = 1;
            if(stock.PriceData[index].ClosingPrice < stock.PriceData[index].LowerBollinger)
            {
                Buy(howManyStocksToBuy, index);
            }
            else if(stock.PriceData[index].ClosingPrice > stock.PriceData[index].UpperBollinger)
            {
                Buy(howManyStocksToBuy, index);
            }

            return Score;
        }



        public override string ToString()
        {
            return $"{AlgoName}";
        }


    }
}
