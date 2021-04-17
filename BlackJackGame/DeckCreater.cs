using System;
using System.Collections.Generic;

namespace BlackJackGame
{
    class DeckCreater
    {           
        public Stack<Card> CreateDeck(int deckAmout = 1)
        {
            var tempDeck = new List<Card>();
            var suits = new List<Suits>() { Suits.Club, Suits.Heart, Suits.Spade, Suits.Diamond };
            // Generate clear deck
            for (int decks = 0; decks < deckAmout; decks++)
            {
                for (int value = 2; value < 15; value++) 
                    foreach (var item in suits)
                        tempDeck.Add(new Card(item, value));
            }
            Random rand = new Random();
            // Shulfe deck
            for (int i = 0; i < tempDeck.Count * 5; i++) 
            {
                var firstCard  = rand.Next(tempDeck.Count);
                var secondCard = rand.Next(tempDeck.Count);

                var temp = tempDeck[firstCard];
                tempDeck[firstCard] = tempDeck[secondCard];
                tempDeck[secondCard] = temp;
            }

            // copy from List to Stack
            var deck = new Stack<Card>();
            foreach (var card in tempDeck)
                deck.Push(card);

            return deck;
        }
    }
}
