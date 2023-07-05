using socket.io;
using UnityEngine;
using Leguar.TotalJSON;
using UnityEngine.UI;
using System.Collections.Generic;

public class ServiceGameIO : MonoBehaviour
{
    public static ServiceGameIO Instance { get; private set; }
    Socket socket;

    private const string TRUE_STR = "true";
    private const string FALSE_STR = "false";

    // Room id that user currently plays in
    public string RoomId { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Connect()
    {
        string token = ServiceWeb.Instance.Token;

#if UNITY_EDITOR
        socket = Socket.Connect("http://localhost:3000/?authToken=" + token);
#elif UNITY_ANDROID
            socket = Socket.Connect("http://18.189.250.248:3000/?authToken=" + token);
#endif
        DeclareListeners();
    }

    void DeclareListeners()
    {
        socket.On(SystemEvents.connect, () =>
        {
            Debug.Log("Socket connected");
        });

        socket.On("socketError", (string data) =>
        {
            Debug.LogError("Event socketError");
            Debug.LogError(data);
        });

        socket.On("winnerOfDraw", (string data) =>
        {
            //
            //GameMasterOnline.instance.RoundEnd(); // Add parameters
        });

        socket.On("dealPoints", (string data) =>
        {
            //
            //GameMasterOnline.instance.DistributionEnd(); // Add parameters
        });

        socket.On("winnerOfGame", (string data) =>
        {
            //
            //GameMasterOnline.instance.GameEnd(); // Add parameters
        });

        socket.On("playerHavingJackOfClubs", (string data) =>
        {
            //
            //GameMasterOnline.instance.SetTrumpPlayer(); // Add parameters
        });

        // ------------- New events -----------------

        // Event saying that game was found
        socket.On("singlePlayerMatchMaking", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            RoomId = jSON.GetString("roomId");

            JArray jArray = jSON.GetJArray("users");
            string[] users = new string[4];
            for (int i = 0; i < jArray.Length; i++)
            {
                users[i] = jArray.GetJSON(i).GetString("userId");
            }

            // Finish it when wait for ready panel is ready..
        });

        // Event which player answered isReady 
        socket.On("lobbyData", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            JArray jArray = jSON.GetJArray("users");
            string[] users = new string[4];
            for (int i = 0; i < jArray.Length; i++)
            {
                users[i] = jArray.GetJSON(i).GetString("userId");
            }

            // Finish it when wait for ready panel is ready..
        });

