using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class BlackListElement : FriendListItem{
    
    public TextMeshProUGUI dateTxt;
    [FormerlySerializedAs("userNameTxt")] public TextMeshProUGUI userName;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI statusTxt;

    [SerializeField] private Image _status;
    [SerializeField] private Image _shield;
    [SerializeField] private Button lockBtn;
    [SerializeField] private Button _button;

    private BlackListUserData _data;
    public override void Init(UserData data){
        BlackListUserData blackList = (BlackListUserData) data;
        _data = blackList;
        
        ID = blackList.UserID;
        userName.text = data.Name;
       
        levelTxt.text = blackList.RankValue.ToString();
        LoadImage(blackList.Avatar, avatar);
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + blackList.RankType);
        
        if (blackList.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
    }

    public override string GetName(){
        return userName.text;
    }

    private void OnEnable(){
        lockBtn.onClick.AddListener(UnLock);
        _button.onClick.AddListener(OpenFriendView);
    }

    private void OnDisable(){
        lockBtn.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OpenFriendView);
    }

    private void UnLock(){
        ServiceWeb.Instance.RemoveBlackList(_data.UserID);
        Friends.Instance.RemoveFromBlackList(_data.UserID);
    }
    
    private void OpenFriendView()
    {
        FriendView.Instance.Open(ID);// pass friend info for display here
    }
    
}