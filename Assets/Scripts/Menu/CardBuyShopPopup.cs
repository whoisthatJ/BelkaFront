using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBuyShopPopup : MonoBehaviour{
    public static CardBuyShopPopup Instance;
    [SerializeField] private GameObject _container;
    [SerializeField] private Button _close;
    [SerializeField] private GridLayoutGroup _group;
    private void Awake(){
        Instance = this;
    }

    private void SetSizeCards(){
        switch (ScreenAspectRatio.CalcAspect()){
            case "0,56":
                _group.cellSize = new Vector2(168, 247);
                _group.spacing = new Vector2(80, 80);
                break;
            case "0,50":
                _group.cellSize = new Vector2(168, 247);
                _group.spacing = new Vector2(60, 60);
                break;
            case "0,46":
                _group.cellSize = new Vector2(168/1.1f, 247/1.1f);
                _group.spacing = new Vector2(60, 60);
                break;
            case "0,43":
                _group.cellSize = new Vector2(168/1.1f, 247/1.1f);
                _group.spacing = new Vector2(50, 50);
                break;
        }
    }
    
    private void OnEnable(){
        _close.onClick.AddListener(Close);
    }

    private void OnDisable(){
        _close.onClick.RemoveAllListeners();
    }
    
    public void Open(){
        SetSizeCards();
        Shop.Instance.GetComponent<Canvas>().sortingOrder = 50;
        _container.SetActive(true);
    }

    public void Close(){
        Shop.Instance.GetComponent<Canvas>().sortingOrder = 1;
        _container.SetActive(false);
    }
}
