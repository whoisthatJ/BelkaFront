using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TasksMenu : MonoBehaviour
{
    public static TasksMenu Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button gameBtn;
    [SerializeField] private Button conversationBtn;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject conversationPanel;
    [SerializeField] private GameObject dailyTasksPanel;

    [Space]
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private GameObject taskBottomPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(Close);
        startBtn.onClick.AddListener(StartButtonPressed);
        gameBtn.onClick.AddListener(GameButtonPressed);
        conversationBtn.onClick.AddListener(ConversationButtonPressed);
        MenuBottomBar.BottomBarButtonPressed += Close;
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();
        gameBtn.onClick.RemoveAllListeners();
        conversationBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= Close;
    }

    private void Start()
    {
        FillDailyTasks();
        FillStartPanel();
    }

    public void Open() {
        if (startPanel.activeSelf) {
            startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
            gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            startBtn.transform.GetChild(1).gameObject.SetActive(true);
            gameBtn.transform.GetChild(1).gameObject.SetActive(false);
            conversationBtn.transform.GetChild(1).gameObject.SetActive(false);
        } else if (gamePanel.activeSelf) {
            gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
            startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            startBtn.transform.GetChild(1).gameObject.SetActive(false);
            gameBtn.transform.GetChild(1).gameObject.SetActive(true);
            conversationBtn.transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (conversationPanel.activeSelf) {
            conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
            gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
            startBtn.transform.GetChild(1).gameObject.SetActive(false);
            gameBtn.transform.GetChild(1).gameObject.SetActive(false);
            conversationBtn.transform.GetChild(1).gameObject.SetActive(true);
        }
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void StartButtonPressed()
    {
        startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
        gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        conversationPanel.SetActive(false);
        startBtn.transform.GetChild(1).gameObject.SetActive(true);
        gameBtn.transform.GetChild(1).gameObject.SetActive(false);
        conversationBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void GameButtonPressed()
    {
        gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
        startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        conversationPanel.SetActive(false);
        startBtn.transform.GetChild(1).gameObject.SetActive(false);
        gameBtn.transform.GetChild(1).gameObject.SetActive(true);
        conversationBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void ConversationButtonPressed()
    {
        conversationBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.3058824f, 0.3254902f, 0.427451f, 1f);
        gameBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        startBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
        startPanel.SetActive(false);
        gamePanel.SetActive(false);
        conversationPanel.SetActive(true);
        startBtn.transform.GetChild(1).gameObject.SetActive(false);
        gameBtn.transform.GetChild(1).gameObject.SetActive(false);
        conversationBtn.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void FillDailyTasks()
    {
        for (int i = 0; i < 3; i++)
            Instantiate(taskPrefab, dailyTasksPanel.transform);
    }

    private void FillStartPanel()
    {
        for (int i = 0; i < 5; i++)
            Instantiate(taskBottomPrefab, startPanel.transform);
    }
}
