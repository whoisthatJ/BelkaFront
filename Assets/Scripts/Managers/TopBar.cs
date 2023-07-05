using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopBar : MonoBehaviour
{
    [SerializeField] Button settingsBtn;
    [SerializeField] Button pauseBtn;
    [SerializeField] TextMeshProUGUI softCurrencyTxt;

    public delegate void PauseEvent();
    public static PauseEvent Pause;

    // Use this for initialization
    void Start()
    {
        if (Preloader.Instance.GetCurrentScene() == "Game" || Preloader.Instance.GetCurrentScene() == "GameOnline")
        {
            settingsBtn.gameObject.SetActive(false);
            pauseBtn.gameObject.SetActive(true);
        }
        else
        {
            settingsBtn.gameObject.SetActive(true);
            pauseBtn.gameObject.SetActive(false);
        }
        RenderSoftCurrency();
    }

    private void OnEnable()
    {
        FinishPopup.completeOpen += PlayerWon;
        FinishPopup.defeatOpen += PlayerLost;
        MainModel.OnSoftCurrencyAmountChanged += RenderSoftCurrency;
        settingsBtn.onClick.AddListener(OpenSettings);
        pauseBtn.onClick.AddListener(TogglePause);
    }

    private void OnDisable()
    {
        FinishPopup.completeOpen -= PlayerWon;
        FinishPopup.defeatOpen -= PlayerLost;
        MainModel.OnSoftCurrencyAmountChanged -= RenderSoftCurrency;
        settingsBtn.onClick.RemoveAllListeners();
        pauseBtn.onClick.RemoveAllListeners();
    }

    private void RenderSoftCurrency()
    {
        softCurrencyTxt.text = MainRoot.Instance.mainModel.SoftCurrency.ToString();
    }

    private void OpenSettings()
    {
        Settings.Instance.OpenSettings();
    }

    private void TogglePause()
    {
        if (Pause != null)
            Pause();
    }
    //change top bar after player won
    private void PlayerWon()
    {
        pauseBtn.gameObject.SetActive(false);
        settingsBtn.gameObject.SetActive(true);
    }
    //change top bar after player lost
    private void PlayerLost()
    {
        pauseBtn.gameObject.SetActive(false);
        settingsBtn.gameObject.SetActive(true);
    }
}
