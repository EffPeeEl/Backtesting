using Backtesting.TradeAlgos;
using LiveCharts;
using LiveCharts.Defaults;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.PointMarkers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using System.Configuration;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

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





        public SciChartStockGrapher stockGrapher;


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

        
        private List<TradeAlgorithm> _tradeAlgoList;
        public List<TradeAlgorithm> TradeAlgoList
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

        private ObservableCollection<StockButton> _stockListBox;
        public ObservableCollection<StockButton> StockListBox
        {
            get { return _stockListBox; }
            set
            {
                if (_stockListBox != value)
                {
                    _stockListBox = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _logList;
        public ObservableCollection<string> LogList
        {
            get { return _logList; }
            set
            {
                if (_logList != value)
                {
                    _logList = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            DataContext = this;
            Settings = new SettingClass();
            LogList = new ObservableCollection<string>();
            StockListBox = new ObservableCollection<StockButton>();

            ActivateSciChartLicense();

            InitializeComponent();
            InitializeAlgos();

            CreateStockPanel();


            stockGrapher = new SciChartStockGrapher(5);
            CandleDataSeries = new OhlcDataSeries<DateTime, double>();
            


        }




        //Creates object for all trading 
        public void InitializeAlgos()
        {
            TradeAlgoList = new List<TradeAlgorithm>();



        }


        public void CreateStockPanel()
        {
 
            DirectoryInfo d = new DirectoryInfo(@"D:\Finance\OMX Stonks");

            foreach (var file in d.GetFiles("*.csv"))
            {
                var s = new StockButton(Stock.GetTickerFromFileName(file.Name), new Stock(file.Name));
                StockListBox.Add(s);



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



        public string SelectedStockUID;

        private void ButtonCreatedByCode_Click(object sender, RoutedEventArgs e)
        {
            CandleDataSeries = new OhlcDataSeries<DateTime, double>();
            
            var SenderButton = (Button)sender;

            Stock S = new Stock(SenderButton.Uid);


            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                for (int i = 0; i < S.PriceData.Count; i += Settings.CandleSizeDays)
                {
                    (DateTime x, OhlcPoint y) = stockGrapher.GetNextDayDataCandle(S, i, Settings.CandleSizeDays);
                    
                    CandleDataSeries.Append(x, y.Open, y.High, y.Low, y.Close);


                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        LogList.Insert(0, $"{x.ToShortDateString()}: {y.Close}");
                    });
                    
                    

                    Thread.Sleep(10);

                }
            }).Start();

            

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
            Settings.Reset();
        }


        private void ActivateSciChartLicense()
        {
            SciChartSurface.SetRuntimeLicenseKey("rNAB2MFuo8b/BfBJaA65zkGCGLwUJIj6+jE9/98maJ4DNw6D9EYCDOWVrq5KrfjR8oJgA2PhiIzcTxPYCDplieDXG0RMBlZ/lwcnShlEyoU+htq3BSqnsIhPbsJ1vVH19P/NOQaV2hog/5rT1eb0FH3uJgpLPT0XurFY7utMhux3tKstOvHH9t1/pKWbA6nvPsRfd7QiEqEoupuk11p9ZVWGeOI+m1y+ftxnNpcTvz9doX3DPy1jmDXkZWzd3OROKuYOtUAB6fUwUu8SIBVVY6K5KXqFay4QNSQa7JWkq541rrvqYZDKRWZ/+nPn3DRQcggZfZkezoFI9XjwqTs8VOpTX1e4ypzDNdWNG35Lbu/SISwIehKB3u38rRdhd2eGEkWP7mpTvasHHSrDZnOnVSK4vLXWSG47meOkNMzi4YhH5av+WOsCNsnkx+coXd/5iR8WaW2muk/n6UQ+rgE+N1UvrEOPi7O3BtKRZuBytbu1Lu7nlpgqjgKnr23B88tVsIlop2k=");

        }

        private void CorrelationButton_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void ConfigEditBox_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings["StockFilesLocation"] == null)
                {
                    settings.Add("StockFilesLocation", FileLocationConfigBox.Text);
                }
                else
                {
                    settings["StockFilesLocation"].Value = FileLocationConfigBox.Text;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }

        }

        private void Stocklist_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
