using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour{
    public static Shop Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button premiumBtn;
    [SerializeField] private Button itemsBtn;
    [SerializeField] private Button chestsBtn;
    [SerializeField] private Button goldBtn;
    [SerializeField] private Button _styles;
    [SerializeField] private List<Panel> _panels;
    [SerializeField] private SwitchSpriteSelectedButtons _pages;

    private MainModel _mm;

    private List<GoldItemShop> _golds = new List<GoldItemShop>();
    private Transform _contentItems;
    private Transform _contentChests;

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        _mm = MainRoot.Instance.mainModel;
        _contentItems = _panels[0].obj.GetComponent<ScrollRect>().content;
        _contentChests = _panels[3].obj.GetComponent<ScrollRect>().content;
        //LoadItems();
    }

    private void OnEnable(){
        premiumBtn.onClick.AddListener(BuyPremium);
        itemsBtn.onClick.AddListener(ItemsButtonPressed);
        chestsBtn.onClick.AddListener(ChestsButtonPressed);
        goldBtn.onClick.AddListener(GoldButtonPressed);
        _styles.onClick.AddListener(StylesButtonPressed);
        MainModel.OnItemShopLoaded += LoadItems;
        MainModel.OnChestShopLoaded += LoadLootBox;
    }

    private void OnDisable(){
        premiumBtn.onClick.RemoveAllListeners();
        itemsBtn.onClick.RemoveAllListeners();
        chestsBtn.onClick.RemoveAllListeners();
        goldBtn.onClick.RemoveAllListeners();
        _styles.onClick.RemoveAllListeners();
        MainModel.OnItemShopLoaded -= LoadItems;
        MainModel.OnChestShopLoaded -= LoadLootBox;
    }

    public void Open(){
        panel.SetActive(true);
    }

    public void Close(){
        panel.SetActive(false);
    }

    public void OpenGoldPage(){
        _pages.Select(_pages.GetImage(goldBtn));
        OpenPanels("Gold");
    }

    //Items
    private void LoadItems(){
        foreach (var shopItem in _mm.ShopItems){
            int category = shopItem.Category;
            Transform _container = _contentItems.Find(category.ToString());
            if (_container != null){
                GameObject obj = Instantiate(Resources.Load("Shop/ItemShop") as GameObject, _container);
                var item = obj.GetComponent<ItemShop>();
                item.Item = shopItem;
                item.SetPreview(shopItem.Picture);
                item.SetPrice(shopItem.Price[0].Price, shopItem.CurrencyType);
                if (shopItem.Discount != null)
                    item.SetDiscount(shopItem.Discount.PercentageValue);
                else item.SetDiscount(0);
            }
        }

        Transform attack = _contentItems.Find("1");
        Transform defence = _contentItems.Find("2");
        Transform gift = _contentItems.Find("4");
        Transform caps = _contentItems.Find("8");

        CreateFakeItems(attack.childCount, attack);
        CreateFakeItems(defence.childCount, defence);
        CreateFakeItems(gift.childCount, gift);
        CreateFakeItems(caps.childCount, caps);
    }

    private void CreateFakeItems(int value, Transform container){
        if (value > 2) return;

        if (value == 1) value = 2;
        else value = 1;

        for (int i = 0; i < value; i++){
            GameObject obj = Instantiate(Resources.Load("Shop/ItemShop") as GameObject, container);
            obj.GetComponent<Button>().enabled = false;
            obj.transform.Find("Fake").gameObject.SetActive(true);
        }
    }

    //LootBox

    private void LoadLootBox(){
        for (int i = 0; i < _mm.ShopLootBox.Count; i++){
            var shopChest = _mm.ShopLootBox[(_mm.ShopLootBox.Count - 1) - i];
            GameObject obj = Instantiate(Resources.Load("Shop/ChestItemShop") as GameObject, _contentChests);
            var item = obj.GetComponent<ChestItemShop>();
            item.Item = shopChest;
            item.Item.Picture = "Chests/Chest_" + i;
            item.SetPreview(i);
            item.SetPrice(shopChest.Prices[0].Amount, shopChest.CurrencyType);
        }
    }

    private void BuyPremium(){
        BuyPremiumMenu.Instance.Open();
    }

    private void OpenPanels(string key){
        foreach (var panel in _panels){
            panel.obj.SetActive(false);
        }

        var p = _panels.Find(x => x.Name == key);
        p?.obj.SetActive(true);
    }

    private void ItemsButtonPressed(){
        OpenPanels("Item");
    }

    private void ChestsButtonPressed(){
        OpenPanels("Chest");
    }

    private void GoldButtonPressed(){
        OpenPanels("Gold");
    }

    private void StylesButtonPressed(){
        OpenPanels("Style");
    }
}


[System.Serializable]
public class Panel{
    public string Name;
    public GameObject obj;
}