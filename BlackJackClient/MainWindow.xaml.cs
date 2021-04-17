using System;
using System.Threading;
using System.Windows;

namespace BlackJackClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client client;

        public MainWindow()
        {
            InitializeComponent();
            pop.IsEnabled = false;
            stand.IsEnabled = false;
            @double.IsEnabled = false;
            ready.IsEnabled = false;
        }

        private void pop_Click(object sender, RoutedEventArgs e)
        {            
            client.SendMessage("p");
            Thread.Sleep(100);
        }

        private void stand_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage("s");
            Thread.Sleep(100);
        }

        private void double_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage("d");
            Thread.Sleep(100);
        }

        private void ready_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(bet.Text))
            {
                status.Text = "WRONG BET";
                return;
            }
            try
            {
                decimal.Parse(bet.Text);
            }
            catch (Exception)
            {
                status.Text = "WRONG BET";
                return;
            }
            
            client.SendMessage($"r|{bet.Text}");
            Thread.Sleep(100);
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            client = new Client(this, ip.Text);
        }
    }
}
