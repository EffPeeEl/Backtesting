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

        public MainWindow()
        {
            DataContext = this;
            Settings = new SettingClass();
            

            InitializeComponent();

            CreateStockPanel();
            stockGrapher = new StockGrapher();
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

        
        private void ButtonCreatedByCode_Click(object sender, RoutedEventArgs e)
        {

            string senderUID = ((FrameworkElement)sender).Uid;

            HomeScreenChart.Series = stockGrapher.CreateGraph(senderUID, "Candle", Settings.CandleSizeDays, Settings.BollingerStDevs, Settings.BollingerAvgDays);
            HomeScreenChart.AxisX.Clear();
            HomeScreenChart.AxisX.Add(stockGrapher.axe);
            
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
    }
}
