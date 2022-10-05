using System;
using System.Collections.Generic;
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

            Buys = new List<PriceData>();
            Sells = new List<PriceData>();

            Score = 0;

            

        }

        public override double RunAlgorithm(Stock stock)
        {
            return RunAlgorithm(stock, stock.PriceData[0].Date, stock.PriceData[stock.PriceData.Count - 1].Date);
        }

        public override double RunAlgorithm(Stock stock, DateTime startDate)
        {
            return RunAlgorithm(stock, startDate, stock.PriceData[stock.PriceData.Count - 1].Date);
        }

        public override double RunAlgorithm(Stock stock, DateTime startDate, DateTime endDate)
        {
            return Score;
        }


    }
}
