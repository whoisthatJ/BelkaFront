using System;
using Leguar.TotalJSON;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServiceWeb : MonoBehaviour
{
    public static ServiceWeb Instance
    {
        get;
        private set;
    }

    #region CONSTS
    private const string DASH = "/";

    private const string CHECK_INTERNET = "https://www.google.com/";

    private const string SERVER_URI_LOCAL = "http://localhost:3001";
    private const string SERVER_URI_EXTERNAL = "http://18.189.250.248:3001";
    private const string SERVER_URI_ASEMULATOR = "http://10.0.2.2:3001";
    private const string SERVER_URI_PHONE = "http://192.168.0.12:3001/?authToken=";

    private const string REQUEST_HEADER = "authorization";

    private const string API_SERVER_RESPONSE = "/v1/serverResponse";

    private const string API_USER_LOGIN_SOCIAL = "/v1/user/socialLogin";
    private const string API_USER_LOGOUT = "/v1/user/logout";
    private const string API_USER_PROFILE = "/v1/user/profile";
    private const string API_USER_RENAME = "/v1/user/nickName";
    
    private const string API_LIST_USERS = "/v1/user/listOfUsers";
    private const string API_UPLOAD_AVATAR = "/v2/user/uploadAvtar";
    private const string API_OTHER_PROFILE = "/v1/user/otherUserProfile";
    
    private const string API_ROOM_CREATE = "/v1/room";
    private const string API_ROOM_FETCH_OPENED = "/v1/user/openRooms";

    private const string API_FRIEND_ADD = "/v1/friend/add";
    private const string API_FRIEND_REQUEST = "/v1/friend/request";
    private const string API_FRIEND_FETCH = "/v1/friend";
    private const string API_FRIEND_REMOVE = "/v1/friend/removeFriend";

    private const string API_BLACKLIST_ADD = "/v1/blackList";
    private const string API_BLACKLIST_DELETE = "/v1/blackList";
    private const string API_BLACKLIST_FETCH = "/v1/blackList";
    private const string API_BLACKLIST_REMOVE = "/v1/removeFromBlackList";
        

    private const string API_HISTORYCHAT_USER = "/v1/chat/oneToOne";
    private const string API_LATEST_DIALOGS = "/v1/chat/chatScreen";
    
    private const string API_UPLOAD_FILE = "/v1/uploadFile";
    private const string API_GET_PREMIUM = "/v1/user/getPremium";

    private const string API_INFO_OF_MULTIPLE_USERS = "/v1/user/infoOfMultipleUsers";
    
    private const string API_ITEM_GET_ITEMS = "/v1/item/getItems";
    private const string API_ITEM_BUY = "/v1/item/buyItem";
    
    private const string API_RANK_INFO = "/v1/rankType/ranksInfo";
    
    private const string API_LOOTBOX = "/v1/lootBox";
    #endregion

    #region LOGIN VALUES
    [SerializeField] private LoginTypes loginType;
    [SerializeField] private string deviceId;
    [SerializeField] private string userName;
    #endregion

    public string Token { get; private set; }
    private bool isLoggedIn;
    public bool IsLoggedIn
    {
        get
        {
            return isLoggedIn;
        }
        private set { }
    }

    private bool isReady;
    private string serverUri;

    void Awake()
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

    public void RewriteLoginParams(LoginTypes _loginType, string _deviceId, string _userName)
    {
        loginType = _loginType;
        deviceId = _deviceId;
        userName = _userName;
    }

    public void Init()
    {
        if (loginType == LoginTypes.Emulator) serverUri = SERVER_URI_ASEMULATOR;
        else if (loginType == LoginTypes.External) serverUri = SERVER_URI_EXTERNAL;
        else if (loginType == LoginTypes.Local) serverUri = SERVER_URI_LOCAL;
        else if (loginType == LoginTypes.Phone) serverUri = SERVER_URI_PHONE;

        StartCoroutine(Initialization());
    }

    IEnumerator Initialization()
    {
        // Check internet connection
        //isReady = false;
        //StartCoroutine(CheckInternet());
        //
        //yield return new WaitUntil(() => isReady);

        // Check server response
        isReady = false;
        StartCoroutine(ServerResponse());
        yield return new WaitUntil(() => isReady);

        // Login
        isReady = false;
        StartCoroutine(Login());
        yield return new WaitUntil(() => isReady);
        
        //RankInfo
        isReady = false;
        StartCoroutine(GetRankTypeInfo());
        yield return new WaitUntil(() => isReady);
        
        //Request and Friends
        isReady = false;
        StartCoroutine(GetFriendRequests());
        yield return new WaitUntil(() => isReady);
        
        /*//Get all friends
        isReady = false;
        StartCoroutine(GetFriends());
        yield return new WaitUntil(() => isReady);*/
        
        //Get blacklist users
        isReady = false;
        StartCoroutine(GetBlackListUsers());
        //yield return new WaitUntil(() => isReady);
        
        isReady = false;
        StartCoroutine(GetLatestDialogs());
       // yield return new WaitUntil(() => isReady);
        
        isReady = false;
        //yield return new WaitForSeconds(.1f);
        StartCoroutine(GetItemsShop());
        StartCoroutine(GetLootBoxShop());
        // yield return new WaitUntil(() => isReady);

    }

    public void UpdateProfile(bool isFullName, bool isAge, bool isCity, bool isEmail)
    {
        MainModel mm = MainRoot.Instance.mainModel;

        string fullName = (isFullName) ? mm.FullName : null;
        int age = (isAge) ? mm.Age : 0;
        string city = (isCity) ? mm.City : null;
        string email = (isEmail) ? mm.Email : null;

        StartCoroutine(UpdateProfileCor(fullName, age, city, email));
    }

    IEnumerator CheckInternet()
    {
        Debug.Log("Checking internet connection...");

        UnityWebRequest www = UnityWebRequest.Get(CHECK_INTERNET);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
            Debug.LogError("Internet is not available");
        }
        else
        {
            Debug.Log("Checking internet connection.. Success");
            isReady = true;
        }
    }

    IEnumerator ServerResponse()
    {
        Debug.Log("Server response...");
        
        UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_SERVER_RESPONSE));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("Server response.. Success");
            isReady = true;
        }
    }

    IEnumerator Login()
    {
        Debug.Log("Login...");

        WWWForm form = new WWWForm();

        form.AddField("deviceId", deviceId);
        form.AddField("loginType", 1);
        form.AddField("socialId", deviceId);
        form.AddField("name", userName);

        UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_USER_LOGIN_SOCIAL), form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            
            JSON jSON = JSON.ParseString(www.downloadHandler.text);
            Token = jSON.GetString("token");
         
            JSON data = jSON.GetJSON("data");
            //Debug.Log(www.downloadHandler.text);
            MainModel mm = MainRoot.Instance.mainModel;
            
            mm.Id = data.GetString("_id");
            mm.UserName = data.GetString("name");
            mm.Rank = data.GetInt("rankValue");
            //mm.RankName = data.GetString("rankName");
            mm.Stars = data.GetInt("stars");
            //mm.StarsForNextRank = data.GetInt("starsForNextRank");
            mm.HardCurrency = data.GetInt("golds");
            mm.SoftCurrency = data.GetInt("chips");
            
            if (data.ContainsKey("rankType"))
                mm.RankType = data.GetString("rankType");
            
            if (data.ContainsKey("avtarUrl"))
            mm.Avatar = data.GetString("avtarUrl");
            
            if (data.ContainsKey("fullName"))
                mm.FullName = data.GetString("fullName");
            if (data.ContainsKey("age"))
                mm.Age = data.GetInt("age");
            if (data.ContainsKey("city"))
                mm.City = data.GetString("city");
            if (data.ContainsKey("email"))
                mm.Email = data.GetString("email");

            mm.IsLegend = data.GetBool("isLegend");
            
            if (mm.IsLegend && data.ContainsKey("legendRank"))
            {
                mm.LegendRank = data.GetInt("legendRank");
            }

            if (data.ContainsKey("premiumSubscription"))
               mm.IsPremium = data.GetBool("premiumSubscription");

            /*var restored = data.GetJSON("isRestored");
            mainModel.IsPremium = restored.GetBool("status");*/
            if (data.ContainsKey("rankGameSearchStatus"))
            {
                JSON gameSearch = data.GetJSON("rankGameSearchStatus");
                if (gameSearch.ContainsKey("isActiveGame"))
                    mm.IsActiveGame = gameSearch.GetBool("isActiveGame");
                if (gameSearch.ContainsKey("roomId"))
                    mm.RoomId = gameSearch.GetString("roomId");
            }

            if (data.ContainsKey("rankGameSearchBlockTime"))
            {
                mm.GameSearchBlockTime = DateTime.Parse(data.GetString("rankGameSearchBlockTime"));
            }

            if (data.ContainsKey("numberOfTimesNickNameChanged"))
                mm.CountRename = data.GetInt("numberOfTimesNickNameChanged");
            isLoggedIn = true;
            isReady = true;
            Debug.Log("Login.. Success");
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator Logout()
    {
        Debug.Log("Logout...");

        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);

            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_USER_LOGOUT), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Token = null;
                isLoggedIn = false;
                Debug.Log("Logout.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    IEnumerator GetProfile()
    {
        Debug.Log("Get profile...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_USER_PROFILE));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                JSON data = jSON.GetJSON("data");

                MainModel mm = MainRoot.Instance.mainModel;
                UserProfileData userData = new UserProfileData();
                
                userData.Name = data.GetString("name");
                userData.RankValue = data.GetInt("rankValue");
                userData.RankType = data.GetString("rankType");
                mm.HardCurrency = data.GetInt("golds");
                mm.SoftCurrency = data.GetInt("chips");
                userData.Stars = data.GetInt("stars");
                userData.TotalGamesPlayed = data.GetInt("totalGamesPlayed");
                userData.TotalGameLeaved = data.GetInt("totalGamesLeaved");
                userData.TotalGameLost = data.GetInt("totalGamesLost");
                userData.TotalFlawlessWin = data.GetInt("totalFlawlessWin");
                userData.TotalGameWon = data.GetInt("totalGameWon");
                userData.TotalNakedWin = data.GetInt("totalNakedWin");
                if (data.ContainsKey("avtarUrl"))
                    userData.Avatar = data.GetString("avtarUrl");
                /*if (data.ContainsKey("rankName"))
                userData.RankName = data.GetString("rankName");
                if (data.ContainsKey("starsForNextRank"))
                userData.StarsForNextRank = data.GetInt("starsForNextRank");*/
                if (data.ContainsKey("premiumSubscription"))
                    MainRoot.Instance.mainModel.IsPremium = true;
                else MainRoot.Instance.mainModel.IsPremium = false;
                //userData.IsLengend = data.GetBool("isLegend");
                /*if (mm.IsLengend)
                {
                    mm.LegendRank = data.GetInt("legendRank");
                }*/
                mm.UserProfileData = userData;
                Debug.Log("Get profile.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetProfileUser(){
        StartCoroutine(GetProfile());
    }
    
    IEnumerator GetOtherProfileUser(string ID)
    {
        Debug.Log("Get other profile..." + ID);

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_OTHER_PROFILE) + "?userId=" + ID);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                JSON data = jSON.GetJSON("data");

                MainModel mm = MainRoot.Instance.mainModel;
                OtherProfileData userData = new OtherProfileData();

                userData.ID = data.GetString("_id");
                userData.Name = data.GetString("name");
                userData.RankValue = data.GetInt("rankValue");
                userData.RankType = data.GetString("rankType");
                userData.Golds = data.GetInt("golds");
                userData.Chips = data.GetInt("chips");
                userData.Stars = data.GetInt("stars");
                userData.TotalGamesPlayed = data.GetInt("totalGamesPlayed");
                userData.TotalGameLeaved = data.GetInt("totalGamesLeaved");
                userData.TotalGameLost = data.GetInt("totalGamesLost");
                userData.TotalFlawlessWin = data.GetInt("totalFlawlessWin");
                userData.TotalGameWon = data.GetInt("totalGameWon");
                userData.TotalNakedWin = data.GetInt("totalNakedWin");
                if (data.ContainsKey("avtarUrl"))
                    userData.Avatar = data.GetString("avtarUrl");
                /*if (data.ContainsKey("rankName"))
                userData.RankName = data.GetString("rankName");*/
                userData.IsBlocked = data.GetBool("isBlocked");
                userData.IsBlockedYou = data.GetBool("isBlockedYou");
                userData.IsFriend = data.GetBool("isFriend");
                /*if (data.ContainsKey("starsForNextRank"))
                userData.StarsForNextRank = data.GetInt("starsForNextRank");*/
                userData.IsLegend = data.GetBool("isLegend");
                mm.OtherProfile = userData;
                
                Debug.Log("Get other profile.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetOtherProfile(string ID){
        StartCoroutine(GetOtherProfileUser(ID));
    }
    
    IEnumerator UpdateProfileCor(string fullName, int age, string city, string email)
    {
        Debug.Log("Update profile...");

        if (!string.IsNullOrEmpty(Token))
        {
            JSON jSON = new JSON();
            if (fullName != null) jSON.Add("fullName", fullName);
            if (age != 0) jSON.Add("age", age);
            if (city != null) jSON.Add("city", city);
            if (email != null) jSON.Add("email", email);

            UnityWebRequest www = UnityWebRequest.Put(string.Concat(serverUri, API_USER_PROFILE), jSON.CreateString());
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON response = JSON.ParseString(www.downloadHandler.text);
                JSON data = response.GetJSON("data");
                MainModel mm = MainRoot.Instance.mainModel;

                if (fullName != null) mm.FullName = data.GetString("fullName");
                if (age != 0) mm.Age = data.GetInt("age");
                if (city != null) mm.City = data.GetString("city");
                if (email != null) mm.Email = data.GetString("email");
                Debug.Log(data);
                Debug.Log("Update profile.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    IEnumerator CreateRoom(string name)
    {
        Debug.Log("Create room...");

        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("name", name);

            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_ROOM_CREATE), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Create room.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }
    public delegate void AddFriendEvent(string ID);
    public static event AddFriendEvent OnAddFriend;
    
    IEnumerator AddFriend(string friendId)
    {
        Debug.Log("Add friend..." + friendId);

        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("friendId", friendId);

            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_FRIEND_ADD), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                OnAddFriend?.Invoke(friendId);
                Debug.Log("Add friend.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void AddFriendRequest(string ID){
        StartCoroutine(AddFriend(ID));
    }
    
    IEnumerator UpdateFriendRequest(string requestId, int status)
    {
        Debug.Log("Update friend request...");
        
        if (!string.IsNullOrEmpty(Token))    
        {
            JSON jSON = new JSON();
            jSON.Add("requestStatus", status);
            
            UnityWebRequest www = UnityWebRequest.Put(string.Concat(serverUri, API_FRIEND_REQUEST, DASH, requestId), jSON.CreateString());
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else{
                StartCoroutine(GetFriends());
                var mm = MainRoot.Instance.mainModel;
                mm.IsNotification();
                Debug.Log("Update friend request.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void UpdateFriend(string ID, int status){
        StartCoroutine(UpdateFriendRequest(ID, status));
    }
    
    IEnumerator GetFriendRequests()
    {
        Debug.Log("Get friend requests...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_FRIEND_REQUEST));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                JArray data = jSON.GetJArray("data");
                
                for (int i = 0; i < data.Length; i++)
                {
                    RequestData requestData = new RequestData();
                    var element = data.GetJSON(i);
                    
                    requestData.ID = element.GetString("_id");
                    requestData.RequestStatus = element.GetInt("status");
                    
                    var fromData = element.GetJSON("fromData");
                    
                    requestData.UserID = fromData.GetString("userId");
                    requestData.Name = fromData.GetString("name");
                    requestData.Stars = fromData.GetInt("stars");
                    requestData.IsLegend = fromData.GetBool("isLegend");
                    requestData.RankType = fromData.GetString("rankType");
                    requestData.RankValue = fromData.GetInt("rankValue");
                    requestData.ExpPoints = fromData.GetInt("expPoints");
                    if (fromData.ContainsKey("avtarUrl"))
                        requestData.Avatar = fromData.GetString("avtarUrl");
                    
                    mm.RequestDatas.Add(requestData);
                    Debug.Log($"{requestData.Name} {requestData.UserID}");
                }
                mm.IsNotification();
                isReady = true;
                mm.IsRequestUserLoaded = true;
                StartCoroutine(GetFriends());
                Debug.Log("Get friend requests.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetFriendsRequest(){
        StartCoroutine(GetFriendRequests());
    }

    IEnumerator GetFriends()
    {
        Debug.Log("Get friends...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_FRIEND_FETCH) + "?filter=2");
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                JArray data = jSON.GetJArray("data");
                
                for (int i = 0; i < data.Length; i++)
                {
                    FriendData friend = new FriendData();
                    var element = data.GetJSON(i);
                    friend.ID = element.GetString("userId");
                    friend.Name = element.GetString("name");
                    if (element.ContainsKey("isOnline"))
                    friend.IsStatus = element.GetBool("isOnline");
                    friend.Stars = element.GetInt("stars");
                    friend.IsLegend = element.GetBool("isLegend");
                    friend.RankType = element.GetString("rankType");
                    friend.RankValue = element.GetInt("rankValue");
                    friend.ExpPoints = element.GetInt("expPoints");
                    if (element.ContainsKey("avtarUrl"))
                    friend.Avatar = element.GetString("avtarUrl");
                    
                    mm.FriendDatas.Add(friend);
                    Debug.Log($"{friend.Name} {friend.Avatar}");
                }
                isReady = true;
                mm.IsFriendLoaded = true;
                mm.IsNotification();
                Debug.Log("Get friend requests.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetUserFriends(){
        StartCoroutine(GetFriends());
    }
    
    public delegate void RemoveFriendEvent(string ID);
    public static event RemoveFriendEvent OnRemoveFriendEvent;
    
    IEnumerator RemoveFromFriend(string ID)
    {
        Debug.Log("Remove friend request...");
        
        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("friendId", ID);
            
            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_FRIEND_REMOVE), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                OnRemoveFriendEvent?.Invoke(ID);
                Debug.Log("Remove friend request.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void RemoveFriend(string ID){
        StartCoroutine(RemoveFromFriend(ID));
    }
    
    public delegate void AddBlackListEvent(string ID);
    public static event AddBlackListEvent OnAddBlackList;
    
    IEnumerator AddUserBlackList(string ID)
    {
        Debug.Log("Add user blacklist..." + ID);
        
        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", ID);
            
            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_BLACKLIST_ADD), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                OnAddBlackList?.Invoke(ID);
                Debug.Log("Add user blacklist.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void AddBlackList(string userID){
        StartCoroutine(AddUserBlackList(userID));
    }
    
    IEnumerator GetBlackListUsers()
    {
        Debug.Log("Get blacklist user...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_BLACKLIST_FETCH));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                var data = jSON.GetJArray("data");

                for (int i = 0; i < data.Length; i++)
                {
                    var element = data.GetJSON(i);
                    BlackListUserData blackListUser = new BlackListUserData();
                    blackListUser.Name = element.GetString("name");
                    blackListUser.ID = element.GetString("_id");
                    blackListUser.UserID = element.GetString("userId");
                    if (element.ContainsKey("avtarUrl"))
                        blackListUser.Avatar = element.GetString("avtarUrl");
                    blackListUser.BlockedDate = DateTime.Parse(element.GetString("blockedDate"));
                    
                    mm.BlackListData.Add(blackListUser);
                    Debug.Log("ID:[" + element.GetString("_id") + "] Name[" + element.GetString("name") +"]");   
                }
                isReady = true;
                mm.IsBlackListLoaded = true;
                Debug.Log("Get blacklist user requests.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }
    
    public delegate void RemoveFromBlackListEvent(string ID);
    public static event RemoveFromBlackListEvent OnRemoveBlackList;
    
    IEnumerator RemoveFromBlackList(string ID)
    {
        Debug.Log("Remove friend request...");
        
        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", ID);
            
            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_BLACKLIST_REMOVE), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                OnRemoveBlackList?.Invoke(ID);
                Debug.Log("Remove friend request.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void RemoveBlackList(string userID){
        StartCoroutine(RemoveFromBlackList(userID));
    }
    
    IEnumerator GetChatHistoryUser(string ID, int countSkip)
    {
        Debug.Log("Get chat history user...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www =
                UnityWebRequest.Get(string.Concat(serverUri, API_HISTORYCHAT_USER) + "?toUserId=" + ID + "&skip=" + countSkip);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                mm.HistoryUser.Clear();
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                var data = jSON.GetJArray("data");

                for (int i = 0; i < data.Length; i++)
                {
                    var element = data.GetJSON(i);
                    MessageChat msg = new MessageChat();
                    msg.ID = element.GetString("_id");
                    msg.Message = element.GetString("message");
                    msg.FromUserID = element.GetString("fromUserId");
                    DateTime date = DateTime.Parse(element.GetString("time"));
                    msg.Date = date;

                    mm.HistoryUser.Add(msg);
                   // Debug.Log("ID:[" + msg.ID + "] Message[" + msg.Message +"]");   
                }
                mm.IsHistoryLoaded = true;
                Debug.Log("Get chat history user... Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetHistoryChatUser(string ID, int countSkip){
        StartCoroutine(GetChatHistoryUser(ID, countSkip));
    }
    
    IEnumerator GetLatestDialogs()
    {
        Debug.Log("Get the latest dialogs...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_LATEST_DIALOGS));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                mm.DialogUserDatas.Clear();
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                var data = jSON.GetJArray("data");

                for (int i = 0; i < data.Length; i++)
                {
                    var element = data.GetJSON(i);
                    DialogUserData dialogUserData = new DialogUserData();
                    UnseenMessages unseen = new UnseenMessages();
                    
                    var friendInfo = element.GetJSON("friendInfo");
                    dialogUserData.Name = friendInfo.GetString("name");
                    dialogUserData.RankValue = friendInfo.GetInt("rankValue");
                    dialogUserData.RankType = friendInfo.GetString("rankType");
                    dialogUserData.ID = friendInfo.GetString("userId");
                    if (friendInfo.ContainsKey("isLegend"))
                        dialogUserData.IsLegend = friendInfo.GetBool("isLegend");
                    dialogUserData.IsBlocked = element.GetBool("isBlocked");
                    dialogUserData.IsBlockedYou = element.GetBool("isBlockedYou");

                    unseen.ID = dialogUserData.ID;
                    if (element.ContainsKey("unseenMessages"))
                        unseen.CountMessage = element.GetInt("unseenMessages");
                    if (friendInfo.ContainsKey("avtarUrl"))
                    dialogUserData.Avatar = friendInfo.GetString("avtarUrl");
                    var lastMessage = element.GetJSON("lastMessage");
                    dialogUserData.Message = lastMessage.GetString("text");
                    DateTime date = DateTime.Parse(lastMessage.GetString("createdAt"));
                    dialogUserData.CreatedAt = date;
                    
                    mm.DialogUserDatas.Add(dialogUserData);
                    mm.UnseenMessageses.Add(unseen);
                    
                    Debug.Log("ID:[" + dialogUserData.ID + "] Message[" + dialogUserData.Message +"]");   
                }
                mm.IsNotification();
                mm.IsLastDialogLoaded = true;
                isReady = true;
                Debug.Log("Get the latest dialogs... Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    private IEnumerator ListOfUsers(string search){
        Debug.Log("Get list of users...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_LIST_USERS) + "?search=" + search);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                mm.Users.Clear();
                
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                var data = jSON.GetJArray("data");

                for (int i = 0; i < data.Length; i++)
                {
                    var element = data.GetJSON(i);
                    
                    RequestData user = new RequestData();
                    user.UserID = element.GetString("userId");
                    
                    user.Name = element.GetString("name");
                    if (element.ContainsKey("avtarUrl"))
                        user.Avatar = element.GetString("avtarUrl");
                    user.RankValue = element.GetInt("rankValue");
                    user.RequestStatus = element.GetInt("requestStatus");
                    user.IsBlocked = element.GetBool("isBlocked");
                    user.IsBlockedYou = element.GetBool("isBlockedYou");
                    user.RankValue = element.GetInt("rankValue");
                    mm.Users.Add(user);
                    
                    //Debug.Log("ID:[" + element.GetString("userId") + "] Name[" + element.GetString("name") +"]");   
                }
                mm.IsListOfUsers = true;
                Debug.Log("Get list of users.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetListOfUsers(string search){
        StartCoroutine(ListOfUsers(search));
    }

    public void UploadFile(Texture2D tex){
        StartCoroutine(UploadFileFS(tex));
    }

    private IEnumerator UploadFileFS(Texture2D tex){
        Debug.Log("Upload file...");

        if (!string.IsNullOrEmpty(Token)){
            byte[] myData = tex.EncodeToPNG();
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", myData);
         
            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_UPLOAD_FILE), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                StartCoroutine(UploadAvatar(jSON.GetString("fileUrl")));
                Debug.Log("Upload file.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public delegate void UploadDAvatar(string url);
    public static event UploadDAvatar OnUploadAvatar;
    
    private IEnumerator UploadAvatar(string avatar){
        Debug.Log("Upload avatar...");

        if (!string.IsNullOrEmpty(Token)){
            
            WWWForm form = new WWWForm();
            form.AddField("avtarUrl", avatar);
         
            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_UPLOAD_AVATAR), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                Debug.Log(jSON.GetInt("statusCode"));
                Debug.Log(jSON.GetString("msg"));
                Debug.Log(jSON.GetBool("status"));
                OnUploadAvatar?.Invoke(avatar);
                Debug.Log("Upload avatar.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetPremiumRequest(){
        StartCoroutine(GetPremium());
    }
    
    private IEnumerator GetPremium()
    {
        Debug.Log("Get premium request...");
        
        if (!string.IsNullOrEmpty(Token))
        {
            WWWForm form = new WWWForm();

            UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_GET_PREMIUM), form);
            www.SetRequestHeader(REQUEST_HEADER, Token);
            yield return www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else{
                MainRoot.Instance.mainModel.IsPremium = true;
                Debug.Log("Get premium request.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    /*IEnumerator FetchOpenedRooms()
    {
        Debug.Log("Fetch opened rooms...");

        UnityWebRequest www = UnityWebRequest.Get(string.Concat(SERVER_URI, API_ROOM_FETCH_OPENED));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            JSON jSON = JSON.ParseString(www.downloadHandler.text);
            JArray data = jSON.GetJArray("data");
            JSON element = data.GetJSON(0);
            OpenedRoomId = element.GetString("_id");

            Debug.Log("Fetch opened rooms.. Success");
        }
    }*/
    public void GetInfoOfMultipleUsers(string[] userIds)
    {
        StartCoroutine(GetInfoOfMultipleUsersCor(userIds));
    }

    IEnumerator GetInfoOfMultipleUsersCor(string [] userIds)
    {
        Debug.Log("GetInfoOfMultipleUsers...");

        WWWForm form = new WWWForm();
        string arr = "[";
        for (int i=0; i<userIds.Length - 1; i++)
        {
            arr += ("\"" + userIds[i] + "\",");
        }
        arr += ("\"" + userIds[userIds.Length - 1] + "\"]");
        form.AddField("userIds", arr);
        UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_INFO_OF_MULTIPLE_USERS), form);
        www.SetRequestHeader(REQUEST_HEADER, Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {

            JSON jSON = JSON.ParseString(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
            JArray data = jSON.GetJArray("data");
            FriendData[] users = new FriendData[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var element = data.GetJSON(i);
                FriendData user = new FriendData();
                user.ID = element.GetString("_id");
                user.RankValue = element.GetInt("rankValue");
                user.Name = element.GetString("name");
                user.ExpPoints = element.GetInt("expPoints");
                user.Stars = element.GetInt("stars");
                user.IsLegend = element.GetBool("isLegend");
                user.RankType = element.GetString("rankType");
                //Debug.Log(element.GetString("rankName"));
                users[i] = user;
            }
            Debug.Log("GetInfoOfMultipleUsers.. Success");
            ServiceIO.Instance.PlayersInfoUpdate(users);
        }
    }

    public delegate void RenameError();
    public static event RenameError OnRenameError;
    
    public void Rename(string name){
        StartCoroutine(RenameUser(name));
    }
    
    IEnumerator RenameUser(string name)
    {
        Debug.Log("Rename user...");

        if (!string.IsNullOrEmpty(Token))
        {
            JSON jSON = new JSON();
            if (!string.IsNullOrEmpty(name))  jSON.Add("name", name);

            UnityWebRequest www = UnityWebRequest.Put(string.Concat(serverUri, API_USER_RENAME), jSON.CreateString());
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                OnRenameError?.Invoke();
                Debug.LogError(www.error);
            }
            else
            {
                MainModel mm = MainRoot.Instance.mainModel;
                mm.UserName = name;
                
                Debug.Log("Rename user.. Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

    public void GetItems(){
        StartCoroutine(GetItemsShop());
    }
    
    IEnumerator GetItemsShop()
    {
        Debug.Log("Get Items...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_ITEM_GET_ITEMS));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                //Debug.Log(www.downloadHandler.text);
                JArray data = jSON.GetJArray("data");
                
                MainModel mm = MainRoot.Instance.mainModel;
                
                for (int i = 0; i < data.Length; i++)
                {
                    Item item = new Item();
                    var element = data.GetJSON(i);
                    item.ID = element.GetString("_id");
                    item.Name = element.GetString("name");
                    item.Description = element.GetString("description");
                    item.Picture = element.GetString("pictureUrl");
                    item.CurrencyType = element.GetInt("currencyType");
                    if (element.ContainsKey("category"))
                    item.Category = element.GetInt("category");
                    
                    if (element.ContainsKey("raretyCategory"))
                        item.RarityCategory = element.GetInt("raretyCategory");
                    
                    JArray prices = element.GetJArray("price");

                    for (int j = 0; j < prices.Length; j++){
                        Item.PriceItem price = new Item.PriceItem();
                        var p = prices.GetJSON(j);
                        price.ID = p.GetString("_id");
                        if (p.ContainsKey("price"))
                        price.Price = p.GetInt("price");
                        price.Quantity = p.GetInt("quantity");
                        item.Price.Add(price);
                    }

                    if (element.ContainsKey("discountDetails")){
                        var discountsDetails = element.GetJSON("discountDetails");

                        Item.DiscountDetails discount = new Item.DiscountDetails();

                        discount.PercentageValue = discountsDetails.GetInt("percentageValue");
                        discount.ValidFrom = DateTime.Parse(discountsDetails.GetString("validFrom"));
                        discount.ValidTill = DateTime.Parse(discountsDetails.GetString("validTill"));
                        item.Discount = discount;
                    }

                    if (element.ContainsKey("itemStatus") && element.GetInt("itemStatus") == 1){
                        mm.ShopItems.Add(item);
                    }

                    //Debug.Log($"{item.Name} {item.ID}");
                }
                isReady = true;
                mm.IsItemShopLoaded = true;
                Debug.Log("Get Items... Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }
    
    
    IEnumerator GetRankTypeInfo()
    {
        Debug.Log("Get Rank info...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_RANK_INFO));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                //Debug.Log(www.downloadHandler.text);
                JArray data = jSON.GetJArray("data");
                
                MainModel mm = MainRoot.Instance.mainModel;
                
                for (int i = 0; i < data.Length; i++){
                    var element = data.GetJSON(i);
                    RankInfo rank = new RankInfo();
                    
                    rank.ID = element.GetString("_id");
                    var safeZone = element.GetJArray("safeZone");

                    rank.SafeZone = new List<int>();
                    foreach (var zone in safeZone.Values){
                        rank.SafeZone.Add(int.Parse(zone.CreateString()));
                    }

                    rank.IsDefault = element.GetBool("isDefault");
                    rank.Name = element.GetString("name");
                    rank.minRank = element.GetInt("minRank");
                    rank.maxRank = element.GetInt("maxRank");
                    rank.WinGamesStars = element.GetInt("winGameStars");
                    rank.LoseGamesStars = element.GetInt("loseGameStars");
                    rank.LeaveGameStars = element.GetInt("leaveGameStars");
                    rank.StarsForNextRank = element.GetInt("starsForNextRank");

                    mm.RankInfos.Add(rank);
                    //Debug.Log($"{rank.minRank} {rank.maxRank}");
                }
        
                if (mm.Rank != 0){
                    mm.RankName = mm.RankInfos.Find(x => x.minRank <= mm.Rank && x.maxRank >= mm.Rank).Name;
                    mm.StarsForNextRank = mm.RankInfos.Find(x => x.minRank <= mm.Rank && x.maxRank >= mm.Rank)
                        .StarsForNextRank;
                }

                isReady = true;
                mm.IsRankInfoLoaded = true;
                Debug.Log("Get Rank info... Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }
    
    
      IEnumerator GetLootBoxShop()
    {
        Debug.Log("Get LootBox...");

        if (!string.IsNullOrEmpty(Token))
        {
            UnityWebRequest www = UnityWebRequest.Get(string.Concat(serverUri, API_LOOTBOX));
            www.SetRequestHeader(REQUEST_HEADER, Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                JSON jSON = JSON.ParseString(www.downloadHandler.text);
                //Debug.Log(www.downloadHandler.text);
                JArray data = jSON.GetJArray("data");
                
                MainModel mm = MainRoot.Instance.mainModel;
                
                for (int i = 0; i < data.Length; i++)
                {
                    LootBoxItem item = new LootBoxItem();
                    
                    var element = data.GetJSON(i);
                    item.ID = element.GetString("_id");
                    item.Title = element.GetString("title");
                    item.Description = element.GetString("description");
                    item.ItemCount = element.GetInt("itemCount");
                    item.CurrencyType = element.GetInt("currencyType");
                    item.ItemCount = element.GetInt("itemCount");
                    
                    JArray prices = element.GetJArray("price");

                    for (int j = 0; j < prices.Length; j++){
                        LootBoxItem.Price price = new  LootBoxItem.Price();
                        var p = prices.GetJSON(j);
                        price.ID = p.GetString("_id");
                        if (p.ContainsKey("amount"))
                        price.Amount = p.GetInt("amount");
                        price.Quantity = p.GetInt("quantity");
                        
                        item.Prices.Add(price);
                    }
                    
                    JArray itemChances = element.GetJArray("itemChances");

                    for (int j = 0; j < itemChances.Length; j++){
                        LootBoxItem.ItemChance chance = new  LootBoxItem.ItemChance();
                        var p = itemChances.GetJSON(j);
                        chance.ID = p.GetString("_id");
                        chance.RaretyCategory = p.GetInt("raretyCategory");
                        chance.Percentange = p.GetInt("percentage");
                        item.ItemChances.Add(chance);
                    }
                    
                    mm.ShopLootBox.Add(item);
                    //Debug.Log($"{item.Name} {item.ID}");
                }
                isReady = true;
                mm.IsChestShopLoaded = true;
                Debug.Log("Get LootBox... Success");
            }
        }
        else
        {
            Debug.LogError("Not authorized");
        }
    }

      public delegate void BuyItemEvent(string ID, int price, float count);
      public static event BuyItemEvent OnBuyItem;
      
      public void BuyItemShop(string itemID, int count, float price){
          StartCoroutine(BuyItem(itemID, count, price));
      }
      
      private IEnumerator BuyItem(string itemID, int count, float price)
      {
          Debug.Log("Buy item...");
        
          if (!string.IsNullOrEmpty(Token))
          {
              WWWForm form = new WWWForm();

             
              form.AddField("itemId", itemID);
              form.AddField("quantity", count);
              
              
              UnityWebRequest www = UnityWebRequest.Post(string.Concat(serverUri, API_ITEM_BUY), form);
              www.SetRequestHeader(REQUEST_HEADER, Token);
              yield return www.SendWebRequest();
              
              if (www.isNetworkError || www.isHttpError)
              {
                  Debug.LogError(www.error);
              }
              else{
                  OnBuyItem?.Invoke(itemID, count, price);
                  Debug.Log("Buy item... Success");
              }
          }
          else
          {
              Debug.LogError("Not authorized");
          }
      }
}
public enum LoginTypes
{
    Local,
    External,
    Emulator,
    Phone
}