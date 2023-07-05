using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableBuyShopPopup : MonoBehaviour
{
    public static TableBuyShopPopup Instance;
    [SerializeField] private GameObject _container;
    [SerializeField] private Button _close;
    private void Awake(){
        Instance = this;
    }

    private void OnEnable(){
        _close.onClick.AddListener(Close);
    }

    private void OnDisable(){
        _close.onClick.RemoveAllListeners();
    }

    public void Open(){
        Shop.Instance.GetComponent<Canvas>().sortingOrder = 50;
        _container.SetActive(true);
    }

    public void Close(){
        Shop.Instance.GetComponent<Canvas>().sortingOrder = 1;
        _container.SetActive(false);
    }
}
