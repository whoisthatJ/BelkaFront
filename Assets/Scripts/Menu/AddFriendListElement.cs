using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AddFriendListElement : MonoBehaviour
{
    public Image Avatar;
    public TextMeshProUGUI Text;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void OnEnable() {
        yesBtn.onClick.AddListener(Confirm);
        noBtn.onClick.AddListener(Cancel);
    }

    private void OnDisable() {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }

    private void Confirm() {
        FriendInvitation.Instance.Open();
    }

    private void Cancel() {
        Debug.Log("Cancel");
    }
}
