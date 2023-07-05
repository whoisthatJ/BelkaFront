using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutPopUp : MonoBehaviour
{
    public static AboutPopUp Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
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
