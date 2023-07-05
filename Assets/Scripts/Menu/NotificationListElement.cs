using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NotificationListElement : MonoBehaviour
{
    public Image image;
    public RectTransform bg;
    public TextMeshProUGUI text;

    private void Update()
    {
        AdjustBG();
    }
    private void AdjustBG()
    {
        bg.sizeDelta = new Vector2(bg.sizeDelta.x, text.preferredHeight + 240f);
    }
}
