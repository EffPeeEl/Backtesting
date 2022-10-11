using Backtesting.TradeAlgos;
using LiveCharts;
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




        public MainWindow()
        {
            DataContext = this;
            Settings = new SettingClass();

            

            ActivateSciChartLicense();

            InitializeComponent();
            InitializeAlgos();

            CreateStockPanel();

            

            


        }
        private SeriesCollection _sc;
        public SeriesCollection SC
        {
            get { return _sc; }
            set
            {
                if (_sc != value)
                {
                    _sc = value;
                    OnPropertyChanged();
                }
            }
        }


        //Creates object for all trading 
        public void InitializeAlgos()
        {
            TradeAlgoList = new List<TradeAlgorithm>();



        }


        public void CreateStockPanel()
        {
 
            DirectoryInfo d = new DirectoryInfo(@"C:\Users\Felix\Desktop\Finance");

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



        public string SelectedStockUID;

        private void ButtonCreatedByCode_Click(object sender, RoutedEventArgs e)
        {
            LvChartGrapher Grapher = new LvChartGrapher();
            var x = (Button)sender;

            SC = Grapher.CreateChart(new Stock(x.Uid));

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
            CorrelationMatrixWindow subWindow = new CorrelationMatrixWindow(stockGrapher.SelectedStock);
            subWindow.Show();
        }


    }
}
