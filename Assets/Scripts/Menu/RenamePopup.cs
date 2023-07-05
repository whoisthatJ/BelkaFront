using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RenamePopup : MonoBehaviour{
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private TextMeshProUGUI _descriptionName;
    [SerializeField] private Button _rename;
    [SerializeField] private TextMeshProUGUI _renameTitle;
    [SerializeField] private ContentSizeFitter _fitter;
    [SerializeField] private Button _balance;
    [SerializeField] private Button _close;
    [SerializeField] private TextMeshProUGUI _notEnough;
    [SerializeField] private VerticalLayoutGroup _group;
    
    private int _countRename;
    
    private void Update(){
        if (_name.textComponent.textInfo.lineCount < 15){
            _fitter.SetLayoutVertical();
        }
    }

    private void OnEnable(){
        _rename.onClick.AddListener(Rename);
        ServiceWeb.OnRenameError += RenameError;
        _balance.onClick.AddListener(MoveShop);
        MainModel.OnUserNameChanged += RefreshUserName;
        MainModel.OnHardCurrencyAmountChanged += HardCurrencyAmountChanged;
        _close.onClick.AddListener(Close);
    }
    
    private void OnDisable(){
        _rename.onClick.RemoveAllListeners();
        ServiceWeb.OnRenameError -= RenameError;
        _balance.onClick.RemoveAllListeners();
        MainModel.OnUserNameChanged -= RefreshUserName;
        MainModel.OnHardCurrencyAmountChanged -= HardCurrencyAmountChanged;
        _close.onClick.RemoveAllListeners();
    }
    
    public void Open(){
        var mm = MainRoot.Instance.mainModel;
        
        _countRename = mm.CountRename;
        
        gameObject.SetActive(true);
        if (_countRename < 1){
            _renameTitle.text = "Бесплатно";
            _rename.image.color = new Color(0.4901961f, 0.3568628f, 0.8235295f, 1);  
            _renameTitle.color = Color.white;
            _group.padding.bottom = 50;
        }
        else{
            if (mm.HardCurrency >= 5){
                _renameTitle.text = "Купить <sprite=0> 5";
                _rename.image.color = new Color(0.4901961f, 0.3568628f, 0.8235295f, 1);
                _renameTitle.color = Color.white;
                _group.padding.bottom = 50;
            }
            else if (mm.HardCurrency < 5){
                _renameTitle.text = "Купить <sprite=0> 5";
                _rename.interactable = false;
                _balance.gameObject.SetActive(true);
                _notEnough.gameObject.SetActive(true);
                _group.padding.bottom = 130;
                _rename.image.color = new Color(0.9490197f, 0.9568628f, 0.9764706f, 1);
                _renameTitle.color = Color.black;
            }
        }
    }

    public void Close(){
        _descriptionName.gameObject.SetActive(false);
        _rename.interactable = true;
        _rename.image.color = new Color(0.4901961f, 0.3568628f, 0.8235295f, 1);
        _renameTitle.color = Color.white;
        _group.padding.bottom = 50;
        gameObject.SetActive(false);
    }
    
    public void SetName(string text){
        _name.text = text;
    }

    public void RenameError(){
        _descriptionName.text = "Данный никнейм уже занят. Выберите другой.";
        _descriptionName.gameObject.SetActive(true);
        _rename.interactable = true;
    }
    
    private void HardCurrencyAmountChanged(){
        var mm = MainRoot.Instance.mainModel;
        if (mm.HardCurrency >= 5){
            _balance.gameObject.SetActive(false);
            _notEnough.gameObject.SetActive(false);
        }
    }
    private void Rename(){
         var mm = MainRoot.Instance.mainModel;

         if (!string.IsNullOrEmpty(_name.text)){
             if (mm.HardCurrency >= 5 || _countRename < 1){
                 ServiceWeb.Instance.Rename(_name.text);
                 _rename.interactable = false;
             }
         }
    }

    private void RefreshUserName()
    {
        var mm = MainRoot.Instance.mainModel;
        mm.CountRename++;
        if (mm.CountRename > 1) mm.HardCurrency -= 5;
        Close();
    }
    
    private void MoveShop(){
        Shop.Instance.Open();
        ProfileMenu.Instance.Close();
        Close();
    }
}
