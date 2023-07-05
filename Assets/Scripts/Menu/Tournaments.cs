using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tournaments : MonoBehaviour
{
    public static Tournaments Instance;

    [SerializeField] GameObject panel;
    [SerializeField] Button closeBtn;
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;

    [SerializeField] private List<GameObject> listGamePanels;

    private int indexGamePlanel = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        leftBtn.onClick.AddListener(MoveLeft);
        rightBtn.onClick.AddListener(MoveRight);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        panel.SetActive(true);
        indexGamePlanel = 0;
        listGamePanels[indexGamePlanel].SetActive(true);
    }

    public void Close()
    {
        listGamePanels[indexGamePlanel].SetActive(false);
        panel.SetActive(false);
    }

    private void MoveLeft()
    {
        if (indexGamePlanel > 0)
        {
            listGamePanels[indexGamePlanel].SetActive(false);
            indexGamePlanel--;
            listGamePanels[indexGamePlanel].SetActive(true);
            if (!rightBtn.gameObject.activeSelf)
                rightBtn.gameObject.SetActive(true);
        }
        if (indexGamePlanel == 0)
        {
            leftBtn.gameObject.SetActive(false);
        }
    }

    private void MoveRight()
    {
        if (indexGamePlanel < listGamePanels.Count - 1)
        {
            listGamePanels[indexGamePlanel].SetActive(false);
            indexGamePlanel++;
            listGamePanels[indexGamePlanel].SetActive(true);
            if (!leftBtn.gameObject.activeSelf)
                leftBtn.gameObject.SetActive(true);
        }
        if (indexGamePlanel == listGamePanels.Count - 1)
        {
            rightBtn.gameObject.SetActive(false);
        }
    }
}
