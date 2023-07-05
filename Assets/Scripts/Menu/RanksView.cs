using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RanksView : MonoBehaviour
{
    public static RanksView Instance;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button backBtn;
    [SerializeField] private RankElement _rankPrefab;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private CurrentRank _currentRank;
    [SerializeField] private Sprite _currRank;
    [SerializeField] private Sprite _currRankLegend;
    [SerializeField] private Sprite _rank;
    [SerializeField] private Sprite _legend;
    [SerializeField] private PointSafe _safeZone;
    [SerializeField] private GameObject _help;
    [SerializeField] private GameObject _transparent;
    
    private RectTransform _scrollRect;
    private List<RankElement> _rankElements = new List<RankElement>();
    private List<PointSafe> _safeZones = new List<PointSafe>();
    private int _userRank;
    private void Awake()
    {
        Instance = this;
        _scrollRect = _scroll.GetComponent<RectTransform>();
    }
    
    
    private void OnEnable()
    {
        backBtn.onClick.AddListener(Close);
        MenuBottomBar.BottomBarButtonPressed += Close;
        _scroll.onValueChanged.AddListener(Scrolling);
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        MenuBottomBar.BottomBarButtonPressed -= Close;
        _scroll.onValueChanged.RemoveAllListeners();
    }

    private void Start(){

    }

    public void Open(){
        var mm = MainRoot.Instance.mainModel;
        /*mm.IsLegend = true;
        mm.Rank = 0;*/

        _currentRank.SetRank(mm.Rank, HardCodeValue.GetColorShieldTitle(mm.RankName, mm.IsLegend));
        _currentRank.SetTitle(mm.RankName, mm.IsLegend);
        
        Sprite sp = Resources.Load<Sprite>("Shields/" + mm.RankName);
        _currentRank.SetShieldPreview(sp);
        
        _currentRank.SetStars(mm.Stars, mm.StarsForNextRank);
        _userRank = mm.Rank;
        _currentRank.SetBackground(_currRank);
        
        if (mm.IsLegend){
            sp = Resources.Load<Sprite>("Shields/Legend");
            _currentRank.SetShieldPreview(sp);
            _currentRank.SetStars(-1, 0);
            _currentRank.SetTitle("Легенда", mm.IsLegend);
            _currentRank.SetLegendPoint(mm.LegendRank);
            _currentRank.SetBackground(_currRankLegend);
        }
        
        float sizeSafe = 0;
        
        for (int i = 0; i < 26; i++){
            RankElement element = Instantiate(_rankPrefab, _scroll.content);
            var rankInfo = mm.RankInfos.Find(x => x.minRank <= 25 - i && x.maxRank >= 25 - i);
            Sprite preview = null;
            
            if (rankInfo != null){
                element.SetRank(25 - i, HardCodeValue.GetColorShieldTitle(rankInfo.Name, false));
                preview = Resources.Load<Sprite>("Shields/" + rankInfo.Name);

                element.SetShieldPreview(preview);
                element.SetTitle(rankInfo.Name, false);
                element.SetStars(0, rankInfo.StarsForNextRank);
                element.SetCountDay(false, 0);
            }

            if (25 - i == mm.Rank){
                element.SetBackground(_currRank);
                element.SetStars(mm.Stars, mm.StarsForNextRank);
                element.SetCountDay(true, 25);
            }

            if (25 - i == 0){
                if (!mm.IsLegend){
                    element.SetBackground(_legend);
                    element.SetCountDay(false, -1);
                }
                else{
                    element.SetBackground(_currRankLegend);
                    element.SetCountDay(true, 25);
                }
                
                element.SetStars(-1, 0);
                preview = Resources.Load<Sprite>("Shields/Legend");
                element.SetShieldPreview(preview);
                element.SetLegendPoint(mm.LegendRank);
                element.SetTitle("Легенда", true);
                element.SetRank(25 - i, HardCodeValue.GetColorShieldTitle("", true));
            }
            
            if (i != 25)
            if (rankInfo.SafeZone.FindIndex(x => x == 25 - i) != -1){
                if (i != 0){
                    var indexElement = element.GetComponent<RectTransform>().GetSiblingIndex();
                    PointSafe safePoint = Instantiate(_safeZone, _scroll.content);
                    safePoint.GetComponent<RectTransform>().SetSiblingIndex(indexElement);
                    if ((25 - i) >= mm.Rank) sizeSafe += 160;
                    safePoint.Help.onClick.AddListener(HelpSafePoint);
                    _safeZones.Add(safePoint);
                }
            }
            
            _rankElements.Add(element);
        }

        float size = _rankPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float content = size * _rankElements.Count;
        
        _scroll.content.anchoredPosition = new Vector3(_scroll.content.anchoredPosition.x, content - (mm.Rank + 1) * size + sizeSafe);
        _panel.SetActive(true);
    }

    public void Close()
    {
        if (_scroll.content.childCount > 1)
        foreach (Transform item in _scroll.content){
            Destroy(item.gameObject);
        }
        foreach (PointSafe safe in _safeZones){
            safe.Help.onClick.RemoveAllListeners();
        }
        _safeZones.Clear();
        _rankElements.Clear();
        _panel.SetActive(false);
    }

    private void Scrolling(Vector2 vec){
        Vector3[] worldCorners = new Vector3[4];
        _currentRank.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        float currentRank = worldCorners[0].y;
        
        worldCorners = new Vector3[4];
        if (_rankElements.Count > 0){
            _rankElements[_rankElements.Count - (_userRank + 1)].GetComponent<RectTransform>()
                .GetWorldCorners(worldCorners);
            float nextRank = worldCorners[0].y;

            if (nextRank > currentRank){
                _currentRank.gameObject.SetActive(true);
                _transparent.SetActive(false);
            }
            else{
                _currentRank.gameObject.SetActive(false);
                if (MainRoot.Instance.mainModel.Rank != 25)
                _transparent.SetActive(true);
            }
        }
    }

    private void HelpSafePoint(){
        _help.SetActive(true);
    }
}
