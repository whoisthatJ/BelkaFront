using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class OnlineGameRankedPanel : MonoBehaviour
{
    public static OnlineGameRankedPanel Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject searchRankedPanel;
    [SerializeField] private GameObject foundRankedPanel;
    [SerializeField] private GameObject pairInvitePanel;
    [SerializeField] private GameObject details;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI rankTxt;
    [SerializeField] private Image rank;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI searchTimerTxt;
    [SerializeField] private Image pendingCircle;

    [SerializeField] private TextMeshProUGUI stakeTxt;
    [SerializeField] private Button readyBtn;
    [SerializeField] private TextMeshProUGUI readyTimerTxt;
    [SerializeField] private Image readyTimerCircle;

    [Space]
    [SerializeField] private Image[] playerAvatars;    
    [Space]
    [SerializeField] private GameObject[] playersReady;
    [SerializeField] private GameObject[] playersRanks;
    [SerializeField] private TextMeshProUGUI[] playersRanksTxt;
    [SerializeField] private GameObject friendsElementPrefab;
    [SerializeField] private GameObject friendsPanel;
    [SerializeField] private Button inviteFriendBtn;
    [SerializeField] private Button closeInviteBtn;

    private string roomID;
    private Tween rotateTween;
    private DateTime searchDateTime;

    private DateTime readyDeadlineDateTime;

    private List<FriendsInviteListElement> friendsList;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        readyBtn.onClick.AddListener(ReadyPressed);
        inviteFriendBtn.onClick.AddListener(InvitePressed);
        closeInviteBtn.onClick.AddListener(CloseInvite);
        ServiceIO.OnPlayersInfoUpdated += PlayerInfoUpdate;
        MainModel.OnRankChanged += RefreshRank;
        MainModel.OnStarsChanged += RefreshStars;
        RefreshRank();
        RefreshStars();
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        readyBtn.onClick.RemoveAllListeners();
        inviteFriendBtn.onClick.RemoveAllListeners();
        closeInviteBtn.onClick.RemoveAllListeners();
        ServiceIO.OnPlayersInfoUpdated -= PlayerInfoUpdate;
        MainModel.OnRankChanged -= RefreshRank;
        MainModel.OnStarsChanged -= RefreshStars;
        if (rotateTween != null)
        {
            rotateTween.Kill();
        }
    }

    private void Update()
    {
        if (searchDateTime != DateTime.MinValue)
        {
            searchTimerTxt.text = (DateTime.Now - searchDateTime).ToString(@"mm\:ss");            
        }
        float secondsLeft = (float)(readyDeadlineDateTime - DateTime.Now).TotalSeconds;
        if (secondsLeft >= 0f)
        {
            readyTimerCircle.fillAmount = secondsLeft / 20f;
            readyTimerTxt.text = ((int)secondsLeft + 1).ToString();
        }
    }

    private void Init()
    {
        friendsList = new List<FriendsInviteListElement>();
        var mm = MainRoot.Instance.mainModel;

        if (mm.FriendDatas.Count < 1)
        {
            MainModel.OnFriendLoaded += () => {
                foreach (var friend in friendsList)
                {
                    if (friend != null)
                        Destroy(friend.gameObject);
                }
                friendsList.Clear();
                for (int i = 0; i < mm.FriendDatas.Count; i++)
                {
                    FillFriendsScroll(mm.FriendDatas[i]);
                }

                //if (mm.FriendDatas.Count > 0) _separationFriend.gameObject.SetActive(true);
            };
        }
        else
        {
            foreach (var friend in friendsList)
            {
                Destroy(friend.gameObject);
            }
            friendsList.Clear();
            for (int i = 0; i < mm.FriendDatas.Count; i++)
            {
                FillFriendsScroll(mm.FriendDatas[i]);
            }

            //if (mm.FriendDatas.Count > 0) _separationFriend.gameObject.SetActive(true);
        }
    }
    private void FillFriendsScroll(FriendData data)
    {
        GameObject temp = Instantiate(friendsElementPrefab, friendsPanel.transform) as GameObject;
        FriendsInviteListElement friendListElement = temp.GetComponent<FriendsInviteListElement>();
        friendListElement.transform.SetAsLastSibling();

        friendListElement.Init(data);
        friendsList.Add(friendListElement);
    }
    public void FriendsClearSelection()
    {
        foreach (FriendsInviteListElement f in friendsList)
        {
            f.invited.SetActive(false);
        }
    }

    public void SetInviteButton(bool ok)
    {
        inviteFriendBtn.interactable = ok;
    }
    public void InvitePressed()
    {

    }
    private void CloseInvite()
    {
        panel.SetActive(false);
        pairInvitePanel.SetActive(false);
    }
    public void Open()
    {
        ServiceIO.Instance.StartSingleSearch();
        panel.SetActive(true);
        SearchGame();
    }

    public void Close()
    {
        panel.SetActive(false);
        ServiceIO.Instance.CancelSearch();
        searchDateTime = DateTime.MinValue;
        if (rotateTween != null)
        {
            rotateTween.Kill();
        }
    }

    public void OpenPair()
    {
        panel.SetActive(true);
        pairInvitePanel.SetActive(true);
        searchRankedPanel.SetActive(false);
        foundRankedPanel.SetActive(false);

    }

    public void SearchGame()
    {
        searchRankedPanel.SetActive(true);
        foundRankedPanel.SetActive(false);
        pairInvitePanel.SetActive(false);
        rotateTween = pendingCircle.rectTransform.DOLocalRotate(Vector3.back * 179f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        searchDateTime = DateTime.Now;
    }
    public void FoundGame(string roomId, bool[] ready)
    {
        searchRankedPanel.SetActive(false);
        foundRankedPanel.SetActive(true);
        searchDateTime = DateTime.MinValue;
        if (roomID != roomId)
        {
            readyDeadlineDateTime = DateTime.Now.AddSeconds(20);
        }
        roomID = roomId;
        PlayerReadyUpdate(ready);
        if (rotateTween != null)
        {
            rotateTween.Kill();
        }
    }

    private void RefreshRank()
    {
        rankTxt.text = MainRoot.Instance.mainModel.Rank.ToString();
        /*var mm = MainRoot.Instance.mainModel;
        rankTxt.color = HardCodeValue.GetColorShieldTitle(mm.RankName, mm.IsLegend);
        Sprite sp = Resources.Load<Sprite>("Shields/" + mm.RankName);

        if (mm.IsLegend)
        {
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        rank.sprite = sp;*/
    }

    private void RefreshStars()
    {
        int c = 0;
        for (int i = 0; i< MainRoot.Instance.mainModel.Stars; i++)
        {
            stars[i].SetActive(true);
            c++;
        }
        for (int i = c; i < 3; i++)
        {
            stars[i].SetActive(false);
        }
    }

    private void ReadyPressed()
    {
        ServiceIO.Instance.SendIsReady(roomID, true);
    }

    public void PlayerReadyUpdate(bool[] ready)
    {
        searchRankedPanel.SetActive(false);
        foundRankedPanel.SetActive(true);
        bool allReady = true;
        for (int i = 0; i < ready.Length; i++)
        {
            playersReady[i].SetActive(ready[i]);
            if(!ready[i])
                allReady = false;
        }
        if (allReady)
        {
            ClearPlayersReady();
            details.SetActive(false);
            Preloader.Instance.LoadNewScene("GameOnline");
        }
    }

    public void PlayerInfoUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            playersRanksTxt[i].text = ServiceIO.Instance.PlayersData[i].RankValue.ToString();
        }
    }

    private void ClearPlayersReady()
    {
        foreach (GameObject g in playersReady)
        {
            g.SetActive(false);
        }
    }

    public void BackToGameSearch()
    {
        SearchGame();
        ClearPlayersReady();
        roomID = string.Empty;
    }

    public void KickedFromMatch()
    {
        ClearPlayersReady();
        roomID = string.Empty;
        panel.SetActive(false);
        MainMenuManager.instance.BlockRankedButtons();
    }
}
