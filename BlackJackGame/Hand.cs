using System.Collections.Generic;

namespace BlackJackGame
{
    public class Hand
    {
        private readonly List<Card> cards;

        public int Value { get; private set; }

        public Hand()
        {
            cards = new List<Card>();
            Value = 0;
        }

        public void TakeCard(Card card)
        {
            cards.Add(card);

            Value += card.Value;

            int i = 0;
            while (Value > 21 && i < cards.Count)
            {
                // find Ace
                if (cards[i].ChangeAce())
                {
                    Value -= 10;
                    i = 0;
                }
                else
                    i++;
            }
        }

        public List<Card> GetHand() => cards;

        public bool CheckOnBlackJack()
        {
            if (Value == 21)
                return true;
            return false;
        }

        public void ClearHand() 
        { 
            cards.Clear();
            Value = 0;
        } 
    }
}
