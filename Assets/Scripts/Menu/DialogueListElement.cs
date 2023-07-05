using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Serialization;

public class DialogueListElement : FriendListItem
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI messageTxt;
    public TextMeshProUGUI dateTxt;
    public TextMeshProUGUI statusTxt;
    public TextMeshProUGUI UnreadMessage;
    public DateTime LastDateCommunication;
    
    [SerializeField] private Image _status;
    [SerializeField] private Image _shield;
    [SerializeField] private Button _button;
    [SerializeField] private Button chatBtn;

    private DialogUserData _data;
    
    private void OnEnable()
    {
        _button.onClick.AddListener(OpenFriendView);
        chatBtn.onClick.AddListener(OpenDialogueView);
        ServiceIO.OnAddToBlackListReceive += AddToBlackListReceive;
        ServiceIO.OnRemoveBlackListReceive += RemoveFromBlackListReceice;
        ServiceWeb.OnAddBlackList += AddBlackList;
        ServiceWeb.OnRemoveBlackList += RemoveFromBlackList;
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
        chatBtn.onClick.RemoveAllListeners();
        ServiceIO.OnAddToBlackListReceive -= AddToBlackListReceive;
        ServiceIO.OnRemoveBlackListReceive -= RemoveFromBlackListReceice;
        ServiceWeb.OnAddBlackList -= AddBlackList;
        ServiceWeb.OnRemoveBlackList -= RemoveFromBlackList;
    }

    //use this to pass information about the dialogue
    public override void Init(UserData data)
    {
        var mm = MainRoot.Instance.mainModel;
        DialogUserData dialog = (DialogUserData) data;
        _data = dialog;
        Name = dialog.Name;
        ID = dialog.ID;
        userName.text = Name;

        
       //mm.FriendDatas.Find(x => x.ID == data.ID).ISStatus;
        
       //SetStatus(status);
        
        SetDate(dialog.CreatedAt);
        SetMessage(dialog.Message);
        levelTxt.text = dialog.RankValue.ToString();
        if (LastDateCommunication == DateTime.MinValue) dateTxt.text = string.Empty;
        
        var unseenMessage = mm.UnseenMessageses.Find(x => x.ID == ID);
        if (unseenMessage != null){
            SetUnseenMessages(unseenMessage.CountMessage);
            mm.IsNotification();
        }

        _data.RankName = mm.RankInfos.Find(x => x.minRank <= _data.RankValue && x.maxRank >= _data.RankValue).Name;

        LoadImage(dialog.Avatar, avatar);
        Sprite sp = Resources.Load<Sprite>("Shields/" + dialog.RankName);

        if (dialog.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
    }

    public override string GetName()
    {
        return userName.text;
    }

    public override void SetStatus(bool isStatus){
        base.SetStatus(isStatus);
        switch (isStatus)
        {
            case true:
                _status.color = Color.green;
                break;
            case false:
                _status.color = Color.gray;
                break;
        }
    }

    public void SetDate(DateTime dateTime)
    {
        dateTxt.text = dateTime.ToString("dd/MM/yyyy");
        LastDateCommunication = dateTime;
    }

    public void SetMessage(string msg)
    {
        messageTxt.text = msg;
    }

    public void SetUnseenMessages(int count){
        if (count == 0){
            UnreadMessage.transform.parent.gameObject.SetActive(false);
            return;
        }
        
        UnreadMessage.transform.parent.gameObject.SetActive(true);
        UnreadMessage.text = count.ToString();
        var mm = MainRoot.Instance.mainModel;
        mm.IsNotification();
    }
    
    private void OpenDialogueView()
    {
        SetUnseenMessages(0);
        var mm = MainRoot.Instance.mainModel;
        var unseenMessage = mm.UnseenMessageses.Find(x => x.ID == ID);
        if (unseenMessage != null)
        unseenMessage.CountMessage = 0;
        mm.IsNotification();
        DialogueView.Instance.Open(_data);//pass dialogue info here for display 
    }

    private void OpenFriendView() {
        FriendView.Instance.Open(_data.ID);// pass friend info for display here
    }
    
    private void RemoveFromBlackListReceice(string ID){
        if (_data.ID == ID){
            var user = Friends.Instance.GetBlackListElement(ID);
            if (user == null){
                _data.IsBlocked = false;
                _data.IsBlockedYou = false;
            }
            else{
                _data.IsBlocked = true;
                _data.IsBlockedYou = false;
            }
        }
    }

    private void AddToBlackListReceive(string ID){
        if (_data.ID == ID){
            var user = Friends.Instance.GetBlackListElement(ID);
            if (user == null){
                _data.IsBlocked = false;
                _data.IsBlockedYou = true;
            }
            else{
                _data.IsBlocked = true;
                _data.IsBlockedYou = false;
            }
        }
    }
    
    private void AddBlackList(string ID){
        if (_data.ID == ID){
            _data.IsBlocked = true;
        }
    }
    
    private void RemoveFromBlackList(string ID){
        if (_data.ID == ID){
            _data.IsBlocked = false;
        }
    }
}
