using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtesting
{
    public class StockButton
    {
        public string Ticker { get; private set; }
        public Stock _stock { get; private set; }
        public double LatestPrice
        {
            get
            {
                return _stock.PriceData.Last().ClosingPrice;
            }
            set
            {
                
            }
        }
    
            

        public StockButton(string ticker, Stock stock)
        {
            this.Ticker = ticker;
            this._stock = stock;
        }

        

    }
}
