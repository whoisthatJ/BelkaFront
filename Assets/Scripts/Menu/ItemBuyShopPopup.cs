using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuyShopPopup : MonoBehaviour
{
    public static ItemBuyShopPopup Instance;
    [SerializeField] private CSDropDownTMP _count;
    [SerializeField] private CSButtonTMP _balance;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _preview;
    
    private Item _item;
    private int _countItems;
    private float _price;
    
    private ItemBuyShopPopup(){
        Instance = this;
    }

    private void Start(){
        ServiceWeb.OnBuyItem += BuyItemAnswer;
    }

    private void OnEnable(){
        _balance.onClick.AddListener(BuyItem);
        _count.OnOpenList += CreateListDropDown;
        _count.onValueChanged.AddListener(OnValueChanged);
        
    }

    private void OnDisable(){
        _balance.onClick.RemoveAllListeners();
        _count.OnOpenList -= CreateListDropDown;
        _count.onValueChanged.RemoveAllListeners();
    }

    private void OnDestroy(){
        ServiceWeb.OnBuyItem -= BuyItemAnswer;
    }

    public void Open(Item item){
        _item = item;
        gameObject.SetActive(true);
        _count.options.Clear();

        if (item.Price.Count < 2){
            _count.gameObject.SetActive(false);
            _countItems = 1;
        }
        else{
            _count.gameObject.SetActive(true);
            _countItems = item.Price[0].Quantity;
        }
        
        for (int i = 0; i < item.Price.Count; i++){
            var optionData = new CSDropDownTMP.OptionData();
            optionData.text = item.Price[i].Quantity + " штуки";
            _count.options.Add(optionData);    
        }

        _count.captionText.text = _count.options[0].text;
        LoadPreview(item.Picture);
        _title.text = item.Name;
        _description.text = item.Description;

        string currencyType = String.Empty;
        currencyType = item.CurrencyType == 1 ? currencyType = "<sprite=1>" : currencyType = "<sprite=0>";

        float percentage = 0;
        if (item.Discount != null)
            percentage = item.Discount.PercentageValue;
        
        _price = item.Price[0].Price;
        float discount = 0;
        
        if (percentage > 0){
            discount = item.Price[0].Price * percentage / 100;
            _price = (item.Price[0].Price - discount);
        }
        
        if (item.Price.Count > 0)
            _balance.Text.text = "Купить   " + currencyType + "  " + (item.Price[0].Price - discount).ToString();
        else _balance.Text.text = "Бесплатно";
    }

    public void Close(){
        _count.ClearOptions();
        gameObject.SetActive(false);
    }

    private void LoadPreview(string url){
        ServiceResources.LoadImage(url, _preview);
    }
    
    private void BuyItem(){
        var mm = MainRoot.Instance.mainModel;

        if (mm.SoftCurrency >= _price){    
            ServiceWeb.Instance.BuyItemShop(_item.ID, _countItems, _price);
        }
        else{
            Shop.Instance.OpenGoldPage();
        }

        Close();
    }

    private void CreateListDropDown(GameObject obj){
        StartCoroutine(WaitList(obj));
    }

    private void OnValueChanged(int value){

        string currencyType = String.Empty;
        currencyType = _item.CurrencyType == 1 ? currencyType = "<sprite=1>" : currencyType = "<sprite=0>";
        
        float percentage = 0;
        if (_item.Discount != null)
            percentage = _item.Discount.PercentageValue;
        
        _price = _item.Price[value].Price;
        
        float discount = 0;
        
        if (percentage > 0){
            discount = _item.Price[value].Price * percentage / 100;
            _price = _item.Price[value].Price - discount;
        }
        
        if (_item.Price.Count > 0)
            _balance.Text.text = "Купить   " + currencyType + "  " + (_item.Price[value].Price - discount).ToString();
        else _balance.Text.text = "Бесплатно";
        
        _countItems = _item.Price[value].Quantity;
        
    }

    private IEnumerator WaitList(GameObject obj){
        yield return new WaitForSeconds(.05f);
        RectTransform rect = obj.GetComponent<ScrollRect>().content;
        Transform tr = rect.GetChild(_count.value + 1);
        tr.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.63f, 0.46f, 1f);
    }

    private void BuyItemAnswer(string ID, int count, float price){
        MainRoot.Instance.mainModel.SoftCurrency -= (int)price;
    }
}
