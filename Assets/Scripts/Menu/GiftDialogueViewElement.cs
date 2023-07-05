using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GiftDialogueViewElement : MonoBehaviour, IPointerClickHandler
{    
    public void OnPointerClick(PointerEventData eventData) {
        DialogueView.Instance.OpenGiftPreview();
    }
}
