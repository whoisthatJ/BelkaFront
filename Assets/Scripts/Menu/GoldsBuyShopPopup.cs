using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldsBuyShopPopup : MonoBehaviour
{
    public static GoldsBuyShopPopup Instance;

    [SerializeField] private GameObject _container;
    [SerializeField] private Button _close;

    private GoldsBuyShopPopup(){
        Instance = this;
    }
    
    private void OnEnable(){
        _close.onClick.AddListener(Close);
    }

    private void OnDisable(){
        _close.onClick.RemoveAllListeners();
    }

    public void Open(){
        gameObject.SetActive(true);
    }

    public void Close(){
        gameObject.SetActive(false);
    }
}
