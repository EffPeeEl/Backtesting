using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SciChart.Charting.Visuals;
using SciChart.Charting.Model.DataSeries;
using System.Collections.ObjectModel;
using Backtesting.TradeAlgos;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Backtesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<TradeAlgo> AlgoList;

        public StockGrapher stockGrapher;

        

        private SettingClass _settings;
        public SettingClass Settings
        {
            get { return _settings; }
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<TradeAlgo> _tradeAlgoList;
        public List<TradeAlgo> TradeAlgoList
        {
            get { return _tradeAlgoList; }
            set
            {
                if (_tradeAlgoList != value)
                {
                    _tradeAlgoList = value;
                    OnPropertyChanged();
                }
            }
        }

        private OhlcDataSeries<DateTime, double> _candleDataSeries;
        public OhlcDataSeries<DateTime, double> CandleDataSeries
        {
            get { return _candleDataSeries; }
            set
            {
                if (_candleDataSeries != value)
                {
                    _candleDataSeries = value;
                    OnPropertyChanged();
                }
            }
        }

        private XyDataSeries<DateTime, double> _lowerBollingerBandSeries;
        public XyDataSeries<DateTime, double> LowerBollingerBandSeries
        {
            get { return _lowerBollingerBandSeries; }
            set
            {
                if (_lowerBollingerBandSeries != value)
                {
                    _lowerBollingerBandSeries = value;
                    OnPropertyChanged();
                    
                }
            }
        }

        private XyDataSeries<DateTime, double> _upperBollingerBandSeries;
        public XyDataSeries<DateTime, double> UpperBollingerBandSeries
        {
            get { return _upperBollingerBandSeries; }
            set
            {
                if (_upperBollingerBandSeries != value)
                {
                    _upperBollingerBandSeries = value;
                    OnPropertyChanged();
                }
            }
        }




        public MainWindow()
        {
            DataContext = this;
            Settings = new SettingClass();

            ActivateSciChartLicense();

            InitializeComponent();
            InitializeAlgos();

            CreateStockPanel();
            




        }

        //Creates object for all trading 
        public void InitializeAlgos()
        {
            TradeAlgoList = new List<TradeAlgo>();

        }


        public void CreateStockPanel()
        {
 
            DirectoryInfo d = new DirectoryInfo(@"D:\Finance\OMX Stonks");

            foreach (var file in d.GetFiles("*.csv"))
            {
                var s = new Button 
                {
                    Content = file.Name.Remove(file.Name.IndexOf('-'), file.Name.Length - file.Name.IndexOf('-')),
                    Uid = file.Name,

                };
                s.Click += ButtonCreatedByCode_Click;
                Stocklist.Children.Add(s);
            }
        }

       

        public bool isSimulationOn;
        private void RunSimulation(object sender, RoutedEventArgs e)
        {
            

            string senderUID = ((FrameworkElement)sender).Uid;
            LowerBollingerBandSeries = new XyDataSeries<DateTime, double>();

            if (rSeriesXAxis != null)
            {
                //Axis stays att first one if this step isnt done for some reason
                rSeriesXAxis.VisibleRange = null;
                rSeriesYAxis.VisibleRange = null;
            }

            isSimulationOn = true;


            /// HAS to be on different thread or it data wont render until function has returned. 
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                for (int i = 0; i < 100; i++)
                {
                    var x = stockGrapher.GetNextDayData();

                    using (LowerBollingerBandSeries.SuspendUpdates())
                    {
                        LowerBollingerBandSeries.Append(x.Item1, x.Item2);

                    }
                    Thread.Sleep(10);

                }
            }).Start();
            

            //while (isSimulationOn)
            //{
                
            //    isSimulationOn = !stockGrapher.isIndexMax();
                
            //}

        }



        private void ButtonCreatedByCode_Click(object sender, RoutedEventArgs e)
        {

            string senderUID = ((FrameworkElement)sender).Uid;

            stockGrapher = new StockGrapher(senderUID);

            if(rSeriesXAxis != null)
            {
                //Axis stays att first one if this step isnt done for some reason
                rSeriesXAxis.VisibleRange = null;
                rSeriesYAxis.VisibleRange = null;
            }

            //for (int i = 0; i < x.Count; i++)
            //{
            //    if (x.XValues[i] == y.XValues[i])
            //    {
            //        Trace.WriteLine("TRUE");
            //    }
            //    else
            ////        Trace.WriteLine($"false at X:{x.XValues[i]} and Y: {y.YValues[i]} ");
            //}

            CandleDataSeries = stockGrapher.CreateGraphSci(senderUID, "Candle", Settings.CandleSizeDays);
            UpperBollingerBandSeries = stockGrapher.CreateBollingerSci(Settings.CandleSizeDays, true, Settings.BollingerStDevs, Settings.BollingerAvgDays);
            LowerBollingerBandSeries = stockGrapher.CreateBollingerSci(Settings.CandleSizeDays, false, Settings.BollingerStDevs, Settings.BollingerAvgDays);

            

        }




        #region MenuBarButtons
        private void StocksButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(StockGrid);


        }

        private void AnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(WebViewer);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(SettingsGrid);

        }

        private void EvalButton_Click(object sender, RoutedEventArgs e)
        {
            //OpenPage(EvalPanel);
        }

        private void InternetButton_Click(object sender, RoutedEventArgs e)
        {

            //StockList sl = new StockList();
            //sl.Show();
        }

        private void OpenPage(Grid sender)
        {
            foreach (Grid sp in AllDashboards.Children)
            {
                sp.Visibility = Visibility.Collapsed;
            }
            sender.Visibility = Visibility.Visible;
        }

        #endregion

        private void LoadPDFButton_Click(object sender, RoutedEventArgs e)
        {
            //pdfWebViewer.Navigate(@"D:\Finance\AnnRep\EVO.pdf");
        }

        private void DefaultSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings = new SettingClass();
        }


        private void ActivateSciChartLicense()
        {
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("usEKPqPHWbWjbdDc6gtx/Id8l0CVAclKElC/4pw347GRLeeVN5X/Zb/Wk4u89+jiVXh13m3QjvsKlWLnJw3FlzX0ab133kzE3KbrpkvJivAq6KwjbjH93gh5uig96kVpi8/q46EssYS/+d4PpLL7Vfv1FiAD16F/nY4MbrEPNNv7V59Rn3p59DEgMdYAA1Diuv6fZ01ae2mAwfSbkJczKlugwVqfgVReObtGwvVH6HzdCsfklrYuIDTAyBlfM0cHia/piNXcxhvU6UyWP39LUjlXd3XR1u4t6NsjrqZjwaf98jyE6xCIB9SffOOnXFKzYl1NMBBK3P+7beNqpGzLqZVGb2qxW6guD8UHfuILufnq6x5qbu/E8QgrlzY5+gow0h9NfYCuldy5oOqQ/f5vSSPE+Xl9sW5F3QRJ994K0dn0P+FV7FBCoawRyee8/XaAxqZOZfoU6AEb+8jJUdYQBtjEuOUF8ctIay8tThVp4L4S6ivyrMl5m2w1dtA1Rv6lV5qQpPybnCf7+tBl0h4TiFvHv2zLpq2FTf5CdaQIxaF1CKo2amcYPACvfLmFZ62A6VHuUw==");


        }

        private void CorrelationButton_Click(object sender, RoutedEventArgs e)
        {
            CorrelationMatrixWindow subWindow = new CorrelationMatrixWindow(stockGrapher._stock);
            subWindow.Show();
        }
    }
}
