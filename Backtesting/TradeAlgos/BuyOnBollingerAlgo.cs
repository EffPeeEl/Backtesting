using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.TradeAlgos
{
    class BuyOnBollingerAlgo : TradeAlgo
    {

        private int SMADays;
        private double StDevs;



        public BuyOnBollingerAlgo(double _StDevs, int _SMAdays)
        {
            AlgoName = $"Buy/Sell on Bollinger [{_StDevs} - {_SMAdays}]";

            StDevs = _StDevs;
            SMADays = _SMAdays;

            Buys = new List<PriceData>();
            HistoricBuys = new List<PriceData>();

            Sells = new List<PriceData>();
            HistoricSells = new List<PriceData>();

            Score = 0;


        }

        public override string Run(int index)
        {

            AlgoSelectedStock.CreateBollingerValues(StDevs, SMADays);
            if (AlgoSelectedStock.PriceData[index].ClosingPrice < AlgoSelectedStock.PriceData[index].LowerBollinger)
            {

                Buy(index, 1);
                
                return $"[{ Buys[Buys.Count - 1].Date}] Bought for { Buys[Buys.Count - 1].ClosingPrice}";
            }


            if (AlgoSelectedStock.PriceData[index].ClosingPrice > AlgoSelectedStock.PriceData[index].UpperBollinger
                && Buys.Count > 0)
            {
                int BC = Buys.Count;
                foreach(PriceData pd in Buys)
                {
                    Sell(index, BC);
                }
                Buys.Clear();

                return $"[{AlgoSelectedStock.PriceData[index].Date}] Sold {BC}x for { BC * AlgoSelectedStock.PriceData[index].ClosingPrice }"; 
            }
            return "";
        }

        public override void Run()
        {
            
            for (int i = 0; i < AlgoSelectedStock.PriceData.Count; i++)
            {
                if (AlgoSelectedStock.PriceData[i].ClosingPrice < AlgoSelectedStock.PriceData[i].LowerBollinger)
                    Buys.Add(AlgoSelectedStock.PriceData[i]);

            }
        }



        public override void Buy(int index, int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                Buys.Add(AlgoSelectedStock.PriceData[index]);
                HistoricBuys.Add(AlgoSelectedStock.PriceData[index]);
                Score -= AlgoSelectedStock.PriceData[index].ClosingPrice;
            }
            

        }

        public override void Sell(int index, int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                Sells.Add(AlgoSelectedStock.PriceData[index]);
                HistoricSells.Add(AlgoSelectedStock.PriceData[index]);
                Score += AlgoSelectedStock.PriceData[index].ClosingPrice;
            }

        }

        

        

    }
}
