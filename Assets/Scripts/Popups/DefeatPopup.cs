using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class DefeatPopup : MonoBehaviour
{
	/*[SerializeField] private Slider timeSlider;

	[Space(10)]
	[SerializeField] private TextMeshProUGUI timerText;

	[Space(10)]
	[SerializeField] private Button continueAdsBtn;
	[SerializeField] private Button continueSoftBtn;*/


    [SerializeField] private List<GameObject> starsImage = new List<GameObject>();
    [SerializeField] private GameObject starsContainer;
    [SerializeField] private Slider reward;
    [SerializeField] private TextMeshProUGUI rewardTxt;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button revengeBtn;
    

    private void OnEnable() {
        homeBtn.onClick.AddListener(BackMenu);
        exitBtn.onClick.AddListener(BackMenu);
        revengeBtn.onClick.AddListener(Revenge);
    }

    
    private void OnDisable() {
        homeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        revengeBtn.onClick.RemoveAllListeners();
    }
    private void BackMenu() {
        if (!Preloader.Instance.skipPreloaderForLevels)
            Preloader.Instance.LoadNewScene("Menu");
        else
            Preloader.Instance.LoadScene("Menu");
    }

    private void Revenge() {
        Debug.Log("Revenge");
    }

    public void SetStars(int stars = 0) {
        if (stars > 0) {
            starsContainer.SetActive(true);
            for (int i = 0; i < stars; i++) {
                starsImage[i].SetActive(true);
                StartCoroutine(Stars(stars));
            }
        } else {
            starsContainer.SetActive(false);
        }
    }
    IEnumerator Stars(int stars) {
        for (int i = 0; i < stars; i++) {
            starsImage[i].SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void SetSliderReward(float value) {        
        StartCoroutine(SetSliderValue(value));
    }

    public void SetAnticipatoryExit(bool isFlag) {
        if (isFlag) {
            exitBtn.gameObject.SetActive(false);
            revengeBtn.gameObject.SetActive(false);
            rewardTxt.gameObject.SetActive(true);
            description.gameObject.SetActive(true);
            homeBtn.gameObject.SetActive(true);
            reward.gameObject.SetActive(true);            
        }
        else {
            exitBtn.gameObject.SetActive(true);
            revengeBtn.gameObject.SetActive(true);
            rewardTxt.gameObject.SetActive(false);
            description.gameObject.SetActive(false);
            homeBtn.gameObject.SetActive(false);
            reward.gameObject.SetActive(false);
        }
    }

    private IEnumerator SetSliderValue(float value) {        
        float speed = value / 1;
        while (reward.value < value) {
            reward.value += Time.deltaTime * speed;
            rewardTxt.text = Mathf.RoundToInt(reward.value) + " %";
            yield return null;
        }
        reward.value = value;
        rewardTxt.text = Mathf.RoundToInt(reward.value) + " %";
    }

    /*//View buttons in popup defeat
    public void SetButtonsContinue(bool isContinueAds, bool isContinueSoft)
	{
		continueAdsBtn.gameObject.SetActive(isContinueAds);
		continueSoftBtn.gameObject.SetActive(isContinueSoft);
	}

	public void SetTimerEnable(bool isFlag)
	{
		timeSlider.gameObject.SetActive(false);
		timerText.gameObject.SetActive(false);
	}

	#region Continue ADS
	public void ContinueAdsBtnClik()
	{
		//Replay Ads	
		if (ServiceFirebaseAnalytics.Instance.GetAdsRate() > 0)
		{
		   *//* if (!ServiceIronSource.Instance.CallShowRewardedVideo(WatchAdCallback))
		    {*//*
				FinishPopup.instance.ShowPopUp(Lean.Localization.LeanLocalization.GetTranslationText("No Ads"));
		    //}          
		}       
		else
		{
		    WatchAdCallback(true);
		}
	} 

	private void WatchAdCallback(bool success)
	{
		if (success)
		{
			//Continue Game
			FinishPopup.isContinue = true;
		}
	}
	#endregion

	#region Continue Soft
	public void ContinueSoftBtnClick()
    {
		//Replay Soft Currency
		FinishPopup.isContinue = true;
    }
	#endregion

	
	//Update timer slider for continue buttons
	public void SetSliderTimer(float timer, bool isTextTimer = true, bool isSlider = true)
	{
		if (isSlider)
		{
			currentTimer = timer;
			timeSlider.value = timeSlider.maxValue;
			Sequence timerUpdate = DOTween.Sequence().SetUpdate(true);
			timerUpdate.Append(timeSlider.DOValue(0, timer).SetEase(Ease.Linear).OnComplete(() =>
			{
				timeSlider.gameObject.SetActive(false);
				continueAdsBtn.gameObject.SetActive(false);
				continueSoftBtn.gameObject.SetActive(false);
				timerText.gameObject.SetActive(false);
			}).OnStart(() =>
			{
				if (isTextTimer)
					timerUpdate.OnUpdate(TweenCallback);
				else
					timerText.gameObject.SetActive(false);
			}));
		}
		else
		{
			timeSlider.gameObject.SetActive(false);
			timerText.gameObject.SetActive(false);
		}
	}

	private float currentTimer;
	private void TweenCallback()
	{
		currentTimer -= Time.deltaTime;
		timerText.text = Mathf.RoundToInt(currentTimer).ToString();
	}*/
}