        // Event that user recieve when he connects to the game after game
        // starts to get ongoing game information
        socket.On("isProcessed", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            // sitting arrangements
            JArray jArrangements = jSON.GetJArray("sittingArrangement");
            Dictionary<string, int> arrangements = new Dictionary<string, int>();
            for (int i = 0; i < jArrangements.Length; i++)
            {
                JSON item = jArrangements.GetJSON(i);
                string userId = item.GetString("userId");
                int position = item.GetInt("playerPosition");
                arrangements.Add(userId, position);
            }
            // set arrangements

            // dealer index..
            int dealerIndex = jSON.GetInt("dealerIndex");
            // set dealer index..

            // index of current turn player (player whose turn is right now)
            int playerTurnIndex = jSON.GetInt("playerTurnIndex");
            // set player turn index..

            // round suit
            // 101 - diamonds, 102 - hearts, 103 - spades, 104 - clubs
            int roundSuit = jSON.GetInt("roundSuit");
            // set round suit..

            // card that currently are on the table
            JArray jDrawCards = jSON.GetJArray("drawCards");
            for (int i = 0; i < jDrawCards.Length; i++)
            {
                JSON item = jDrawCards.GetJSON(i);
                // write converter from backend card to frontend card..
                int nominal = item.GetInt("card");
                int suit = item.GetInt("suit");
                int playerIndex = item.GetInt("playerIndex");
            }
            // set draw cards..

            // match points of the round
            int team1MatchPoints = jSON.GetInt("team1MatchPoints");
            int team2MatchPoints = jSON.GetInt("team2MatchPoints");
            // set match points..

            // was last deal result draw
            bool isEgg = jSON.GetBool("isEgg");
            // set is egg..

            // player cards
            JArray jPlayerCards = jSON.GetJArray("cardsOfPlayer");
            for (int i = 0; i < jPlayerCards.Length; i++)
            {
                JSON item = jPlayerCards.GetJSON(i);
                // write converter from backend card to frontend card..
                int nominal = item.GetInt("card");
                int suit = item.GetInt("suit");
            }
            // set player cards..

            // how much time player has for turn
            int pendingTimeOfTurn = jSON.GetInt("pendingTimeOfTurn");
            // set..

            // room id
            string roomId = jSON.GetString("roomId");
            // set..
        });

        socket.On("sittingArrangement", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            JArray jArray = jSON.GetJArray("users");
            Dictionary<string, int> arrangment = new Dictionary<string, int>();
            for (int i = 0; i < jArray.Length; i++)
            {
                JSON entery = jArray.GetJSON(i);
                string userId = entery.GetString("userId");
                int position = entery.GetInt("playerPosition");
                arrangment.Add(userId, position);
            }
            //GameManager.gameMaster.ArrangePlayers(users); // Finish it..
        });

        socket.On("dealerAndSuit", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int dealerIndex = jSON.GetInt("dealerIndex");
            int roundSuit = jSON.GetInt("roundSuit");

            // set..
            //GameMasterOnline.instance.SetDealer(); // Add parameters..
        });

        /*socket.On("userCards", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            JArray jArray = jSON.GetJArray("cards");
            int[] cards = new int[jArray.Length];
            for (int i = 0; i < jArray.Length; i++)
            {
                JSON cardJSON = jArray.GetJSON(i);
                int nominal = cardJSON.GetInt("card");
                int suit = cardJSON.GetInt("suit");
                cards[i] = Card.ParseCard(suit, nominal);
            }
            GameManager.gameMaster.GetCards(cards);
        });*/

        // index of player that whose turn
        socket.On("playerTurn", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerTurnIndex");
            // set..
        });

        socket.On("cardsAbleToPlay", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            JArray jCards = jSON.GetJArray("cards");
            for (int i = 0; i < jCards.Length; i++)
            {
                JSON item = jCards.GetJSON(i);
                int nominal = item.GetInt("card");
                int suit = item.GetInt("suit");
            }

            // set..
        });

        socket.On("playedCard", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerIndex");

            JSON jCard = jSON.GetJSON("playedCard");
            int nominal = jSON.GetInt("card");
            int suit = jSON.GetInt("suit");

            // set..

            //GameMasterOnline.instance.CardPlayed(); // Add parameters
        });

        socket.On("winnerOfDraw", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerIndex");

            JSON jCard = jSON.GetJSON("winCard");
            int nominal = jSON.GetInt("card");
            int suit = jSON.GetInt("suit");

            // set..
        });

        socket.On("dealPoints", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

        });        
    }

    public void JoinRoom(string roomId, string userId)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId); // id of created room
        jSON.Add("userId", userId); // id of the user that is trying to connect
        socket.EmitJson("joinRoom", jSON.CreateString());
        Debug.Log("joinRoom sended..");
    }

    public void PlayCard(string roomId, int card)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId);
        // parse card into nominal and suit..
        // add card
        socket.EmitJson("playCard", jSON.CreateString());
    }

    // New events

    public void StartSingleSearch()
    {
        socket.Emit("singlePlayerMatchMaking");

        Debug.Log("singlePlayerMatchMaking sended..");
    }

    public void SendIsReady(string roomId, bool isReady)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId);
        string _isReady = isReady ? TRUE_STR : FALSE_STR;
        jSON.Add("isReady", _isReady);
        socket.EmitJson("isReady", jSON.CreateString());

        Debug.Log("isReady sended..");
    }

    // Send information on server that game scene was loaded
    public void Loaded(string roomId, bool isProcessed)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId);
        string _isProcessed = isProcessed ? TRUE_STR : FALSE_STR;
        jSON.Add("isProcessed", _isProcessed);
        socket.EmitJson("isProcessed", jSON.CreateString());

        Debug.Log("isProcessed sended..");
    }
}

