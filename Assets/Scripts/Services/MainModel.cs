using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainModel
{
    public delegate void MainModelEvent();
    public static event MainModelEvent OnSoftCurrencyAmountChanged, OnHardCurrencyAmountChanged
        , OnUserNameChanged, OnStarsChanged, OnRankChanged
        , OnFullNameChanged, OnAgeChanged, OnCityChanged
        , OnEmailChanged, OnFriendLoaded, OnBlackListLoaded
        , OnAvatarLoaded, OnHistoryChatLoaded, OnLastDialogLoaded
        , OnReceiveMessage, OnChangeStatus, OnListOfUsersLoaded
        , OnRequestUserLoaded, OnAcceptedFriendship, OnNewFriendRequest
        , OnProfileLoaded, OnOtherProfileLoaded, OnPremiumChanged, OnActiveGameLoaded, OnRoomIdChanged, OnGameSearchBlockTimeLoaded
        , OnItemShopLoaded, OnRankInfoLoaded, OnRankNameChanged, OnRankTypeChanged, OnStarsForNextRankChanged, OnChestShopLoaded;

    public string Id;

    private string userName;
    public string UserName 
    {
        get 
        {
            return userName;
        }
        set
        {
            userName = value;
            OnUserNameChanged?.Invoke();
        } 
    }

    private string fullName;
    public string FullName 
    {
        get
        {
            return fullName;
        }
        set
        {
            fullName = value;
            OnFullNameChanged?.Invoke();
        } 
    }
    private int age;
    public int Age 
    {
        get
        {
            return age;
        }
        set
        {
            age = value;
            OnAgeChanged?.Invoke();
        }
    }
    
    private int _countRename;
    public int CountRename 
    {
        get
        {
            return _countRename;
        }
        set
        {
            _countRename = value;
        }
    }
    
    private string city;
    public string City 
    {
        get
        {
            return city;
        }
        set
        {
            city = value;
            OnCityChanged?.Invoke();
        }
    }
    private string email;
    public string Email
    {
        get
        {
            return email;
        }
        set
        {
            email = value;
            OnEmailChanged?.Invoke();
        }
    }

    private string _avatar;
    public string Avatar 
    {
        get => _avatar;
        set
        {
            _avatar = value;
            OnAvatarLoaded?.Invoke();
        } 
    }
    
    private int rank;
    public int Rank 
    {
        get
        {
            return rank;
        }
        set
        {
            rank = value;
            OnRankChanged?.Invoke();
        }
    }
    
    private string _rankName;
    public string RankName 
    {
        get
        {
            return _rankName;
        }
        set
        {
            _rankName = value;
            OnRankNameChanged?.Invoke();
        }
    }
    
    private string _rankType;
    public string RankType
    {
        get
        {
            return _rankType;
        }
        set
        {
            _rankType = value;
            OnRankTypeChanged?.Invoke();
        }
    }
    
    private int stars;
    public int Stars 
    {
        get
        {
            return stars;
        }
        set
        {
            stars = value;
            OnStarsChanged?.Invoke();
        }
    }
    
    private int _starsForNextRank;
    public int StarsForNextRank 
    {
        get
        {
            return _starsForNextRank;
        }
        set
        {
            _starsForNextRank = value;
            OnStarsChanged?.Invoke();
        }
    }

    private bool _isFriendLoaded;
    public bool IsFriendLoaded
    {
        get => _isFriendLoaded;
        set
        {
            _isFriendLoaded = value;
            OnFriendLoaded?.Invoke();
        }
    }
    
    private bool _isRequestUserLoaded;
    public bool IsRequestUserLoaded
    {
        get => _isRequestUserLoaded;
        set
        {
            _isRequestUserLoaded = value;
            OnRequestUserLoaded?.Invoke();
        }
    }
    
    private bool _isHistoryLoaded;
    public bool IsHistoryLoaded
    {
        get => _isHistoryLoaded;
        set
        {
            _isHistoryLoaded = value;
            OnHistoryChatLoaded?.Invoke();
        }
    }
    
    private bool _isPremium;
    public bool IsPremium
    {
        get => _isPremium;
        set
        {
            _isPremium = value;
            OnPremiumChanged?.Invoke();
        }
    }
    
    private bool _isBlackListLoaded;
    public bool IsBlackListLoaded
    {
        get => _isBlackListLoaded;
        set
        {
            _isBlackListLoaded = value;
            OnBlackListLoaded?.Invoke();
        }
    }
    
    private bool _isLastDialogLoaded;
    public bool IsLastDialogLoaded
    {
        get => _isLastDialogLoaded;
        set
        {
            _isLastDialogLoaded = value;
            OnLastDialogLoaded?.Invoke();
        }
    }

    private bool _isActiveGame;
    public bool IsActiveGame
    {
        get => _isActiveGame;
        set
        {
            _isActiveGame = value;
            OnActiveGameLoaded?.Invoke();
        }
    }

    private bool _isItemShopLoaded;
    public bool IsItemShopLoaded
    {
        get => _isItemShopLoaded;
        set
        {
            _isItemShopLoaded = value;
            OnItemShopLoaded?.Invoke();
        }
    }

    private bool _isChestShopLoaded;
    public bool IsChestShopLoaded
    {
        get => _isChestShopLoaded;
        set
        {
            _isChestShopLoaded = value;
            OnChestShopLoaded?.Invoke();
        }
    }
    
    private bool _isRankInfoLoaded;
    public bool IsRankInfoLoaded
    {
        get => _isRankInfoLoaded;
        set
        {
            _isRankInfoLoaded = value;
            OnRankInfoLoaded?.Invoke();
        }
    }
    
    
    private string _roomId;
    public string RoomId
    {
        get => _roomId;
        set
        {
            _roomId = value;
            OnRoomIdChanged?.Invoke();
        }
    }

    private DateTime _gameSearchBlockTime;
    public DateTime GameSearchBlockTime
    {
        get => _gameSearchBlockTime;
        set
        {
            _gameSearchBlockTime = value;
            OnGameSearchBlockTimeLoaded?.Invoke();
        }
    }
    private MessageData _currentMessage;
    public MessageData CurrentMessage{
        get => _currentMessage;
        set{
            _currentMessage = value;
            OnReceiveMessage?.Invoke();
        }
    }

    private UserData _status;
    public UserData ChangeStatus{
        get => _status;
        set{
            _status = value;
            OnChangeStatus?.Invoke();
        }
    }
    
    private bool _isListOfUsers;
    public bool IsListOfUsers
    {
        get => _isListOfUsers;
        set
        {
            _isListOfUsers = value;
            OnListOfUsersLoaded?.Invoke();
        }
    }
    
    private FriendRequest acceptedFriendship;
    public FriendRequest AcceptedFriendship
    {
        get => acceptedFriendship;
        set
        {
            acceptedFriendship = value;
            OnAcceptedFriendship?.Invoke();
        }
    }

    private RequestData _newFriendRequest;
    public RequestData NewFriendRequest{
        get{ return _newFriendRequest; }
        set{
            _newFriendRequest = value;
            OnNewFriendRequest?.Invoke();
        }
    }

    private bool _isLegend;

    public bool IsLegend{
        get{
            return _isLegend;
        } 
        set{
            if (value) Rank = 0;
            _isLegend = value;
        }
    }
    public int LegendRank { get; set; }

    // Friends
    private List<Friend> friends;

    private int _softCurrency;
    public int SoftCurrency
    {
        get{
            return _softCurrency;
        }
        set
        {
            _softCurrency = value;
            OnSoftCurrencyAmountChanged?.Invoke();
        }
    }
    private int _hardCurrency;
    public int HardCurrency
    {
        get
        {
            return _hardCurrency;
        }
        set
        {
            _hardCurrency = value;
            OnHardCurrencyAmountChanged?.Invoke();
        }
    }
    
    private UserProfileData _userProfileData = new UserProfileData();
    public UserProfileData UserProfileData{
        get => _userProfileData;
        set{
            _userProfileData = value;
            OnProfileLoaded?.Invoke();
        }
    }
    
    private OtherProfileData _otherProfileData = new OtherProfileData();
    public OtherProfileData OtherProfile{
        get => _otherProfileData;
        set{
            _otherProfileData = value;
            OnOtherProfileLoaded?.Invoke();
        }
    }

    public List<MessageChat> HistoryUser = new List<MessageChat>();
    public List<FriendData> FriendDatas = new List<FriendData>();
    public List<BlackListUserData> BlackListData = new List<BlackListUserData>();
    public List<DialogUserData> DialogUserDatas = new List<DialogUserData>();
    public List<UnseenMessages> UnseenMessageses = new List<UnseenMessages>();
    public List<Item> ShopItems = new List<Item>();
    public List<LootBoxItem> ShopLootBox = new List<LootBoxItem>();
    public List<RankInfo> RankInfos = new List<RankInfo>();
    
    public delegate void Notification(bool isNotification);
    public static event Notification OnNotification;
    public void IsNotification(){
        foreach (var message in UnseenMessageses){
            if (message.CountMessage > 0){
                OnNotification?.Invoke(true);
                return;
            }
        }

        if (RequestDatas != null && RequestDatas.Count > 0){
            OnNotification?.Invoke(true);
            return;
        }

        //Debug.Log(UnseenMessageses.Count + " " + RequestDatas.Count);
        OnNotification?.Invoke(false);
    }
    
    public List<RequestData> Users = new List<RequestData>();
    public List<RequestData> RequestDatas = new List<RequestData>();

    public MainModel()
    {

    }
}

