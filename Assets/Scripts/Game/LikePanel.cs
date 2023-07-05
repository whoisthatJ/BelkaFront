using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LikePanel : MonoBehaviour
{
    [SerializeField] private GameObject userPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button OKBtn;
    [SerializeField] private Button cancelBtn;

    private void Start() {
        for (int i = 0; i < 3; i++) {
            CreateUser();
        }
    }

    private void OnEnable() {
        OKBtn.onClick.AddListener(CloseLikePanel);
        cancelBtn.onClick.AddListener(CloseLikePanel);
    }

    private void OnDisable() {
        OKBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }

    public void CreateUser() {
        GameObject cloneUser = Instantiate(userPrefab, contentPanel);
    }

    public void CloseLikePanel() {
        FinishPopup.instance.completePopup.CloseLikePanel();
    }
}
