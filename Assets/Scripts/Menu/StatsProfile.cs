using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsProfile : MonoBehaviour{
    [SerializeField] private TextMeshProUGUI _totalGamesPlayed;
    [SerializeField] private TextMeshProUGUI _totalGameWon;
    [SerializeField] private TextMeshProUGUI _totalGameLost;
    [SerializeField] private StatsItem _nakedGames;
    [SerializeField] private StatsItem _flawlesGames;
    [SerializeField] private StatsItem _leaveGames;
    
    public void SetStats(ProfileStats stats){
        _totalGamesPlayed.text = stats.TotalGamesPlayed.ToString();
        _totalGameWon.text = stats.TotalGameWon.ToString();
        _totalGameLost.text = stats.TotalGameLost.ToString();
        
        if (stats.TotalGamesPlayed > 0){
            float percentage =  (((float)stats.TotalNakedWin / stats.TotalGamesPlayed) * 100);
            float fillAmount =  ((float)stats.TotalNakedWin / stats.TotalGamesPlayed);
            _nakedGames.SetText(percentage, fillAmount, stats.TotalNakedWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGameWon > 0){
            float percentage = (((float)stats.TotalFlawlessWin / stats.TotalGameWon) * 100);
            float fillAmount =  ((float)stats.TotalGameLeaved / stats.TotalGameWon);
            _flawlesGames.SetText(percentage, fillAmount, stats.TotalFlawlessWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGamesPlayed > 0){
            float percentage = (((float)stats.TotalGameLeaved / stats.TotalGamesPlayed) * 100);
            float fillAmount =  ((float)stats.TotalGameLeaved / stats.TotalGamesPlayed);
            _leaveGames.SetText(percentage, fillAmount, stats.TotalGameLeaved, stats.TotalGameLost);  
        }
    }
}

public class ProfileStats{
    public int TotalGamesPlayed;
    public int TotalGameWon;
    public int TotalGameLost;
    public int TotalGameLeaved;
    public int TotalNakedWin;
    public int TotalFlawlessWin;
}