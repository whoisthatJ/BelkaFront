using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRank : MonoBehaviour{
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _standartReward;
    [SerializeField] private TextMeshProUGUI _premiumReward;
    [SerializeField] private TextMeshProUGUI _countDay;
    [SerializeField] private TextMeshProUGUI _legendPoint;
    
    [SerializeField] private Image _shield;
    [SerializeField] private Transform _starsContainer;
    [SerializeField] private Sprite _star;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Image _background;

    public void SetBackground(Sprite sp){
        if (sp == null) return;
        _background.sprite = sp;
    }
    
    public void SetRank(int level, Color color){
        _level.text = level.ToString();
        _level.color = color;
    }

    public void SetStars(int stars, int generalCountStars){
        if (stars == -1){
            _starsContainer.gameObject.SetActive(false);
            return;
        }
        
        if (stars > 5) return;
        
        foreach (Transform tr in _starsContainer){
            tr.GetComponent<Image>().sprite = _emptyStar;
        }

        for (int i = 0; i < generalCountStars; i++){
            _starsContainer.GetChild(i).gameObject.SetActive(true);
            if (i < stars){
                _starsContainer.GetChild(i).GetComponent<Image>().sprite = _star;
            }
        }
    }
    
    public void SetTitle(string title, bool isLegend){
        _title.text = HardCodeValue.GetRankName(title, isLegend);
        if (isLegend) _title.alignment = TextAlignmentOptions.MidlineLeft;
        else _title.alignment = TextAlignmentOptions.TopLeft;
    }

    public void SetStandartReward(int count){
        _standartReward.text = count.ToString();
    }

    public void SetPremiumReward(string text){
        _premiumReward.text = text;
    }

    public void SetCountDay(int day){
        _countDay.text = string.Format("Осталось: <color=#4E536D>{0} дней</color>", day);
    }

    public void SetShieldPreview(Sprite sp){
        _shield.sprite = sp;
    }
    
    
    public void SetLegendPoint(int point){
        if (point == 0) return;
        _legendPoint.gameObject.SetActive(true);
        _legendPoint.text = point.ToString();
    }
}
