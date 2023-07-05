using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlineGamePanel : MonoBehaviour
{
    public static OnlineGamePanel Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI stakeTxt;
    [SerializeField] private Button centerBtn;
    [Space]
    [SerializeField] private Image playerAvatar_1;
    [SerializeField] private Image playerAvatar_2;
    [SerializeField] private Image playerAvatar_3;
    [SerializeField] private Image playerAvatar_4;
    [Space]
    [SerializeField] private Button removePlayerBtn_2;
    [SerializeField] private Button removePlayerBtn_3;
    [SerializeField] private Button removePlayerBtn_4;
    [Space]
    [SerializeField] private Button addPlayerBtn_2;
    [SerializeField] private Button addPlayerBtn_3;
    [SerializeField] private Button addPlayerBtn_4;
    [Space]
    [SerializeField] private GameObject playerReady_1;
    [SerializeField] private GameObject playerReady_2;
    [SerializeField] private GameObject playerReady_3;
    [SerializeField] private GameObject playerReady_4;

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
        centerBtn.onClick.AddListener(CenterButtonPressed);
        removePlayerBtn_2.onClick.AddListener(RemovePlayer2);
        removePlayerBtn_3.onClick.AddListener(RemovePlayer3);
        removePlayerBtn_4.onClick.AddListener(RemovePlayer4);
        addPlayerBtn_2.onClick.AddListener(AddPlayer2);
        addPlayerBtn_3.onClick.AddListener(AddPlayer3);
        addPlayerBtn_4.onClick.AddListener(AddPlayer4);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        centerBtn.onClick.RemoveAllListeners();
        removePlayerBtn_2.onClick.RemoveAllListeners();
        removePlayerBtn_3.onClick.RemoveAllListeners();
        removePlayerBtn_4.onClick.RemoveAllListeners();
        addPlayerBtn_2.onClick.RemoveAllListeners();
        addPlayerBtn_3.onClick.RemoveAllListeners();
        addPlayerBtn_4.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        panel.SetActive(true);
        bool host = true;
        if (!host)
        {
            removePlayerBtn_2.gameObject.SetActive(false);
            removePlayerBtn_3.gameObject.SetActive(false);
            removePlayerBtn_4.gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void CenterButtonPressed()
    {
        //behaviour is different depending on a situation
        //can be Готов or Отменить
    }

    private void RemovePlayer2()
    {

    }

    private void RemovePlayer3()
    {

    }

    private void RemovePlayer4()
    {

    }

    private void AddPlayer2()
    {
        FriendInvite.Instance.Open();
    }

    private void AddPlayer3()
    {
        FriendInvite.Instance.Open();
    }

    private void AddPlayer4()
    {
        FriendInvite.Instance.Open();
    }
}
