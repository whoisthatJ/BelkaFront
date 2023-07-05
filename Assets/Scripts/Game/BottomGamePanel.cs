using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BottomGamePanel : MonoBehaviour
{
    public static BottomGamePanel Instance;

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject bottomPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject phrasesPanel;
    [SerializeField] private GameObject emojiPanel;

    [SerializeField] private Button itemsBtn;
    [SerializeField] private Button pharasesBtn;
    [SerializeField] private Button emojiBtn;
    [SerializeField] private Button closeBtn;

    private Vector2 startPos;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start() {
        startPos = bottomPanel.transform.localPosition;
    }

    private void OnEnable() {
        itemsBtn.onClick.AddListener(LoadItems);
        pharasesBtn.onClick.AddListener(LoadPharases);
        emojiBtn.onClick.AddListener(LoadEmoji);
        closeBtn.onClick.AddListener(ClosePanel);
    }

    private void OnDisable() {
        itemsBtn.onClick.RemoveAllListeners();
        pharasesBtn.onClick.RemoveAllListeners();
        emojiBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    private void OpenPanel() {
        bottomPanel.transform.DOLocalMoveY(-730, .2f).SetEase(Ease.Linear);
    }

    private void ClosePanel() {
        bottomPanel.transform.DOLocalMoveY(startPos.y, .2f).SetEase(Ease.Linear);
    }

    private void LoadItems() {
        itemsPanel.SetActive(true);
        phrasesPanel.SetActive(false);
        emojiPanel.SetActive(false);
        OpenPanel();
    }

    private void LoadPharases() {
        phrasesPanel.SetActive(true);
        itemsPanel.SetActive(false);
        emojiPanel.SetActive(false);
        OpenPanel();
    }

    private void LoadEmoji() {
        emojiPanel.SetActive(true);
        itemsPanel.SetActive(false);
        phrasesPanel.SetActive(false);
        OpenPanel();
    }
}
