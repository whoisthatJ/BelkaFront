using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendInvite : MonoBehaviour
{
    public static FriendInvite Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Transform content;
    [SerializeField] private FriendInviteListElement friendElementPrefab;
    [SerializeField] private TextMeshProUGUI onlineTxt;

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

    private void OnEnable() {
        closeBtn.onClick.AddListener(Close);
        confirmBtn.onClick.AddListener(Confirm);
    }

    private void OnDisable() {
        closeBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        FillScroll();
    }

    public void Open() {
        panel.SetActive(true);
        onlineTxt.text = "9";
    }

    public void Close() {
        panel.SetActive(false);
    }

    private void Confirm() {

    }

    private void FillScroll()
    {
        for (int i = 0; i < 15; i++)
        {
            Instantiate(friendElementPrefab, content);
        }
    }
}
