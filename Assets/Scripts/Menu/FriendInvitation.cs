using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendInvitation : MonoBehaviour
{
    public static FriendInvitation Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button declineBtn;
    [SerializeField] private Button blockNotificationsBtn;
    [SerializeField] private TextMeshProUGUI friendNameTxt;
    [SerializeField] private Image friendAvatarBigImg;
    [Space]
    [SerializeField] private Image friendAvatarImg_1;
    [SerializeField] private Image friendAvatarImg_2;
    [SerializeField] private Image friendAvatarImg_3;
    [SerializeField] private Image friendAvatarImg_4;


    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        declineBtn.onClick.AddListener(Decline);
        acceptBtn.onClick.AddListener(Accept);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        declineBtn.onClick.RemoveAllListeners();
        acceptBtn.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        panel.SetActive(true);
        //Set avatars and name
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void Accept()
    {

    }

    private void Decline()
    {

    }

    private void BlockNotifications()
    {

    }
}
