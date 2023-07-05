using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestItemShop : MonoBehaviour
{
    public LootBoxItem Item{ get; set; }
    
    //[SerializeField] private TextMeshProUGUI _discount;
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
        ChestBuyShopPopup.Instance.Open(Item);
    }
    
    public void SetPreview(string url){
        ServiceResources.LoadImage(url, _preview);
    }

    public void SetPreview(int number){
        _preview.sprite = Resources.Load<Sprite>("Chests/Chest_" + number);
        float aspectRatio = (float) _preview.sprite.texture.width / (float) _preview.sprite.texture.height;
        _preview.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        //Sprite size bug fixed
        var sprite = _preview.sprite;
        _preview.sprite = null;
        _preview.sprite = sprite;
    }
    
    public void SetPrice(float value, int currencyType){
        string currency = string.Empty;

        currency = currencyType == 1 ? currency = "<sprite=1>" : currency = "<sprite=0>   ";
        if (value > 0)
            _price.text = currency + value.ToString();
        else _price.text = "Бесплатно";
        
        if (Item.Prices.Count > 0)
            _price.text = currency + "  " + (value).ToString();
        else _price.text = "Бесплатно";
    }
}
