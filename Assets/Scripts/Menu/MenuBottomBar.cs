using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuBottomBar : MonoBehaviour{
    public static MenuBottomBar Instance;
    
    [SerializeField] private Button careerBtn;
    [SerializeField] private Button friendsBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button leaderboardBtn;
    [SerializeField] private Button shopBtn;

    [Space]
    [SerializeField] private Sprite career;
    [SerializeField] private Sprite careerHighlighted;
    [SerializeField] private Sprite friends;
    [SerializeField] private Sprite friendsHighlighted;
    [SerializeField] private Sprite leaderboard;
    [SerializeField] private Sprite leaderboardHighlighted;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite shopHighlighted;

    [SerializeField] private Image _notification;
    
    public delegate void BottomBarButtonDelegate();
    public static event BottomBarButtonDelegate BottomBarButtonPressed;

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        MainRoot.Instance.mainModel.IsNotification();
    }

    public void SetActiveNotification(bool isActive){
        _notification.gameObject.SetActive(isActive);
    }
    
    private void OnEnable()
    {
        careerBtn.onClick.AddListener(Careers);
        friendsBtn.onClick.AddListener(OpenFriends);
        homeBtn.onClick.AddListener(Home);
        leaderboardBtn.onClick.AddListener(Leaderboard);
        shopBtn.onClick.AddListener(OpenShop);
        MenuTopBar.TopBarButtonPressed += Home;
        MainModel.OnNotification += SetActiveNotification;
    }

    private void OnDisable()
    {
        careerBtn.onClick.RemoveAllListeners();
        friendsBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        leaderboardBtn.onClick.RemoveAllListeners();
        shopBtn.onClick.RemoveAllListeners();
        MenuTopBar.TopBarButtonPressed -= Home;
        MainModel.OnNotification -= SetActiveNotification;
    }

    private void Careers()
    {
        BottomBarButtonPressed();
        CloseLeaderboard();
        CloseFriends();
        CloseShop();
        //Career.Instance.Open();
        RanksView.Instance.Open();
        careerBtn.transform.GetChild(0).gameObject.SetActive(true);
        careerBtn.transform.GetComponent<Image>().sprite = careerHighlighted;
    }

    private void OpenFriends()
    {
        BottomBarButtonPressed();
        CloseLeaderboard();
        CloseCareer();
        CloseShop();
        Friends.Instance.Open();
        friendsBtn.transform.GetChild(0).gameObject.SetActive(true);
        friendsBtn.transform.GetComponent<Image>().sprite = friendsHighlighted;
    }

    private void Home()
    {
        BottomBarButtonPressed();
        CloseLeaderboard();
        CloseCareer();
        CloseFriends();
        CloseShop();
    }

    private void Leaderboard()
    {
        BottomBarButtonPressed();
        CloseCareer();
        CloseFriends();
        CloseShop();
        LeaderboardMenu.Instance.Open();
        leaderboardBtn.transform.GetChild(0).gameObject.SetActive(true);
        leaderboardBtn.transform.GetComponent<Image>().sprite = leaderboardHighlighted;
    }

    private void OpenShop()
    {
        BottomBarButtonPressed();
        CloseCareer();
        CloseFriends();
        CloseLeaderboard();
        Shop.Instance.Open();
        shopBtn.transform.GetChild(0).gameObject.SetActive(true);
        shopBtn.transform.GetComponent<Image>().sprite = shopHighlighted;
    }

    private void CloseCareer()
    {
        careerBtn.transform.GetChild(0).gameObject.SetActive(false);
        careerBtn.transform.GetComponent<Image>().sprite = career;
        Career.Instance.Close();
    }

    private void CloseFriends()
    {
        friendsBtn.transform.GetChild(0).gameObject.SetActive(false);
        friendsBtn.transform.GetComponent<Image>().sprite = friends;
        Friends.Instance.Close();
    }

    private void CloseLeaderboard()
    {
        leaderboardBtn.transform.GetChild(0).gameObject.SetActive(false);
        leaderboardBtn.transform.GetComponent<Image>().sprite = leaderboard;
        LeaderboardMenu.Instance.Close();
    }

    private void CloseShop()
    {
        shopBtn.transform.GetChild(0).gameObject.SetActive(false);
        shopBtn.transform.GetComponent<Image>().sprite = shop;
        Shop.Instance.Close();
    }
}
