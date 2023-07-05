using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Security.Cryptography;
using Random = System.Random;

public class Friends : MonoBehaviour{
    public static Friends Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private CSButtonTMP dialogueBtn;
    [SerializeField] private CSButtonTMP friendsBtn;
    [SerializeField] private CSButtonTMP blacklistBtn;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject friendsPanel;
    [SerializeField] private GameObject blacklistPanel;
    [SerializeField] private Image _lock;
    [SerializeField] private Separator _separationFind;
    [SerializeField] private Separator _separationFriendShip;
    [SerializeField] private Separator _separationFriend;
    [SerializeField] private TMP_FontAsset regular;
    [SerializeField] private TMP_FontAsset medium;
    [Space] [SerializeField] private GameObject dialogueElementPrefab;
    [SerializeField] private GameObject friendsElementPrefab;
    [SerializeField] private GameObject blackListElementPrefab;
    [SerializeField] private GameObject _requestElementPrefab;

    private List<Button> _pages = new List<Button>();
    private List<FriendsListElement> _friendsList;
    private List<RequestListElement> _userList;
    private List<DialogueListElement> _dialogueList;
    private List<BlackListElement> _blackList;
    private List<RequestListElement> _requestList;

    private CSButtonTMP _activePage;

    private void Awake(){
        Instance = this;
    }

    private void OnDestroy(){
        Instance = null;
    }

    private void OnEnable(){
        searchInputField.onValueChanged.AddListener(Search);
        searchInputField.onEndEdit.AddListener(Search);
        dialogueBtn.onClick.AddListener(DialogueButtonPressed);
        friendsBtn.onClick.AddListener(FriendsButtonPressed);
        blacklistBtn.onClick.AddListener(BlacklistButtonPressed);
        MainModel.OnReceiveMessage += RecievedMessage;
        MainModel.OnChangeStatus += ChangeStatus;
        MainModel.OnListOfUsersLoaded += ListUsersLoaded;
        MainModel.OnAcceptedFriendship += AcceptedFriendship;
        MainModel.OnNewFriendRequest += NewFriendRequest;
        ServiceWeb.OnAddBlackList += AddBlackList;
        ServiceWeb.OnRemoveFriendEvent += RemoveFromFriend;
        ServiceIO.OnRemoveFromFriendReceive += RemoveFromFriend;
        ServiceIO.OnAddToBlackListReceive += RemoveFromFriend;
        MainModel.OnPremiumChanged += ChangePremium;
        BuyPremiumMenu.OnCloseBuyPremium += ClosePremiumPanel;
    }

    private void AddBlackList(string ID){

        var friend = _friendsList.Find(x => x.ID == ID);
        
        BlackListUserData data = new BlackListUserData();

        if (friend != null){
            _friendsList.Remove(friend);
            Destroy(friend.gameObject);
        }
        
        OtherProfileData profile = FriendView.Instance.Data;
        data.Name = profile.Name;
        data.UserID = profile.ID;
        data.Avatar = profile.Avatar;
        data.RankValue = profile.RankValue;
        data.BlockedDate = DateTime.Now;
        data.RankType = profile.RankType;
        
        BlackListScroll(data);
    }

    private void OnDisable(){
        searchInputField.onValueChanged.RemoveAllListeners();
        searchInputField.onEndEdit.RemoveAllListeners();
        dialogueBtn.onClick.RemoveAllListeners();
        friendsBtn.onClick.RemoveAllListeners();
        blacklistBtn.onClick.RemoveAllListeners();
        MainModel.OnReceiveMessage -= RecievedMessage;
        MainModel.OnChangeStatus -= ChangeStatus;
        MainModel.OnListOfUsersLoaded -= ListUsersLoaded;
        MainModel.OnAcceptedFriendship -= AcceptedFriendship;
        MainModel.OnNewFriendRequest -= NewFriendRequest;
        ServiceWeb.OnAddBlackList -= AddBlackList;
        ServiceWeb.OnRemoveFriendEvent -= RemoveFromFriend;
        ServiceIO.OnRemoveFromFriendReceive -= RemoveFromFriend;
        ServiceIO.OnAddToBlackListReceive -= RemoveFromFriend;
        MainModel.OnPremiumChanged -= ChangePremium;
        BuyPremiumMenu.OnCloseBuyPremium -= ClosePremiumPanel;
    }

