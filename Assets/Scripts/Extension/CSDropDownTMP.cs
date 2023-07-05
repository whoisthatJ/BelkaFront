using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSDropDownTMP : TMP_Dropdown{
    public delegate void OpenList(GameObject obj);

    public OpenList OnOpenList;
    
    public delegate void CloseList();

    public CloseList OnCloseList;
    
    protected override GameObject CreateDropdownList(GameObject template){
        GameObject obj = base.CreateDropdownList(template); 
        OnOpenList?.Invoke(obj);
        StartCoroutine(Wait(obj));
        return obj;
    }

    private IEnumerator Wait(GameObject obj){
        yield return new WaitForSeconds(.1f);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(388.8f, 419f);
    }
    
    protected override void DestroyDropdownList(GameObject dropdownList){
        OnCloseList?.Invoke();
        base.DestroyDropdownList(dropdownList);
    }
}

