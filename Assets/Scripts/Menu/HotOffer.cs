using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotOffer : MonoBehaviour
{
    [SerializeField] private Button purchaseBtn;

    private void OnEnable()
    {
        purchaseBtn.onClick.AddListener(Purchase);
    }

    private void OnDisable()
    {
        purchaseBtn.onClick.RemoveAllListeners();
    }

    private void Purchase()
    {

    }
}