    private void Start(){
        Init();
        _pages.Add(dialogueBtn);
        _pages.Add(friendsBtn);
        _pages.Add(blacklistBtn);
        
        _lock.gameObject.SetActive(!MainRoot.Instance.mainModel.IsPremium);
        
        SortedFriend();
        dialoguePanel.SetActive(true);
    }

    private void Init(){
        _friendsList = new List<FriendsListElement>();
        _blackList = new List<BlackListElement>();
        _dialogueList = new List<DialogueListElement>();
        _userList = new List<RequestListElement>();
        _requestList = new List<RequestListElement>();

        var mm = MainRoot.Instance.mainModel;

        //Warning Memory leaks!!!
        if (mm.RequestDatas.Count < 1){
            MainModel.OnRequestUserLoaded += () => {
                for (int i = 0; i < mm.RequestDatas.Count; i++){
                    FillRequestScroll(mm.RequestDatas[i]);
                }

                if (mm.RequestDatas.Count > 0)
                    _separationFriendShip.gameObject.SetActive(true);
                _separationFriendShip.SetText("Запрос в друзья");
                _separationFriendShip.transform.SetAsFirstSibling();
            };
        }
        else{
            for (int i = 0; i < mm.RequestDatas.Count; i++){
                FillRequestScroll(mm.RequestDatas[i]);
            }

            if (mm.RequestDatas.Count > 0)
                _separationFriendShip.gameObject.SetActive(true);
            _separationFriendShip.SetText("Запрос в друзья");
            _separationFriendShip.transform.SetAsFirstSibling();
        }
        
        if (mm.FriendDatas.Count < 1){
            MainModel.OnFriendLoaded += () => {
                 foreach (var friend in _friendsList){
                     if (friend != null)
                    Destroy(friend.gameObject);
                }
                _friendsList.Clear();
                _separationFriend.SetText("Друзья");
                _separationFriend.transform.SetAsLastSibling();
                for (int i = 0; i < mm.FriendDatas.Count; i++){
                    FillFriendsScroll(mm.FriendDatas[i]);
                }

                if (mm.FriendDatas.Count > 0) _separationFriend.gameObject.SetActive(true);
            };
        }
        else{
            foreach (var friend in _friendsList){
                Destroy(friend.gameObject);
            }
            _friendsList.Clear();
            _separationFriend.SetText("Друзья");
            _separationFriend.transform.SetAsLastSibling();
            for (int i = 0; i < mm.FriendDatas.Count; i++){
                FillFriendsScroll(mm.FriendDatas[i]);
            }

            if (mm.FriendDatas.Count > 0) _separationFriend.gameObject.SetActive(true);
        }

        if (mm.BlackListData.Count < 1){
            MainModel.OnBlackListLoaded += () => {
                for (int i = 0; i < mm.BlackListData.Count; i++){
                    BlackListScroll(mm.BlackListData[i]);
                }
            };
        }
        else{
            for (int i = 0; i < mm.BlackListData.Count; i++){
                BlackListScroll(mm.BlackListData[i]);
            }
        }

        if (mm.DialogUserDatas.Count < 1){
            MainModel.OnLastDialogLoaded += () => {
                for (int i = 0; i < mm.DialogUserDatas.Count; i++){
                    FillDialogueScroll(mm.DialogUserDatas[i]);
                }
            };
        }
        else{
            for (int i = 0; i < mm.DialogUserDatas.Count; i++){
                FillDialogueScroll(mm.DialogUserDatas[i]);
            }
        }
        if (MainRoot.Instance.mainModel.IsPremium) _lock.gameObject.SetActive(false);
        else _lock.gameObject.SetActive(true);
    }

