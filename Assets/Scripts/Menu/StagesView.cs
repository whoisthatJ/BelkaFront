using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagesView : MonoBehaviour
{
    public static StagesView Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button backBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(Close);
        MenuBottomBar.BottomBarButtonPressed += Close;
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= Close;
    }

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
