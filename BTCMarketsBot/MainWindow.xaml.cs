using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BTCMarketsBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsGuiReady = false;
        private ObservableCollection<int> listProfitMargins = new ObservableCollection<int>();
        private ObservableCollection<int> listIntervalsBuySell = new ObservableCollection<int>();

        public MainWindow()
        {
            InitializeComponent();

            // Load collections
            for (int i = 5; i <= 30; i = i + 5)
            {
                listProfitMargins.Add(i);
                listIntervalsBuySell.Add(i);
            }

            // Load controls
            foreach (ExchangeType item in Helpers.GetEnums<ExchangeType>())
            {
                cboBuySell.Items.Add(item.GetDescription());
            }
            cboBuySell.SelectedIndex = 0;

            cboProfitMargin.ItemsSource = listProfitMargins;
            cboProfitMargin.SelectedIndex = 0;

            cboIntervals.ItemsSource = listIntervalsBuySell;
            cboIntervals.SelectedIndex = 5;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read API_KEY and PRIVATE_KEY
            string fp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BTCMarketsAuth.txt");
            if (File.Exists(fp))
            {
                using (StreamReader sr = new StreamReader(fp))
                {
                    ApplicationConstants.API_KEY = sr.ReadLine();
                    ApplicationConstants.PRIVATE_KEY = sr.ReadLine();
                    sr.Close();
                }
            }

            IsGuiReady = true;
        }

        private void UpdateTitle()
        {
            MarketTickData data = BTCMarketsHelper.GetMarketTick();
            Title = $"BTCMarkets Bot | {data.bestAsk} {data.currency} = 1 {data.instrument}";
        }

        private void cboBuySell_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cboBuySell_DropDownClosed(object sender, EventArgs e)
        {
            if (IsGuiReady)
            {
                string txtUnit1 = cboBuySell.Text.Split('/')[0];
                string txtUnit2 = cboBuySell.Text.Split('/')[1];
                lblUnit1.Text = lblUnit1_1.Text = txtUnit1;
                lblUnit2.Text = lblUnit2_2.Text = txtUnit2;
            }
        }
    }
}