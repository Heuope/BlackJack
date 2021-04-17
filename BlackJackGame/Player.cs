namespace BlackJackGame
{
    public class Player
    {
        private readonly Hand hand = new Hand();

        private decimal money;

        public string Id { get; private set; }

        // 1 => "MUCH",
        // 2 => "WAIT",
        // 3 => "BLACKJACK",
        // 4 => "DEALER",
        // 5 => "NOT IN GAME",
        // 6 => "YOU WIN",
        // 7 => "YOU LOSE",
        // 8 => "PUSH",
        // 9 => "DOUBLED",
        public int StatusCode { get; set; }

        public bool IsReady { get; set; }

        public decimal Bet { get; private set; }

        public Player(string id, decimal money)
        {
            IsReady = false;
            this.money = money;
            Id = id;
            StatusCode = 5;
        }

        public bool PlaceBet(decimal bet)
        {
            Bet = bet;
            if (money - Bet > 0 && Bet > 0)
                return true;
            return false;
        }

        public void TakeCard(Card card) => hand.TakeCard(card);

        public Hand GetHand() => hand;

        public int GetHandValue() => hand.Value;

        public void TakeBet(decimal bet) => money += bet;

        public void ClearPlayerHand() => hand.ClearHand();

        public decimal GetMoney() => money;

        public void SubMoney(decimal money) => this.money -= money;
    }
}
