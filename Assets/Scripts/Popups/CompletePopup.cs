using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class CompletePopup : MonoBehaviour
{
    [SerializeField] private List<GameObject> starsImage = new List<GameObject>();
    [SerializeField] private List<GameObject> users = new List<GameObject>();
    [SerializeField] private GameObject starsContainer;
    [SerializeField] private LikePanel likePanel;
    [SerializeField] private Button likeBtn;
    [SerializeField] private Button disLikeBtn;
    [SerializeField] private Button revengeBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button shareBtn;
    [SerializeField] private TextMeshProUGUI winScoreTxt;

    private void OnEnable() {
        likeBtn.onClick.AddListener(OpenLikePanel);
        disLikeBtn.onClick.AddListener(OpenLikePanel);
        revengeBtn.onClick.AddListener(Revenge);
        homeBtn.onClick.AddListener(BackMenu);
        shareBtn.onClick.AddListener(ShareUsers);
    }

   

    private void OnDisable() {
        likeBtn.onClick.RemoveAllListeners();
        disLikeBtn.onClick.RemoveAllListeners();
        revengeBtn.onClick.RemoveAllListeners();
        shareBtn.onClick.RemoveAllListeners();
    }
    private void ShareUsers() {
        Debug.Log("Share");
    }

    private void BackMenu() {
        if (!Preloader.Instance.skipPreloaderForLevels)
            Preloader.Instance.LoadNewScene("Menu");
        else
            Preloader.Instance.LoadScene("Menu");
    }

    public void SetStars(int stars = 0)
    {
        if (stars > 0)
        {
            starsContainer.SetActive(true);
            for (int i = 0; i < stars; i++)
            {
                starsImage[i].SetActive(true);
                StartCoroutine(Stars(stars));
            }
        }
        else
        {
            starsContainer.SetActive(false);
        }
    }
    IEnumerator Stars(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            starsImage[i].SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }   

    private void Revenge() {
        Debug.Log("Revenge");
    }

    public void OpenLikePanel() {        
        foreach (var user in users) {
            user.SetActive(false);
        }
        winScoreTxt.gameObject.SetActive(false);
        likeBtn.gameObject.SetActive(false);
        disLikeBtn.gameObject.SetActive(false);
        revengeBtn.gameObject.SetActive(false);
        homeBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        likePanel.gameObject.SetActive(true);
    }

    public void CloseLikePanel() {
        foreach (var user in users) {
            user.SetActive(true);
        }
        winScoreTxt.gameObject.SetActive(true);
        likeBtn.gameObject.SetActive(true);
        disLikeBtn.gameObject.SetActive(true);
        revengeBtn.gameObject.SetActive(true);
        homeBtn.gameObject.SetActive(true);
        shareBtn.gameObject.SetActive(true);
        likePanel.gameObject.SetActive(false);
    }
}

