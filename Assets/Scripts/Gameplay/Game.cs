using System.Collections.Generic;
using System.Linq;

public class Game
{
    private int id;
    private int[] players;
    private Suit[] playersSuits;
    private int teamOneEyes;
    private int teamTwoEyes;
    private Deal[] deals;

    public Game(int id, int[] playersIds)
    {
        this.id = id;
        players = playersIds;
        deals = new Deal[0];
    }

    public Deal AddDeal(int playerWF)
    {
        List<Deal> dealsList = deals.ToList();

        int dealerInd = 0;
        Suit trump = Suit.clubs;
        if (dealsList.Count > 0)
        {
            dealerInd = dealsList.Last().GetNextDealer();
            trump = playersSuits[playerWF];
        }
        dealsList.Add(new Deal(dealerInd, trump));
        deals = dealsList.ToArray();

        return deals.Last();
    }
    public Deal AddDealOnline(int dealerInd, Suit trump)
    {
        List<Deal> dealsList = deals.ToList();

        dealsList.Add(new Deal(dealerInd, trump));
        deals = dealsList.ToArray();

        return deals.Last();
    }
    public void SetSuits(Suit[] suits)
    {
        playersSuits = suits;
    }

    public Suit[] GetPlayersSuits()
    {
        return playersSuits;
    }

    public int GetDealsAmount()
    {
        return deals.Length; 
    }

    public bool GetPreviousEggs()
    {
        return deals[deals.Length - 2].GetEggs();
    }

    public void AddEyes(int eyes, int team)
    {
        if (team == 0)
        {
            teamOneEyes += eyes;
        }
        else if (team == 1)
        {
            teamTwoEyes += eyes;
        }

        if (teamOneEyes > 12) teamOneEyes = 12;
        if (teamTwoEyes > 12) teamTwoEyes = 12;
    }
    public void SetEyes(int eyes, int team)
    {
        if (team == 0)
        {
            teamOneEyes = eyes;
        }
        else if (team == 1)
        {
            teamTwoEyes = eyes;
        }

    }
    public int GetTeamOneEyes()
    {
        return teamOneEyes;
    }

    public int GetTeamTwoEyes()
    {
        return teamTwoEyes;
    }

    public int[] GetPlayersIds()
    {
        return players;
    }

    public int GetVisualId(int id)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == id)
                return i;
        }
        return 0;
    }
}
