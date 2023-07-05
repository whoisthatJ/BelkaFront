using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NotificationPopUpElement : MonoBehaviour, IDragHandler
{
    public Image Avatar;
    public TextMeshProUGUI friendNameTxt;
    public GameObject gamePanel;
    public GameObject tournamentPanel;
    public GameObject rankedPanel;
    [SerializeField] private Button gameBtn;
    [SerializeField] private Button tournamentBtn;
    [SerializeField] private Button rankedBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        gameBtn.onClick.AddListener(ConfirmGame);
        tournamentBtn.onClick.AddListener(ConfirmTournament);
        rankedBtn.onClick.AddListener(ConfirmRanked);
        closeBtn.onClick.AddListener(Close);
        scrollRect.viewport = transform.parent.GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        gameBtn.onClick.RemoveAllListeners();
        tournamentBtn.onClick.RemoveAllListeners();
        rankedBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    private void ConfirmGame()
    {
        Debug.Log("Confirm");
    }

    private void ConfirmTournament()
    {
        Debug.Log("ConfirmTournament");
    }

    private void ConfirmRanked()
    {
        Debug.Log("ConfirmRanked");
    }

    private void Close()
    {
        Debug.Log("Close");
        MenuTopBar.Instance.NotificationClosed(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(transform.localPosition.x) > 250f)
        {
            scrollRect.enabled = false;
            transform.DOLocalMoveX((transform.localPosition.x > 0 ? 1f : -1f) * 1100f, 0.4f).OnComplete(Close);
        }
    }
}