    public void ChangeStatus(){
        MainModel mm = MainRoot.Instance.mainModel;
        var data = mm.ChangeStatus;

        var friendStatus = _friendsList.Find(x => x.ID == data.ID);
        if (friendStatus) friendStatus.SetStatus(data.IsStatus);

        var dialogStatus = _dialogueList.Find(x => x.ID == data.ID);
        if (dialogStatus) dialogStatus.SetStatus(data.IsStatus);

        var blackList = _blackList.Find(x => x.ID == data.ID);
        if (blackList) blackList.SetStatus(data.IsStatus);
    }

    public void Open(){
        panel.SetActive(true);
        SetActivePage(friendsBtn);
        var image = _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().GetImage(_activePage);
        _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().Select(image);
        friendsPanel.SetActive(true);
    }

    public void Close(){
        searchInputField.text = string.Empty;
        panel.SetActive(false);
    }

    private void Search(string searchValue){
        if (friendsPanel.activeSelf){
            SearchSortedItems(searchValue, _friendsList.Cast<FriendListItem>().ToList());
            SearchSortedItems(searchValue, _requestList.Cast<FriendListItem>().ToList(), true);
            SearchListOfUsers(searchValue);
        }
        else if (dialoguePanel.activeSelf){
            SearchSortedItems(searchValue, _dialogueList.Cast<FriendListItem>().ToList());
        }
        else if (blacklistPanel.activeSelf){
            SearchSortedItems(searchValue, _blackList.Cast<FriendListItem>().ToList());
        }
    }

    private void SearchSortedItems(string search, List<FriendListItem> list, bool isRequest = false){
        foreach (var item in list){
            item.gameObject.SetActive(true);
        }

        string s = search.ToLower();
        var notSearch = list.Where(t => !t.GetName().ToLower().Contains(s));

        if (!isRequest){
            _separationFriend.gameObject.SetActive(notSearch.Count() != _friendsList.Count);
        }
        else{
            _separationFriendShip.gameObject.SetActive(notSearch.Count() != _requestList.Count);
        }

        foreach (var item in notSearch){
            item.gameObject.SetActive(false);
        }
    }

    private void SearchListOfUsers(string search){
        foreach (var user in _userList){
            Destroy(user.gameObject);
        }

        _userList.Clear();
        _separationFind.gameObject.SetActive(false);
        _separationFind.SetText("Поиск");
        if (!string.IsNullOrEmpty(search)){
            ServiceWeb.Instance.GetListOfUsers(search);
        }
        else{
            MainRoot.Instance.mainModel.Users.Clear();
        }
    }

    private void ListUsersLoaded(){
        foreach (var user in _userList){
            Destroy(user.gameObject);
        }
        _userList.Clear();
        
        var mm = MainRoot.Instance.mainModel;
        
        if (mm.Users.Count == 0) return;
        _separationFind.transform.SetAsLastSibling();
        _separationFind.gameObject.SetActive(true);
        for (int i = 0; i < mm.Users.Count; i++){
            GameObject temp = Instantiate(_requestElementPrefab, friendsPanel.transform) as GameObject;
            RequestListElement user = temp.GetComponent<RequestListElement>();
            user.SetText("Добавить в друзья?");
            user.EnableCancelFriend(false);
            
            if (!mm.Users[i].IsBlocked && !mm.Users[i].IsBlockedYou) user.BlockedUser(true, 1);
            else if (mm.Users[i].IsBlocked) user.BlockedUser(false, 2);
            if (mm.Users[i].IsBlockedYou) user.BlockedUser(false, 3);
            
            if (mm.Users[i].RequestStatus == 1 && !mm.Users[i].IsBlocked && !mm.Users[i].IsBlockedYou) {
                user.InteractableAddFriend(false);
                user.SetText("Запрос отправлен");
            }

            if (mm.Users[i].IsBlocked && mm.Users[i].IsBlockedYou){
                user.InteractableAddFriend(false);
            }
            
            RequestData data = new RequestData();
            data.UserID = mm.Users[i].UserID;
            data.Name = mm.Users[i].Name;
            data.Avatar = mm.Users[i].Avatar;
            data.RankValue = mm.Users[i].RankValue;
            
            user.Init(data);
            _userList.Add(user);
        }
        _separationFriendShip.transform.SetAsFirstSibling();
    }

