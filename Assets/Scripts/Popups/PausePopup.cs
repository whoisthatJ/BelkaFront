using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PausePopup : MonoBehaviour
{
    [SerializeField] private GameObject pausePnl;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutGamePanel;
    [SerializeField] private GameObject tasks;
    [SerializeField] private Button GooglePlayBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button aboutGameBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button switchUser;
    [SerializeField] private Button backBtn;
    [SerializeField] private PopUpUI popUpUI;
    [SerializeField] private Toggle push;
    [SerializeField] private Toggle vibro;
    [SerializeField] private Toggle music;
    [SerializeField] private Toggle sounds;
    [SerializeField] private Toggle nightMode;
    [SerializeField] private TMP_Dropdown language;
    [SerializeField] private TextMeshProUGUI title;

    private bool isPause;

    private void Start()
    {
        sounds.isOn = MainRoot.Instance.userConfig.isSound;
        music.isOn = MainRoot.Instance.userConfig.isMusic;
        vibro.isOn = MainRoot.Instance.userConfig.isVibro;
        SetEnableButton();        
    }

    private void OnEnable()
    {        
        Preloader.BackButtonPressed += BackButton;
        TopBar.Pause += Pause;
        GooglePlayBtn.onClick.AddListener(GooglePlay);
        soundBtn.onClick.AddListener(Sound);
        musicBtn.onClick.AddListener(Music);
        resumeBtn.onClick.AddListener(Resume);
        homeBtn.onClick.AddListener(Home);
        restartBtn.onClick.AddListener(RestartGame);
        settingsBtn.onClick.AddListener(() => { SettingsPanel(true); });
        aboutGameBtn.onClick.AddListener(() => { AboutGamePanel(true); });
        backBtn.onClick.AddListener(() => { AboutGamePanel(false); SettingsPanel(false); });
        vibro.onValueChanged.AddListener(delegate { Vibro(); });
        music.onValueChanged.AddListener(delegate { Music(); });
        sounds.onValueChanged.AddListener(delegate { Sound(); });
        switchUser.onClick.AddListener(SwitchUser);        
    }

    private void OnDisable()
    {
        Preloader.BackButtonPressed -= BackButton;
        TopBar.Pause -= Pause;
        GooglePlayBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        musicBtn.onClick.RemoveAllListeners();
        resumeBtn.onClick.RemoveAllListeners();
        homeBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        aboutGameBtn.onClick.RemoveAllListeners();
        push.onValueChanged.RemoveAllListeners();
        vibro.onValueChanged.RemoveAllListeners();
        music.onValueChanged.RemoveAllListeners();
        sounds.onValueChanged.RemoveAllListeners();
        nightMode.onValueChanged.RemoveAllListeners();
        switchUser.onClick.RemoveAllListeners();
    }

    private void Pause()
    {
        if (!isPause)
        {
            pausePnl.SetActive(true);
            Time.timeScale = 0f;
            Preloader.Instance.AddPanelToTheList(pausePnl);
            isPause = true;
        }
        else
        {
            Resume();
        }
    }

    private void GooglePlay()
    {

    }

    private void Sound()
    {
        if (!sounds.isOn)
        {
            MainRoot.Instance.userConfig.isSound = false;
            CSSoundManager.instance.SetVolumeSound(0);
        }
        else
        {
            MainRoot.Instance.userConfig.isSound = true;
            CSSoundManager.instance.SetVolumeSound(1);
        }        
    }

    private void Music()
    {        
        if (!music.isOn)
        {            
            MainRoot.Instance.userConfig.isMusic = false;
            CSSoundManager.instance.SetVolumeMusic(0);
        }
        else
        {
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

    private void SwitchUser() {
        Debug.Log("Switch User");
    }

    private void SetEnableButton()
    {        
        if (sounds.isOn) sounds.GetComponent<Animator>().Play("On");
        else sounds.GetComponent<Animator>().Play("Off");
        if (music.isOn) music.GetComponent<Animator>().Play("On");
        else music.GetComponent<Animator>().Play("Off");
        if (vibro.isOn) vibro.GetComponent<Animator>().Play("On");
        else vibro.GetComponent<Animator>().Play("Off");
        if (nightMode.isOn) nightMode.GetComponent<Animator>().Play("On");
        else nightMode.GetComponent<Animator>().Play("Off");
        if (push.isOn) push.GetComponent<Animator>().Play("On");
        else push.GetComponent<Animator>().Play("Off");
    }

    private void Resume()
    {
        isPause = false;
        Time.timeScale = 1f;
        if (!settingsPanel.activeSelf && !aboutGamePanel.activeSelf) {
            pausePnl.SetActive(false);
            Preloader.Instance.RemovePanelFromTheList(pausePnl);
        }
        if (settingsPanel.activeSelf) {
            settingsPanel.SetActive(false);
            Preloader.Instance.RemovePanelFromTheList(settingsPanel);
            HideButtons(false);
        }
        if (aboutGamePanel.activeSelf) {
            aboutGamePanel.SetActive(false);
            Preloader.Instance.RemovePanelFromTheList(aboutGamePanel);
            HideButtons(false);
        }        
    }

    private void Home()
    {
        Time.timeScale = 1f;        
        if (!Preloader.Instance.skipPreloaderForLevels) {
            popUpUI.OpenQuestion("Вы уверены? Ляляляля",
                () => { FinishPopup.instance.OpenPanel("Вы покинули игру", 0, 50, false, true, 3);
                    if (GameManager.instance.online)
                        ServiceIO.Instance.ExitGame();
                });
        }            
        else {
            popUpUI.OpenQuestion("Вы уверены? Ляляляля", 
                () => { FinishPopup.instance.OpenPanel("Вы покинули игру", 0, 50, false, true, 3);
                        if (GameManager.instance.online)
                            ServiceIO.Instance.ExitGame();
                });
            
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        pausePnl.SetActive(false);
        if (!Preloader.Instance.skipPreloaderForLevels)
            Preloader.Instance.LoadNewScene("Game");
        else
            Preloader.Instance.LoadScene("Game");
    }

    private void SettingsPanel(bool isActive) {
        HideButtons(isActive);
        settingsPanel.SetActive(isActive);
        if (isActive)
            Preloader.Instance.AddPanelToTheList(settingsPanel);
        if (isActive) SetEnableButton();
        backBtn.GetComponent<Image>().color = new Color(0.6666667f, 0.7098039f, 0.7803922f, 1f);
    }

    private void AboutGamePanel(bool isActive) {

        HideButtons(isActive);
        aboutGamePanel.SetActive(isActive);
        if (isActive)
            Preloader.Instance.AddPanelToTheList(aboutGamePanel);
        backBtn.GetComponent<Image>().color = Color.white;
    }

    private void HideButtons(bool isActive) {
        if (isActive) {
            backBtn.gameObject.SetActive(true);
            resumeBtn.gameObject.SetActive(false);
            aboutGameBtn.gameObject.SetActive(false);
            settingsBtn.gameObject.SetActive(false);
            homeBtn.gameObject.SetActive(false);
            title.gameObject.SetActive(false);
            tasks.SetActive(false);
        } else {
            backBtn.gameObject.SetActive(false);
            title.gameObject.SetActive(true);
            resumeBtn.gameObject.SetActive(true);
            aboutGameBtn.gameObject.SetActive(true);
            settingsBtn.gameObject.SetActive(true);
            homeBtn.gameObject.SetActive(true);
            tasks.SetActive(true);
        }
    }

    private void BackButton()
    {
        if (pausePnl.activeSelf)
            Resume();
        else
            Pause();
    }
}
