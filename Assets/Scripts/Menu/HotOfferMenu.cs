using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotOfferMenu : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject hotOfferPrefab;
    [SerializeField] private Transform dotParent;
    [SerializeField] private Sprite dotEmpty;
    [SerializeField] private Sprite dotFilled;

    private List<Image> dotList;
    private int scrollElementAmount, currentIndex = 0;
    private float elementWidth;
    // Start is called before the first frame update
    void Start()
    {
        FillScroll();
    }

    private void FillScroll()
    {
        scrollElementAmount = 4;
        for (int i=0; i < scrollElementAmount; i++)
        {
            Instantiate(hotOfferPrefab, scrollContent);
        }
        elementWidth = scrollContent.GetChild(0).GetComponent<RectTransform>().rect.width;
        //dotList = new List<Image>();
        //dotList.Add(dotParent.GetChild(0).GetComponent<Image>());
        //dotParent.GetChild(0).gameObject.SetActive(true);
        
        /*for (int i = 1; i < scrollElementAmount; i++)
        {
            GameObject temp = Instantiate(dotParent.GetChild(0).gameObject, dotParent) as GameObject;
            dotList.Add(temp.GetComponent<Image>());
            temp.gameObject.SetActive(true);
        }*/
        //dotList[0].sprite = dotFilled;
    }

    public void OnDrag(PointerEventData data)
    {
        /*currentIndex = (int) Mathf.Abs(scrollContent.GetComponent<RectTransform>().anchoredPosition.x / elementWidth);
        for (int i=0;i<dotList.Count;i++)
        {
            if (currentIndex == i)
                dotList[i].sprite = dotFilled;
            else
                dotList[i].sprite = dotEmpty;
        }*/
    }

    public void OnEndDrag(PointerEventData data)
    {
        /*currentIndex = (int)Mathf.Abs(scrollContent.GetComponent<RectTransform>().anchoredPosition.x / elementWidth);
        for (int i = 0; i < dotList.Count; i++)
        {
            if (currentIndex == i)
                dotList[i].sprite = dotFilled;
            else
                dotList[i].sprite = dotEmpty;
        }
        scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1f* currentIndex * elementWidth, scrollContent.GetComponent<RectTransform>().anchoredPosition.y);*/
    }
}