public class FriendData : UserData
{
    public int Stars;
    public int ExpPoints;
}

public class BlackListUserData : UserData
{
    public string UserID;
    public int ExpPoints;
    public DateTime BlockedDate;
}

public class DialogUserData : UserData{
    public DateTime CreatedAt;
}

public class UserData
{
    public string ID;
    public string Name;
    public bool IsStatus;
    public string Avatar;
    public int RankValue;
    public string RankName;
    public string Message;
    public bool IsBlocked;
    public bool IsBlockedYou;
    public string RankType;
    public bool IsLegend;
}

public class UnseenMessages{
    public string ID;
    public int CountMessage;
}
[Serializable]
public class RequestData : FriendData{
    public int RequestStatus;
    public string UserID;
}

public class UserProfileData : UserData{
    public int Stars;
    public int TotalGamesPlayed;
    public int TotalGameWon;
    public int TotalGameLost;
    public int TotalGameLeaved;
    public int TotalNakedWin;
    public int TotalFlawlessWin;
    public string RankName;
    public int StarsForNextRank;
}

public class OtherProfileData : UserProfileData{
    public int Golds;
    public int Chips;
    public bool IsFriend;
    public bool IsBlockedYou;
    public bool IsBlocked;
}

public class FriendRequest{
    public string UserID;
    public string RequestID;
    public int RequestStatus;
}

public class Item{
    public string ID;
    public string Name;
    public string Description;
    public string Picture;
    public int Category;
    public int CurrencyType;
    public int RarityCategory;
    public List<PriceItem> Price = new List<PriceItem>();
    public DiscountDetails Discount;
    
    public class PriceItem{
        public string ID;
        public int Price;
        public int Quantity;
     
    }

    public class DiscountDetails{
        public int PercentageValue;
        public DateTime ValidFrom;
        public DateTime ValidTill;
    }
}

public class LootBoxItem{
    public string ID;
    public string Title;
    public int ItemCount;
    public string Description;
    public int CurrencyType;
    public string Picture;
    public List<Price> Prices = new List<Price>();
    public List<ItemChance> ItemChances = new List<ItemChance>();
    
    public class Price{
        public string ID;
        public int Amount;
        public int Quantity;
    }
    
    public class ItemChance{
        public string ID;
        public int RaretyCategory;
        public int Percentange;
    }
}

public class RankInfo{
    public string ID;
    public List<int> SafeZone;
    public bool IsDefault;
    public string Name;
    public int minRank;
    public int maxRank;
    public int WinGamesStars;
    public int LoseGamesStars;
    public int LeaveGameStars;
    public int StarsForNextRank;
}