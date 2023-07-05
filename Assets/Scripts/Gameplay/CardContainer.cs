using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    [SerializeField] private float angleDif = 8f;
    [SerializeField] private float xSpacing = 0f;
    [SerializeField] private float ySpacing = 0f;
    [SerializeField] private int maxCardsCount = 8;

    private int childCount;
    private float cardRotation;
    private float cardXPosition;
    private float cardYPosition;

    private List<GameObject> cardGOs;

    private void Awake()
    {
        cardGOs = new List<GameObject>();
    }

    public Quaternion GetCardLocalRotation()
    {
        float angle = (maxCardsCount - 1) * angleDif * -.5f + (transform.childCount - 1) * angleDif;
        return Quaternion.Euler(0, 0, angle);
    }

    public Vector3 GetCardLocalPosition()
    {
        float xPos = (maxCardsCount - 1) * xSpacing * -.5f + (transform.childCount - 1) * xSpacing;
        float yPos = (maxCardsCount - 1) * ySpacing * -.5f + (transform.childCount - 1) * ySpacing;
        return new Vector3(xPos, (yPos > 0) ? -yPos : yPos);
    }

    public void RelocateCards()
    {
        childCount = transform.childCount;
        cardRotation = (childCount - 1) * angleDif * -.5f;
        cardXPosition = (childCount - 1) * xSpacing * -.5f;
        cardYPosition = (childCount - 1) * ySpacing * -.5f;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            child.localRotation = Quaternion.Euler(0, 0, cardRotation);
            cardRotation += angleDif;

            child.localPosition = new Vector3(cardXPosition, (cardYPosition > 0) ? -cardYPosition : cardYPosition, 0);
            cardXPosition += xSpacing;
            cardYPosition += ySpacing;
        }
    }

    public void AddCardGO(GameObject cardGO)
    {
        cardGOs.Add(cardGO);
    }

    public GameObject GetCardGO(int id)
    {
        GameObject cardGO = null;
        List<GameObject> newGOs = new List<GameObject>();
        for (int i = 0; i < cardGOs.Count; i++)
        {
            DragAndDropCard cardPrefabInfo = cardGOs[i].GetComponent<DragAndDropCard>();
            if (cardPrefabInfo.GetId() == id)
            {
                cardGO = cardGOs[i];
            }
            else
            {
                newGOs.Add(cardGOs[i]);
            }
        }
        cardGOs = newGOs;
        return cardGO;
    }

    public void RemoveCardGO()
    {
        List<GameObject> newGOs = new List<GameObject>();
        for (int i = 1; i < cardGOs.Count; i++)
        {
            newGOs.Add(cardGOs[i]);
        }
        Destroy(cardGOs[0]);
        cardGOs = newGOs;
    }
    public void RemovePlayerCardGO(int cardId)
    {
        string s = GameResources.instance.GetCardPrefab(cardId).name;
        GameObject card = cardGOs.Find(c => c.name.Contains(s));
        if (card != null)
        {
            cardGOs.Remove(card);
            Destroy(card);
        }
    }
}
