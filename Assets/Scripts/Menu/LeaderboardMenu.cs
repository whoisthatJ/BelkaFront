using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardMenu : MonoBehaviour
{
    public static LeaderboardMenu Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject scrollToday;
    [SerializeField] private GameObject scrollWeek;
    [SerializeField] private GameObject scrollAllTime;
    [SerializeField] private Transform scrollParentToday;
    [SerializeField] private Transform scrollParentWeek;
    [SerializeField] private Transform scrollParentAllTime;
    [SerializeField] private GameObject leaderboardElementPrefab;
    [SerializeField] private Button TodayBtn;
    [SerializeField] private Button WeekBtn;
    [SerializeField] private Button AllTimeBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        TodayBtn.onClick.AddListener(Today);
        WeekBtn.onClick.AddListener(Weekly);
        AllTimeBtn.onClick.AddListener(AllTime);
    }

    private void OnDisable()
    {
        TodayBtn.onClick.RemoveAllListeners();
        WeekBtn.onClick.RemoveAllListeners();
        AllTimeBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        FillScroll();
        Weekly();
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void FillScroll()
    {
        //get info about leaderboards and instantiate buttons
        //today
        GameObject temp = Instantiate(leaderboardElementPrefab, scrollParentToday) as GameObject;
        LeaderboardButton leaderboardButton = temp.GetComponent<LeaderboardButton>();
        leaderboardButton.Init();//pass info about leaderboard
        //weekly
        temp = Instantiate(leaderboardElementPrefab, scrollParentWeek) as GameObject;
        leaderboardButton = temp.GetComponent<LeaderboardButton>();
        leaderboardButton.Init();//pass info about leaderboard
        //alltime
        temp = Instantiate(leaderboardElementPrefab, scrollParentAllTime) as GameObject;
        leaderboardButton = temp.GetComponent<LeaderboardButton>();
        leaderboardButton.Init();//pass info about leaderboard
    }

    private void Today()
    {
        scrollToday.SetActive(true);
        scrollWeek.SetActive(false);
        scrollAllTime.SetActive(false);
        TodayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        TodayBtn.transform.GetChild(1).gameObject.SetActive(true);
        WeekBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        WeekBtn.transform.GetChild(1).gameObject.SetActive(false);
        AllTimeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        AllTimeBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void Weekly()
    {
        scrollToday.SetActive(false);
        scrollWeek.SetActive(true);
        scrollAllTime.SetActive(false);
        TodayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255); ;
        TodayBtn.transform.GetChild(1).gameObject.SetActive(false);
        WeekBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        WeekBtn.transform.GetChild(1).gameObject.SetActive(true);
        AllTimeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        AllTimeBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void AllTime()
    {
        scrollToday.SetActive(false);
        scrollWeek.SetActive(false);
        scrollAllTime.SetActive(true);
        TodayBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        TodayBtn.transform.GetChild(1).gameObject.SetActive(false);
        WeekBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        WeekBtn.transform.GetChild(1).gameObject.SetActive(false);
        AllTimeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        AllTimeBtn.transform.GetChild(1).gameObject.SetActive(true);
    }
}
