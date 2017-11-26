using ShareX.HelpersLib;
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

namespace BTCMarketsBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load controls
            foreach (ExchangeType item in Helpers.GetEnums<ExchangeType>())
            {
                cboBuySell.Items.Add(item.GetDescription());
            }
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
        }

        private void btnGetMarketTick_Click(object sender, RoutedEventArgs e)
        {
            txtMartetData.Text = BTCMarketsHelper.GetMarketTick();
        }
    }
}