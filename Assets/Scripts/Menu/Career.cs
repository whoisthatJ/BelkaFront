using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Career : MonoBehaviour
{
    public static Career Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button rankBtn;
    [SerializeField] private TextMeshProUGUI rankNumberTxt;
    [SerializeField] private TextMeshProUGUI rankNameTxt;
    [SerializeField] private Button stagesBtn;
    [SerializeField] private TextMeshProUGUI stageNumberTxt;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        rankBtn.onClick.AddListener(OpenRanks);
        stagesBtn.onClick.AddListener(OpenStages);
    }

    private void OnDisable()
    {
        rankBtn.onClick.RemoveAllListeners();
        stagesBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        ShowRank();
        ShowStage();
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void ShowRank()//subscribe to rank change
    {
        rankNumberTxt.text = "1";
        rankNameTxt.text = "Newbie";
    }

    private void ShowStage()//subscribe to stage change
    {
        stageNumberTxt.text = "1";
    }

    private void OpenRanks()
    {
        RanksView.Instance.Open();
    }

    private void OpenStages()
    {
        StagesView.Instance.Open();
    }
}
