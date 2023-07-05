using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FinishPopup : MonoBehaviour
{
    public static FinishPopup instance;

    public static bool isContinue;

    [Header("---Popups---")]
    public CompletePopup completePopup;
    public DefeatPopup defeatPopup;
    public PopUpUI popUpUI;


    [Header("---UI---")]    
    [SerializeField] private Button restartGameBtn;
    [SerializeField] private Button shareBtn;

    [Space(10)]
    [SerializeField] private GameObject container;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform users;

    [Header("---Settings Finish Popup---")]
    [Tooltip("Time show buttons in finish popup")]
    [SerializeField] private float timeShowButtons = 2;

    //Events
    public delegate void CompleteEvent();
    public static CompleteEvent completeOpen;

    public delegate void DefeatEvent();
    public static DefeatEvent defeatOpen;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    private void OnEnable()
    {
        InitListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGameClick();
        }
    }

    private void InitListeners()
    {      
        restartGameBtn.onClick.AddListener(RestartGameClick);
        shareBtn.onClick.AddListener(Share);
    }

    private void RemoveListeners()
    {       
        restartGameBtn.onClick.RemoveAllListeners();
        shareBtn.onClick.RemoveAllListeners();
    }

    public void OpenPanel(string titleText, float timeDelay, float rewardValue, bool isComplete = false, bool isAnticipatoryExit = false, int stars = 0, Action action = null)
    {
        title.text = titleText;
        switch (isComplete)
        {
            case true:
                StartCoroutine(OpenPanelComplete(timeDelay, stars));
                break;
            case false:
                StartCoroutine(OpenPanelDefeat(timeDelay, rewardValue, stars, isAnticipatoryExit));
                break;
        }
        action?.Invoke();
    }

    public void ShowPopUp(string text)
    {
        popUpUI.Open(text);
    }

    private IEnumerator ShowButtons(float time)
    {
        container.SetActive(true);
        yield return new WaitForSecondsRealtime(time);
    }

    private void BackMenuBtnClick()
    {
        if (!Preloader.Instance.skipPreloaderForLevels)
            Preloader.Instance.LoadNewScene("Menu");
        else
            Preloader.Instance.LoadScene("Menu");        
    }

    private void OpenSettingPanel()
    {
        Settings.Instance.OpenSettings();
    }

    private void RestartGameClick()
    {
        if (!Preloader.Instance.skipPreloaderForLevels)
            Preloader.Instance.LoadNewScene("Game");
        else
            Preloader.Instance.LoadScene("Game");        
    }

    #region CompletePanel
    private IEnumerator OpenPanelComplete(float timeDelay, int stars = 0)
    {
        yield return new WaitForSeconds(timeDelay);
        completeOpen?.Invoke();
        StartCoroutine(ShowButtons(0));
        completePopup.gameObject.SetActive(true);
        completePopup.SetStars(stars);
        users.gameObject.SetActive(true);
        users.transform.localPosition = Vector2.zero;
        //title.gameObject.SetActive(true);        
        //if Game Contains Levels
        //nextLevelBtn.gameObject.SetActive(true);
        //LevelsSave.instance.LevelSaveAt(MainRoot.Instance.userConfig.currentLevel, stars);
    }
    #endregion

    #region DefeatPanel
    private IEnumerator OpenPanelDefeat(float timeDelay, float rewardValue, int stars = 0, bool isAnticipatoryExit = false)
    {
        yield return new WaitForSeconds(timeDelay);
        defeatOpen?.Invoke();
        defeatPopup.gameObject.SetActive(true);        
        StartCoroutine(ShowButtons(0));       
        title.gameObject.SetActive(true);
        defeatPopup.SetStars(stars);
        defeatPopup.SetSliderReward(rewardValue);
        if (!isAnticipatoryExit) {
            users.gameObject.SetActive(true);
            users.transform.localPosition = new Vector2(0, 350);
            defeatPopup.SetAnticipatoryExit(false);
            shareBtn.gameObject.SetActive(true);
        }
        else {
            users.gameObject.SetActive(false);
            users.transform.localPosition = new Vector2(0, 0);
            defeatPopup.SetAnticipatoryExit(true);
            shareBtn.gameObject.SetActive(false);
        }
    }
    #endregion

    private void Share() {
        Debug.Log("Share");
    }

/*    #region Custom Call Methods
#if UNITY_EDITOR
    [SHOW_IN_HIER]
    public void CSVictory() {
        OpenPanel("Victory", 1, 50, true, true, 3);
    }
    [SHOW_IN_HIER]
    public void CSDefeat() {
        OpenPanel("Вы покинули игру", 1, 50, false, true, 0);
    }

    [SHOW_IN_HIER]
    public void PopUp() {
        ShowPopUp("Test");
    }
#endif
    #endregion*/
}