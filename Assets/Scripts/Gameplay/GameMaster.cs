using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    [SerializeField] private float turnTime = .6f;
    [SerializeField] private float roundEndTime = .6f;
    [SerializeField] private float botDelay = .6f;
    [SerializeField] private float distributionTime = 5f;

    protected Game cGame;
    protected Deal cDeal;
    protected Round cRound;
    private PlayerInfo[] playerInfos;
    private int cPlayer;
    protected int playerWF;

    protected int playerInd;

    protected int[][] playersCards { get; private set; }
    private int[] possibleCards;

    private bool readyToPlaceCard;
    private bool cardsDistributed;

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

    private void StartNewDeal()
    {
        ViewManager.instance.RefreshTeamsPoints(0, 0);
        cDeal = cGame.AddDeal(playerWF);

        DistributeCards();
        FindPlayerWF();
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

    private void DistributeCards()
    {
        for (int i = 0; i < 4; i++)
        {
            playersCards[i] = new int[8];
        }
        int[] deck = Card.GenerateDeck();
        int n = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                playersCards[i][j] = deck[n];
                n++;
            }
        }

        cardsDistributed = false;
    }

    private void FindPlayerWF()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (playersCards[i][j] == 31)
                {
                    playerWF = i;
                    return;
                }
            }
        }
        playerWF = -1;
    }

    private void SetSuits()
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
    }

    private void StartNewRound()
    {
        cRound = cDeal.AddRound();
        if (cRound == null)
        {
            // Last round of the deal
            EndDeal();
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
        else
            EndRound();
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

    public void MakeTurn(int _playerInd, int card)
    {
        readyToPlaceCard = false;

        // if it's a first card in the round set played suits
        if (cRound.GetFirstCard() == -1)
        {
            int playedSuit = Card.GetSuit(card, cDeal.GetTrump());
            cDeal.PlayedSuit(playedSuit);
        }
        if (_playerInd == cPlayer) cRound.MakeTurn(_playerInd, card);

        List<int> playerCards = playersCards[_playerInd].ToList();
        int cardInd = playerCards.FindIndex(c => c == card);
        if (cardInd != -1) playerCards.RemoveAt(cardInd);
        playersCards[_playerInd] = playerCards.ToArray();

        if (_playerInd == playerInd)
        {
            ViewManager.instance.MakeTurnPlayer(card);
        }
        else
        {
            ViewManager.instance.MakeTurnOther(card, _playerInd);
        }
        StartCoroutine(StartNextTurnWD());
    }

    // WD - with delay
    private IEnumerator StartNextTurnWD()
    {
        yield return new WaitForSeconds(turnTime);

        NextTurn();
    }

    private void EndRound()
    {
        // Find round winner
        int wonPoints = 0;
        int roundWinner = Card.GetWinner(cRound.playedCards, cDeal.GetTrump(), cRound.roundStarter, out wonPoints);
        cRound.SetWinner(roundWinner);
        cDeal.AddPoints(roundWinner, wonPoints);
        ViewManager.instance.GiveWinnerCards(roundWinner, cDeal.GetTeamOnePoints(), cDeal.GetTeamTwoPoints());
        StartCoroutine(StartNewRoundWD(roundEndTime));
    }

    private IEnumerator StartNewRoundWD(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartNewRound();
    }

    private void EndDeal()
    {
        int teamOnePoints = cDeal.GetTeamOnePoints();
        int teamTwoPoints = cDeal.GetTeamTwoPoints();
        if (teamOnePoints != teamTwoPoints)
        {
            int wonEyes = 0;

            if (teamOnePoints == 0 || teamTwoPoints == 0)
            {
                wonEyes = 12;
                //Debug.Log("NAKED");
            }
            else
            {
                if (cGame.GetDealsAmount() < 2)
                {
                    wonEyes = 2;
                    //Debug.Log("FIRST ROUND");
                }
                else
                {
                    if (cGame.GetPreviousEggs())
                    {
                        wonEyes = 4;
                        //Debug.Log("PREVIOUS EGGS");
                    }
                    else
                    {
                        if (teamOnePoints > teamTwoPoints)
                        {
                            if (playerWF == 0 || playerWF == 2)
                            {
                                wonEyes = 1;
                                //Debug.Log("WON WITH TRUMP");
                            }
                            else
                            {
                                wonEyes = 2;
                                //Debug.Log("WON WITHOUT TRUMP");
                            }
                        }
                        else if (teamOnePoints < teamTwoPoints)
                        {
                            if (playerWF == 1 || playerWF == 3)
                            {
                                wonEyes = 1;
                                //Debug.Log("WON WITH TRUMP");
                            }
                            else
                            {
                                wonEyes = 2;
                                //Debug.Log("WON WITHOUT TRUMP");
                            }
                        }
                        if (teamOnePoints < 30 || teamTwoPoints < 30)
                        {
                            wonEyes++;
                            //Debug.Log("NO SAFE");
                        }
                    }
                }
            }

            if (teamOnePoints > teamTwoPoints)
            {
                cGame.AddEyes(wonEyes, 0);
            }
            else if (teamOnePoints < teamTwoPoints)
            {
                cGame.AddEyes(wonEyes, 1);
            }
        }
        else
        {
            cDeal.SetEggs(true);
            //Debug.Log("EGGS");
        }

        if (cGame.GetDealsAmount() < 2) ViewManager.instance.SetUpEyes(cGame.GetPlayersSuits());
        if (cDeal.GetTeamOnePoints() > cDeal.GetTeamTwoPoints())
        {
            ViewManager.instance.OpenEyes(false, cGame.GetTeamOneEyes());
        }
        else if (cDeal.GetTeamOnePoints() < cDeal.GetTeamTwoPoints())
        {
            ViewManager.instance.OpenEyes(true, cGame.GetTeamTwoEyes());
        }

        if (cGame.GetTeamOneEyes() < 12 && cGame.GetTeamTwoEyes() < 12)
        {
            StartNewDeal();
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        GameplayManager.instance.GameEnd(cGame.GetTeamOneEyes() >= 12);
    }

    public void CardDistributed()
    {
        cardsDistributed = true;
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
}