    public void UpdateSearch(){
        if (!string.IsNullOrEmpty(searchInputField.text)){
            Search(searchInputField.text);
        }
    }
    
    //Friends
    private void SortedFriend(){
        foreach (var item in _friendsList){
            item.gameObject.SetActive(true);
        }

        var friendOnline = _friendsList.Where(fr => fr.IsStatus).OrderBy(fr => fr.GetName());
        var friendOffline = _friendsList.Where(fr => !fr.IsStatus).OrderBy(fr => fr.GetName());

        List<FriendsListElement> list = friendOnline.ToList();
        list.AddRange(friendOffline);

        for (int i = 0; i < list.Count; i++){
            list[i].transform.SetSiblingIndex(i);
        }

        _friendsList.Clear();
        _friendsList = list;
        if (_friendsList.Count > 0){
            int index = _friendsList[0].transform.GetSiblingIndex();
            _separationFriend.transform.SetSiblingIndex(index);
        }
    }

    //Diaglos
    private void SortedDialog(){
        foreach (var item in _dialogueList){
            item.gameObject.SetActive(true);
        }

        var itemsDate =
            (_dialogueList.OrderByDescending(x => x.LastDateCommunication)
                .Where(x => x.LastDateCommunication != DateTime.MinValue));
        var emptyDiaglos = (_dialogueList.Where(x => x.LastDateCommunication == DateTime.MinValue));

        List<DialogueListElement> list = itemsDate.ToList();
        list.AddRange(emptyDiaglos);

        for (int i = 0; i < list.Count; i++){
            list[i].transform.SetSiblingIndex(i);
        }

        _dialogueList.Clear();
        _dialogueList = list;
    }

    //Blacklist

    private void SortedBlacklist(){
        foreach (var item in _blackList){
            item.gameObject.SetActive(true);
        }

        var itemsDate = (_blackList.OrderBy(x => x.GetName()));

        List<BlackListElement> list = itemsDate.ToList();

        for (int i = 0; i < list.Count; i++){
            list[i].transform.SetSiblingIndex(i);
        }
        
        _blackList.Clear();
        _blackList = list;
    }

    private void SetActivePage(CSButtonTMP acitvePage){
        foreach (var user in _userList){
            Destroy(user.gameObject);
        }

        _userList.Clear();
        _separationFind.gameObject.SetActive(false);

        foreach (CSButtonTMP page in _pages){
            page.Text.font = regular;
            page.Text.fontSize = 38;
            page.Text.color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        }

        dialoguePanel.SetActive(false);
        friendsPanel.SetActive(false);
        blacklistPanel.SetActive(false);

        acitvePage.Text.font = medium;
        acitvePage.Text.fontSize = 40;
        acitvePage.Text.color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
        _activePage = acitvePage;
        searchInputField.text = string.Empty;
        if (_requestList.Count > 0){
            _separationFriendShip.gameObject.SetActive(true);
            _separationFriendShip.transform.SetAsFirstSibling();
            int index = _separationFriendShip.transform.GetSiblingIndex() + 1;
            foreach (var request in _requestList){
                request.transform.SetSiblingIndex(index);
                index++;
            }
        }
        else{
            _separationFriendShip.gameObject.SetActive(false);
        }

        if (_friendsList.Count > 0){
            _separationFriend.gameObject.SetActive(true);
        }
        else _separationFriend.gameObject.SetActive(false);
    }

    private void FillDialogueScroll(DialogUserData data){
        GameObject temp = Instantiate(dialogueElementPrefab, dialoguePanel.transform);
        DialogueListElement dialogueListElement = temp.GetComponent<DialogueListElement>();

        dialogueListElement.Init(data);
        _dialogueList.Add(dialogueListElement);
    }

