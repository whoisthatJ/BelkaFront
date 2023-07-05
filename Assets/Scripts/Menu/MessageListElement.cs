using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MessageListElement : MonoBehaviour, IPointerClickHandler
{
    public Image Image;
    public TextMeshProUGUI Text;

    public void OnPointerClick(PointerEventData eventData) {
        //DialogueView.Instance.Open("Bla bla bla");
    }
}
