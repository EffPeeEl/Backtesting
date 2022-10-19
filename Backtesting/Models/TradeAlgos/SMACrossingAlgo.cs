using Backtesting.TradeAlgos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting.Models.TradeAlgos
{
    public class SMACrossingAlgo : TradeAlgorithm
    {

        public int SMA1Threshold { get; private set; }
        public int SMA2Threshold { get; private set; }

        private ObservableCollection<double> SMA1;

        private ObservableCollection<double> SMA2;

        
        public SMACrossingAlgo(int smaThreshhold1, int smaThreshhold2)
        {
            this.SMA1Threshold = smaThreshhold1;
            this.SMA2Threshold = smaThreshhold2;
            SMA1 = new ObservableCollection<double>();
            SMA2 = new ObservableCollection<double>();
        }

        
        public override AlgoAction RunAlgorithmSingleStep(Stock stock, int index, int candleSize)
        {
            bool didBuy = false;

            if (index + candleSize > stock.PriceData.Count)
                candleSize = stock.PriceData.Count - index;

            for (int i = 0; i < candleSize; i++)
            {

                SMA1.Add(CalculateSMAs(stock, index, SMA1Threshold));
                SMA2.Add(CalculateSMAs(stock, index, SMA2Threshold));


                if (index > 10)
                {
                    if (SMA1[index - 1] < SMA2[index - 1])
                    {
                        if (SMA1[index] > SMA2[index])
                        {
                            Buy(stock, 1, index);

                            didBuy = true;
                        }
                    }
                }


                index++;

            }
            if(didBuy) return Actions.Last();

            return null;



        }

        private double CalculateSMAs(Stock stock, int index, int threshhold)
        {
            int startIndex = index - threshhold;

            if(index < threshhold)
            {
                startIndex = 0;
            }
            double AVG = 0;
            int rounds = 0;

            

            for(int i = startIndex; i < index; i++)
            {
                AVG += stock.PriceData[i].ClosingPrice;
                rounds++;
            }
            return (AVG / rounds);



        }

        public override string ToString()
        {
            return $"SMA Crossing Algorithm {SMA1Threshold} - {SMA2Threshold}";
        }

        public override AlgoAction RunAlgorithmSingleStep(Stock stock, int index)
        {
            throw new NotImplementedException();
        }
    }
}
