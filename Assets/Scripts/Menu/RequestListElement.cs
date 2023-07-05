using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestListElement : FriendListItem
{
    
    [SerializeField] private TextMeshProUGUI _userName;
    [SerializeField] private TextMeshProUGUI _levelTxt;
    [SerializeField] private TextMeshProUGUI _description;
    
    [SerializeField] private Button button;
    [SerializeField] private Button _addFriend;
    [SerializeField] private Button _cancelFriend;
    [SerializeField] private Image _shield;
    
    public RequestData Data{ get; set; }
    
    private bool _isRequest;
    private bool _isInteractable;
    private void OnEnable(){
        _addFriend.onClick.AddListener(AddFriend);
        _cancelFriend.onClick.AddListener(CancelFriend);
        button.onClick.AddListener(OpenFriendView);
        ServiceWeb.OnAddFriend += AddFriendResponce;
        ServiceWeb.OnAddBlackList += AddBlackList;
        ServiceWeb.OnRemoveBlackList += RemoveFromBlackListResponce;
        ServiceIO.OnAddToBlackListReceive += AddToBlackListReceive;
        ServiceIO.OnRemoveBlackListReceive += RemoveFromBlackListResponce;
        MainModel.OnNewFriendRequest += OnNewFriendRequest;
    }

    private void OnDisable(){
        _addFriend.onClick.RemoveAllListeners();
        _cancelFriend.onClick.RemoveAllListeners();
        button.onClick.RemoveAllListeners();
        ServiceWeb.OnAddFriend -= AddFriendResponce;
        ServiceWeb.OnRemoveBlackList -= RemoveFromBlackListResponce;
        ServiceWeb.OnAddBlackList -= AddBlackList;
        ServiceIO.OnAddToBlackListReceive -= AddToBlackListReceive;
        ServiceIO.OnRemoveBlackListReceive -= RemoveFromBlackListResponce;
    }

    public override void Init(UserData data){
        
        var mm = MainRoot.Instance.mainModel;
        
        RequestData requestData = (RequestData) data;
        Data = requestData;
        _userName.text = data.Name;
        _levelTxt.text = data.RankValue.ToString();
        ID = Data.UserID;
        Name = Data.Name;
        Avatar = Data.Avatar;
        RankValue = Data.RankValue;
        LoadImage(requestData.Avatar, avatar);
        
        data.RankName = mm.RankInfos.Find(x => x.minRank <= Data.RankValue && x.maxRank >= Data.RankValue).Name;
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + requestData.RankName);
        
        if (requestData.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
    }

    public override string GetName(){
        return _userName.text;
    }

    public void SetText(string text){
        _description.text = text;
    }

    public void EnableCancelFriend(bool isActive){
        _cancelFriend.gameObject.SetActive(isActive);
        _isRequest = !isActive;
        _isInteractable = !isActive;
    }

    public void InteractableAddFriend(bool isActive){
        _addFriend.interactable = isActive;
        _isInteractable = isActive;
        _addFriend.gameObject.SetActive(isActive);
    }

    public void BlockedUser(bool isActive, int status){
        _addFriend.gameObject.SetActive(isActive);

        switch (status){
            case 1:
                _description.text = "Добавить игрока";
                break;
            case 2:
                _description.text = "Игрок в черном списке";
                break;
            case 3:
                _description.text = "Вы в черном списке у игрока";
                break;
        }
    }
    
    private void OpenFriendView()
    {
        FriendView.Instance.Open(Data.UserID);// pass friend info for display here
        FriendView.Instance.AcceptAddedFriend(_isInteractable);
        FriendView.Instance.FriendsID = Data.ID;
    }
    
    private void AddFriend(){
        if (!_isRequest){
            ServiceWeb.Instance.UpdateFriend(Data.ID, 2);
            Friends.Instance.AddFriend(Data.UserID);
            MainModel mm = MainRoot.Instance.mainModel;
            var request = mm.RequestDatas.Find(x => x.UserID == Data.UserID);
            mm.RequestDatas.Remove(request);
        }
        else{
            ServiceWeb.Instance.AddFriendRequest(Data.UserID);
            _description.text = "Запрос отправлен";
        }
        _addFriend.interactable = false;
        _addFriend.gameObject.SetActive(false);
        _isInteractable = false;
    }

    private void CancelFriend(){
        ServiceWeb.Instance.UpdateFriend(Data.ID, 3);
        Friends.Instance.CancelFriend(Data.UserID);
        
        MainModel mm = MainRoot.Instance.mainModel;
        var request = mm.RequestDatas.Find(x => x.UserID == Data.UserID);
        mm.RequestDatas.Remove(request);
    }

    private void AddFriendResponce(string ID){
        if (Data.UserID == ID){
            _addFriend.interactable = false;
            _addFriend.gameObject.SetActive(false);
            _isInteractable = false;
            _description.text = "Запрос отправлен";
        }
    }

    private void RemoveFromBlackListResponce(string ID){
        if (Data.UserID == ID){
            _addFriend.interactable = true;
            _isInteractable = true;
            _addFriend.gameObject.SetActive(true);
            Friends.Instance.UpdateSearch();
        }
    }

    private void AddBlackList(string ID){
        if (Data.UserID == ID){
            _isInteractable = false;
            Friends.Instance.UpdateSearch();
            Friends.Instance.RemoveFromRequestList(ID);
        }
    }

    private void AddToBlackListReceive(string ID){
        if (Data.UserID == ID){
            _isInteractable = false;
            Friends.Instance.UpdateSearch();
            Friends.Instance.RemoveFromRequestList(ID);
        }
    }
    
    private void OnNewFriendRequest(){
        var mm = MainRoot.Instance.mainModel;
        var user = mm.NewFriendRequest;
        if (Data.UserID == user.UserID){
            Friends.Instance.UpdateSearch();
        }
    }
}
