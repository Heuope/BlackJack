using System.Collections.Generic;

namespace BlackJackGame
{
    public class Game
    {
        private readonly Dealer dealer;

        public bool IsGameStart { get; set; }

        public Dictionary<string, Player> players;

        public Game()
        {
            dealer = new Dealer();
            players = new Dictionary<string, Player>();
            IsGameStart = false;
        }

        public void InitializeGame() => dealer.CreateDeck();

        public void AddPlayer(string id, decimal money) => players.Add(id, new Player(id, money));

        public Dealer GetDealer() => dealer;

        public Dictionary<string, Player> GetGameInstance() => players;

        public void Action(string action)
        {
            var arg = action.Split(',');
            // arg[0] --> ID
            // arg[1] --> action
            if (arg[1] == "d")
            {
                if (!players[arg[0]].PlaceBet(players[arg[0]].Bet * 2))
                    return;

                players[arg[0]].TakeCard(dealer.GetTopCard());
                players[arg[0]].StatusCode = 9;
            }

            if (arg[1] == "p")
                players[arg[0]].TakeCard(dealer.GetTopCard());
            
            CheckPlayerHand(arg[0]);
        }

        public void StartGame()
        {
            ClearHands();
            if (dealer.GetDeck().Count < 52)
                dealer.CreateDeck();

            InitStartCards();
            IsGameStart = true;

            foreach (var player in players.Values)
            {
                player.StatusCode = 2;
            }
        }

        public void EndGame()
        {
            while (dealer.TakeCard(dealer.GetTopCard())) { }

            foreach (var player in players.Values)
                player.IsReady = false;

            IsGameStart = false;
            GetResults();
        }

        public void ClearHands()
        {
            foreach (var player in players.Values)
                player.ClearPlayerHand();
            dealer.ClearHand();
        }

        public void InitStartCards()
        {
            foreach (var player in players.Values)
            {
                player.TakeCard(dealer.GetTopCard());
                player.TakeCard(dealer.GetTopCard());
            }

            dealer.TakeCard(dealer.GetTopCard());
            dealer.TakeCard(dealer.GetTopCard());
        }

        private void CheckPlayerHand(string id)
        {
            if (players[id].GetHandValue() == 21)
                players[id].StatusCode = 3;
            if (players[id].GetHandValue() > 21)
                players[id].StatusCode = 1;
        }

        private void GetResults()
        {
            foreach (var player in players.Values)
            {
                if (player.GetHandValue() > 21)
                {
                    player.StatusCode = 7;
                    player.SubMoney(player.Bet);
                    continue;
                }
                if (player.GetHandValue() == dealer.GetHand().Value)
                {
                    player.StatusCode = 8;
                    continue;
                }
                if (player.GetHandValue() > dealer.GetHand().Value && dealer.GetHand().Value <= 21)
                {
                    if (player.StatusCode == 3)
                    {
                        player.TakeBet(player.Bet);
                        player.TakeBet(player.Bet);
                    }
                    player.StatusCode = 6;
                    player.TakeBet(player.Bet);
                    continue;
                }
                if (player.GetHandValue() < dealer.GetHand().Value && dealer.GetHand().Value <= 21)
                {
                    player.StatusCode = 7;
                    continue;
                }
                if (player.GetHandValue() <= 21 && dealer.GetHand().Value > 21)
                {
                    player.StatusCode = 6;
                    continue;
                }
            }
        }
    }
}
