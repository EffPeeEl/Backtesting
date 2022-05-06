using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Backtesting
{
    public class SettingClass : INotifyPropertyChanged
    {
        public SettingClass()
        {
            ConfigLocation = Path.Combine(Directory.GetCurrentDirectory(), "\\config.txt");

            BollingerStDevs = 2;
            BollingerAvgDays = 20;
            CandleSizeDays = 5;
            

        }

        private string ConfigLocation;
        private string StockDataPath;

        public string TypeOfChart { get; private set; }


        private double _bollingerStDevs;
        public double BollingerStDevs
        {
            get { return _bollingerStDevs; }
            set
            {
                if (_bollingerStDevs != value)
                {
                    _bollingerStDevs = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _bollingerAvgDays;
        public int BollingerAvgDays
        {
            get { return _bollingerAvgDays; }
            set
            {
                if (_bollingerAvgDays != value)
                {
                    _bollingerAvgDays = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _candleSizeForChart;
        public int CandleSizeDays
        {
            get { return _candleSizeForChart; }
            set
            {
                if (_candleSizeForChart != value)
                {
                    _candleSizeForChart = value;
                    OnPropertyChanged();
                }
            }
        }





        public void ChangeStockDataLocation(string newPath)
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
