using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerResponseSimulator : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playersReady = new bool[] { true, true, true, true };
            SetUsersInfo();
            FoundGame();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //FriendData [] info = new FriendData[4];
            //info[0] = new FriendData();

            SittingArrangement();
            SetDealerAndSuit();
            SetCards();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            //FriendData [] info = new FriendData[4];
            //info[0] = new FriendData();            
            SetDealerAndSuit();
            SetCards();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SetUsersInfo();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerTurn == 0)
            {
                PlayerTurn();
            }
            else
            {
                GameMasterOnline.instance.MakeTurn(playerTurn, Random.Range(0, 32));
            }
            playerTurn++;
            if (playerTurn > 3)
                playerTurn = 0;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            EndRound();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            EndDeal();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            EndGame();
        }
    }

    public bool[] playersReady;
    [ContextMenu("FoundGame")]
    public void FoundGame()
    {
        OnlineGameRankedPanel.Instance.FoundGame("101", playersReady);
    }

    [ContextMenu("ReadyUpdate")]
    public void ReadyUpdate()
    {
        OnlineGameRankedPanel.Instance.PlayerReadyUpdate(playersReady);
    }

    [ContextMenu("SittingArrangement")]
    public void SittingArrangement()
    {
        string[] uids = { MainRoot.Instance.mainModel.Id, "_id1", "_id2", "_id3" }; ;
        Dictionary<string, int> arrangment = new Dictionary<string, int>();
        arrangment.Add(MainRoot.Instance.mainModel.Id, 0);
        for (int i = 1; i < 4; i++)
        {
            string userId = uids[i];
            int position = i;
            arrangment.Add(userId, position);
        }
        GameMasterOnline.instance.ArrangePlayers(arrangment);
    }
    public int dealerIndex;
    public int roundSuit = 101;
    [ContextMenu("SetDealerAndSuit")]
    public void SetDealerAndSuit()
    {
        roundSuit = Random.Range(102, 105);
        GameMasterOnline.instance.SetDealer(dealerIndex, roundSuit);
    }
    public int[] cards = { 28, 31, 4, 10, 17, 6, 13, 20};
    [ContextMenu("SetCards")]
    public void SetCards()
    {
        GameMasterOnline.instance.SetCards(cards);
    }
    public int playerTurn;
    public int turnType = 1;
    [ContextMenu("PlayerTurn")]
    public void PlayerTurn()
    {
        GameMasterOnline.instance.PlayerTurn(playerTurn, 1);
        PossibleCards();
    }
    [ContextMenu("PlayerTurnSmall")]
    public void PlayerTurnSmall()
    {
        GameMasterOnline.instance.PlayerTurn(playerTurn, 2);
        PossibleCards();
    }
    [ContextMenu("BotPlaying")]
    public void BotPlaying()
    {
        GameMasterOnline.instance.BotPlaying(MainRoot.Instance.mainModel.Id);
    }
    [ContextMenu("BotPlayingOpp")]
    public void BotPlayingOpp()
    {
        GameMasterOnline.instance.BotPlaying("_id" + Random.Range(1, 4)); 
    }
    public int[] possibleCards = { 28, 31, 4, 10, 17, 6, 13, 20 };
    [ContextMenu("PossibleCards")]
    public void PossibleCards()
    {
        GameMasterOnline.instance.SetPossibleCards(possibleCards);
    }
    public int playedCard = 28;
    [ContextMenu("MakeTurn")]
    public void MakeTurn()
    {
        GameMasterOnline.instance.MakeTurn(playerTurn, playedCard);
        playerTurn++;
        playerTurn = playerTurn > 3 ? 0 : playerTurn;
    }
    public int winnerIndex = 0;
    public int roundPoints = 10;
    [ContextMenu("EndRound")]
    public void EndRound()
    {
        winnerIndex = Random.Range(0, 4);
        GameMasterOnline.instance.EndRound(winnerIndex, roundPoints);
    }
    public int teamOnePoints = 51;
    public int teamTwoPoints = 29;
    public int teamOneEyes = 6;
    public int teamTwoEyes = 8;
    [ContextMenu("EndDeal")]
    public void EndDeal()
    {
        teamOnePoints = Random.Range(30, 90);
        teamTwoPoints = Random.Range(30, 90);
        if (teamOnePoints > teamTwoPoints)
            teamOneEyes += Random.Range(1, 4);
        if (teamOnePoints < teamTwoPoints)
            teamTwoEyes += Random.Range(1, 4); GameMasterOnline.instance.EndDeal(teamOnePoints, teamTwoPoints, teamOneEyes, teamTwoEyes);
    }
    public int gameWinnerTeam = 0;
    [ContextMenu("EndGame")]
    public void EndGame()
    {
        GameMasterOnline.instance.EndGame(gameWinnerTeam);
    }


    private void SetUsersInfo()
    {
        FriendData[] users = new FriendData[4];
        FriendData user = new FriendData();
        user.ID = MainRoot.Instance.mainModel.Id;
        user.RankValue = MainRoot.Instance.mainModel.Rank;
        user.Name = MainRoot.Instance.mainModel.UserName;
        user.Stars = MainRoot.Instance.mainModel.Stars;
        user.IsLegend = MainRoot.Instance.mainModel.IsLegend;
        user.RankType = MainRoot.Instance.mainModel.RankType;
        users[0] = user;
        //Debug.Log(element.GetString("rankName"));
        for (int i = 1; i < 4; i++)
        {
            FriendData u = new FriendData();
            u.ID = "_id" + i;
            u.RankValue = 10 + i;
            u.Name = "name" + i;
            u.Stars = i - 1;
            u.IsLegend = false;
            u.RankType = MainRoot.Instance.mainModel.RankType;
            //Debug.Log(element.GetString("rankName"));
            users[i] = u;
        }
        ServiceIO.Instance.PlayersInfoUpdate(users);
    }
}
