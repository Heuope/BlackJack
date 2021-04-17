using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BlackJackGame;
using MessageParser;

namespace BlackJackServer
{
    public class ServerObject
    {
        private static TcpListener tcpListener;
        private readonly List<ClientObject> clients = new List<ClientObject>();
        public Game game;
        private string turn;

        protected internal void AddConnection(ClientObject clientObject)
        {
            game.AddPlayer(clientObject.Id, 5000);
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        
        protected internal void Listen()
        {
            game = new Game();
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 88);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                DisconnectAll();
            }
        }

        protected void StartGame()
        {
            if (game.IsGameStart)
                return;

            foreach (var player in game.players.Values)
            {
                if (!player.IsReady)
                    return;
            }

            turn = clients[0].Id;
            game.StartGame();

            game.players[turn].StatusCode = 0;
        }

        private int IndexOfTurn()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == turn)
                    return i;
            }
            return 0;
        }

        private void ChangeTurn()
        {
            bool isEnd = true;

            for (int i = IndexOfTurn() + 1; i < clients.Count; i++)
            {
                if (game.players[clients[i].Id].IsReady)
                {
                    turn = clients[i].Id;
                    isEnd = false;
                    game.players[turn].StatusCode = 0;
                    break;
                }
            }

            if (isEnd)
            {
                game.EndGame();
                var parser = new Parser();
                string asnswer = parser.ParseGameInstance(game.GetGameInstance(), game.GetDealer());
                BroadcastMessage(asnswer);
            }
        }

        protected internal void HandleAnswer(string message, string id)
        {
            if (message == "s" && game.IsGameStart)
            {
                game.players[id].StatusCode = 2;
                ChangeTurn();
            }

            if (message.Substring(0,1) == "r" && !game.IsGameStart && !game.players[id].IsReady)
            {
                if (!game.players[id].PlaceBet(decimal.Parse(message.Split('|')[1])))
                    return;

                game.players[id].IsReady = true;
                StartGame();
            }

            if (game.IsGameStart && game.players[id].IsReady)
            {
                game.Action($"{id},{message}");

                if (game.players[id].StatusCode == 1 || game.players[id].StatusCode == 3 || game.players[id].StatusCode == 9)
                    ChangeTurn();

                if (game.IsGameStart)
                {
                    var parser = new Parser();
                    string asnswer = parser.ParseGameInstance(game.GetGameInstance(), game.GetDealer());
                    BroadcastMessage(asnswer);
                }
            }
        }

        protected internal void SendMessage(string message, string Id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);

            foreach (var client in clients)
            {
                if (client.Id == Id)
                    client.Stream.Write(data, 0, data.Length);
            }
        }

        protected internal void BroadcastMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);

            foreach (var client in clients)
                client.Stream.Write(data, 0, data.Length);
        }

        protected internal void DisconnectAll()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
