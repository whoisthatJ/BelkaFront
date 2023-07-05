using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendView : MonoBehaviour{
    public static FriendView Instance;

    public OtherProfileData Data;
    public string FriendsID{ get; set; }
    
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Image level;
    [SerializeField] private Image _avatar;
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private TextMeshProUGUI nameUserTxt;
    [SerializeField] private TextMeshProUGUI countGamesTxt;
    [SerializeField] private StatsItem _countWinGamesTxt;
    [SerializeField] private StatsItem _nakeWinsTxt;
    [SerializeField] private StatsItem _flawlessTxt;
    [SerializeField] private TextMeshProUGUI statisticTxt;
    [SerializeField] private Button addFriendBtn;
    [SerializeField] private Button _removeFriend;
    [SerializeField] private Button writeMessageBtn;
    [SerializeField] private Button _blackListBtn;
    [SerializeField] private Image _blackList;
    [SerializeField] private TextMeshProUGUI _blackListTxt;
    [SerializeField] private GameObject giftItemPrefab;
    [SerializeField] private StatsOtherProfile _statsPanel;
    [SerializeField] private Image _lockInfo;
    [SerializeField] private Image _lockInfoImage;
    [SerializeField] private TextMeshProUGUI _lockBigInfo;
    [SerializeField] private Sprite _lockIcon;
    [SerializeField] private Sprite _blackListIcon;
    [SerializeField] private Sprite _unlockIcon;
    [SerializeField] private Sprite _blackListIconPremium;
    [SerializeField] private Sprite _youBlocked;
    [SerializeField] private Sprite _emptyAvatar;
    [SerializeField] private Sprite _whiteBtn;
    [SerializeField] private Image _userImage;
    [SerializeField] private Image _opponentImage;
    [SerializeField] private TextMeshProUGUI _statsVS;
    [SerializeField] private Image _shield;
    [SerializeField] private Image _legend;
    
    private bool _isBlocked;
    private Vector2 _startPosLevel;
    private Vector2 _startPosLegend;
    
    private void Awake(){
        Instance = this;
    }

    private void Start(){
        _startPosLevel = level.GetComponent<RectTransform>().localPosition;
        _startPosLegend = _legend.GetComponent<RectTransform>().localPosition;
    }

    private void OnEnable(){
        closeBtn.onClick.AddListener(Close);
        writeMessageBtn.onClick.AddListener(WriteMessage);
        _blackListBtn.onClick.AddListener(LockUser);
        MenuBottomBar.BottomBarButtonPressed += CloseEverything;
        MainModel.OnOtherProfileLoaded += LoadInfo;
        addFriendBtn.onClick.AddListener(AddFriend);
        _removeFriend.onClick.AddListener(RemoveFriend);
        MainModel.OnAcceptedFriendship += AcceptedFriendship;
        ServiceWeb.OnRemoveFriendEvent += RemoveFriendEvent;
        ServiceIO.OnRemoveFromFriendReceive += RemoveFromFriendReceive;
        ServiceIO.OnRemoveBlackListReceive += RemoveFromBlackListReceive;
        ServiceIO.OnAddToBlackListReceive += AddToBlackListReceive;
        MainModel.OnNewFriendRequest += NewFriendRequest;
        MainModel.OnPremiumChanged += BuyPremiumReceive;
    }

    private void OnDisable(){
        closeBtn.onClick.RemoveAllListeners();
        writeMessageBtn.onClick.RemoveAllListeners();
        _blackListBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= CloseEverything;
        MainModel.OnOtherProfileLoaded -= LoadInfo;
        addFriendBtn.onClick.RemoveAllListeners();
        _removeFriend.onClick.RemoveAllListeners();
        ServiceWeb.OnRemoveFriendEvent -= RemoveFriendEvent;
        ServiceIO.OnRemoveFromFriendReceive -= RemoveFromFriendReceive;
        ServiceIO.OnRemoveBlackListReceive -= RemoveFromBlackListReceive;
        ServiceIO.OnAddToBlackListReceive -= AddToBlackListReceive;
        MainModel.OnNewFriendRequest -= NewFriendRequest;
        MainModel.OnPremiumChanged -= BuyPremiumReceive;
    }

    private void CloseEverything(){
        panel.SetActive(false);
        Friends.Instance.Close();
    }


    public void Open(string ID){
        //MainRoot.Instance.mainModel.IsPremium = true;
        ServiceWeb.Instance.GetOtherProfile(ID);
        panel.SetActive(true);
        gameObject.GetComponent<Canvas>().sortingOrder = 12;
    }

    public void AcceptAddedFriend(bool isAccept){
        EnableAddFriend(isAccept);
    }

    private void LoadInfo(){
        var mm = MainRoot.Instance.mainModel;
        _legend.transform.localPosition = _startPosLegend;
        level.transform.localPosition = _startPosLevel;
        Data = mm.OtherProfile;
        
        Data.RankName = mm.RankInfos.Find(x => x.minRank <= Data.RankValue && x.maxRank >= Data.RankValue).Name;
        Data.StarsForNextRank = mm.RankInfos.Find(x => x.minRank <= Data.RankValue && x.maxRank >= Data.RankValue).StarsForNextRank;
        
        nameUserTxt.text = Data.Name;
        nameUserTxt.alignment = TextAlignmentOptions.Left;
        levelTxt.text = Data.RankValue.ToString();
        countGamesTxt.text = "Кол-во игр: " + "<b>" + Data.TotalGamesPlayed + "</b>";
        _isBlocked = Data.IsBlocked;

        Sprite sp = Resources.Load<Sprite>("Shields/" + Data.RankName);
        
        if (Data.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
        levelTxt.color = HardCodeValue.GetColorShieldTitle(Data.RankName, Data.IsLegend);        
        
        if (Data.IsFriend){
            _removeFriend.gameObject.SetActive(true);
            writeMessageBtn.gameObject.SetActive(true);
        }
        else{
            addFriendBtn.gameObject.SetActive(true);
            writeMessageBtn.gameObject.SetActive(true);
        }

        if (_isBlocked){
            _lockInfo.gameObject.SetActive(true);
            _removeFriend.gameObject.SetActive(false);
            addFriendBtn.gameObject.SetActive(false);
            writeMessageBtn.gameObject.SetActive(false);
            _lockInfoImage.sprite = _lockIcon;
            _lockInfoImage.SetNativeSize();
            _lockBigInfo.text = "Игрок в черном списке";
            _blackList.sprite = _unlockIcon;
            _blackListTxt.text = "Разблокировать";
        }
        else{
            _blackList.sprite = _lockIcon;
            _blackListTxt.text = "Блокировать";
            writeMessageBtn.gameObject.SetActive(true);
        }
        
        if (mm.IsPremium){
            _blackList.sprite = _blackListIconPremium;
        }
        else{
            _blackList.sprite = _blackListIcon;
        }

        //Если ты заблокирован
        if (Data.IsBlockedYou){
            _lockInfo.gameObject.SetActive(true);
            _removeFriend.gameObject.SetActive(false);
            addFriendBtn.gameObject.SetActive(false);
            writeMessageBtn.gameObject.SetActive(false);
            if (!_isBlocked){
                _lockInfoImage.sprite = _youBlocked;
                _lockInfoImage.SetNativeSize();
                _lockBigInfo.text = "Игрок заблокировал вас";
            }
        }
        
        _blackList.SetNativeSize();

        ProfileStats stats = new ProfileStats();
        stats.TotalGamesPlayed = Data.TotalGamesPlayed;
        stats.TotalFlawlessWin = Data.TotalFlawlessWin;
        stats.TotalGameLeaved = Data.TotalGameLeaved;
        stats.TotalGameLost = Data.TotalGameLost;
        stats.TotalGameWon = Data.TotalGameWon;
        stats.TotalNakedWin = Data.TotalNakedWin;

        _statsPanel.SetStats(stats);

        ServiceResources.LoadImage(Data.Avatar, _avatar);
        ServiceResources.LoadImage(mm.Avatar, _userImage);
        ServiceResources.LoadImage(Data.Avatar, _opponentImage);
        _statsVS.text = "0 <color=#E1E3F1>VS</color> 0";
        
        if (Data.IsLegend){
            nameUserTxt.alignment = TextAlignmentOptions.Midline;
            _legend.gameObject.SetActive(true);
            _legend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
            _legend.transform.localPosition = new Vector2(_legend.transform.localPosition.x - 55, _legend.transform.localPosition.y);
            level.transform.localPosition = new Vector2(level.transform.localPosition.x - 55, level.transform.localPosition.y);
        }
    }

    public void Close(){
        panel.SetActive(false);
        _lockInfo.gameObject.SetActive(false);
        _removeFriend.gameObject.SetActive(false);
        addFriendBtn.gameObject.SetActive(false);
        writeMessageBtn.gameObject.SetActive(false);
        _avatar.sprite = _emptyAvatar;
        _userImage.sprite = _emptyAvatar;
        _opponentImage.sprite = _emptyAvatar;
        addFriendBtn.interactable = true;
        _removeFriend.interactable = true;
        EnableAddFriend(true);
        FriendsID = string.Empty;
        gameObject.GetComponent<Canvas>().sortingOrder = 11;
    }

    private void WriteMessage(){
        gameObject.GetComponent<Canvas>().sortingOrder = 11;
        DialogueView.Instance.Open(Data);
    }

    private void LockUser(){
        if (MainRoot.Instance.mainModel.IsPremium){
            if (_isBlocked){
                ServiceWeb.Instance.RemoveBlackList(Data.ID);
                Friends.Instance.RemoveFromBlackList(Data.ID);
                _blackList.sprite = _blackListIconPremium;
                _blackListTxt.text = "Блокировать";
                
                if (!Data.IsBlockedYou){
                    addFriendBtn.gameObject.SetActive(true);
                    writeMessageBtn.gameObject.SetActive(true);
                    _lockInfo.gameObject.SetActive(false);
                }

                EnableAddFriend(true);
                _lockInfoImage.sprite = _youBlocked;
                _lockInfoImage.SetNativeSize();
                _lockBigInfo.text = "Игрок заблокировал вас";
                _isBlocked = false;
            }
            else{
                ServiceWeb.Instance.AddBlackList(Data.ID);
                _blackList.sprite = _unlockIcon;
                _blackListTxt.text = "Разблокировать";
                _removeFriend.gameObject.SetActive(false);
                addFriendBtn.gameObject.SetActive(false);
                writeMessageBtn.gameObject.SetActive(false);
                _lockInfoImage.sprite = _lockIcon;
                _lockInfoImage.SetNativeSize();
                _lockBigInfo.text = "Игрок в черном списке";
                _lockInfo.gameObject.SetActive(true);
                FriendsID = string.Empty;
                _isBlocked = true;
            }

            _blackList.SetNativeSize();
        }
        else{
            BuyPremiumMenu.Instance.Open();
        }
    }

    private void AddFriend(){
        var dialog = Friends.Instance.GetDialogElement(Data.ID);
        if (dialog != null)
            dialog.SetUnseenMessages(0);
        var mm = MainRoot.Instance.mainModel;
        var unseenMessage = mm.UnseenMessageses.Find(x => x.ID == Data.ID);
        if (unseenMessage != null)
            unseenMessage.CountMessage = 0;
        
        if (!string.IsNullOrEmpty(FriendsID)){
            ServiceWeb.Instance.UpdateFriend(FriendsID, 2);
            Friends.Instance.AddFriend(Data.ID);
            _removeFriend.gameObject.SetActive(true);
            addFriendBtn.gameObject.SetActive(false);
        }
        else{
            ServiceWeb.Instance.AddFriendRequest(Data.ID);
            EnableAddFriend(false);
        }
    }

    private void EnableAddFriend(bool isActive){
        if (isActive){
            addFriendBtn.interactable = true;
            addFriendBtn.image.sprite = _whiteBtn;
            var txt = addFriendBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 
            txt.color = Color.white;
            txt.text = "Добавить в друзья";
            addFriendBtn.GetComponent<Image>().color = new Color(0.51f, 0.38f, 0.84f);
            addFriendBtn.interactable = true;
        }
        else{
            addFriendBtn.interactable = false;
            addFriendBtn.image.sprite = _lockInfo.sprite;
            var txt = addFriendBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 
            txt.color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1);
            txt.text = "Заявка отправлена";
            addFriendBtn.GetComponent<Image>().color = Color.white;
            addFriendBtn.interactable = false;
        }
    }
    
    private void RemoveFriend(){
        ServiceWeb.Instance.RemoveFriend(Data.ID);
        _removeFriend.gameObject.SetActive(false);
        EnableAddFriend(true);
        FriendsID = string.Empty;
    }

    private void RemoveFriendEvent(string ID){
        if (Data.ID == ID){
            _removeFriend.interactable = true;
            _removeFriend.gameObject.SetActive(false);
            addFriendBtn.gameObject.SetActive(true);
            FriendsID = string.Empty;
        }
    }

    private void AcceptedFriendship(){
        var mm = MainRoot.Instance.mainModel;
        FriendRequest friendRequest = mm.AcceptedFriendship;
        
        if (Data != null && friendRequest.UserID == Data.ID){
            if (friendRequest.RequestStatus == 2){
                addFriendBtn.interactable = true;
                addFriendBtn.gameObject.SetActive(false);
                _removeFriend.gameObject.SetActive(true);
            }
            else if (friendRequest.RequestStatus == 3){
                addFriendBtn.interactable = true;
                addFriendBtn.gameObject.SetActive(true);
                _removeFriend.gameObject.SetActive(false);
                writeMessageBtn.gameObject.SetActive(true);
                EnableAddFriend(true);
            }
        }
    }

    private void RemoveFromBlackListReceive(string ID){
        if (Data != null){
            if (Data.ID == ID){
                Data.IsBlockedYou = false;
                if (!_isBlocked){
                    addFriendBtn.gameObject.SetActive(true);
                    writeMessageBtn.gameObject.SetActive(true);
                    _lockInfo.gameObject.SetActive(false);
                    EnableAddFriend(true);
                }
                else{
                    _lockInfoImage.sprite = _lockIcon;
                    _lockInfoImage.SetNativeSize();
                    _lockBigInfo.text = "Игрок в черном списке";
                    _lockInfo.gameObject.SetActive(true);
                }
                FriendsID = string.Empty;
            }
        }
    }

    private void RemoveFromFriendReceive(string ID){
        if (Data != null){
            if (Data.ID == ID){
                _removeFriend.gameObject.SetActive(false);
                addFriendBtn.gameObject.SetActive(true);
                writeMessageBtn.gameObject.SetActive(true);
                FriendsID = string.Empty;
                EnableAddFriend(true);
            }
        }
    }

    private void AddToBlackListReceive(string ID){
        if (Data != null){
            if (Data.ID == ID){
                Data.IsBlockedYou = true;
                if (!_isBlocked){
                    _lockBigInfo.text = "Игрок заблокировал вас";
                    _lockInfoImage.sprite = _youBlocked;
                }
                else{
                    _lockBigInfo.text = "Игрок в черном списке";
                    _lockInfoImage.sprite = _lockIcon;
                }
                _lockInfoImage.SetNativeSize();
                _removeFriend.gameObject.SetActive(false);
                addFriendBtn.gameObject.SetActive(false);
                writeMessageBtn.gameObject.SetActive(false);
                _lockInfo.gameObject.SetActive(true);
                EnableAddFriend(true);
                FriendsID = string.Empty;
            }
        }
    }
    
    private void NewFriendRequest(){
        MainModel mm = MainRoot.Instance.mainModel;
        var user = mm.NewFriendRequest;
        FriendsID = user.ID;
    }

    private void BuyPremiumReceive(){
        if (Data != null){
            if (Data.IsBlockedYou){
                _blackList.sprite = _lockIcon;
            }

            if (Data.IsBlocked){
                _blackList.sprite = _unlockIcon;
            }

            if (!Data.IsBlocked && !Data.IsBlockedYou){
                _blackList.sprite = _blackListIconPremium;
            }
        }
        else{
            _blackList.sprite = _blackListIconPremium;
        }
        _blackList.SetNativeSize();
    }
}