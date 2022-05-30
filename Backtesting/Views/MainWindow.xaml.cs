using Backtesting.TradeAlgos;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.PointMarkers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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
            isActiveSimulationRun = false;
            IsNotActiveSimulationRun = true;
            

            ActivateSciChartLicense();

            InitializeComponent();
            InitializeAlgos();

            CreateStockPanel();

        }

        //Creates object for all trading 
        public void InitializeAlgos()
        {
            TradeAlgoList = new List<TradeAlgo>();
            TradeAlgoList.Add(new BuyOnBollingerAlgo(Settings.BollingerStDevs, Settings.BollingerAvgDays));
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

        private string _logString;
        public string LogString
        {
            get { return _logString; }
            set
            {
                if (_logString != value)
                {
                    _logString = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _disabledText;
        public string disabledText
        {
            get { return _disabledText; }
            set
            {
                if (_disabledText != value)
                {
                    _disabledText = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isActiveSimulationRun;
        public bool isActiveSimulationRun
        {
            get { return _isActiveSimulationRun; }
            set
            {
                if (_isActiveSimulationRun != value)
                {
                    _isActiveSimulationRun = value;
                    IsNotActiveSimulationRun = !_isActiveSimulationRun;

                    if(value)
                        disabledText = "Unable to change settings while simulation is running";
                    else
                        disabledText = "";

                    OnPropertyChanged();
                }
            }
        }

        //XAML cant inverse bools so this is the spaghettiest option since Control.Setters in Settings-Tab is needs to EnableBind to a bool
        private bool _IsNotActiveSimulationRun;
        public bool IsNotActiveSimulationRun
        {
            get { return _IsNotActiveSimulationRun; }
            set
            {
                if (_IsNotActiveSimulationRun != value)
                {
                    _IsNotActiveSimulationRun = value;
                    OnPropertyChanged();
                }
            }
        }

        private void RunSimulation(object sender, RoutedEventArgs e)
        {

            if (rSeriesXAxis != null)
            {
                //Axis stays att first one if this step isnt done for some reason
                rSeriesXAxis.VisibleRange = null;
                rSeriesYAxis.VisibleRange = null;
            }

            isSimulationOn = true;
            
            if (!isActiveSimulationRun)
            {
                ThreadSimulation();
            }

        }

        private TradeAlgo _tempAlgo;
        public TradeAlgo tempAlgo
        {
            get { return _tempAlgo; }
            set
            {
                if (_tempAlgo != value)
                {
                    _tempAlgo = value;
                    OnPropertyChanged();
                }
            }
        }


        private void StopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            isSimulationOn = false;
        }

        private void ThreadSimulation()
        {
            LowerBollingerBandSeries = new XyDataSeries<DateTime, double>();
            UpperBollingerBandSeries = new XyDataSeries<DateTime, double>();
            CandleDataSeries = new OhlcDataSeries<DateTime, double>();
            isActiveSimulationRun = true;

            tempAlgo = TradeAlgoList[AlgoListBox.SelectedIndex];

            /// HAS to be on different thread or it data wont render until function has returned. 
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;


                for (int i = 0; i < Math.Ceiling((double)stockGrapher.SelectedStock.PriceData.Count / Settings.CandleSizeDays); i++)
                {
                    //In order to pause Simulation with button
                    while (!isSimulationOn) ;

                    for (int j = 0; j < Settings.CandleSizeDays; j++)
                    {
                        if (i * Settings.CandleSizeDays + j >= stockGrapher.SelectedStock.PriceData.Count)
                            break;

                        string returned = tempAlgo.Run(i*Settings.CandleSizeDays + j);



                        Application.Current.Dispatcher.Invoke((Action)delegate {

                            if (returned != "")
                            {
                                CandleStickRenderableSeries.PointMarker = new EllipsePointMarker() { Width = 50, Height = 50, StrokeThickness = 10 };
                                LogString = returned + "\n" + LogString ;
                            }


                        });

                       
                    }

                    var candleData = stockGrapher.GetNextDayDataCandle(CandleDataSeries.Count * Settings.CandleSizeDays, Settings.CandleSizeDays);
                    var lowerLineData = stockGrapher.GetNextDayDataBollinger(LowerBollingerBandSeries.Count * Settings.CandleSizeDays, false);
                    var upperLineData = stockGrapher.GetNextDayDataBollinger(UpperBollingerBandSeries.Count * Settings.CandleSizeDays, true);

                    using (CandleDataSeries.SuspendUpdates())
                    {
                        CandleDataSeries.Append(candleData.Item1, candleData.Item2.Open, candleData.Item2.High, candleData.Item2.Low, candleData.Item2.Close);
                        LowerBollingerBandSeries.Append(lowerLineData.Item1, lowerLineData.Item2);
                        UpperBollingerBandSeries.Append(upperLineData.Item1, upperLineData.Item2);

                        

                        
                    }
                    Thread.Sleep(15);

                }
                tempAlgo.CalculateFinalScore();

                isActiveSimulationRun = false;

            }).Start();
        }


        private void ButtonCreatedByCode_Click(object sender, RoutedEventArgs e)
        {

            string senderUID = ((FrameworkElement)sender).Uid;

            stockGrapher = new StockGrapher(senderUID);
            TradeAlgoList[0].AlgoSelectedStock = new Stock(senderUID);


            if (rSeriesXAxis != null)
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
            CorrelationMatrixWindow subWindow = new CorrelationMatrixWindow(stockGrapher.SelectedStock);
            subWindow.Show();
        }


    }
}
