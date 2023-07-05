using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using Random = System.Random;

public class DialogueView : MonoBehaviour
{
    public static DialogueView Instance;
    
    public string ToID{ get; set; }
    public bool IsOpen;
    
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private CSButtonTMP profileBtn;
    [SerializeField] private TextMeshProUGUI _userName;
    [SerializeField] private Button playBtn;
    [SerializeField] private RectTransform giftPanel;
    [SerializeField] private Button giftBtn;
    [SerializeField] private Button closeGiftBtn;
    [SerializeField] private TextMeshProUGUI chatTxt;
    [SerializeField] private TMP_InputField inputTxt;
    [SerializeField] private Button sendMsgBtn;
    [SerializeField] private MessageChatElement prefabChatElement;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect chatScroll;
    [SerializeField] private GameObject giftPreview;
    [SerializeField] private DateChatElement _dateChatPrefab;
    [SerializeField] private GameObject _textAreaInput;
    [SerializeField] private GameObject _blockUser;
    [SerializeField] private TextMeshProUGUI _blockUserTxt;
    [SerializeField] private Sprite _empty;
    [SerializeField] private Image _loader;
    
    private List<MessageChatElement> listMessages = new List<MessageChatElement>();
    
    private string _name;
    private MainModel _mm;
    private RectTransform _inputText;
    private RectTransform _chatRect;
    private RectTransform _caretInput;
    private RectTransform _textComponent;
    private ContentSizeFitter _fitter;
    private Vector2 _caretOffsetMin;
    private Vector2 _caretOffsetMax;
    private Vector2 _textComponentOffsetMin;
    private Vector2 _textComponentOffsetMax;
    private int _countSkip;
    private bool _isLoaded;
    private void Awake()
    {
        Instance = this;
    }

    private void Start(){
        _mm = MainRoot.Instance.mainModel;
        _inputText = inputTxt.GetComponent<RectTransform>();
        _chatRect = chatScroll.GetComponent<RectTransform>();
        _fitter = _inputText.GetComponent<ContentSizeFitter>();
        
        _textComponent = inputTxt.textComponent.GetComponent<RectTransform>();
        _textComponentOffsetMin = _textComponent.offsetMin;
        _textComponentOffsetMax = _textComponent.offsetMax;
        
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        giftBtn.onClick.AddListener(OpenGiftPanel);
        closeGiftBtn.onClick.AddListener(CloseGiftPanel);
        sendMsgBtn.onClick.AddListener(delegate { SendMessageUser(inputTxt.text, true); });
        MainModel.OnHistoryChatLoaded += LoadHistory;
        inputTxt.onValueChanged.AddListener(SetText);
        ServiceIO.OnRemoveBlackListReceive += RemoveFromBlackListRecevie;
        ServiceIO.OnAddToBlackListReceive += AddToBlackListReceive;
        profileBtn.onClick.AddListener(OpenFriendView);
        chatScroll.onValueChanged.AddListener(Scroll);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        closeGiftBtn.onClick.RemoveAllListeners();
        giftBtn.onClick.RemoveAllListeners();
        sendMsgBtn.onClick.RemoveAllListeners();
        chatScroll.onValueChanged.RemoveAllListeners();
        MainModel.OnHistoryChatLoaded -= LoadHistory;
        inputTxt.onValueChanged.RemoveAllListeners();
        ServiceIO.OnRemoveBlackListReceive -= RemoveFromBlackListRecevie;
        ServiceIO.OnAddToBlackListReceive -= AddToBlackListReceive;
        profileBtn.onClick.RemoveAllListeners();
    }

    private void Scroll(Vector2 vec){
        if (!_isLoaded && content.anchoredPosition.y + 100 < -content.sizeDelta.y && listMessages.Count > 0){
            ServiceWeb.Instance.GetHistoryChatUser(ToID, _countSkip);
            _isLoaded = true;
            _loader.gameObject.SetActive(true);
        }
    }
    
    public void Open(UserData data){

        if (!IsOpen){
            IsOpen = true;
            panel.SetActive(true);
            if (_caretInput == null){
                _caretInput = _textAreaInput.transform.Find("InputMessage Input Caret").GetComponent<RectTransform>();
                _caretOffsetMin = _caretInput.offsetMin;
                _caretOffsetMax = _caretInput.offsetMax;
            }

            if (!string.IsNullOrEmpty(data.Message)) chatTxt.text = data.Message;
            ToID = data.ID;

            _name = data.Name;
            _userName.text = data.Name;
            if (!string.IsNullOrEmpty(data.Avatar))
                Loader.Instance.LoadImage(data.Avatar, profileBtn.image, () => {
                    if (profileBtn.image.sprite != null){
                        float aspectRatio = (float) profileBtn.image.sprite.texture.width /
                                            (float) profileBtn.image.sprite.texture.height;
                        profileBtn.image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
                        //Sprite size bug fixed
                        var sprite = profileBtn.image.sprite;
                        profileBtn.image.sprite = null;
                        profileBtn.image.sprite = sprite;
                    }
                });

            ServiceWeb.Instance.GetHistoryChatUser(ToID, 0);
            ServiceIO.Instance.SetUnseenMessages(ToID);

            if (!data.IsBlocked){
                if (data.IsBlockedYou){
                    _blockUser.SetActive(true);
                    _blockUserTxt.text = "Вы заблокированы";
                }
            }
            else{
                _blockUser.SetActive(true);
                _blockUserTxt.text = "Пользователь заблокирован";
            }
        }

        gameObject.GetComponent<Canvas>().sortingOrder = 12;
    }

