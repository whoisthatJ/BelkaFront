using socket.io;
using UnityEngine;
using Leguar.TotalJSON;
using System.Collections.Generic;

public class ServiceIO : MonoBehaviour
{
    private const string TRUE_STR = "true";
    private const string FALSE_STR = "false";

    private const string SERVER_URI_LOCAL = "http://localhost:3001/?authToken=";
    private const string SERVER_URI_EXTERNAL = "http://18.189.250.248:3001/?authToken=";
    private const string SERVER_URI_ASEMULATOR = "http://10.0.2.2:3001/?authToken=";
    private const string SERVER_URI_PHONE = "http://192.168.0.12:3001/?authToken=";

    public static ServiceIO Instance { get; private set; }
    Socket socket;

    // Room id that user currently plays in
    public string RoomId { get; private set; }
    private string searchId = string.Empty;
    private float searchTime;
    private float[] searchTimeStones = { 10f, 8f, 6f };
    private int timeStoneIndex;
    private string serverUri;

    private FriendData[] playersData;
    public FriendData[] PlayersData {
        get
        {
            return playersData;
        }
        set
        {
            playersData = value;
            OnPlayersInfoUpdated?.Invoke();
        }
    }

    [SerializeField] private LoginTypes loginType;

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

    private void OnEnable()
    {
        MainModel.OnRoomIdChanged += RoomIdLoaded;
    }

    private void OnDisable()
    {
        MainModel.OnRoomIdChanged -= RoomIdLoaded;
    }

    public void ChangeServer(LoginTypes _loginType)
    {
        loginType = _loginType;
    }

    public void Connect()
    {
        string token = ServiceWeb.Instance.Token;

        if (loginType == LoginTypes.Local) serverUri = SERVER_URI_LOCAL;
        else if (loginType == LoginTypes.External) serverUri = SERVER_URI_EXTERNAL;
        else if (loginType == LoginTypes.Emulator) serverUri = SERVER_URI_ASEMULATOR;
        else if (loginType == LoginTypes.Phone) serverUri = SERVER_URI_PHONE;

        socket = Socket.Connect(serverUri + token);
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

        socket.On("playerHavingJackOfClubs", (string data) =>
        {
            //
            //GameMasterOnline.instance.SetTrumpPlayer(); // Add parameters
        });

        // ------------- New events -----------------
        SearchStarted();
        AgainSearchStarted();
        // Event saying that game was found
        FoundMatch();
        // Event which player answered isReady
        PlayersReady();
        KickedOutFromMatch();
        // Event that user recieve when he connects to the game after game
        // starts to get ongoing game information
        ReconnectSittingArrangement();

        SittingArrangement();
        DealerAndSuit();
        UserCards();

        // index of player that whose turn
        PlayerTurn();

        CardsAbleToPlay();
        PlayedCard();
        WinnerOfDraw();
        DealPoints();
        WinnerOfGame();
        BotPlaying();
        PlayerReturnPlaying();


        ReceiveMessage();
        SetStatus();
        AcceptedFriendship();
        NewFriendRequest();
        RemoveFromFriendReceive();
        AddToBlackListReceive();
        RemoveFromBlackListReceive();
    }

