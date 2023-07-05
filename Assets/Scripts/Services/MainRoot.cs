using UnityEngine;

public class MainRoot : MonoBehaviour
{
	public delegate void SoundEvent (bool isFlag);
	public static SoundEvent SoundEnable;

	public delegate void MusicEvent(bool isFlag);
	public static MusicEvent MusicEnable;

    public static MainRoot Instance
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

    private const string FILE_USER_CONFIG = "user_config";

    public UserConfig userConfig;
    public MainModel mainModel;

    private void Start()
    {
        InitProgress();
    }

    void InitProgress()
    {
        userConfig = (UserConfig)ServiceGameSave.Instance.Load(FILE_USER_CONFIG);
        if (userConfig == null)
        {
            Caching.ClearCache();
            userConfig = UserConfig.GetDefault();                     
        }

        mainModel = new MainModel();
    }
    
    public void SaveProgress()
    {
        ServiceGameSave.Instance.Save(userConfig, FILE_USER_CONFIG);
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProgress();
        }
    }
}