using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileMenu : MonoBehaviour
{
    public static ProfileMenu Instance;
    [SerializeField] private ProfileSharePanel sharePanel;
    [SerializeField] private GameObject panel;

    [SerializeField] private Button backBtn;
    [SerializeField] private Button helpBtn;
    [SerializeField] private Button settingsBtn;

    [SerializeField] private Image _avatar;
    
    [SerializeField] private Button softCurrencyBtn;
    [SerializeField] private TextMeshProUGUI _softCurrency;
    [SerializeField] private Button hardCurrencyBtn;
    [SerializeField] private TextMeshProUGUI _hardCurrency;
    
    [SerializeField] private Button buyPremiumBtn;
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private Button historyBtn;
    [SerializeField] private Button statsBtn;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject historyPanel;
    [SerializeField] private StatsProfile statsPanel;
    [SerializeField] private Button shareBtn;
    [SerializeField] private Button _photo;
    
    [SerializeField] private Button editNameBtn;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _rank;
    [SerializeField] private TextMeshProUGUI _rankName;
    [SerializeField] private Sprite _starEmpty;
    [SerializeField] private Sprite _star;
    [SerializeField] private Transform _starsContainer;
    [SerializeField] private RenamePopup _renamePopup;
    [SerializeField] private RectTransform _content;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private RectTransform _topProfileInfo;
    [SerializeField] private RectTransform _profileBasicInfo;
    [SerializeField] private RectTransform _transparent;
    [SerializeField] private Image _shield;
    [SerializeField] private Image _legend;
    [SerializeField] private TextMeshProUGUI _legendRank;
    
    [Space]
    [SerializeField] private GameObject historyElementPrefab;

    private Texture2D _screenshot;
    private int _starsForNextRank;
    private RectTransform _scrollRect;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        backBtn.onClick.AddListener(Close);
        helpBtn.onClick.AddListener(Help);
        settingsBtn.onClick.AddListener(OpenSettings);
        softCurrencyBtn.onClick.AddListener(OpenShop);
        hardCurrencyBtn.onClick.AddListener(OpenShop);
        buyPremiumBtn.onClick.AddListener(OpenBuyPremium);
        inventoryBtn.onClick.AddListener(InventoryButtonPressed);
        historyBtn.onClick.AddListener(HistoryButtonPressed);
        statsBtn.onClick.AddListener(StatsButtonPressed);
        shareBtn.onClick.AddListener(OpenSharePanel);

        editNameBtn.onClick.AddListener(EditNameBtnClicked);
        MainModel.OnProfileLoaded += LoadInfo;
        _scroll.onValueChanged.AddListener(Scrolling);
        _photo.onClick.AddListener(()=> {
            OpenGallery(1024);
        });
        ServiceWeb.OnUploadAvatar += ReloadAvatar;
        MainModel.OnUserNameChanged += RefreshUserName;
        MainModel.OnSoftCurrencyAmountChanged += SoftCurrencyChanged;
        MainModel.OnHardCurrencyAmountChanged += HardCurrencyChanged;
    }
    
    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        helpBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        softCurrencyBtn.onClick.RemoveAllListeners();
        hardCurrencyBtn.onClick.RemoveAllListeners();
        buyPremiumBtn.onClick.RemoveAllListeners();
        inventoryBtn.onClick.RemoveAllListeners();
        historyBtn.onClick.RemoveAllListeners();
        statsBtn.onClick.RemoveAllListeners();
        shareBtn.onClick.RemoveAllListeners();

        editNameBtn.onClick.RemoveAllListeners();
        MainModel.OnProfileLoaded -= LoadInfo;
        _scroll.onValueChanged.RemoveAllListeners();
        _photo.onClick.RemoveAllListeners();
        ServiceWeb.OnUploadAvatar -= ReloadAvatar;
        MainModel.OnUserNameChanged -= RefreshUserName;
        MainModel.OnSoftCurrencyAmountChanged -= SoftCurrencyChanged;
        MainModel.OnHardCurrencyAmountChanged -= HardCurrencyChanged;
    }

    private void OpenSharePanel() {
        sharePanel.OpenPanel();
        Share();
    }

    private void Start(){
        _scrollRect = _scroll.GetComponent<RectTransform>();
        /*for (int i = 0; i < 5; i++)
            FillHistoryScroll();*/
        BottomOffset();
    }

    private void BottomOffset(){
        float aspect = (float) Screen.height / Screen.width;
        if (aspect < 1.8) _content.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 550, 200);
        if (aspect > 1.9 && aspect < 2) _content.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 550, 100);
        if (aspect > 2) _content.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 550, 0);
    }
    
    private void Scrolling(Vector2 vec){
        Vector3[] worldCorners = new Vector3[4];
        _scrollRect.GetWorldCorners(worldCorners);
        float yScroll = worldCorners[1].y;
        
        worldCorners = new Vector3[4];
        _topProfileInfo.GetWorldCorners(worldCorners);
        float yInfo = worldCorners[1].y;
        
        worldCorners = new Vector3[4];
        _profileBasicInfo.GetWorldCorners(worldCorners);
        float yInfoBasic = worldCorners[1].y;

        if (yInfo > yScroll){
            _topProfileInfo.transform.SetParent(panel.transform);
            _topProfileInfo.transform.SetSiblingIndex(3);
            _topProfileInfo.transform.localPosition = new Vector2(_topProfileInfo.transform.localPosition.x, -579.5f);
            _transparent.transform.SetParent(panel.transform);
            _transparent.transform.SetAsLastSibling();
            _transparent.transform.localPosition = new Vector2(_transparent.transform.localPosition.x, -311f);
        }
        if (yInfoBasic < yScroll) {
            _topProfileInfo.transform.SetParent(_profileBasicInfo.transform);
            _topProfileInfo.transform.SetAsFirstSibling();
            _topProfileInfo.transform.localPosition = new Vector2(_topProfileInfo.transform.localPosition.x, -307.8f);
            _transparent.transform.SetParent(_profileBasicInfo.transform);
            _transparent.transform.SetAsLastSibling();
            _transparent.transform.localPosition = new Vector2(_transparent.transform.localPosition.x, -38.93f);
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        InventoryButtonPressed();
        ServiceWeb.Instance.GetProfileUser();
        _scroll.verticalNormalizedPosition = 1;
    }
    
    public void Close()
    {
        panel.SetActive(false);
    }

    private void ReloadAvatar(string url){
        Debug.Log(url + " reload");
        ServiceResources.LoadImage(url, _avatar);
        sharePanel.SetImage(url);
    }
    
    private void LoadInfo(){
        var mm = MainRoot.Instance.mainModel;
        _name.text = mm.UserProfileData.Name;
        _rank.text = mm.UserProfileData.RankValue.ToString();
        _softCurrency.text = mm.SoftCurrency.ToString();
        _hardCurrency.text = mm.HardCurrency.ToString();
        _rankName.text = mm.RankName;
        ServiceResources.LoadImage(mm.UserProfileData.Avatar, _avatar);
        _starsForNextRank = mm.StarsForNextRank;
        _starsContainer.gameObject.SetActive(true);
        
        for (int i = 0; i < _starsContainer.childCount; i++){
            _starsContainer.GetChild(i).gameObject.SetActive(true);
            _starsContainer.GetChild(i).GetComponent<Image>().sprite = _starEmpty;
            if (i >= _starsForNextRank){
                _starsContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        if (mm.Stars < 6)
        for (int i = 0; i < mm.Stars; i++){
            _starsContainer.GetChild(i).GetComponent<Image>().sprite = _star;
        }

        switch (_starsForNextRank){
            case 3:
                _starsContainer.GetComponent<HorizontalLayoutGroup>().spacing = -160;
                break;
            case 4:
                _starsContainer.GetComponent<HorizontalLayoutGroup>().spacing = -120;
                break;
            case 5:
                _starsContainer.GetComponent<HorizontalLayoutGroup>().spacing = -80;
                break;
        }
        
        ProfileStats stats = new ProfileStats();
        stats.TotalGamesPlayed = mm.UserProfileData.TotalGamesPlayed;
        stats.TotalFlawlessWin = mm.UserProfileData.TotalFlawlessWin;
        stats.TotalGameLeaved = mm.UserProfileData.TotalGameLeaved;
        stats.TotalGameLost = mm.UserProfileData.TotalGameLost;
        stats.TotalGameWon = mm.UserProfileData.TotalGameWon;
        stats.TotalNakedWin = mm.UserProfileData.TotalNakedWin;
        
        statsPanel.SetStats(stats);
        
        sharePanel.SetName(mm.UserProfileData.Name);
        sharePanel.SetStats(stats);
        sharePanel.SetStars(mm.Stars, _starsForNextRank);
        sharePanel.SetRank(mm.UserProfileData.RankValue, mm.RankName);
        sharePanel.SetImage(mm.UserProfileData.Avatar);
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + mm.RankName);
        
        if (mm.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
        _rank.color = HardCodeValue.GetColorShieldTitle(mm.RankName, mm.IsLegend);
        _renamePopup.SetName(mm.UserProfileData.Name);

        if (mm.IsLegend){
            _starsContainer.gameObject.SetActive(false);
            _legend.gameObject.SetActive(true);
            _legend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
            _legendRank.text = mm.LegendRank.ToString();
        }
        
        _rankName.text = HardCodeValue.GetRankName(mm.RankName, mm.IsLegend);
    }

    private void RefreshUserName()
    {
        _name.text = MainRoot.Instance.mainModel.UserName;
    }
    
    private void SoftCurrencyChanged()
    {
        _softCurrency.text = MainRoot.Instance.mainModel.SoftCurrency.ToString();
    }

    private void HardCurrencyChanged()
    {
        _hardCurrency.text = MainRoot.Instance.mainModel.HardCurrency.ToString();
    }
    
    private void Help()
    {
        HelpPopUp.Instance.Open();
    }

    private void OpenSettings()
    {
        Settings.Instance.OpenSettings();
    }

    private void OpenShop()
    {
        Close();
        Shop.Instance.Open();
    }

    private void OpenBuyPremium()
    {
        BuyPremiumMenu.Instance.Open();
    }

    private void InventoryButtonPressed()
    {
        inventoryPanel.SetActive(true);
        historyPanel.SetActive(false);
        statsPanel.gameObject.SetActive(false);
        inventoryBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        inventoryBtn.transform.GetChild(1).gameObject.SetActive(true);
        historyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        historyBtn.transform.GetChild(1).gameObject.SetActive(false);
        statsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        statsBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void HistoryButtonPressed()
    {
        inventoryPanel.SetActive(false);
        historyPanel.SetActive(true);
        statsPanel.gameObject.SetActive(false);
        inventoryBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        inventoryBtn.transform.GetChild(1).gameObject.SetActive(false);
        historyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        historyBtn.transform.GetChild(1).gameObject.SetActive(true);
        statsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        statsBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void StatsButtonPressed()
    {
        inventoryPanel.SetActive(false);
        historyPanel.SetActive(false);
        statsPanel.gameObject.SetActive(true);
        inventoryBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        inventoryBtn.transform.GetChild(1).gameObject.SetActive(false);
        historyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        historyBtn.transform.GetChild(1).gameObject.SetActive(false);
        statsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        statsBtn.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void FillHistoryScroll()
    {
        Instantiate(historyElementPrefab, historyPanel.transform);
    }

    private void EditNameBtnClicked()
    {    
        _renamePopup.Open();
    }

    private void Share(){
        StartCoroutine(TakeSSAndShare());
    }
    
    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();
        if (_screenshot == null)
        {
            _screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            _screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            _screenshot.Apply();
        }
        Debug.Log(Application.temporaryCachePath);
        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, _screenshot.EncodeToPNG());

        // To avoid memory leaks
        Destroy(_screenshot);

        new NativeShare().AddFile(filePath).SetSubject("Belka").SetText("Share").Share();
        //yield return new WaitForEndOfFrame();
        sharePanel.gameObject.SetActive(false);
        
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }

    private void OpenGallery(int maxSize){
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }
                RenderTexture tmp = RenderTexture.GetTemporary( 
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

                // Blit the pixels on texture to the RenderTexture
                Graphics.Blit(texture, tmp);
                // Backup the currently set RenderTexture
                RenderTexture previous = RenderTexture.active;
                // Set the current RenderTexture to the temporary one we created
                RenderTexture.active = tmp;
                // Create a new readable Texture2D to copy the pixels to it
                Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
                // Copy the pixels from the RenderTexture to the new Texture
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();
                // Reset the active RenderTexture
                RenderTexture.active = previous;
                // Release the temporary RenderTexture
                RenderTexture.ReleaseTemporary(tmp);
                
                ServiceWeb.Instance.UploadFile(myTexture2D);
                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy( texture, 5f );
            }
        }, "Select a PNG image", "image/png" );

        Debug.Log( "Permission result: " + permission );
    }
}
