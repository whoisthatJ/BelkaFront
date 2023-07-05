using System.Collections.Generic;

public static class BotLogic
{
    public static int MakeTurn()
    {
        int botOrd = GameMaster.instance.GetCrntPlrOrder();
        int[][] cards = GameMaster.instance.GetPossibleCards();
        

        if (botOrd == 0)
        {
            List<int> winningCards = new List<int>();

            for (int i = 0; i < cards[0].Length; i++)
            {
                bool[] playedSuits = new bool[4];
                GameMaster.instance.GetPlayedSuits().CopyTo(playedSuits, 0);

                playedSuits[Card.GetSuit(cards[0][i], GameMaster.instance.GetTrump())] = true;

                int[] _cards1 = Card.GetPossibleCards(cards[0][i], GameMaster.instance.GetTrump(), cards[1], playedSuits);
                int[] _cards2 = Card.GetPossibleCards(cards[0][i], GameMaster.instance.GetTrump(), cards[2], playedSuits);
                int[] _cards3 = Card.GetPossibleCards(cards[0][i], GameMaster.instance.GetTrump(), cards[3], playedSuits);

                int[] team1Cards = new int[_cards2.Length + 1];
                team1Cards[0] = cards[0][i];
                _cards2.CopyTo(team1Cards, 1);

                int[] team2Cards = new int[_cards1.Length + _cards3.Length];
                _cards1.CopyTo(team2Cards, 0);
                _cards3.CopyTo(team2Cards, _cards1.Length);

                bool isWinning = Card.CompareCards(cards[0][i], team1Cards, team2Cards, GameMaster.instance.GetTrump());
                if (isWinning)
                {
                    winningCards.Add(cards[0][i]);
                }
            }

            if (winningCards.Count > 0)
            {
                int result = Card.GetBiggestCard(winningCards.ToArray(), GameMaster.instance.GetTrump());
                return result;
            }
            else
            {
                int result = Card.GetSmallestCard(cards[0], GameMaster.instance.GetTrump());
                return result;
            }
        }
        else
        {
            int[] team1Cards;
            int[] team2Cards;

            if (botOrd % 2 == 0)
            {
                team1Cards = new int[cards[0].Length + cards[2].Length];
                cards[0].CopyTo(team1Cards, 0);
                cards[2].CopyTo(team1Cards, cards[0].Length);

                team2Cards = new int[cards[1].Length + cards[3].Length];
                cards[1].CopyTo(team2Cards, 0);
                cards[3].CopyTo(team2Cards, cards[1].Length);
            }
            else
            {
                team1Cards = new int[cards[1].Length + cards[3].Length];
                cards[1].CopyTo(team1Cards, 0);
                cards[3].CopyTo(team1Cards, cards[1].Length);

                team2Cards = new int[cards[0].Length + cards[2].Length];
                cards[0].CopyTo(team2Cards, 0);
                cards[2].CopyTo(team2Cards, cards[0].Length);
            }

            int firstCard = GameMaster.instance.GetFirstCard();

            bool isWinning = Card.CompareCards(firstCard, team1Cards, team2Cards, GameMaster.instance.GetTrump());
            if (isWinning)
            {
                int[] winningCards = Card.GetWinningCards(firstCard, team1Cards, team2Cards, GameMaster.instance.GetTrump());

                // Check if only bot has winning cards
                int count = 0;
                for (int i = 0; i < winningCards.Length; i++)
                {
                    if (Card.CheckIfHas(cards[botOrd], winningCards[i])) count++;
                }

                if (count == winningCards.Length)
                {
                    int result = Card.GetBiggestCard(winningCards, GameMaster.instance.GetTrump());
                    return result;
                }
                else
                {
                    int result = Card.GetBiggestCard(cards[botOrd], GameMaster.instance.GetTrump());
                    return result;
                }
            }
            else
            {
                int result = Card.GetSmallestCard(cards[botOrd], GameMaster.instance.GetTrump());
                return result;
            }
        }
    }
}