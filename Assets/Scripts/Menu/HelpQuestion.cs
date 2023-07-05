using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HelpQuestion : MonoBehaviour
{
    [SerializeField] private RectTransform panelRct;
    [SerializeField] private Button openBtn;
    [SerializeField] private Button closeBtn;

    [SerializeField] private TextMeshProUGUI questionTxt;
    [SerializeField] private TextMeshProUGUI questionDetailsTxt;
    [SerializeField] private float openingRange;
    [SerializeField] private float closingRange;

    private void OnEnable()
    {
        openBtn.onClick.AddListener(Open);
        closeBtn.onClick.AddListener(Close);        
    }

    private void OnDisable()
    {
        openBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    private void Open()
    {
        openBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(true);
        panelRct.DOKill();
        panelRct.DOSizeDelta(new Vector2(panelRct.sizeDelta.x, questionTxt.preferredHeight + questionDetailsTxt.preferredHeight + openingRange), 0.2f);
    }

    private void Close()
    {
        openBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(false);
        panelRct.DOKill();
        panelRct.DOSizeDelta(new Vector2(panelRct.sizeDelta.x, questionTxt.preferredHeight + closingRange), 0.2f);

    }
}
