using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _standartReward;
    [SerializeField] private TextMeshProUGUI _premiumReward;
    [SerializeField] private TextMeshProUGUI _countDay;
    [SerializeField] private TextMeshProUGUI _legendPoint;
    
    [SerializeField] private Image _content;
    [SerializeField] private Image _shield;
    
    [SerializeField] private Transform _starsContainer;
    [SerializeField] private Sprite _star;
    [SerializeField] private Sprite _emptyStar;
    
    public void SetRank(int level, Color color){
        _level.text = level.ToString();
        _level.color = color;
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

    public void SetBackground(Sprite sp){
        _content.sprite = sp;
    }

    public void SetShieldPreview(Sprite sp){
        _shield.sprite = sp;
    }

    public void SetLegendPoint(int point){
        if (point == 0) return;
        _legendPoint.gameObject.SetActive(true);
        _legendPoint.text = point.ToString();
    }

    public void SetCountDay(bool isActive, int day){
        _countDay.gameObject.SetActive(isActive);
        _countDay.text = string.Format("Осталось: <color=#4E536D>{0} дней</color>", day);
    }
    
    public void SetStars(int stars = -1, int generalCountStars = 0){

        if (stars == -1){
            _starsContainer.gameObject.SetActive(false);
            return;
        }
        
        if (stars > 5) return;
        
        _starsContainer.gameObject.SetActive(true);
        
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
}
