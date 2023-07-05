using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [Header("---UI---")]
    [SerializeField] Button startGameBtn;
    [SerializeField] Button startRankedSoloGameBtn;
    [SerializeField] Button startRankedPairGameBtn;
    [SerializeField] Button reconnectRankedGameBtn;
    [SerializeField] Button startFriendsGameBtn;
    [SerializeField] Button participateInTournamentBtn;
    [SerializeField] Button yourTasksBtn;
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;
    [SerializeField] GameObject blockedBtn;
    [SerializeField] TextMeshProUGUI blockedTimeTxt;

    [SerializeField] private List<RectTransform> listGamePanels;

    private int indexGamePlanel = 0;
    private Vector2 startPos;
    private DateTime blockTime;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InitListeners();
        MainModel.OnActiveGameLoaded += SetReconnectButton;
        MainModel.OnGameSearchBlockTimeLoaded += SetBlockedButton;
    }

    private void OnDisable()
    {
        RemoveListeners();
        MainModel.OnActiveGameLoaded -= SetReconnectButton;
        MainModel.OnGameSearchBlockTimeLoaded -= SetBlockedButton;
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        Application.targetFrameRate = 60;
        InitAudio();
        FinishPopup.isContinue = false;
        startPos = listGamePanels[1].localPosition;
        leftBtn.gameObject.SetActive(false);
        MoveRight();
        blockTime = MainRoot.Instance.mainModel.GameSearchBlockTime;
        yield return new WaitForSeconds(2f);
        //Analytics
        //Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen("MainMenu", "Menu");        
    }

    private void InitListeners()
    {
        startGameBtn.onClick.AddListener(StartNewGame);
        startRankedSoloGameBtn.onClick.AddListener(StartNewOnlineGame);
        startRankedPairGameBtn.onClick.AddListener(StartNewOnlinePairGame);
        reconnectRankedGameBtn.onClick.AddListener(Reconnect);
        startFriendsGameBtn.onClick.AddListener(StartNewOnlineGame);
        participateInTournamentBtn.onClick.AddListener(ParticipateInTournament);
        yourTasksBtn.onClick.AddListener(OpenTasksPanel);
        leftBtn.onClick.AddListener(MoveLeft);
        rightBtn.onClick.AddListener(MoveRight);
    }

    private void RemoveListeners()
    {
        startGameBtn.onClick.RemoveAllListeners();
        startRankedSoloGameBtn.onClick.RemoveAllListeners();
        startRankedPairGameBtn.onClick.RemoveAllListeners();
        reconnectRankedGameBtn.onClick.RemoveAllListeners();
        startFriendsGameBtn.onClick.RemoveAllListeners();
        participateInTournamentBtn.onClick.RemoveAllListeners();
        yourTasksBtn.onClick.RemoveAllListeners();
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
    }

    private void StartNewGame()
    {
        //Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen("Game", "Game");
        Preloader.Instance.LoadNewScene("Game");
    }

    private void StartNewOnlineGame()
    {
        OnlineGameRankedPanel.Instance.Open();
    }
    private void StartNewOnlinePairGame()
    {
        OnlineGameRankedPanel.Instance.OpenPair();
    }
    private void ParticipateInTournament()
    {
        Tournaments.Instance.Open();
    }

    private void Reconnect()
    {
        Preloader.Instance.LoadNewScene("GameOnline");
    }

    private void MoveLeft() {
        if (indexGamePlanel > 0) {
            listGamePanels[indexGamePlanel].localPosition = startPos;
            indexGamePlanel--;            
            listGamePanels[indexGamePlanel].localPosition = Vector2.zero;
            if (!rightBtn.gameObject.activeSelf)
                rightBtn.gameObject.SetActive(true);
        }
        if (indexGamePlanel == 0) {
            leftBtn.gameObject.SetActive(false);
        }
    }

    private void MoveRight() {
        if (indexGamePlanel < listGamePanels.Count - 1) {
            listGamePanels[indexGamePlanel].localPosition = -startPos;
            indexGamePlanel++;            
            listGamePanels[indexGamePlanel].localPosition = Vector2.zero;
            if (!leftBtn.gameObject.activeSelf)
            leftBtn.gameObject.SetActive(true);
        }
        if (indexGamePlanel == listGamePanels.Count - 1) {
            rightBtn.gameObject.SetActive(false);
        }
    }

    private void OpenTasksPanel()
    {
        TasksMenu.Instance.Open();
    }

    private void OnApplicationFocus(bool isFocus)
    {
        if (!isFocus)
        {
            //ServiceFirebaseAnalytics.Instance.LogEnableMusic(Convert.ToInt32((MainRoot.Instance.userConfig.isMusic)));
            //ServiceFirebaseAnalytics.Instance.LogEnableSound(Convert.ToInt32((MainRoot.Instance.userConfig.isSound)));           
        }
    }

    public void BlockRankedButtons()
    {
        MainRoot.Instance.mainModel.GameSearchBlockTime = DateTime.Now.AddMinutes(5);
    }

    private void SetReconnectButton()
    {
        if (MainRoot.Instance.mainModel.IsActiveGame)
        {
            startRankedPairGameBtn.gameObject.SetActive(false);
            startRankedSoloGameBtn.gameObject.SetActive(false);
            reconnectRankedGameBtn.gameObject.SetActive(true);
        }
        else
        {
            startRankedPairGameBtn.gameObject.SetActive(true);
            startRankedSoloGameBtn.gameObject.SetActive(true);
            reconnectRankedGameBtn.gameObject.SetActive(false);
        }
    }

    private void SetBlockedButton()
    {
        blockTime = MainRoot.Instance.mainModel.GameSearchBlockTime;
    }
    private void Update()
    {
        if (blockTime != DateTime.MinValue)
        {
            if (blockTime > DateTime.Now)
            {
                startRankedPairGameBtn.gameObject.SetActive(false);
                startRankedSoloGameBtn.gameObject.SetActive(false);
                reconnectRankedGameBtn.gameObject.SetActive(false);
                blockedBtn.SetActive(true);
                blockedTimeTxt.text = (blockTime - DateTime.Now).ToString(@"mm\:ss");
            }
            else
            {
                blockedBtn.SetActive(false);
                SetReconnectButton();
                blockTime = DateTime.MinValue;
            }
        }
    }

    #region Audio
    private void InitAudio()
    {
        CSSoundManager.instance.StopMusic(1);
        CSSoundManager.instance.PlayLoopingMusic(0);
    }
    #endregion
}
