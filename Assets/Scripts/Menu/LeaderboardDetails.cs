using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardDetails : MonoBehaviour
{
    public static LeaderboardDetails Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button backBtn;
    [SerializeField] private TextMeshProUGUI leaderboardNameTxt;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(Close);
        MenuBottomBar.BottomBarButtonPressed += Close;
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= Close;
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    //pass info about leaderboard here
    public void DisplayInfo()
    {
        leaderboardNameTxt.text = "Leaderboard Name";
    }
}
