using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompleteTask : MonoBehaviour
{
    [SerializeField] private Button pickupGiftBtn;

    private void OnEnable() {
        pickupGiftBtn.onClick.AddListener(PickUpGift);
    }

    private void OnDisable() {
        pickupGiftBtn.onClick.RemoveAllListeners();
    }

    private void PickUpGift() {
        Debug.Log("Pick up gift");
    }
}
