using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsItem : MonoBehaviour{
   [SerializeField] private TextMeshProUGUI _percentage;
   [SerializeField] private TextMeshProUGUI _count;
   [SerializeField] private Image _fill;

   public void SetText(float percentage, float fillAmount, int count,  int totalGameWon){
      _count.text = "<color=#000000><b>" + count + "</b></color>" + "/" + totalGameWon;
      _fill.fillAmount = fillAmount;
      _percentage.text = Mathf.RoundToInt(percentage) + "%";
   }
}
