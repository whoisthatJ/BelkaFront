using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuStars : MonoBehaviour
{
    [SerializeField] List<Image> stars;

    [SerializeField] Sprite activeStarSpr;
    [SerializeField] Sprite nonActiveStarSpr;

    public void SetOpenedStars(int playerAmount, int rankAmount)
    {
        if (playerAmount > 5) return;
        
        for (int i = 0; i < stars.Capacity; i++)
        {
            if (i < rankAmount)
            {
                stars[i].enabled = true;
                if (i < playerAmount)
                    stars[i].sprite = activeStarSpr;
                else
                    stars[i].sprite = nonActiveStarSpr;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }
}
