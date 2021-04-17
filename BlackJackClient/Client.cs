using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BlackJackClient
{
    class Client
    {
        private readonly string host;
        private readonly int port;
        private string id;
        private readonly string pathToCards = "cards";

        private readonly MainWindow mainWindow;
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly Dispatcher uiDispatcher;

        public Client(MainWindow mainWindow, string ip)
        {
            uiDispatcher = Dispatcher.CurrentDispatcher;
            this.mainWindow = mainWindow;
            BlockScreen("");
            try
            {
                var temp = ip.Split(':');
                host = temp[0];
                port = int.Parse(temp[1]);
            }
            catch (Exception)
            {
                Disconnect();
                return;
            }
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                string message = "";
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

                mainWindow.con.IsEnabled = false;
                mainWindow.ready.IsEnabled = true;
            }
            catch (Exception)
            {
                Disconnect();
            }
            Thread.Sleep(500);
            mainWindow.status.Text = id;
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void ShowGame(string gameInstance)
        {
            uiDispatcher.BeginInvoke(() =>
            { 
                var hands = gameInstance.Split(",");

                mainWindow.dealer.Children.Clear();
                mainWindow.player.Children.Clear();
                mainWindow.opponents.Children.Clear();

                foreach (var hand in hands)
                {
                    var temp = hand.Split("|");
                    mainWindow.money.Content = temp[3];
                    if (temp[0] == "dealer")
                    {
                        for (int i = 0; i < temp[1].Length; i += 2)
                        {
                            var image = new Image();
                            var imagePath = new Uri($"{Path.GetFullPath(pathToCards)}\\{temp[1].Substring(i, 2)}.png");
                            image.Source = new BitmapImage(imagePath);
                            image.Stretch = Stretch.Uniform;
                            mainWindow.dealer.Children.Add(image);
                        }
                    }
                    else if (temp[0] == id)
                    {
                        for (int i = 0; i < temp[1].Length; i += 2)
                        {
                            var image = new Image();
                            var imagePath = new Uri($"{Path.GetFullPath(pathToCards)}\\{temp[1].Substring(i, 2)}.png");
                            image.Source = new BitmapImage(imagePath);
                            image.Stretch = Stretch.Uniform;
                            mainWindow.player.Children.Add(image);
                        }

                        if (temp[2] == "0")
                        {
                            UnlockScreen();
                            mainWindow.status.Text = "YOUR TURN";
                        }
                        else
                            BlockScreen(temp[2]);
                    }
                    else
                    {
                        var stackOp = new StackPanel();
                        for (int i = 0; i < temp[1].Length; i += 2)
                        {
                            var image = new Image();
                            var imagePath = new Uri($"{Path.GetFullPath(pathToCards)}\\{temp[1].Substring(i, 2)}.png");
                            image.Source = new BitmapImage(imagePath);
                            image.Stretch = Stretch.Uniform;
                            stackOp.Children.Add(image);
                        }
                        stackOp.Margin = new Thickness(10, 0, 10, 0);
                        stackOp.Orientation = Orientation.Horizontal;
                        mainWindow.opponents.Children.Add(stackOp);
                    }
                }
            });
        }

        private void BlockScreen(string code)
        {
            mainWindow.pop.IsEnabled = false;
            mainWindow.stand.IsEnabled = false;
            mainWindow.@double.IsEnabled = false;
            mainWindow.status.Text = code switch
            {
                "1" => "MUCH",
                "2" => "WAIT",
                "3" => "BLACKJACK",
                "4" => "DEALER",
                "5" => "NOT IN GAME",
                "6" => "YOU WIN",
                "7" => "YOU LOSE",
                "8" => "PUSH",
                "9" => "DOUBLED",
                _ => "ERROR",
            };
        }

        private void UnlockScreen()
        {
            mainWindow.pop.IsEnabled = true;
            mainWindow.stand.IsEnabled = true;
            mainWindow.@double.IsEnabled = true;
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    if (message.Contains("_id_"))
                    {
                        var temp = message.Substring(4);

                        id = temp.Split('|')[0];
                        uiDispatcher.BeginInvoke(() =>
                        {
                            mainWindow.money.Content = temp.Split('|')[1];
                        });
                    }
                    else
                    {
                        ShowGame(message);
                    }
                }
                catch (Exception ex)
                {
                    Disconnect();
                }
            }
        }

        private void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
