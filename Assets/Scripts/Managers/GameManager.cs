using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    public bool online;
    private void Start()
    {
        InitAudio();
    }

    private void Update()
    {

    }

    private void OnApplicationFocus(bool isFocus)
    {
        if (!isFocus)
        {
            //ServiceFirebaseAnalytics.Instance.LogEnableMusic(Convert.ToInt32((MainRoot.Instance.userConfig.isMusic)));
            //ServiceFirebaseAnalytics.Instance.LogEnableSound(Convert.ToInt32((MainRoot.Instance.userConfig.isSound)));            
#if !UNITY_EDITOR
			TopBar.Pause?.Invoke();
#endif
        }
    }

    #region Audio
    private void InitAudio()
    {
        CSSoundManager.instance.PlayLoopingMusic((int)CSSoundManager.instance.musicLevel);
    }
    #endregion

    #region Preview Methods
    private void MethodsTest()
    {
        //---Finish---
        //FinishPopup --> CompletePopup
        //            --> DefeatPopup

        //Call Victory
        FinishPopup.instance.OpenPanel("Win", 0, 50, true, false, 2);
        //Call Defeat
        FinishPopup.instance.OpenPanel("Defeat", 0, 50, false, false, 0);

        //---Audio---
        //Volume Music Range (0, 1)
        CSSoundManager.instance.SetVolumeMusic(0);
        //Volume Sound Range(0, 1)
        CSSoundManager.instance.SetVolumeSound(0);
        //Play Sound --> SoundManager is in Preloader
        CSSoundManager.instance.PlaySound(0);
        //Play Music --> SoundManager is in Preloader
        CSSoundManager.instance.PlayMusic(0);
        //Play Looping Sound --> SoundManager is in Preloader add Sound in SoundSources
        CSSoundManager.instance.PlayLoopingSound(0);
        //Play Looping Music --> SoundManager is in Preloader add Music in MusicSources
        CSSoundManager.instance.PlayLoopingMusic(0);
        //Stop All Audio
        CSSoundManager.instance.StopAll();
        //Stop Sound 
        CSSoundManager.instance.StopSound(0);
        //Stop Music
        CSSoundManager.instance.StopMusic(0);

        //Display Variables
        Debug.Log(CSPlayerPrefs.GetFloat("TestVar"));
        Debug.Log(CSPlayerPrefs.GetVector3("TestVector"));
    }
    #endregion
}
