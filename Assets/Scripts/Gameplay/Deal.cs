using System.Collections.Generic;
using System.Linq;

public class Deal
{
    private int[][] cards;
    private Suit trump;
    private int dealer;
    private Round[] rounds;
    private int teamOnePoints;
    private int teamTwoPoints;
    private bool[] playedSuits;

    private bool eggs;

    public Deal(int dealer, Suit trump)
    {
        this.dealer = dealer;
        this.trump = trump;
        rounds = new Round[0];
        playedSuits = new bool[4];
    }

    public Round AddRound()
    {
        if (rounds.Length > 7) return null;

        List<Round> roundsList = rounds.ToList();
        int roundNumber = roundsList.Count;
        int roundStarter = dealer;
        if (roundNumber > 0) roundStarter = roundsList.Last().roundWinner;
        roundsList.Add(new Round(roundStarter));
        rounds = roundsList.ToArray();

        return rounds.Last();
    }

    public int GetNextDealer()
    {
        if (eggs) return dealer;
        int result = dealer;
        result++;
        if (result > 3) result -= 4;
        return result;
    }

    public void SetTrump(Suit suit)
    {
        trump = suit;
    }

    public Suit GetTrump()
    {
        return trump;
    }

    public bool[] GetPlayedSuits()
    {
        return playedSuits;
    }

    public void AddPoints(int playerInd, int points)
    {
        if (playerInd == 0 || playerInd == 2)
        {
            teamOnePoints += points;
        }
        else if (playerInd == 1 || playerInd == 3)
        {
            teamTwoPoints += points;
        }
    }
    public void SetPoints(int teamOne, int teamTwo)
    {
        teamOnePoints = teamOne;
        teamTwoPoints = teamTwo;
    }
    public int GetTeamOnePoints()
    {
        return teamOnePoints;
    }

    public int GetTeamTwoPoints()
    {
        return teamTwoPoints;
    }

    public bool GetEggs()
    {
        return eggs;
    }

    public int GetDealer()
    {
        return dealer;
    }

    public void SetEggs(bool _eggs)
    {
        eggs = _eggs;
    }

    public void PlayedSuit(int suit)
    {
        playedSuits[suit] = true;
    }
}