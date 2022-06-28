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
            

            RangeLowerAvgDays = 5;
            RangeUpperAvgDays = 50;
            IncrementDays = 1;

            RangeLowerStDevs = 1;
            RangeUpperStDevs = 3;
            IncrementStDevs = 0.1;

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



        private int _rangeLowerAvgDays;
        public int RangeLowerAvgDays
        {
            get { return _rangeLowerAvgDays; }
            set
            {
                if (_rangeLowerAvgDays != value)
                {
                    _rangeLowerAvgDays = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }

        private int _rangeUpperAvgDays;
        public int RangeUpperAvgDays
        {
            get { return _rangeUpperAvgDays; }
            set
            {
                if (_rangeUpperAvgDays != value)
                {
                    _rangeUpperAvgDays = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }

        private int _incrementDays;
        public int IncrementDays
        {
            get { return _incrementDays; }
            set
            {
                if (_incrementDays != value)
                {
                    _incrementDays = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }


        private double _rangeLowerStDevs;
        public double RangeLowerStDevs
        {
            get { return _rangeLowerStDevs; }
            set
            {
                if (_rangeLowerStDevs != value)
                {
                    _rangeLowerStDevs = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }

        private double _rangeUpperStDevs;
        public double RangeUpperStDevs
        {
            get { return _rangeUpperStDevs; }
            set
            {
                if (_rangeUpperStDevs != value)
                {
                    _rangeUpperStDevs = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }

        private double _incrementStDevs;
        public double IncrementStDevs
        {
            get { return _incrementStDevs; }
            set
            {
                if (_incrementStDevs != value)
                {
                    _incrementStDevs = value;
                    updateRangeString();
                    OnPropertyChanged();
                }
            }
        }

        private void updateRangeString()
        {
            AlgoRangeString = $"Simulate for RANGE [{RangeLowerStDevs}:{RangeUpperStDevs} + {IncrementStDevs}] - [{RangeLowerAvgDays}:{RangeUpperAvgDays} + {IncrementDays}]";
        }

        private string _algoRangeString;
        public string AlgoRangeString
        {
            get
            {
                return _algoRangeString;
            }
            set
            {
                if (_algoRangeString != value)
                {
                    _algoRangeString = value;
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

        internal void Reset()
        {
            BollingerStDevs = 2;
            BollingerAvgDays = 20;
            CandleSizeDays = 5;


            RangeLowerAvgDays = 5;
            RangeUpperAvgDays = 50;
            IncrementDays = 1;

            RangeLowerStDevs = 1;
            RangeUpperStDevs = 3;
            IncrementStDevs = 0.1;
        }
    }
}