    private void BlackListScroll(BlackListUserData data){
        GameObject temp = Instantiate(blackListElementPrefab, blacklistPanel.transform) as GameObject;
        BlackListElement blackListElement = temp.GetComponent<BlackListElement>();

        blackListElement.Init(data);
        _blackList.Add(blackListElement);
    }

    private void FillFriendsScroll(FriendData data){
        GameObject temp = Instantiate(friendsElementPrefab, friendsPanel.transform) as GameObject;
        FriendsListElement friendListElement = temp.GetComponent<FriendsListElement>();
        friendListElement.transform.SetAsLastSibling();
        
        friendListElement.Init(data);
        _friendsList.Add(friendListElement);
    }

    private void FillRequestScroll(RequestData data){
        GameObject temp = Instantiate(_requestElementPrefab, friendsPanel.transform) as GameObject;
        RequestListElement requestListElement = temp.GetComponent<RequestListElement>();
        requestListElement.transform.SetAsFirstSibling();
        _separationFriendShip.transform.SetAsFirstSibling();
        
        if (data.RequestStatus == 1) requestListElement.InteractableAddFriend(true);
        
        requestListElement.Init(data);
        _requestList.Add(requestListElement);
    }

    public void RecievedMessage(){
        MainModel mm = MainRoot.Instance.mainModel;
        var data = mm.CurrentMessage;

        Debug.Log(DialogueView.Instance.ToID + " " + data.FromUserID);

        AddDialog(data);
        DialogueListElement element = _dialogueList.Find(x => x.ID == data.FromUserID);
        var unseenMessage = mm.UnseenMessageses.Find(x => x.ID == data.FromUserID);

        if (DialogueView.Instance.ToID == data.FromUserID){
            DialogueView.Instance.ReceiveMessage(data.Message);
            unseenMessage.CountMessage = 0;
        }
        
        mm.IsNotification();
        
        element.SetUnseenMessages(unseenMessage.CountMessage);
    }

    public void AddDialog(MessageData data){
        if (_dialogueList.Count > 0){
            DialogueListElement dialog = _dialogueList.Find(x => x.ID == data.FromUserID);
            if (dialog != null){
                dialog.messageTxt.text = data.Message;
                dialog.SetDate(DateTime.Now);
            }
            else{
                GameObject temp = Instantiate(dialogueElementPrefab, dialoguePanel.transform);
                DialogueListElement dialogueListElement = temp.GetComponent<DialogueListElement>();
                DialogUserData user = new DialogUserData();
                user.Message = data.Message;
                user.Name = data.UserName;
                user.ID = data.FromUserID;
                user.CreatedAt = DateTime.Now;
                
                var element = _friendsList.Find(x => x.ID == data.FromUserID);
               
                if (element != null){
                    user.Avatar = element.Avatar;
                    user.RankValue = element.RankValue;
                    user.IsStatus = element.IsStatus;
                    user.RankType = element.RankType;
                }
                var item = MainRoot.Instance.mainModel.Users.Find(x => x.UserID == data.FromUserID);
                
                if (item != null){
                    user.Avatar = item.Avatar;
                    user.RankValue = item.RankValue;
                    user.IsStatus = item.IsStatus;
                    user.RankType = item.RankType;
                }

                var request = MainRoot.Instance.mainModel.RequestDatas.Find(x => x.UserID == data.FromUserID);
            
                if (request != null){
                    user.Avatar = request.Avatar;
                    user.RankValue = request.RankValue;
                    user.IsStatus = request.IsStatus;
                    user.RankType = request.RankType;
                }
                
                dialogueListElement.Init(user);
                _dialogueList.Add(dialogueListElement);
            }
        }
        else{
            GameObject temp = Instantiate(dialogueElementPrefab, dialoguePanel.transform);
            DialogueListElement dialogueListElement = temp.GetComponent<DialogueListElement>();
            DialogUserData user = new DialogUserData();
            user.Message = data.Message;
            user.Name = data.UserName;
            user.ID = data.FromUserID;
            user.CreatedAt = DateTime.Now;

            var friend = _friendsList.Find(x => x.ID == data.FromUserID);

            if (friend != null){
                user.Avatar = friend.Avatar;
                user.RankValue = friend.RankValue;
                user.IsStatus = friend.IsStatus;
                user.RankType = friend.RankType;
            }
            
            var item = MainRoot.Instance.mainModel.Users.Find(x => x.UserID == data.FromUserID);
            
            if (item != null){
                user.Avatar = item.Avatar;
                user.RankValue = item.RankValue;
                user.IsStatus = item.IsStatus;
                user.RankType = item.RankType;
            }

            var request = MainRoot.Instance.mainModel.RequestDatas.Find(x => x.UserID == data.FromUserID);
            
            if (request != null){
                user.Avatar = request.Avatar;
                user.RankValue = request.RankValue;
                user.IsStatus = request.IsStatus;
                user.RankType = request.RankType;
            }

            //user.Avatar = _userList.Find(x => x.ID == data.FromUserID).Avatar;
            //user.RankValue = _userList.Find(x => x.ID == data.FromUserID).RankValue;
            //user.ISStatus = _userList.Find(x => x.ID == data.FromUserID).IsStatus;

            dialogueListElement.Init(user);
            _dialogueList.Add(dialogueListElement);
        }
    }


