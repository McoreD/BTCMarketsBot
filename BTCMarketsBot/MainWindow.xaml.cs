﻿using ShareX.HelpersLib;
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
            App.LoadSettings();

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
            cboBuySell.SelectedIndex = App.Settings.ExchangeTypeIndex;
            BTCMarketsHelper.ExchangeType = cboBuySell.Text;

            cboProfitMargin.ItemsSource = listProfitMargins;
            cboProfitMargin.SelectedIndex = App.Settings.ProfitMarginIndex;

            cboIntervals.ItemsSource = listIntervalsBuySell;
            cboIntervals.SelectedIndex = App.Settings.IntervalIndex;
        }

        private void MarketTickTimer_Tick(object sender, EventArgs e)
        {
            TaskEx.Run(() => BTCMarketsHelper.GetMarketTick());

            IsGuiReady = true;
            UpdateTitle();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Read API_KEY and PRIVATE_KEY
            string fp = Path.Combine(App.PersonalFolder, "BTCMarketsAuth.txt");
            if (File.Exists(fp))
            {
                using (StreamReader sr = new StreamReader(fp))
                {
                    ApplicationConstants.API_KEY = sr.ReadLine();
                    ApplicationConstants.PRIVATE_KEY = sr.ReadLine();
                    sr.Close();
                }
            }

            TaskEx.Run(() => BTCMarketsHelper.GetMarketTick());

            UpdateGuiControls();
        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Settings.ExchangeTypeIndex = cboBuySell.SelectedIndex;
            App.Settings.ProfitMarginIndex = cboProfitMargin.SelectedIndex;
            App.Settings.IntervalIndex = cboIntervals.SelectedIndex;

            App.SaveSettings();
        }

        private void UpdateGuiControls()
        {
            if (IsGuiReady)
            {
                BTCMarketsHelper.ProfitMargin = listProfitMargins[cboProfitMargin.SelectedIndex];
                BTCMarketsHelper.ExchangeType = cboBuySell.Text;

                string txtUnit1 = cboBuySell.Text.Split('/')[0];
                string txtUnit2 = cboBuySell.Text.Split('/')[1];
                lblUnit1.Text = lblUnit1_1.Text = txtUnit1;
                lblUnit2.Text = lblUnit2_2.Text = txtUnit2;
                btnBuy.Content = lblBuy.Text = $"Buy {txtUnit1}";
                btnSell.Content = lblSell.Text = $"Sell {txtUnit2}";

                TradingData tradingData = TradingHelper.GetTradingData();

                if (!string.IsNullOrEmpty(txtVolume1.Text))
                {
                    double.TryParse(txtVolume1.Text, out TradingHelper.BuyVolumeInput);

                    txtPrice1.Text = tradingData.BuyPrice.ToString("0.########");

                    txtVolume2.Text = tradingData.SellVolume.ToString("0.########");
                    txtPrice2.Text = tradingData.SellPrice.ToString("0.########");
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