using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsOtherProfile : MonoBehaviour
{
    [SerializeField] private StatsItem _totalGamesWon;
    [SerializeField] private StatsItem _nakedGames;
    [SerializeField] private StatsItem _flawlesGames;

    public void SetStats(ProfileStats stats){

        if (stats.TotalGamesPlayed > 0){
            float percentage =  (((float)stats.TotalNakedWin / stats.TotalGamesPlayed) * 100);
            float fillAmount =  ((float)stats.TotalNakedWin / stats.TotalGamesPlayed);
            _nakedGames.SetText(percentage, fillAmount, stats.TotalNakedWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGameWon > 0){
            float percentage = (((float)stats.TotalFlawlessWin / stats.TotalGameWon) * 100);
            float fillAmount =  ((float)stats.TotalFlawlessWin / stats.TotalGameWon);
            _flawlesGames.SetText(percentage, fillAmount, stats.TotalFlawlessWin, stats.TotalGameWon);  
        }
        
        if (stats.TotalGamesPlayed > 0){
            float percentage =  (((float)stats.TotalGameWon / stats.TotalGamesPlayed) * 100);
            float fillAmount = ((float)stats.TotalGameWon / stats.TotalGamesPlayed);
            _totalGamesWon.SetText(percentage, fillAmount, stats.TotalGameWon, stats.TotalGamesPlayed);  
        }
    }
}
