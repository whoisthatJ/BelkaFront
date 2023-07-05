using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendsListElement : FriendListItem
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI statusTxt;

    [SerializeField] private Image _status;
    [SerializeField] private Image _shield;
    [SerializeField] private Button button;
    [SerializeField] private Button chatBtn;
    [SerializeField] private Button _sendMsg;

    public FriendData _data;
    
    private void OnEnable()
    {
        button.onClick.AddListener(OpenFriendView);
        chatBtn.onClick.AddListener(OpenChatView);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        chatBtn.onClick.RemoveAllListeners();
    }

    //use this to pass information about the Friend
    public override void Init(UserData data)
    {
        var mm = MainRoot.Instance.mainModel;
        
        ID = data.ID;
        FriendData friend = (FriendData) data;
        _data = friend;
        
        SetStatus(friend.IsStatus);

        Name = friend.Name;
        userName.text = friend.Name;
        levelTxt.text = friend.RankValue.ToString();
        Avatar = friend.Avatar;
        RankValue = data.RankValue;
        RankType = data.RankType;

        data.RankName = mm.RankInfos.Find(x => x.minRank <= _data.RankValue && x.maxRank >= _data.RankValue).Name;
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + friend.RankName);
        
        if (friend.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;

        LoadImage(friend.Avatar, avatar);
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
                statusTxt.text = "Онлайн";
                statusTxt.color = Color.green;
                break;
            case false:
                _status.color = Color.gray;
                statusTxt.text = "Офлайн";
                statusTxt.color = Color.gray;
                break;
        }
    }

    private void OpenFriendView()
    {
        FriendView.Instance.Open(ID);// pass friend info for display here
    }

    private void OpenChatView(){
        var dialog = Friends.Instance.GetDialogElement(_data.ID);
        if (dialog != null)
        dialog.SetUnseenMessages(0);
        
        var mm = MainRoot.Instance.mainModel;
        var unseenMessage = mm.UnseenMessageses.Find(x => x.ID == ID);
        if (unseenMessage != null)
        unseenMessage.CountMessage = 0;
        DialogueView.Instance.Open(_data);
        
    }
}