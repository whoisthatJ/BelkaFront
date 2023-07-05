public class Round
{
    public int roundStarter { get; private set; }
    public int[] playedCards { get; private set; }
    public int roundWinner { get; private set; }

    public Round(int roundStarter)
    {
        this.roundStarter = roundStarter;

        playedCards = new int[] { -1, -1, -1, -1 };
    }

    public int GetNextPlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            int playerInd = roundStarter + i;
            if (playerInd > 3) playerInd -= 4;
            if (playedCards[playerInd] == -1)
                return playerInd;
        }
        return -1;
    }

    public int GetFirstCard()
    {
        return playedCards[roundStarter];
    }

    public void MakeTurn(int playerInd, int card)
    {
        playedCards[playerInd] = card;
    }

    public void SetWinner(int winnerInd)
    {
        roundWinner = winnerInd;
    }

    public int GetPlayerOrder(int playerInd)
    {
        int order = playerInd + roundStarter;
        if (order > 3) order -= 4;
        return order;
    }
}
