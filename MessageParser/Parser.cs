using System.Collections.Generic;
using System.Text;
using BlackJackGame;

namespace MessageParser
{
    public class Parser
    {
        public string ParseGameInstance(Dictionary<string, Player> players, Dealer dealer)
        {
            var message = new StringBuilder();

            message.Append(HandToString(dealer.GetHand(), "dealer", 4, 0));

            foreach (var player in players.Values)
            {
                message.Append(",");
                message.Append(HandToString(player.GetHand(), player.Id, player.StatusCode, player.GetMoney()));
            }

            return message.ToString();
        }

        private string HandToString(Hand hand, string id, int statusCode, decimal money)
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in hand.GetHand())
            {
                stringBuilder.Append($"{item.CardName}{SuitToString(item.Suit)}");
            }
           
            return $"{id}|{stringBuilder}|{statusCode}|{money}";
        }

        private string SuitToString(Suits suit)
        {
            return suit switch
            {
                Suits.Spade => "s",
                Suits.Club => "c",
                Suits.Diamond => "d",
                Suits.Heart => "h",
                _ => "",
            };
        }
    }
}