    public DialogueListElement GetDialogElement(string ID){
        if (_dialogueList.Count == 0) return null;

        DialogueListElement element = _dialogueList.Find(x => x.ID == ID);
        return element;
    }

    public FriendsListElement GetFriendElement(string ID){
        if (_friendsList.Count == 0) return null;

        FriendsListElement element = _friendsList.Find(x => x.ID == ID);
        return element;
    }

    public BlackListElement GetBlackListElement(string ID){
        if (_blackList.Count == 0) return null;

        BlackListElement element = _blackList.Find(x => x.ID == ID);
        return element;
    }

    public RequestListElement GetRequestListElementElement(string ID){
        if (_blackList.Count == 0) return null;

        RequestListElement element = _requestList.Find(x => x.ID == ID);
        return element;
    }
    
    private void DialogueButtonPressed(){
        SortedDialog();
        SetActivePage(dialogueBtn);
        dialoguePanel.SetActive(true);
    }

    private void FriendsButtonPressed(){
        SortedFriend();
        SetActivePage(friendsBtn);
        friendsPanel.SetActive(true);
    }

    private void BlacklistButtonPressed(){
        if (MainRoot.Instance.mainModel.IsPremium){
            SortedBlacklist();
            SetActivePage(blacklistBtn);
            blacklistPanel.SetActive(true);
        }
        else{
            BuyPremiumMenu.Instance.Open();
        }
    }

    private void ChangePremium(){
        if (MainRoot.Instance.mainModel.IsPremium){
            _lock.gameObject.SetActive(false);
            BuyPremiumMenu.Instance.Close();

            if (_activePage != null){
                _activePage = _pages[2] as CSButtonTMP;
                SetActivePage(_activePage);
                var image = _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().GetImage(_activePage);
                _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().Select(image);
            }
        }
        else{
            _lock.gameObject.SetActive(true);
        }
    }

    private void ClosePremiumPanel(){
        if (!MainRoot.Instance.mainModel.IsPremium){
            SortedFriend();
            if (_activePage != null){
                SetActivePage(_activePage);
                var image = _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().GetImage(_activePage);
                _activePage.GetComponentInParent<SwitchSpriteSelectedButtons>().Select(image);
            }

            var index = _pages.FindIndex(x=>x.name.Contains(_activePage.name));
            
            if (index == 0) dialoguePanel.SetActive(true);
            else friendsPanel.SetActive(true);
        }
    }
    
