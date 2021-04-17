using System.Collections.Generic;

namespace BlackJackGame
{
    public class Dealer
    {
        private Stack<Card> deck = new Stack<Card>();

        private readonly Hand dealerHand = new Hand();

        public void CreateDeck()
        {
            var deckCreater = new DeckCreater();

            deck = deckCreater.CreateDeck(4);
        }

        public Card GetTopCard() => deck.Pop();

        public bool TakeCard(Card card)
        {
            if (dealerHand.Value >= 17)
                return false;

            dealerHand.TakeCard(card);
            return true;
        }


        public Stack<Card> GetDeck() => deck;

        public Hand GetHand() => dealerHand;

        public int GetValue() => dealerHand.Value;

        public void ClearHand() => dealerHand.ClearHand();
    }
}
