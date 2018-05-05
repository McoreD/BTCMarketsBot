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
using System.Windows.Threading;

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
        private DispatcherTimer marketTickTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Bot.LoadSettings();

            marketTickTimer.Interval = new TimeSpan(0, 0, 20); // every 20 seconds
            marketTickTimer.Tick += MarketTickTimer_Tick;
            marketTickTimer.Start();

            // Load collections

            for (int i = 5; i <= 30; i += 5)
            {
                listProfitMargins.Add(i);
            }

            for (int i = 30; i <= 1440; i += 30)
            {
                listIntervalsBuySell.Add(i);
            }

            // Load controls
            foreach (ExchangeType item in Helpers.GetEnums<ExchangeType>())
            {
                cboBuySell.Items.Add(item.GetDescription());
            }
            cboBuySell.SelectedIndex = Bot.Settings.ExchangeTypeIndex;
            BTCMarketsHelper.ExchangeType = cboBuySell.Text;

            cboProfitMargin.ItemsSource = listProfitMargins;
            cboProfitMargin.SelectedIndex = Bot.Settings.ProfitMargin;

            cboIntervals.ItemsSource = listIntervalsBuySell;
            cboIntervals.SelectedIndex = Bot.Settings.IntervalIndex;
        }

        private void MarketTickTimer_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            marketTickTimer.Interval = new TimeSpan(0, 0, rnd.Next(10, 30));
            TaskEx.Run(() => BTCMarketsHelper.GetMarketTick());

            UpdateTitle();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TaskEx.Run(() =>
            {
                BTCMarketsHelper.GetMarketTick();
                IsGuiReady = true;
            });

            UpdateGuiControls();
        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bot.Settings.ExchangeTypeIndex = cboBuySell.SelectedIndex;
            Bot.Settings.ProfitMargin = cboProfitMargin.SelectedIndex;
            Bot.Settings.IntervalIndex = cboIntervals.SelectedIndex;

            Bot.SaveSettings();
        }

        private void UpdateGuiControls()
        {
            BTCMarketsHelper.ProfitMargin = listProfitMargins[cboProfitMargin.SelectedIndex];
            BTCMarketsHelper.ExchangeType = cboBuySell.Text;

            string txtUnit1 = cboBuySell.Text.Split('/')[0];
            string txtUnit2 = cboBuySell.Text.Split('/')[1];
            lblUnit1.Text = lblUnit1_1.Text = txtUnit1;
            lblUnit2.Text = lblUnit2_1.Text = lblUnit2_2.Text = txtUnit2;
            btnBuy.Content = lblBuy.Text = $"Buy {txtUnit1}";
            btnSell.Content = lblSell.Text = $"Sell {txtUnit2}";

            if (IsGuiReady)
            {
                if (!string.IsNullOrEmpty(txtVolume1.Text))
                {
                    decimal buyVolume;
                    decimal.TryParse(txtVolume1.Text, out buyVolume);

                    TradingData tradingData = TradingHelper.GetTradingData(BTCMarketsHelper.MarketTickData, Bot.Settings.ProfitMarginSplit, buyVolume);
                    txtPrice1.Text = tradingData.BuyPrice.ToString();
                    txtVolume2.Text = tradingData.SellVolume.ToString();
                    txtPrice2.Text = tradingData.SellPrice.ToString();

                    lblSpendTotal.Text = tradingData.SpendTotal.ToString();
                }

                UpdateTitle();
            }
        }

        private void UpdateTitle()
        {
            if (IsGuiReady)
                Title = $"1 {BTCMarketsHelper.MarketTickData.instrument} = {BTCMarketsHelper.MarketTickData.bestAsk} {BTCMarketsHelper.MarketTickData.currency} | BTC Markets Bot";
        }

        private void cboBuySell_DropDownClosed(object sender, EventArgs e)
        {
            BTCMarketsHelper.GetMarketTick();

            UpdateGuiControls();
        }

        private void txtVolume1_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGuiControls();
        }

        private void btnGetMarketData_Click(object sender, RoutedEventArgs e)
        {
            UpdateGuiControls();
        }

        private void cboProfitMargin_DropDownClosed(object sender, EventArgs e)
        {
            UpdateGuiControls();
        }
    }
}