using Leguar.TotalJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card", order = 1)]
public class Card : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    public GameObject GetCardPrefab()
    {
        return prefab;
    }

    /* diamonds: 7 -  0, 8 -  1, 9 -  2, queen -  3, king -  4, 10 -  5, ace -  6
     * spades:   7 -  7, 8 -  8, 9 -  9, queen - 10, king - 11, 10 - 12, ace - 13
     * hearts:   7 - 14, 8 - 15, 9 - 16, queen - 17, king - 18, 10 - 19, ace - 20
     * clubs:    7 - 21, 8 - 22, 9 - 23, queen - 24, king - 25, 10 - 26, ace - 27
     * 
     * jack:     diamonds - 28, hearts - 29, spaced - 30, clubs    - 31 
     * sixes:    clubs    - 32, spades - 33, hearts - 34, diamonds - 35 */
    
    private static int[][] cards =
    {
        new int[] { 21, 22, 23, 24, 25, 26, 27 },
        new int[] { 14, 15, 16, 17, 18, 19, 20 },
        new int[] { 7, 8, 9, 10, 11, 12, 13 },
        new int[] { 0, 1, 2, 3, 4, 5, 6 },
        new int[] { 28, 29, 30, 31 }
    };

    private static int[] sixes = { 32, 33, 34, 35 };

    private static int[] cardsValues = { 0, 0, 0, 3, 4, 10, 11, 2 };

    public static int[] GenerateDeck()
    {
        int[] result = new int[32];
        int n = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            for (int j = 0; j < cards[i].Length; j++)
            {
                result[n] = cards[i][j];
                n++;
            }
        }
        System.Random random = new System.Random();
        result = result.OrderBy(x => random.Next()).ToArray();
        return result;
    }

    public static int[] SortCards(Suit trump, int[] deck)
    {
        int[] sortDeck = GetSortDeck(trump, 0);

        int[] result = new int[deck.Length];
        int rCounter = 0;
        for (int i = 0; i < sortDeck.Length; i++)
        {
            for (int j = 0; j < deck.Length; j++)
            {
                if (sortDeck[i] == deck[j])
                {
                    result[rCounter] = deck[j];
                    rCounter++;
                    break;
                }
            }
        }

        return result;
    }

    //Generate full deck sorted by trump
    private static int[] GetSortDeck(Suit trump, int firstCardSuit)
    {
        int[] sortDeck = new int[32];

        int n = 0;

        // Non-trump cards
        for (int i = 0; i < 4; i++)
        {
            if (i != (int)trump && i != firstCardSuit)
            {
                for (int j = 0; j < cards[i].Length; j++)
                {
                    sortDeck[n] = cards[i][j];
                    n++;
                }
            }
        }
        // First card suit cards
        if (firstCardSuit != (int)trump)
        {
            for (int i = 0; i < cards[firstCardSuit].Length; i++)
            {
                sortDeck[n] = cards[firstCardSuit][i];
                n++;
            }
        }
        // Trump cards
        for (int i = 0; i < cards[(int)trump].Length; i++)
        {
            sortDeck[n] = cards[(int)trump][i];
            n++;
        }
        // Jacks
        for (int i = 0; i < cards[4].Length; i++)
        {
            sortDeck[n] = cards[4][i];
            n++;
        }

        return sortDeck;
    }

    public static int[] GetPossibleCards(int firstCard, Suit trump, int[] _deck, bool[] playedSuits)
    {
        // Check if player is the first in the round
        if (firstCard == -1) return _deck;

        // Check if player has first card suit cards
        int fcSuit = GetSuit(firstCard, trump);
        List<int> possibleCards = new List<int>();
        for (int i = 0; i < cards[fcSuit].Length; i++)
        {
            for (int j = 0; j < _deck.Length; j++)
            {
                if (cards[fcSuit][i] == _deck[j])
                    possibleCards.Add(_deck[j]);
            }
        }
        // Add jacks if the first card is trump
        if (fcSuit == (int)trump)
        {
            for (int i = 0; i < _deck.Length; i++)
            {
                for (int j = 0; j < cards[4].Length; j++)
                {
                    if (cards[4][j] == _deck[i])
                        possibleCards.Add(_deck[i]);
                }
            }
        }

        if (possibleCards.Count > 0)
        {
            return possibleCards.ToArray();
        }
        else
        {
            // Player doesn't have first card suit
            int[] result = ExcludeNonPlayedAces(trump, playedSuits, _deck);
            if (result.Length > 0)
                return result;
            else
                // Player has only non-played aces
                return _deck;
        }
    }

    public static int GetSuit(int card, Suit trump)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < cards[i].Length; j++)
            {
                if (cards[i][j] == card) return i;
            }
        }

        for (int i = 0; i < cards[4].Length; i++)
        {
            if (cards[4][i] == card) return (int)trump;
        }

        return -1;
    }

    private static int[] ExcludeNonPlayedAces(Suit trump, bool[] playedSuits, int[] deck)
    {
        List<int> result = deck.ToList();

        for (int i = 0; i < 4; i++)
        {
            if (!playedSuits[i] && (int)trump != i)
            {
                int aceInd = result.FindIndex(c => c == cards[i][6]);
                if (aceInd != -1) result.RemoveAt(aceInd);
            }
        }

        return result.ToArray();
    }

    public static int GetWinner(int[] playedCards, Suit trump, int firstCardIndex, out int wonPoints)
    {
        wonPoints = CountPoints(playedCards);

        int firstCardSuit = GetSuit(playedCards[firstCardIndex], trump);
        for (int i = 3; i >= 0; i--)
        {
            for (int j = 0; j < playedCards.Length; j++)
            {
                if (playedCards[j] == cards[4][i]) return j;
            }
        }
        for (int i = cards[(int)trump].Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < playedCards.Length; j++)
            {
                if (playedCards[j] == cards[(int)trump][i]) return j;
            }
        }
        if (firstCardSuit != (int)trump)
        {
            for (int i = cards[firstCardSuit].Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < playedCards.Length; j++)
                {
                    if (playedCards[j] == cards[firstCardSuit][i]) return j;
                }
            }
        }

        Debug.LogError("Didn't find winning cards");
        return -1;
    }

    private static int CountPoints(int[] playedCards)
    {
        int result = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < cards[i].Length; j++)
            {
                for (int n = 0; n < playedCards.Length; n++)
                {
                    if (playedCards[n] == cards[i][j])
                    {
                        result += cardsValues[j];
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < cards[4].Length; i++)
        {
            for (int j = 0; j < playedCards.Length; j++)
            {
                if (playedCards[j] == cards[4][i])
                {
                    result += cardsValues[7];
                    break;
                }
            }
        }

        return result;
    }

    public static int[] GetSixes()
    {
        return sixes;
    }

    public static bool CheckIfHas(int[] _cards, int card)
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            if (_cards[i] == card) return true;
        }

        return false;
    }

    public static bool CompareCards(int firstCard, int[] cards1, int[] cards2, Suit _trump)
    {
        int firstCardSuit = GetSuit(firstCard, _trump);
        int[] sortDeck = GetSortDeck(_trump, firstCardSuit);

        for (int i = sortDeck.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < cards1.Length; j++)
            {
                if (sortDeck[i] == cards1[j]) return true;
            }
            for (int j = 0; j < cards2.Length; j++)
            {
                if (sortDeck[i] == cards2[j]) return false;
            }
        }

        return false;
    }

    public static int[] GetWinningCards(int firstCard, int[] cards1, int[] cards2, Suit _trump)
    {
        int firstCardSuit = GetSuit(firstCard, _trump);
        int[] sortDeck = GetSortDeck(_trump, firstCardSuit);

        // Find biggest card in second deck
        int biggestInd = -1;
        bool found = false;
        for (int i = sortDeck.Length - 1; i >= 0; i--)
        {
            for (int j = 0; j < cards2.Length; j++)
            {
                if (sortDeck[i] == cards2[j])
                {
                    biggestInd = i;
                    found = true;
                    break;
                }
            }
            if (found) break;
        }

        // Find winning cards
        List<int> winningCards = new List<int>();
        for (int i = biggestInd + 1; i < sortDeck.Length; i++)
        {
            for (int j = 0; j < cards1.Length; j++)
            {
                if (sortDeck[i] == cards1[j]) winningCards.Add(cards1[j]);
            }
        }

        return winningCards.ToArray();
    }

    public static int GetSmallestCard(int[] _cards, Suit _trump)
    {
        int[] sortDeck = SortCards(_trump, _cards);

        int[] points = new int[sortDeck.Length];
        for (int i = 0; i < sortDeck.Length; i++)
        {
            points[i] = CountPoints(new int[] { sortDeck[i] });
        }

        int resultInd = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i] < points[resultInd]) resultInd = i;
        }

        return sortDeck[resultInd];
    }

    public static int GetBiggestCard(int[] _cards, Suit _trump)
    {
        int[] sortDeck = SortCards(_trump, _cards);

        int[] points = new int[sortDeck.Length];
        for (int i = 0; i < sortDeck.Length; i++)
        {
            points[i] = CountPoints(new int[] { sortDeck[i] });
        }

        int resultInd = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i] > points[resultInd]) resultInd = i;
        }

        return sortDeck[resultInd];
    }
    public static Suit IntToSuit(int i)
    {
        if (i == 101)
            return Suit.diamonds;
        else if (i == 102)
            return Suit.hearts;
        else if (i == 103)
            return Suit.spades;
        return Suit.clubs;
    }
    private static int[] parseSuits = { 104, 102, 103, 101 };
    private static int[] parseNominals = { 7, 8, 9, 12, 13, 10, 1};
    private static int parseJackNominal = 11;
    private static int[] parseJackSuits = { 101, 102, 103, 104 };

    public static int ParseCard(int _suit, int _nominal)
    {
        int n = 0; // nominal
        int s = 0; // suit

        if (_nominal == parseJackNominal)
        {
            s = 4;
            for (int i = 0; i < parseJackSuits.Length; i++)
            {
                if (_suit == parseJackSuits[i])
                    n = i;
            }
        }
        else
        {
            for (int i = 0; i < parseSuits.Length; i++)
            {
                if (_suit == parseSuits[i])
                {
                    s = i;
                    for (int j = 0; i < parseNominals.Length; j++)
                    {
                        if (_nominal == parseNominals[j])
                        {
                            n = j;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return cards[s][n];
    }
    public static int[] IntToSuitAndId(int i)
    {
        int suit;
        int card;
        if (i < 28)
        {
            int d = i / 7;
            if (d == 0)
                suit = 101;
            else if (d == 1)
                suit = 103;
            else if (d == 2)
                suit = 102;
            else
                suit = 104;
            int m = i % 7;
            if (m > 5)
                card = 1;
            else if (m > 4)
                card = 10;
            else if (m > 2)
                card = m + 9;
            else
                card = m + 7;
        }
        else
        {
            suit = i + 73;
            card = 11;
        }
        int[] cds = new int[2];
        cds[0] = suit;
        cds[1] = card;
        return cds;
    }
}

public enum Suit { clubs, hearts, spades, diamonds }
