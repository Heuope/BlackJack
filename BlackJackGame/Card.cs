namespace BlackJackGame
{
    public enum Suits
    {
        Spade = 1,
        Club,
        Diamond,
        Heart
    }

    public class Card
    {
        public int Value { get; private set; }
        public Suits Suit { get; private set; }
        public string CardName { get; private set; }

        public Card(Suits suit, int value)
        {
            Suit = suit;
            Value = value;
            SetCardName(value);
        }

        private void SetCardName(int value)
        {
            CardName = value switch
            {
                11 => "a",
                12 => "j",
                13 => "q",
                14 => "k",
                10 => "1",
                _ => value.ToString(),
            };

            if (value > 11 && value < 15)
            {
                Value = 10;
            }
        }

        public bool ChangeAce()
        {
            if (Value == 11)
            {
                Value = 1;
                return true;
            }
            return false;
        }

        public bool IsEqual(Card card)
        {
            return CardName == card.CardName &&
                   Value == card.Value &&
                   Suit == card.Suit;
        }

        public override bool Equals(object obj)
        {
            var card = obj as Card;
            return Value == card.Value &&
                   Suit == card.Suit &&
                   CardName == card.CardName;
        }

    }
}
