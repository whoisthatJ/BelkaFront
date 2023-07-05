using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{

    public static Settings Instance;

    [SerializeField] private GameObject settingsPnl;
    [SerializeField] private Button GooglePlayBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button removeAdsBtn;
    [SerializeField] private Button restorePurchasesBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button gameRulesBtn;
    [SerializeField] private Toggle vibro;
    [SerializeField] private Toggle music;
    [SerializeField] private Toggle sounds;
    [SerializeField] private TMP_Dropdown language;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        sounds.isOn = MainRoot.Instance.userConfig.isSound;
        music.isOn = MainRoot.Instance.userConfig.isMusic;
        vibro.isOn = MainRoot.Instance.userConfig.isVibro;
        SetEnableButton();
    }

    private void OnEnable()
    {
        Preloader.BackButtonPressed += Back;
        GooglePlayBtn.onClick.AddListener(GooglePlay);
        soundBtn.onClick.AddListener(Sound);
        musicBtn.onClick.AddListener(Music);
        removeAdsBtn.onClick.AddListener(RemoveAds);
        restorePurchasesBtn.onClick.AddListener(RestorePurchases);
        backBtn.onClick.AddListener(Back);
        vibro.onValueChanged.AddListener(delegate { Vibro(); });
        music.onValueChanged.AddListener(delegate { Music(); });
        sounds.onValueChanged.AddListener(delegate { Sound(); });
        gameRulesBtn.onClick.AddListener(GameRules);
    }

    private void OnDisable()
    {
        Preloader.BackButtonPressed -= Back;
        GooglePlayBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        musicBtn.onClick.RemoveAllListeners();
        removeAdsBtn.onClick.RemoveAllListeners();
        restorePurchasesBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        vibro.onValueChanged.RemoveAllListeners();
        music.onValueChanged.RemoveAllListeners();
        sounds.onValueChanged.RemoveAllListeners();
        gameRulesBtn.onClick.RemoveAllListeners();
    }

    public void OpenSettings()
    {
        settingsPnl.SetActive(true);
        Preloader.Instance.AddPanelToTheList(settingsPnl);
        //Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen("Settings", "Menu");
        SetEnableButton();
    }

    private void GooglePlay()
    {

    }

    private void Sound() {
        if (!sounds.isOn) {
            MainRoot.Instance.userConfig.isSound = false;
            CSSoundManager.instance.SetVolumeSound(0);
        } else {
            MainRoot.Instance.userConfig.isSound = true;
            CSSoundManager.instance.SetVolumeSound(1);
        }
    }

    private void Music() {
        if (!music.isOn) {
            MainRoot.Instance.userConfig.isMusic = false;
            CSSoundManager.instance.SetVolumeMusic(0);
        } else {
            MainRoot.Instance.userConfig.isMusic = true;
            CSSoundManager.instance.SetVolumeMusic(1);
        }
    }

    private void Vibro() {
        if (!vibro.isOn) {
            MainRoot.Instance.userConfig.isVibro = false;
        } else {
            MainRoot.Instance.userConfig.isVibro = true;
        }
    }

    private void GameRules() {
        AboutPopUp.Instance.Open();
    }
    private void SetEnableButton()
    {        
        if (sounds.isOn) sounds.GetComponent<Animator>().Play("On");
        else sounds.GetComponent<Animator>().Play("Off");
        if (music.isOn) music.GetComponent<Animator>().Play("On");
        else music.GetComponent<Animator>().Play("Off");
        if (vibro.isOn) vibro.GetComponent<Animator>().Play("On");
        else vibro.GetComponent<Animator>().Play("Off");
    }

    private void RemoveAds()
    {

    }

    private void RestorePurchases()
    {

    }

    private void Back()
    {
        settingsPnl.SetActive(false);
        Preloader.Instance.RemovePanelFromTheList(settingsPnl);
        //Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen("MainMenu", "Menu");
    }
}
