using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Preloader : MonoBehaviour
{
    public static Preloader Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [SerializeField] string loadingSceneName;
    [SerializeField] GameObject preloaderCanvas;
    [SerializeField] Slider sliderBar;
    [SerializeField] bool skipTutorial;
    [SerializeField] bool skipPreloader;
    public bool skipPreloaderForLevels;
    public bool isOpenAllLevels;
    [SerializeField] Image sceneSplash;
    [SerializeField] Sprite[] splashSprites;

    [SerializeField] GameObject firstLoadCanvas;
    [SerializeField] private TextMeshProUGUI _percentage;
    
    public delegate void BackButtonDelegate();
    public static event BackButtonDelegate BackButtonPressed; //<- event called when the back button is pressed
    private List<GameObject> openedPanels = new List<GameObject>(); //<- list stores all panels that are currently opened

    string currentScene;

    private int lastSplashIndex;
    public Sprite lastSplashSprite;

    [Header("Test")]

    [SerializeField] private bool activateLoginTest;
    [SerializeField] private LoginTest loginTest;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        /* if (skipTutorial)
         {
             PlayerPrefs.SetInt("tutor", 1);
         }
         if (PlayerPrefs.GetInt("tutor") == 0)
         {
             LoadNewScene("Menu");
         }
         else
         {
             LoadNewScene(loadingSceneName);
         }*/
        
        if (!activateLoginTest)
            FirstLoad();

        loginTest.SetPanelActive(activateLoginTest);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void FirstLoad()
    {
        StartCoroutine(FirstLoadCor());
    }

    private IEnumerator FirstLoadCor()
    {
        firstLoadCanvas.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Menu");
        ServiceWeb.Instance.Init();

        yield return new WaitUntil(() => ServiceWeb.Instance.IsLoggedIn);
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            sliderBar.value = progress;
            _percentage.text = Mathf.RoundToInt(progress * 100) + " %";
            yield return null;
        }
        
        ServiceIO.Instance.Connect();

        firstLoadCanvas.SetActive(false);
    }

    public void LoadNewScene(string sceneName)
    {
        openedPanels.Clear();
        if (!skipPreloader)
            StartCoroutine(LoadNewSceneCor(sceneName));
        else LoadScene(sceneName);
        currentScene = sceneName;
        if (currentScene == "GameOnline")
            preloaderCanvas.transform.GetChild(0).gameObject.SetActive(false);
    }
    bool nextTimes;
    IEnumerator LoadNewSceneCor(string sceneName)
    {
        preloaderCanvas.SetActive(true);
        sliderBar.value = 0f;

        if (nextTimes)
        {
            if (sceneSplash != null)
                sceneSplash.gameObject.SetActive(true);
            if (splashSprites.Length > 0)
            {
                lastSplashIndex = Random.Range(0, splashSprites.Length);
                sceneSplash.sprite = splashSprites[lastSplashIndex];
                lastSplashSprite = splashSprites[lastSplashIndex];
            }
        }
        yield return new WaitForSeconds(.223f);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            sliderBar.value = progress;
            _percentage.text = Mathf.RoundToInt(progress * 100) + " %";
            yield return null;
        }
        preloaderCanvas.SetActive(false);
        nextTimes = true;
    }

    public string GetCurrentScene()
    {
        return currentScene;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HardWareBtnClick();
    }

    #region Quit Panel
    //call this when opening a new panel
    public void AddPanelToTheList(GameObject panel)
    {
        openedPanels.Add(panel);
    }
    //call this when closing a panel
    public void RemovePanelFromTheList(GameObject panel)
    {
        if (openedPanels.Contains(panel))
            openedPanels.Remove(panel);
    }

    private void HardWareBtnClick()
    {
        //closes opened panels if the list is not empty
        if (openedPanels.Count > 0)
        {
            openedPanels.Clear();
            if (BackButtonPressed != null)
                BackButtonPressed();
        }
        else
        {
            //quits the game, pauses or returns to main menu
            if (currentScene == "Menu")
                Confirmation.Instance.Open("Quit?", YesQuitClick);
            else if (currentScene == "Game")
            {
                if (BackButtonPressed != null)
                    BackButtonPressed();
            }
            else
                LoadNewScene("Menu");
        }
    }

    private void YesQuitClick()
    {
        Application.Quit();
    }
    #endregion
}