    private void SetText(string text){
        
    }

    private void Update(){
        if (_caretInput != null && inputTxt.textComponent.textInfo.characterCount == 1){
            _caretInput.offsetMin = _caretOffsetMin;
            _caretInput.offsetMax = _caretOffsetMax;
            _textComponent.offsetMin = _textComponentOffsetMin;
            _textComponent.offsetMax = _textComponentOffsetMax;
        }
        if (inputTxt.textComponent.textInfo.lineCount < 7){
            _fitter.SetLayoutVertical();
        }
        
        _chatRect.offsetMin = new Vector2(_chatRect.offsetMin.x, _inputText.anchoredPosition.y + _inputText.sizeDelta.y  + 40);

        if (_isLoaded){
            float rot = _loader.transform.eulerAngles.z;
            rot -= 5;
            _loader.transform.eulerAngles = new Vector3(0,  0, rot);
        }
    }

    public void Close()
    {
        foreach (Transform tr in content){
            Destroy(tr.gameObject);
        }

        ToID = string.Empty;
        listMessages.Clear();
        profileBtn.image.sprite = _empty;
        float aspectRatio = (float) profileBtn.image.sprite.texture.width /
                            (float) profileBtn.image.sprite.texture.height;
        profileBtn.image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        panel.SetActive(false);
        _blockUser.SetActive(false);
        gameObject.GetComponent<Canvas>().sortingOrder = 12;
        _mm.HistoryUser.Clear();
        _countSkip = 0;
        IsOpen = false;
    }

    public void SendMessageUser(string msg, bool isUser) {
        if (!string.IsNullOrEmpty(msg)){
            MessageChatElement msgChat = Instantiate(prefabChatElement, content);
            msgChat.gameObject.SetActive(true);
            msgChat.SetMessage(msg, DateTime.Now,  isUser);
            listMessages.Add(msgChat);
            ServiceIO.Instance.SendMessageUser(ToID, msg);
            inputTxt.text = "";
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
            MessageData data = new MessageData();
            data.Message = msg;
            data.UserName = _userName.text;
            data.FromUserID = ToID;
            Friends.Instance.AddDialog(data);
            _countSkip++;
        }
    }

    public void ReceiveMessage(string msg){
        MessageChatElement msgChat = Instantiate(prefabChatElement, content);
        msgChat.gameObject.SetActive(true);
        msgChat.SetMessage(msg, DateTime.Now,  false);
        listMessages.Add(msgChat);
    }
    
    public void OpenGiftPreview() {
        giftPreview.SetActive(true);
        giftPanel.GetComponent<ScrollRect>().enabled = false;
    }

    private void OpenFriendView()
    {
        FriendView.Instance.Open(ToID);// pass friend info for display here
        gameObject.GetComponent<Canvas>().sortingOrder = 11;
    }
    
    private List<DateTime> _dates = new List<DateTime>();
    public void LoadHistory(){
        for (int i = 0; i < _mm.HistoryUser.Count; i++){

            if (_dates.Count > 0){
                int index = _dates.FindIndex(x => x.Date.ToShortDateString() == _mm.HistoryUser[i].Date.ToShortDateString());
                if (index == -1){
                    SetDateElement(i);
                }
            }
            else{
                SetDateElement(i);
            }
            
            MessageChatElement msgChat = Instantiate(prefabChatElement, content);
            msgChat.gameObject.SetActive(true);
            
            if (_isLoaded) msgChat.transform.SetSiblingIndex(i + 1);
            
            if (_mm.HistoryUser[i].FromUserID == ToID){
                msgChat.SetMessage(_mm.HistoryUser[i].Message, _mm.HistoryUser[i].Date, false);
            }
            else{
                msgChat.SetMessage(_mm.HistoryUser[i].Message, _mm.HistoryUser[i].Date, true);
            }
            listMessages.Add(msgChat);
            _countSkip++;
        }

        _isLoaded = false;
        _loader.gameObject.SetActive(false);
    }

    private void SetDateElement(int index){
        DateChatElement element = Instantiate(_dateChatPrefab, content);
        element.Date.text = _mm.HistoryUser[index].Date.ToShortDateString();
        _dates.Add(_mm.HistoryUser[index].Date);
    }
    
    private void OpenGiftPanel() {
        
        giftPanel.DOAnchorPosY(560, .1f);
    }

    private void CloseGiftPanel() {
        giftPanel.DOAnchorPosY(-560, .1f);
        giftPreview.SetActive(false);
        giftPanel.GetComponent<ScrollRect>().enabled = true;
    }

    private void RemoveFromBlackListRecevie(string ID){
        if (ToID == ID){
            var user = Friends.Instance.GetBlackListElement(ID);
            if (user == null)
            _blockUser.SetActive(false);
            else{
                _blockUser.SetActive(true);
                _blockUserTxt.text = "Игрок в черном списке";
            }
        }
    }

    private void AddToBlackListReceive(string ID){
        if (ToID == ID){
            _blockUser.SetActive(true);
            _blockUserTxt.text = "Игрок заблокировал вас";
        }
    }
}
