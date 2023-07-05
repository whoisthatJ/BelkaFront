using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldItemShop : MonoBehaviour
{
    private Button _button;

    private void Start(){
        Init();
    }

    private void Init(){
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PressItem);
    }

    private void OnDestroy(){
        _button.onClick.RemoveAllListeners();
    }

    private void PressItem(){
        GoldsBuyShopPopup.Instance.Open();
    }
}
