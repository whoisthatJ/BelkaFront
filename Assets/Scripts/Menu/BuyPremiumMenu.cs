using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPremiumMenu : MonoBehaviour
{
    public static BuyPremiumMenu Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;

    public delegate void CloseBuyPremium();

    public static CloseBuyPremium OnCloseBuyPremium;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        buyBtn.onClick.AddListener(BuyButtonPressed);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        OnCloseBuyPremium?.Invoke();
        panel.SetActive(false);
    }

    private void BuyButtonPressed(){
        ServiceWeb.Instance.GetPremiumRequest();
    }
}
