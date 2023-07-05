using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileSharePanel : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI userNameTxt;
    [SerializeField] private TextMeshProUGUI ratingTxt;
    [SerializeField] private TextMeshProUGUI _ratingCount;
    [SerializeField] private Transform _starsContainer;
    [SerializeField] private Sprite _starEmpty;
    [SerializeField] private Sprite _star;
    
    //stats
    [SerializeField] private StatsItem _percentageWin;
    [SerializeField] private StatsItem _NakedWin;
    [SerializeField] private StatsItem _FlawlesWin;
    [SerializeField] private TextMeshProUGUI _totalGamesPlayed;
    [SerializeField] private TextMeshProUGUI _totalGameWon;
    [SerializeField] private Image _shield;
    [SerializeField] private Image _legend;
    
    private void Start() {
        Init();    
    }
    
    private void OnEnable() {
     
    }

    private void OnDisable() {
     
    }

    private void Init() {

    }

    public void ClosePanel() {
        gameObject.SetActive(false);
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
        _starsContainer.gameObject.SetActive(true);
        
        var mm = MainRoot.Instance.mainModel;
        
        if (mm.IsLegend){
            _starsContainer.gameObject.SetActive(false);
            _legend.gameObject.SetActive(true);
            _legend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
        }
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + mm.RankName);
        
        if (mm.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
        }
        _shield.sprite = sp;
        ratingTxt.color = HardCodeValue.GetColorShieldTitle(mm.RankName, mm.IsLegend);
    }

    public void SetImage(string url){
        ServiceResources.LoadImage(url, avatar);
    }

    public void SetStats(ProfileStats stats){
        _totalGamesPlayed.text = stats.TotalGamesPlayed.ToString();
        _totalGameWon.text = stats.TotalGameWon.ToString();
        
        if (stats.TotalGamesPlayed > 0){
            float percentage =  (((float)stats.TotalNakedWin / stats.TotalGamesPlayed) * 100);
            float fillAmount =  ((float)stats.TotalNakedWin / stats.TotalGamesPlayed);
            _NakedWin.SetText(percentage, fillAmount, stats.TotalNakedWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGameWon > 0){
            float percentage = (((float)stats.TotalFlawlessWin / stats.TotalGameWon) * 100);
            float fillAmount =  ((float)stats.TotalGameLeaved / stats.TotalGameWon);
            _FlawlesWin.SetText(percentage, fillAmount, stats.TotalFlawlessWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGamesPlayed > 0){
            float percentage = (((float)stats.TotalGameWon / stats.TotalGamesPlayed) * 100);
            float fillAmount =  ((float)stats.TotalGameWon / stats.TotalGamesPlayed);
            _percentageWin.SetText(percentage, fillAmount, stats.TotalFlawlessWin, stats.TotalGameWon);  
        }
    }
    
    public void SetRewards(){
        
    }

    public void SetRank(int level, string rank){
        _ratingCount.text = level.ToString();
        ratingTxt.text = HardCodeValue.GetRankName(rank, MainRoot.Instance.mainModel.IsLegend);
    }

    public void SetName(string name){
        userNameTxt.text = name;
    }

    public void SetStars(int count, int generalCount){
        
        if (count > 5) return;
        
        switch (generalCount){
            case 3:
                _starsContainer.GetChild(0).localPosition = new Vector2(-39f, -40f);
                _starsContainer.GetChild(1).localPosition = new Vector2(0f, -47f);
                _starsContainer.GetChild(2).localPosition = new Vector2(39f, -40f);
                break;
            case 4:
                _starsContainer.GetChild(0).localPosition = new Vector2(-55f, -34f);
                _starsContainer.GetChild(1).localPosition = new Vector2(-20f, -44f);
                _starsContainer.GetChild(2).localPosition = new Vector2(20f, -44f);
                _starsContainer.GetChild(3).localPosition = new Vector2(55f, -34f);
                break;
            case 5:
                _starsContainer.GetChild(0).localPosition = new Vector2(-78f, -34f);
                _starsContainer.GetChild(1).localPosition = new Vector2(-39f, -44f);
                _starsContainer.GetChild(2).localPosition = new Vector2(0f, -50f);
                _starsContainer.GetChild(3).localPosition = new Vector2(39f, -44f);
                _starsContainer.GetChild(4).localPosition = new Vector2(78f, -34f);
                break;
        }
        
        for (int i = 0; i < _starsContainer.childCount; i++){
            _starsContainer.GetChild(i).gameObject.SetActive(true);
            _starsContainer.GetChild(i).GetComponent<Image>().sprite = _starEmpty;
            if (i >= generalCount){
                _starsContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < count; i++){
            _starsContainer.GetChild(i).GetComponent<Image>().sprite = _star;
        }
    }
}
