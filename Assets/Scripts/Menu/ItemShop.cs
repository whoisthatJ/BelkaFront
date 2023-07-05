using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour{

    public Item Item{ get; set; }
    
    [SerializeField] private TextMeshProUGUI _discount;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Image _preview;
    
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
        ItemBuyShopPopup.Instance.Open(Item);
    }

    public void SetDiscount(float value){
        if (value < 1) _discount.transform.parent.gameObject.SetActive(false);
        else{
            _discount.transform.parent.gameObject.SetActive(true);
            _discount.text = "-" + value + "%";
        }
    }

    public void SetPreview(string url){
        ServiceResources.LoadImage(url, _preview);
    }

    public void SetPrice(float value, int currencyType){
        string currency = string.Empty;

        currency = currencyType == 1 ? currency = "<sprite=1>" : currency = "<sprite=0>   ";
        if (value > 0)
        _price.text = currency + value.ToString();
        else _price.text = "Бесплатно";

        float percentage = 0;
        if (Item.Discount != null)
            percentage = Item.Discount.PercentageValue;
        float discount = 0;
        
        if (percentage > 0){
            discount = value * percentage / 100;
        }
        
        if (Item.Price.Count > 0)
            _price.text = currency + "  " + (value - discount).ToString();
        else _price.text = "Бесплатно";
    }
}
