using UnityEngine;

public class GameResources : MonoBehaviour
{
    public static GameResources instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    [SerializeField] private Card[] cards;
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject emptyBigPrefab;

    public Card[] GetCards()
    {
        return cards;
    }

    public GameObject GetCardPrefab(int _card)
    {
        return cards[_card].GetCardPrefab();
    }

    public GameObject GetEmptyPrefab()
    {
        return emptyPrefab;
    }

    public GameObject GetEmptyBigPrefab()
    {
        return emptyBigPrefab;
    }
}
