using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.TradeAlgos
{
    public abstract class TradeAlgo
    {
        Stock StockToTrade;

        List<PriceData> Buys;
        List<PriceData> Sells;

        public double Score { get; private set; }

        public abstract void Buy();
        public abstract void Sell();

        public void Calculate()
        {
            Score = 0;
            int over = 0;
            foreach (PriceData pd in Buys)
            {
                Score -= pd.ClosingPrice;

            }
            foreach (PriceData pd in Sells)
            {
                Score -= pd.ClosingPrice;

            }
            for(int i = 0; i < Buys.Count-Sells.Count;i++)
            {
                Score += StockToTrade._priceData[StockToTrade._priceData.Count - 1].ClosingPrice;
            }

        }

    }
}
