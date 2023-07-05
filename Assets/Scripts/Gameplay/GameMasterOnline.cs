using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMasterOnline : MonoBehaviour
{
    public static GameMasterOnline instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    [SerializeField] private Menu menu;
    [SerializeField] private float turnTime = .6f;
    [SerializeField] private float roundEndTime = .6f;
    [SerializeField] private float botDelay = .6f;
    [SerializeField] private float distributionTime = 5f;

    protected Game cGame;
    protected Deal cDeal;
    protected Round cRound;
    private PlayerInfo[] playerInfos;
    [SerializeField] private int cPlayer;
    protected int playerWF;

    protected int playerInd;

    protected int[][] playersCards { get; private set; }
    private List<int> turnedCards;

    private int[] possibleCards;

    [SerializeField] private bool readyToPlaceCard;
    private bool cardsDistributed;

    private bool arrangementOK;
    private bool dealerOK;
    private bool cardsOK;
    
    private void Start()
    {
        GameManager.instance.online = true;
        ServiceIO.Instance.Loaded(ServiceIO.Instance.RoomId, true);
    }
    public void StartNewGame()
    {
        readyToPlaceCard = false;

        playerInfos = new PlayerInfo[4]
        {
            new PlayerInfo("Player"),
            new PlayerInfo("Vasya"),
            new PlayerInfo("Petya"),
            new PlayerInfo("Karlygash")
        };
        ViewManager.instance.SetPlayerNames(playerInfos);

        playerInd = 0;
        playersCards = new int[4][];

        int[] playersIds = new int[] { 0, 1, 2, 3 };
        cGame = new Game(0, playersIds);

        ViewManager.instance.ClearViews();
        StartNewDeal();
    }

    public void ArrangePlayers(Dictionary<string, int> arrangment)
    {
        playerInfos = new PlayerInfo[4];
        foreach (string s in arrangment.Keys)
        {
            if (s.Equals(MainRoot.Instance.mainModel.Id))
            {
                playerInd = arrangment[s];
            }
        }
        int[] playersIds = new int[4];
        int t = playerInd;
        playersIds[0] = playerInd;
        playerInfos[0] = new PlayerInfo(MainRoot.Instance.mainModel.UserName, MainRoot.Instance.mainModel.Id);
        for (int i = 1; i < playersIds.Length; i++)
        {
            if (t > 2)
                t = -1;
            playersIds[i] = ++t;
            FriendData ud = ServiceIO.Instance.PlayersData.ToList().Find(f => f.ID == arrangment.FirstOrDefault(x => x.Value == t).Key);
            playerInfos[i] = new PlayerInfo(ud.Name, ud.ID);
        }
        cGame = new Game(0, playersIds);
        ViewManager.instance.ClearViews();
        ViewManager.instance.SetPlayerNames(playerInfos);
        ViewManager.instance.SetPlayerRank(MainRoot.Instance.mainModel.Rank);
        arrangementOK = true;
    }
    public void SetDealer(int dealerInd, int suit)
    {
        ViewManager.instance.RefreshTeamsPoints(0, 0);
        cDeal = cGame.AddDealOnline(dealerInd, Card.IntToSuit(suit));

        if (cGame.GetDealsAmount() > 1)
        {
            int i = 0;
            for (i = 0; i < cGame.GetPlayersSuits().Length; i++)
            {
                if (cGame.GetPlayersSuits()[i] == Card.IntToSuit(suit))
                    break;
            }
            ViewManager.instance.OpenSuit(cDeal.GetTrump(), i);
        }
        dealerOK = true;
    }

    public void SetCards(int[] cards)
    {
        playersCards = new int[4][];
        for (int i = 0; i < 4; i++)
        {
            playersCards[i] = new int[8];
        }
        playersCards[0] = cards;
        cardsOK = true;
        StartCoroutine(WaitForDealDataSync());
    }

    private IEnumerator WaitForDealDataSync()
    {
        yield return new WaitUntil(() => arrangementOK);
        yield return new WaitUntil(() => dealerOK);
        yield return new WaitUntil(() => cardsOK);
        turnedCards = new List<int>();
        ViewManager.instance.DistributeCards(playersCards[0], cDeal.GetTrump(), true);
        cRound = cDeal.AddRound();
    }

    public void PlayerTurn(int i, int type)
    {
        cPlayer = i;
        if (cPlayer == playerInd)
            readyToPlaceCard = true;
        ViewManager.instance.SetCurrentTurnTimeCircleId(cGame.GetVisualId(cPlayer), type);
    }
    public void SetPossibleCards(int[] cards)
    {
        possibleCards = cards;
    }
    private void StartNewDeal()
    {
        ViewManager.instance.RefreshTeamsPoints(0, 0);
        cDeal = cGame.AddDeal(playerWF);

        //DistributeCards();
        //FindPlayerWF();
        if (cGame.GetDealsAmount() == 1)
        {
            SetSuits();
        }
        else
        {
            Suit suit = cGame.GetPlayersSuits()[playerWF];
            cDeal.SetTrump(suit);
            ViewManager.instance.OpenSuit(cDeal.GetTrump(), playerWF);
        }
        ViewManager.instance.DistributeCards(playersCards[0], cDeal.GetTrump());
        StartCoroutine(StartNewRoundWD(distributionTime));
    }

    
    public void ReconnectSittingArrangement(Dictionary<string, int> arrangment, int dealerIndex, int playerTurn, int roundSuit,
        Dictionary<int, int> drawCards, int team1points, int team2points, bool isEgg, int[] playerCards, int turnTimeLeft, string roomId)
    {
        ArrangePlayers(arrangment);
        SetDealer(dealerIndex, roundSuit);
        PlayerTurn(playerTurn, 1);
        int cardsLeft = playerCards.Length;
        if (drawCards.Keys.Contains(playerInd))
            cardsLeft++;
        int[][] reconnectedCards = new int[4][];
        reconnectedCards[0] = playerCards;
        for (int i = 1; i < cGame.GetPlayersIds().Length; i++)
        {
            reconnectedCards[i] = new int[cardsLeft - (drawCards.Keys.Contains(cGame.GetPlayersIds()[i]) ? 1 : 0)];
        }
        ViewManager.instance.DistributeReconnected(reconnectedCards, Card.IntToSuit(roundSuit));

        foreach (int i in drawCards.Keys)
        {
            if (i == playerInd)
                ViewManager.instance.MakeTurnPlayerReconnected(drawCards[i]);
            else
                ViewManager.instance.MakeTurnOtherReconnected(drawCards[i], cGame.GetVisualId(i));
        }
        cDeal.SetPoints(team1points, team2points); //it is possible that it is for eyes
        cDeal.SetEggs(isEgg);
        
    }
    
    public void SetPlayerWF(int ind)
    {
        playerWF = ind;
    }

    private bool suitsSet;
    public void SetSuits()
    {
        Suit[] suits = new Suit[4];
        int n = playerWF;
        int s = 0;
        do
        {
            suits[n] = (Suit)s;
            n++;
            if (n > 3) n -= 4;
            s++;
        } while (n != playerWF);
        cGame.SetSuits(suits);
        suitsSet = true;
        Debug.Log("Suits are set!!!");
        foreach (Suit su in suits)
        {
            Debug.Log(su);
        }
    }

    private void StartNewRound()
    {
        cRound = cDeal.AddRound();
        if (cRound == null)
        {
            // Last round of the deal
            //EndDeal();
        }
        else
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        cPlayer = cRound.GetNextPlayer();
        if (cPlayer != -1)
        {
            possibleCards = Card.GetPossibleCards(cRound.GetFirstCard(), GetTrump(), playersCards[cPlayer], cDeal.GetPlayedSuits());
            if (cPlayer != 0) StartCoroutine(BotTurnCor());
            readyToPlaceCard = true;
        }
        //else
            //EndRound();
    }

    private IEnumerator BotTurnCor()
    {
        yield return new WaitForSeconds(botDelay);
        BotTurn();
    }

    private void BotTurn()
    {
        int card = BotLogic.MakeTurn();
        MakeTurn(cPlayer, card);
    }

    public bool CheckIfCanPlace(int card)
    {
        if (readyToPlaceCard && cPlayer == playerInd && cardsDistributed)
        {
            for (int i = 0; i < possibleCards.Length; i++)
            {
                if (possibleCards[i] == card)
                    return true;
            }
        }
        return false;
    }
    public void MakeTurnPlayer(int card)
    {
        if (!suitsSet && cGame.GetDealsAmount() == 1 && card == 31)
        {
            SetPlayerWF(cGame.GetVisualId(cPlayer));
            SetSuits();
        }
        
        readyToPlaceCard = false;
        /*if (cRound.GetFirstCard() == -1)
        {
            int playedSuit = Card.GetSuit(card, cDeal.GetTrump());
            cDeal.PlayedSuit(playedSuit);
        }*/
        turnedCards.Add(card);
        cRound.MakeTurn(cPlayer, card);
        /*List<int> playerCards = playersCards[cPlayer].ToList();
        int cardInd = playerCards.FindIndex(c => c == card);
        if (cardInd != -1) playerCards.RemoveAt(cardInd);
        playersCards[cPlayer] = playerCards.ToArray();*/
        ViewManager.instance.MakeTurnPlayer(card);
        ServiceIO.Instance.PlayCard(ServiceIO.Instance.RoomId, card);
        ViewManager.instance.StopTurnTime(0);
        possibleCards = new int[0];
    }
    public void MakeTurn(int _playerInd, int card)
    {
        readyToPlaceCard = false;

        if (_playerInd == playerInd)
        {
            if (turnedCards.Contains(card))
            {
                return;
            }
            else
            {
                turnedCards.Add(card);
            }
        }
        if (!suitsSet && cGame.GetDealsAmount() == 1 && card == 31)
        {
            SetPlayerWF(cGame.GetVisualId(cPlayer));
            SetSuits();
        }
        
        // if it's a first card in the round set played suits
        /*if (cRound.GetFirstCard() == -1)
        {
            int playedSuit = Card.GetSuit(card, cDeal.GetTrump());
            cDeal.PlayedSuit(playedSuit);
        }*/
        cRound.MakeTurn(_playerInd, card);
        List<int> playerCards = playersCards[_playerInd].ToList();
        int cardInd = playerCards.FindIndex(c => c == card);
        if (cardInd != -1) playerCards.RemoveAt(cardInd);
        playersCards[_playerInd] = playerCards.ToArray();
        
        ViewManager.instance.MakeTurnOther(card, cGame.GetVisualId(_playerInd));
        ViewManager.instance.StopTurnTime(cGame.GetVisualId(_playerInd));
        //StartCoroutine(StartNextTurnWD());
    }

    // WD - with delay
    private IEnumerator StartNextTurnWD()
    {
        yield return new WaitForSeconds(turnTime);

        NextTurn();
    }

    public void EndRound(int winnerIndex, int points)
    {
        // Find round winner
        int wonPoints = points;
        int roundWinner = winnerIndex;
        cRound.SetWinner(roundWinner);
        cDeal.AddPoints(roundWinner, wonPoints);
        ViewManager.instance.GiveWinnerCards(cGame.GetVisualId(winnerIndex), cDeal.GetTeamOnePoints(), cDeal.GetTeamTwoPoints());
        cRound = cDeal.AddRound();
        //StartCoroutine(StartNewRoundWD(roundEndTime));
    }

    private IEnumerator StartNewRoundWD(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartNewRound();
    }

    public void EndDeal(int pointsOne, int pointsTwo, int eyesOne, int eyesTwo)
    {
        dealerOK = false;
        cardsOK = false;
        if (playerInd % 2 > 0)
        {
            int t = pointsOne;
            pointsOne = pointsTwo;
            pointsTwo = t;
            t = eyesOne;
            eyesOne = eyesTwo;
            eyesTwo = t;
        }
        cDeal.SetPoints(pointsOne, pointsTwo);
        cGame.SetEyes(eyesOne, 0);
        cGame.SetEyes(eyesTwo, 1);

        if (cGame.GetDealsAmount() < 2)
        {
            ViewManager.instance.SetUpEyes(cGame.GetPlayersSuits());
            ViewManager.instance.SetUpDealEyes(cGame.GetPlayersSuits());
        }
        if (cDeal.GetTeamOnePoints() > cDeal.GetTeamTwoPoints())
        {
            ViewManager.instance.OpenEyes(false, cGame.GetTeamOneEyes());
            ViewManager.instance.OpenEyesDeal(false, cGame.GetTeamOneEyes());
            ViewManager.instance.SetScore(true, pointsOne, pointsTwo);
        }
        else if (cDeal.GetTeamOnePoints() < cDeal.GetTeamTwoPoints())
        {
            ViewManager.instance.OpenEyes(true, cGame.GetTeamTwoEyes());
            ViewManager.instance.OpenEyesDeal(true, cGame.GetTeamTwoEyes());
            ViewManager.instance.SetScore(false, pointsOne, pointsTwo);
        }

        /*if (cGame.GetTeamOneEyes() < 12 && cGame.GetTeamTwoEyes() < 12)
        {
            StartNewDeal();
        }
        else
        {
            EndGame();
        }*/
    }

    public void EndGame(int winTeam)
    {
        if (winTeam % 2 == playerInd % 2)
            menu.OpenPanel(true);
        else
            menu.OpenPanel(false);
    }

    public void CardDistributed()
    {
        cardsDistributed = true;
    }

    public void BotPlaying(string id)
    {
        int i;
        for (i = 0; i < playerInfos.Length; i++)
        {
            if (playerInfos[i].userId == id)
                break;
        }
        ViewManager.instance.BotPlaying(i);
        if (playerInd == i)
            SetReadyToPlaceCards(false);
    }
    public void PlayerPlaying(string id)
    {
        int i;
        string s = "";
        for (i = 0; i < playerInfos.Length; i++)
        {
            if (playerInfos[i].userId == id)
            {
                s = playerInfos[i].name;
                break;
            }
        }
        ViewManager.instance.PlayerPlaying(i, s);
    }
    // Return cards that players can play in current situation
    public int[][] GetPossibleCards()
    {
        int[][] _possibleCards = new int[4][];
        int startedInd = cRound.roundStarter;
        for (int i = 0; i < 4; i++)
        {
            int playerInd = i + startedInd;
            if (playerInd > 3) playerInd -= 4;

            // If player already played card in current round his possible card is his played card
            if (cRound.playedCards[playerInd] != -1)
            {
                _possibleCards[i] = new int[] { cRound.playedCards[playerInd] };
            }
            else
            {
                if (playerInd == cPlayer) _possibleCards[i] = possibleCards;
                else _possibleCards[i] = Card.GetPossibleCards(cRound.GetFirstCard(), cDeal.GetTrump(), playersCards[playerInd], cDeal.GetPlayedSuits());
            }
        }

        return _possibleCards;
    }

    public int GetCrntPlrOrder()
    {
        int order = cPlayer - cRound.roundStarter;
        if (order < 0) order += 4;
        return order;
    }

    public Suit GetTrump()
    {
        return cDeal.GetTrump();
    }

    public bool[] GetPlayedSuits()
    {
        return cDeal.GetPlayedSuits();
    }

    public int GetFirstCard()
    {
        return cRound.GetFirstCard();
    }

    public PlayerInfo[] GetPlayerInfos()
    {
        return playerInfos;
    }

    public void SetReadyToPlaceCards(bool ready)
    {
        readyToPlaceCard = ready;
    }
}