    public void AddFriend(string ID){
        foreach (Transform tr in friendsPanel.transform){
            if (tr.GetComponent<FriendsListElement>() != null){
                Destroy(tr.gameObject);
            }

            if (tr.GetComponent<RequestListElement>() != null && tr.GetComponent<RequestListElement>().ID == ID){
                Destroy(tr.gameObject);
            }
        }

        _friendsList.Clear();
        var requestFriend = _requestList.Find(x => x.ID == ID);
        if (requestFriend != null)
            _requestList.Remove(requestFriend);
        if (_requestList.Count == 0) _separationFriendShip.gameObject.SetActive(false);
        MainRoot.Instance.mainModel.FriendDatas.Clear();
        //ServiceWeb.Instance.GetUserFriends();
        UpdateSearch();
        var mm = MainRoot.Instance.mainModel;
        mm.IsNotification();
    }

    public void CancelFriend(string ID){
        foreach (Transform tr in friendsPanel.transform){
            if (tr.GetComponent<RequestListElement>() != null && tr.GetComponent<RequestListElement>().ID == ID){
                Destroy(tr.gameObject);
            }
            if (tr.GetComponent<FriendsListElement>() != null && tr.GetComponent<FriendsListElement>().ID == ID){
                Destroy(tr.gameObject);
            }
        }

        var requestFriend = _requestList.Find(x => x.ID == ID);
        if (requestFriend != null)
            _requestList.Remove(requestFriend);
        if (_requestList.Count == 0) _separationFriendShip.gameObject.SetActive(false);
        if (_friendsList.Count == 0) _separationFriend.gameObject.SetActive(false);
        MainRoot.Instance.mainModel.FriendDatas.Clear();
        
        UpdateSearch();
    }

    private void AcceptedFriendship(){
        foreach (Transform tr in friendsPanel.transform){
            if (tr.GetComponent<FriendsListElement>() != null){
                Destroy(tr.gameObject);
            }

            if (tr.GetComponent<RequestListElement>() != null){
                Destroy(tr.gameObject);
            }
        }

        _friendsList.Clear();
        _requestList.Clear();

        MainRoot.Instance.mainModel.FriendDatas.Clear();
        MainRoot.Instance.mainModel.RequestDatas.Clear();

        ServiceWeb.Instance.GetFriendsRequest();
        
        
        if (friendsPanel.activeSelf && !string.IsNullOrEmpty(searchInputField.text)){
            string searchValue = searchInputField.text;
            SearchSortedItems(searchValue, _friendsList.Cast<FriendListItem>().ToList());
            SearchListOfUsers(searchValue);
        }

        if (_requestList.Count == 0) _separationFriendShip.gameObject.SetActive(false);
        _separationFriend.gameObject.SetActive(_friendsList.Count > 0);
    }

    private void NewFriendRequest(){
        MainModel mm = MainRoot.Instance.mainModel;
        FillRequestScroll(mm.NewFriendRequest);
        _separationFriendShip.gameObject.SetActive(true);
        _separationFriendShip.SetText("Запросы");
        _separationFriendShip.transform.SetAsFirstSibling();
        mm.RequestDatas.Add(mm.NewFriendRequest);
        mm.IsNotification();
    }

    public void RemoveFromBlackList(string ID){
        var user = _blackList.Find(x => x.ID == ID);
        _blackList.Remove(user);
        if (user != null)
        Destroy(user.gameObject);
    }

    public void RemoveFromRequestList(string ID){
        var user = _requestList.Find(x => x.ID == ID);
        _requestList.Remove(user);
        if (user != null)
        Destroy(user.gameObject);
        if (_requestList.Count < 1){
            _separationFriendShip.gameObject.SetActive(false);
        }
    }
    
    private void RemoveFromFriend(string ID){
        var friend = _friendsList.Find(x => x._data.ID == ID);
        _friendsList.Remove(friend);
        if (friend != null)
        Destroy(friend.gameObject);
        if (_friendsList.Count < 1) _separationFriend.gameObject.SetActive(false);
    }
}