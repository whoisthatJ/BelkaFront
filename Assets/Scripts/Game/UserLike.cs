using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UserLike : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI userName;
    public Button likeBtn;
    public Button disLikeBtn;

    private void OnEnable() {
        likeBtn.onClick.AddListener(SetLike);
        disLikeBtn.onClick.AddListener(SetDisLike);
    }

    private void OnDisable() {
        likeBtn.onClick.RemoveAllListeners();
        disLikeBtn.onClick.RemoveAllListeners();
    }

    private void SetLike() {
        Debug.Log("Like");
    }

    private void SetDisLike() {
        Debug.Log("DisLike");
    }
}
