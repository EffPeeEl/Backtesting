using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting
{
    class LvChartGrapher
    {

        public SeriesCollection CreateChart(Stock stock)
        {
            

            ChartValues<double> CV = new ChartValues<double>();
    

            for(int i = 0; i < stock.PriceData.Count; i++)
            {
                CV.Add(stock.PriceData[i].ClosingPrice);
            }

            SeriesCollection SC = new SeriesCollection()
            {
                new LineSeries
                {
                    Values = CV
                }
            };

            return SC;
            
        }

    }
}
