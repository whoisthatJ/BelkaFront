using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MenuTopBar : MonoBehaviour
{
    public static MenuTopBar Instance;

    [SerializeField] private Button notificationsBtn;
    [SerializeField] private Button playerBtn;
    [SerializeField] private Image playerAvatarImg;
    [SerializeField] private Button softCurrencyBtn;
    [SerializeField] private Button hardCurrencyBtn;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private TextMeshProUGUI rankTxt;
    [SerializeField] private TextMeshProUGUI softCurrencyTxt;
    [SerializeField] private TextMeshProUGUI hardCurrencyTxt;
    [SerializeField] private TextMeshProUGUI notificationCountTxt;
    [SerializeField] private MenuStars stars;
    [SerializeField] private GameObject notificationCountPanel;
    [SerializeField] private GameObject notificationsPopUpPanel;
    [SerializeField] private GameObject notificationPopUpElementPrefab;
    [SerializeField] private Image _shield;
    [SerializeField] private Image _legend;
    [SerializeField] private TextMeshProUGUI _legendRank;
    
    private List<GameObject> popUpNotificationsList;
    public delegate void TopBarButtonDelegate();
    public static event TopBarButtonDelegate TopBarButtonPressed;

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
        notificationsBtn.onClick.AddListener(OpenNotifications);
        playerBtn.onClick.AddListener(OpenProfile);
        softCurrencyBtn.onClick.AddListener(OpenShop);
        hardCurrencyBtn.onClick.AddListener(OpenShop);
        MenuBottomBar.BottomBarButtonPressed += CloseEverything;

        MainModel.OnUserNameChanged += RefreshUserName;
        MainModel.OnStarsChanged += RefreshStars;
        MainModel.OnRankChanged += RefreshRank;
        MainModel.OnRankInfoLoaded += RefreshStars;
        MainModel.OnRankInfoLoaded += RefreshRank;
        MainModel.OnRankInfoLoaded += RefreshLegendRank;
        MainModel.OnAvatarLoaded += LoadAvatar;
        ServiceWeb.OnUploadAvatar += ReloadAvatar;
        MainModel.OnSoftCurrencyAmountChanged += SoftCurrencyChanged;
        MainModel.OnHardCurrencyAmountChanged += HardCurrencyChanged;
        
    }

  

    private void OnDisable()
    {
        notificationsBtn.onClick.RemoveAllListeners();
        playerBtn.onClick.RemoveAllListeners();
        softCurrencyBtn.onClick.RemoveAllListeners();
        hardCurrencyBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= CloseEverything;

        MainModel.OnUserNameChanged -= RefreshUserName;
        MainModel.OnStarsChanged -= RefreshStars;
        MainModel.OnRankChanged -= RefreshRank;
        MainModel.OnAvatarLoaded -= LoadAvatar;
        ServiceWeb.OnUploadAvatar -= ReloadAvatar;
        MainModel.OnSoftCurrencyAmountChanged -= SoftCurrencyChanged;
        MainModel.OnHardCurrencyAmountChanged -= HardCurrencyChanged;
        
        MainModel.OnRankInfoLoaded -= RefreshStars;
        MainModel.OnRankInfoLoaded -= RefreshRank;
        MainModel.OnRankInfoLoaded -= RefreshLegendRank;
    }
    private void ReloadAvatar(string url){
        ServiceResources.LoadImage(url, playerAvatarImg);
    }
    private void Start()
    {
        popUpNotificationsList = new List<GameObject>();
        ShowPlayerInfo();
        SoftCurrencyChanged();
        HardCurrencyChanged();
        LoadAvatar();
    }

    private void OpenProfile()
    {
        TopBarButtonPressed();
        ProfileMenu.Instance.Open();
    }

    private void OpenShop()
    {
        TopBarButtonPressed();
        Shop.Instance.Open();
        NotificationsMenu.Instance.Close();
    }

    private void OpenNotifications()
    {
        TopBarButtonPressed();
        Shop.Instance.Close();
        NotificationsMenu.Instance.Open();
    }

    private void CloseEverything()
    {
        ProfileMenu.Instance.Close();
        Shop.Instance.Close();
        NotificationsMenu.Instance.Close();
    }

    private void ShowPlayerInfo()//show in case player switched accounts
    {
        playerAvatarImg.sprite = null;
        RefreshUserName();
        RefreshRank();
        RefreshStars();
        RefreshLegendRank();
    }

    private void RefreshUserName()
    {
        playerNameTxt.text = MainRoot.Instance.mainModel.UserName;
    }

    private void RefreshRank()
    {
        rankTxt.text = MainRoot.Instance.mainModel.Rank.ToString();

        var mm = MainRoot.Instance.mainModel;
        
        rankTxt.color = HardCodeValue.GetColorShieldTitle(mm.RankName, mm.IsLegend);
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + mm.RankName);
        
        if (mm.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
    }

    private void RefreshLegendRank(){
        var mm = MainRoot.Instance.mainModel;
        _legendRank.text = mm.LegendRank.ToString();
    }
    
    private void RefreshStars()
    {
        stars.gameObject.SetActive(true);
        stars.SetOpenedStars(MainRoot.Instance.mainModel.Stars, MainRoot.Instance.mainModel.StarsForNextRank);
        
        var mm = MainRoot.Instance.mainModel;
        if (mm.IsLegend){
            stars.gameObject.SetActive(false);
            _legend.gameObject.SetActive(true);
            _legend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
        }
    }

    private void SoftCurrencyChanged()
    {
        softCurrencyTxt.text = MainRoot.Instance.mainModel.SoftCurrency.ToString();
    }

    private void HardCurrencyChanged()
    {
        hardCurrencyTxt.text = MainRoot.Instance.mainModel.HardCurrency.ToString();
    }

    private void LoadAvatar(){
//        Debug.Log(MainRoot.Instance.mainModel.Avatar + " gameobject " + gameObject.name);
        if (!string.IsNullOrEmpty(MainRoot.Instance.mainModel.Avatar))
        Loader.Instance.LoadImage(MainRoot.Instance.mainModel.Avatar, playerAvatarImg, () => {
            if (playerAvatarImg.sprite != null){
                float aspectRatio = (float) playerAvatarImg.sprite.texture.width /
                                    (float) playerAvatarImg.sprite.texture.height;
                playerAvatarImg.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
                //Sprite size bug fixed
                var sprite = playerAvatarImg.sprite;
                playerAvatarImg.sprite = null;
                playerAvatarImg.sprite = sprite;
            }
        });
    }

    private void NotificationsChanged()
    {
        int n = 100; //notifications count
        notificationCountPanel.SetActive(n > 0);
        notificationCountTxt.text = n > 99 ? "99+" : n.ToString();
    }
    private IEnumerator TestPopUp()
    {
        yield return new WaitForSeconds(3f);
        NewNotificationPopUp();
        yield return new WaitForSeconds(1f);
        NewNotificationPopUp();
        yield return new WaitForSeconds(1f);
        NewNotificationPopUp();
        yield return new WaitForSeconds(2f);
        NewNotificationPopUp();
        yield return new WaitForSeconds(2f);
        NewNotificationPopUp();

    }
    private void NewNotificationPopUp()
    {
        for (int i = 0; i < popUpNotificationsList.Count; i++)
        {
            popUpNotificationsList[i].transform.DOKill();
            popUpNotificationsList[i].transform.DOLocalMoveY((popUpNotificationsList.Count - i) * (-225f), 0.5f).SetDelay(0.5f);
        }
        GameObject popUp = Instantiate(notificationPopUpElementPrefab, notificationsPopUpPanel.transform);
        popUp.name = notificationsPopUpPanel.transform.childCount.ToString();
        popUp.transform.localPosition = new Vector3(0f, 470f, 0f);
        popUp.transform.DOLocalMoveY(0f, 1f);
        popUpNotificationsList.Add(popUp);
    }
    public void NotificationClosed(GameObject notification)
    {
        if(popUpNotificationsList.Contains(notification))
            popUpNotificationsList.Remove(notification);
        Destroy(notification);
        UpdateNotificationsPositions();
    }
    private void UpdateNotificationsPositions()
    {
        for (int i = 0; i < popUpNotificationsList.Count; i++)
        {
            popUpNotificationsList[i].transform.DOKill();
            popUpNotificationsList[i].transform.DOLocalMoveY((popUpNotificationsList.Count - i - 1) * (-225f), 0.5f);
        }
    }
}