    public void JoinRoom(string roomId, string userId)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId); // id of created room
        jSON.Add("userId", userId); // id of the user that is trying to connect
        socket.EmitJson("joinRoom", jSON.CreateString());
        Debug.Log("joinRoom sended..");
    }

    public void PlayCard(string roomId, int cardId)
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", roomId);
        JSON card = new JSON();
        int[] suitNCard = Card.IntToSuitAndId(cardId);
        card.Add("suit", suitNCard[0]);
        card.Add("card", suitNCard[1]);
        jSON.Add("playedCard", card);
        // parse card into nominal and suit..
        // add card
        socket.EmitJson("playCard", jSON.CreateString());
        Debug.Log("Player made a turn");
    }

    // New events

    public void StartSingleSearch()
    {
        //socket.Emit("singlePlayerMatchMaking");
        socket.Emit("gameSearch");
        Debug.Log("singlePlayerMatchMaking sended..");
    }

    public void CancelSearch()
    {
        socket.Emit("leaveGameSearch");
        searchId = string.Empty;
        searchTime = 0f;
        timeStoneIndex = 0;
    }
    public void IncreaseSearchRange()
    {
        JSON jSON = new JSON();
        jSON.Add("searchId", searchId);
        socket.EmitJson("increaseSearchingRange", jSON.CreateString());

        Debug.Log("increaseSearchingRange sended..");
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

    public void ExitGame()
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", RoomId);
        socket.EmitJson("exitGame", jSON.CreateString());

        Debug.Log("ExitGame sended..");
        MainRoot.Instance.mainModel.GameSearchBlockTime = System.DateTime.Now.AddMinutes(5);
    }
    public void ResumeGame()
    {
        JSON jSON = new JSON();
        jSON.Add("roomId", RoomId);
        socket.EmitJson("resumeTimer", jSON.CreateString());

        Debug.Log("ResumeTimer sent..");
    }

    public void SendMessageUser(string userID, string message)
    {
        JSON jSON = new JSON();
        jSON.Add("toUserId", userID);
        jSON.Add("message", message);
        socket.EmitJson("sendMessage", jSON.CreateString());
    }

    public void ReceiveMessage()
    {
        socket.On("receiveMessage", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            string message = jSON.GetString("message");
            string fromUserID = jSON.GetString("fromUserId");
            string userName = jSON.GetString("userName");

            MainModel mm = MainRoot.Instance.mainModel;

            MessageData msg = new MessageData();
            msg.Message = message;
            msg.UserName = userName;
            msg.FromUserID = fromUserID;

            var element = mm.UnseenMessageses.Find(x => x.ID == fromUserID);
            if (element != null)
                element.CountMessage++;
            else {
                UnseenMessages unseen = new UnseenMessages();
                unseen.ID = fromUserID;
                unseen.CountMessage++;
                mm.UnseenMessageses.Add(unseen);
            }

            mm.CurrentMessage = msg;

            Debug.Log($"{message} {fromUserID} {userName}");
        });
    }

    public void SetStatus() {
        socket.On("friendStatus", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            var userInfo = jSON.GetJSON("userInfo");
            string id = userInfo.GetString("_id");
            string name = userInfo.GetString("name");

            bool status = jSON.GetBool("isOnline");

            UserData user = new UserData();
            user.ID = id;
            user.Name = name;
            user.IsStatus = status;

            MainModel mm = MainRoot.Instance.mainModel;
            mm.ChangeStatus = user;

            Debug.Log($"status {status} id {id} name {name}");
        });
    }

    public void SetUnseenMessages(string id) {
        socket.On("messageSeen", (string data) =>
        {
            JSON jSON = new JSON();
            jSON.Add("conversationId", id);
            socket.EmitJson("messageSeen", jSON.CreateString());

            Debug.Log($"id {id} ");
        });
    }

    public void AcceptedFriendship() {
        socket.On("responseOfFriendRequest", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            MainModel mm = MainRoot.Instance.mainModel;
            FriendRequest request = new FriendRequest();

            request.UserID = jSON.GetString("userId");
            request.RequestID = jSON.GetString("requestId");
            request.RequestStatus = jSON.GetInt("requestStatus");
            mm.AcceptedFriendship = request;

            Debug.Log($"id {jSON} ");
        });
    }

    public void NewFriendRequest() {
        socket.On("newFriendRequest", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            MainModel mm = MainRoot.Instance.mainModel;

            string requestId = jSON.GetString("requestId");

            var userInfo = jSON.GetJSON("userInfo");
            string id = userInfo.GetString("_id");
            string name = userInfo.GetString("name");
            bool status = userInfo.GetBool("isLegend");
            int expPoint = userInfo.GetInt("expPoints");
            int rankValue = userInfo.GetInt("rankValue");
            string rankType = userInfo.GetString("rankType");
            int stars = userInfo.GetInt("stars");
            // string legendRank = jSON.GetString("legendRank");
            // int legendPoints = jSON.GetInt("legendPoints");
            string avtarUrl = string.Empty;
            if (userInfo.ContainsKey("avtarUrl")) avtarUrl = userInfo.GetString("avtarUrl");

            RequestData user = new RequestData();
            user.ID = requestId;
            user.UserID = id;
            user.Name = name;
            user.IsStatus = status;
            user.ExpPoints = expPoint;
            user.RankValue = rankValue;
            user.RankType = rankType;
            user.Stars = stars;
            user.RequestStatus = 1;
            if (!string.IsNullOrEmpty(avtarUrl))
                user.Avatar = avtarUrl;

            mm.NewFriendRequest = user;

            Debug.Log($"id {id} name {name}");
        });
    }
    private void Update()
    {
        if (!searchId.Equals(string.Empty))
        {
            searchTime += Time.unscaledDeltaTime;
            if (searchTime > searchTimeStones[timeStoneIndex])
            {
                searchTime = 0f;
                timeStoneIndex = (timeStoneIndex + 1) > 2 ? 2 : (timeStoneIndex + 1);
                IncreaseSearchRange();
            }

        }
    }
    #region OnlineRanked
    private void SearchStarted()
    {
        socket.On("gameSearch", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            searchId = jSON.GetString("searchId");
            Debug.Log("gameSearch Started: " + searchId);
        });
    }
    private void AgainSearchStarted()
    {
        socket.On("enterAgainMatchMaking", (string data) =>
        {
            OnlineGameRankedPanel.Instance.BackToGameSearch();
            Debug.Log("enterAgainMatchMaking Started...");
        });
    }
    private void FoundMatch()
    {
        socket.On("gameFound", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            RoomId = jSON.GetString("roomId");

            JArray jArray = jSON.GetJArray("users");
            bool[] users = new bool[4];
            string[] ids = new string[4];
            for (int i = 0; i < jArray.Length; i++)
            {
                users[jArray.GetJSON(i).GetInt("playerPosition")] = jArray.GetJSON(i).GetBool("isReady");
                ids[jArray.GetJSON(i).GetInt("playerPosition")] = jArray.GetJSON(i).GetString("userId");
            }
            searchId = string.Empty;
            searchTime = 0f;
            timeStoneIndex = 0;
            OnlineGameRankedPanel.Instance.FoundGame(RoomId, users);
            ServiceWeb.Instance.GetInfoOfMultipleUsers(ids);
            Debug.Log("gameFound: roomId: " + RoomId);
            // Finish it when wait for ready panel is ready..
        });
    }
    public void PlayersInfoUpdate(FriendData [] users)
    {
        PlayersData = users;
    }

    private void PlayersReady()
    {
        socket.On("lobbyData", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            JArray jArray = jSON.GetJArray("users");
            bool[] users = new bool[4];
            for (int i = 0; i < jArray.Length; i++)
            {
                users[jArray.GetJSON(i).GetInt("playerPosition")] = jArray.GetJSON(i).GetBool("isReady");
            }
            OnlineGameRankedPanel.Instance.PlayerReadyUpdate(users);
            // Finish it when wait for ready panel is ready..
        });
    }

    private void KickedOutFromMatch()
    {
        socket.On("kickedOutFromMatchMaking", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);

            
            OnlineGameRankedPanel.Instance.KickedFromMatch();
            Debug.Log("kickedOutFromMatchMaking...");
        });
    }

    private void ReconnectSittingArrangement()
    {
        socket.On("isProcessed", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            try
            {
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
                Dictionary<int, int> drawCards = new Dictionary<int, int>();
                for (int i = 0; i < jDrawCards.Length; i++)
                {
                    JSON item = jDrawCards.GetJSON(i);
                    // write converter from backend card to frontend card..
                    int nominal = item.GetInt("card");
                    int suit = item.GetInt("suit");
                    int playerIndex = item.GetInt("playerIndex");
                    drawCards.Add(playerIndex, Card.ParseCard(suit, nominal));
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
                int[] playerCards = new int[jPlayerCards.Length];
                for (int i = 0; i < jPlayerCards.Length; i++)
                {
                    JSON item = jPlayerCards.GetJSON(i);
                    // write converter from backend card to frontend card..
                    int nominal = item.GetInt("card");
                    int suit = item.GetInt("suit");
                    playerCards[i] = Card.ParseCard(suit, nominal);
                }
                // set player cards..

                // how much time player has for turn
                int pendingTimeOfTurn = jSON.GetInt("pendingTimeOfTurn");
                // set..

                // room id
                string roomId = jSON.GetString("roomId");
                // set..
                RoomId = roomId;
                GameMasterOnline.instance.ReconnectSittingArrangement(arrangements, dealerIndex, playerTurnIndex, roundSuit, drawCards,
                    team1MatchPoints, team2MatchPoints, isEgg, playerCards, pendingTimeOfTurn, roomId);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                Debug.Log(data);
            }
        });
    }

    private void SittingArrangement()
    {
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
            GameMasterOnline.instance.ArrangePlayers(arrangment);
            //GameManager.gameMaster.ArrangePlayers(users); // Finish it..
        });
    }

    private void DealerAndSuit()
    {
        socket.On("dealerAndSuit", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            Debug.Log("dealerAndSuit: " + data);
            int dealerIndex = jSON.GetInt("dealerIndex");
            int roundSuit;
            try
            {
                roundSuit = jSON.GetInt("roundSuite");
            }
            catch (System.Exception e)
            {
                roundSuit = 104;
                Debug.Log("First Deal...");
            }
            Debug.Log("Dealer and Suit event...");
            // set..
            GameMasterOnline.instance.SetDealer(dealerIndex, roundSuit); // Add parameters..
        });
    }

    private void UserCards()
    {
        socket.On("userCards", (string data) =>
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
            GameMasterOnline.instance.SetCards(cards);

            Debug.Log("userCards event...");
        });
    }

    private void PlayerTurn()
    {
        socket.On("playerTurn", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerTurnIndex");
            int timerType = jSON.GetInt("timerType");
            GameMasterOnline.instance.PlayerTurn(position, timerType);

            Debug.Log("playerTurn event...");
        });
    }

    private void CardsAbleToPlay()
    {
        socket.On("cardsAbleToPlay", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            JArray jCards = jSON.GetJArray("cards");
            int[] cards = new int[jCards.Length];
            for (int i = 0; i < jCards.Length; i++)
            {
                JSON item = jCards.GetJSON(i);
                int nominal = item.GetInt("card");
                int suit = item.GetInt("suit");
                cards[i] = Card.ParseCard(suit, nominal);
            }
            GameMasterOnline.instance.SetPossibleCards(cards);
            Debug.Log("cardsAbleToPlay event...");
            // set..
        });
    }

    private void PlayedCard()
    {
        socket.On("playedCard", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerIndex");

            JSON jCard = jSON.GetJSON("playedCard");
            int nominal = jCard.GetInt("card");
            int suit = jCard.GetInt("suit");

            // set..

            GameMasterOnline.instance.MakeTurn(position, Card.ParseCard(suit, nominal)); // Add parameters
        });
    }

    private void WinnerOfDraw()
    {
        socket.On("winnerOfDraw", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int position = jSON.GetInt("playerIndex");

            /*JSON jCard = jSON.GetJSON("winCard");
            int nominal = jCard.GetInt("card");
            int suit = jCard.GetInt("suit");*/
            int points = jSON.GetInt("points");
            // set..
            GameMasterOnline.instance.EndRound(position, points);
        });
    }

    private void DealPoints()
    {
        socket.On("dealPoints", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int[] mPoints = jSON.GetJArray("matchPoints").AsIntArray();
            int[] uPoints = jSON.GetJArray("userPoints").AsIntArray();
            foreach (int i in mPoints)
            {
                Debug.Log("matchPoints: " + i);
            }
            foreach (int i in uPoints)
            {
                Debug.Log("userPoints: " + i);
            }
            GameMasterOnline.instance.EndDeal(uPoints[0] + uPoints[2], uPoints[1] + uPoints[3], mPoints[0], mPoints[1]);
        });
    }
    private void WinnerOfGame()
    {
        socket.On("winnerOfGame", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            int[] winners = jSON.GetJArray("winners").AsIntArray();
            int winTeam = winners[0] % 2;
            //
            GameMasterOnline.instance.EndGame(winTeam); // Add parameters
        });
    }
    private void BotPlaying()
    {
        socket.On("botStartsPlaying", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            string id = jSON.GetString("userId");
            GameMasterOnline.instance.BotPlaying(id);
            Debug.Log("botStartsPlaying: " );
        });
    }
    private void PlayerReturnPlaying()
    {
        socket.On("playerStartsPlaying", (string data) =>
        {
            Debug.Log("playerStartsPlaying: ");
            JSON jSON = JSON.ParseString(data);
            string id = jSON.GetString("userId");
            GameMasterOnline.instance.PlayerPlaying(id);
            Debug.Log("playerStartsPlaying: ");
        });
    }
    #endregion

    private void RoomIdLoaded()
    {
        RoomId = MainRoot.Instance.mainModel.RoomId;
    }

    public delegate void PlayersInfoUpdatedEvent();
    public static event PlayersInfoUpdatedEvent OnPlayersInfoUpdated;

    public delegate void AddToBlackListEvent(string ID);
    public static event AddToBlackListEvent OnAddToBlackListReceive;
    public void AddToBlackListReceive(){
        socket.On("addToBlackList", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            string userID = jSON.GetString("userId");
            OnAddToBlackListReceive?.Invoke(userID);
            Debug.Log($"userID {userID}");
        });   
    }
    
    
    public delegate void RemoveFromBlackListEvent(string ID);
    public static event RemoveFromBlackListEvent OnRemoveBlackListReceive;
    public void RemoveFromBlackListReceive(){
        socket.On("removeFromBlackList", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            string userID = jSON.GetString("userId");
            OnRemoveBlackListReceive?.Invoke(userID);
            Debug.Log($"userID {userID}");
        });   
    }
    
    public delegate void RemoveFromFriendReceiveEvent(string ID);
    public static event RemoveFromFriendReceiveEvent OnRemoveFromFriendReceive;
    public void RemoveFromFriendReceive(){
        socket.On("removeFromFriendList", (string data) =>
        {
            JSON jSON = JSON.ParseString(data);
            string userID = jSON.GetString("userId");
            OnRemoveFromFriendReceive?.Invoke(userID);
            Debug.Log($"userID {userID}");
        });   
    }
}
