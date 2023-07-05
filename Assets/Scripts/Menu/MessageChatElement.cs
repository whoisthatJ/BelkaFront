using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MessageChatElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userTxt;
    [SerializeField] private TextMeshProUGUI otherTxt;
    [SerializeField] private TextMeshProUGUI userDate;
    [SerializeField] private TextMeshProUGUI otherDate;
    [SerializeField] private GameObject user;
    [SerializeField] private GameObject other;

    public void SetMessage(string msg, DateTime date, bool isUserMessage) {
        if (isUserMessage) {
            user.SetActive(true);
            other.SetActive(false);
            userTxt.text = msg + "\n" + "\n";
            StartCoroutine(SetSize(user));
            
        }
        else {
            user.SetActive(false);
            other.SetActive(true);
            otherTxt.text = msg + "\n" + "\n";
            StartCoroutine(SetSize(other));
        }

        userDate.text = date.ToShortTimeString();
        otherDate.text = date.ToShortTimeString();
    }

    private IEnumerator SetSize(GameObject obj){
        RectTransform side = obj.GetComponent<RectTransform>();
        RectTransform parentTr = GetComponent<RectTransform>();
        VerticalLayoutGroup group = transform.parent.GetComponent<VerticalLayoutGroup>();
        
        yield return new WaitUntil(() => side.sizeDelta.x == 0);        
        if (side.sizeDelta.x > parentTr.sizeDelta.x) {
            obj.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            side.sizeDelta = new Vector2(parentTr.sizeDelta.x, parentTr.sizeDelta.y + 200);
        }
        yield return new WaitWhile(()=> obj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y == 0);
        yield return new WaitForSeconds(.1f);
        parentTr.sizeDelta = new Vector2(parentTr.sizeDelta.x, side.sizeDelta.y);
        side.anchoredPosition = new Vector2(side.anchoredPosition.x, -parentTr.sizeDelta.y / 2);
        group.enabled = false;
        group.enabled = true;
    }
}
