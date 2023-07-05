using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestBuyShopPopup : MonoBehaviour
{
    public static ChestBuyShopPopup Instance;
    [SerializeField] private CSButtonTMP _balance;
    [SerializeField] private Button _question;
    [SerializeField] private CSDropDownTMP _count;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _rarity;
    [SerializeField] private Image _preview;
    [SerializeField] private QuestionPanel _panel;
    
    private LootBoxItem _item;
    private Dictionary<int, int> _rarities = new Dictionary<int, int>();
    private ChestBuyShopPopup(){
        Instance = this;
    }

    private void OnEnable(){
        _balance.onClick.AddListener(MoveToShop);
        _count.OnOpenList += CreateListDropDown;
        _count.onValueChanged.AddListener(OnValueChanged);
        _question.onClick.AddListener(OpenQuestion);
    }

    private void OnDisable(){
        _balance.onClick.RemoveAllListeners();
        _count.OnOpenList -= CreateListDropDown;
        _count.onValueChanged.RemoveAllListeners();
        _question.onClick.RemoveAllListeners();
    }

    public void Open(LootBoxItem item){
        _item = item;
        gameObject.SetActive(true);
        
        if (item.Prices.Count < 2) _count.gameObject.SetActive(false);
        else _count.gameObject.SetActive(true);
        
        for (int i = 0; i < item.Prices.Count; i++){
            var optionData = new CSDropDownTMP.OptionData();
            optionData.text = item.Prices[i].Quantity + " штуки";
            _count.options.Add(optionData);    
        }

        _count.captionText.text = _count.options[0].text;
        LoadPreview();
        _title.text = item.Title;
        _description.text = $"При открытии выпадает {item.ItemCount} случайных предмета из категорий:";

        string currencyType = String.Empty;
        currencyType = item.CurrencyType == 1 ? currencyType = "<sprite=1>" : currencyType = "<sprite=0>";

        if (item.Prices.Count > 0)
            _balance.Text.text = "Купить   " + currencyType + "  " + (item.Prices[0].Amount).ToString();
        else _balance.Text.text = "Бесплатно";
        
        Rarity();
    }

    private void LoadPreview(){
        _preview.sprite = Resources.Load<Sprite>(_item.Picture);
        
        float aspectRatio = (float) _preview.sprite.texture.width / (float) _preview.sprite.texture.height;
        _preview.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        //Sprite size bug fixed
        var sprite = _preview.sprite;
        _preview.sprite = null;
        _preview.sprite = sprite;
    }
    
    public void Close(){
        _rarities.Clear();
        gameObject.SetActive(false);
    }

    private void MoveToShop(){
        Shop.Instance.OpenGoldPage();
        gameObject.SetActive(false);
    }
    
    private void CreateListDropDown(GameObject obj){
        StartCoroutine(WaitList(obj));
    }

    private void OnValueChanged(int value){

        string currencyType = String.Empty;
        currencyType = _item.CurrencyType == 1 ? currencyType = "<sprite=1>" : currencyType = "<sprite=0>";

        if (_item.Prices.Count > 0)
            _balance.Text.text = "Купить   " + currencyType + "  " + (_item.Prices[value].Amount).ToString();
        else _balance.Text.text = "Бесплатно";
    }
    
    private IEnumerator WaitList(GameObject obj){
        yield return new WaitForSeconds(.05f);
        RectTransform rect = obj.GetComponent<ScrollRect>().content;
        Transform tr = rect.GetChild(_count.value + 1);
        tr.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.63f, 0.46f, 1f);
    }

    private void OpenQuestion(){
        _panel.Open(_item.Title, _rarities, _item.ItemCount);
        _panel.SetPreview(_preview.sprite);
    }

    private void Rarity(){
        _rarities.Clear();
        string text = string.Empty;
        foreach (var rarity in _item.ItemChances){
            if (rarity.Percentange > 0){
                if (rarity.RaretyCategory == 1) text = "Частые";
                else if (rarity.RaretyCategory == 2) text += ", <color=#328549>Редкие</color>";
                else if (rarity.RaretyCategory == 3) text += ", <color=#5628A1>Уникальные</color>";
                else if (rarity.RaretyCategory == 4) text += ", <color=#FFA072>Легендарные</color>";
                _rarities.Add(rarity.RaretyCategory, rarity.Percentange);
            }
        }
        
        if (text[0].Equals(',')) text = text.Remove(0, 1);
        
        _rarity.text = text;
        Vector2 textSize = _rarity.GetPreferredValues(text);
        _rarity.GetComponent<RectTransform>().sizeDelta = textSize;
    }
}
