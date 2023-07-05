using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject bg;

    private int cardId;
    private bool isDraggable;

    private Vector3 previousPos;
    private Quaternion previousRot;
    private Vector2 previousPivot;

    private Vector2 halfVector = new Vector2(0.5f, 0.5f);

    private void Awake()
    {
        isDraggable = false;
    }

    public int GetId()
    {
        return cardId;
    }

    public void SetId(int _id)
    {
        cardId = _id;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            RectTransform cardRT = transform.GetComponent<RectTransform>();

            previousPivot = cardRT.pivot;
            previousPos = transform.localPosition;
            previousRot = transform.localRotation;

            cardRT.pivot = halfVector;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            if (transform.localPosition.y > 400f & (GameManager.instance.online ? GameMasterOnline.instance.CheckIfCanPlace(cardId) : GameMaster.instance.CheckIfCanPlace(cardId)))
            {
                if(GameManager.instance.online)
                    GameMasterOnline.instance.MakeTurnPlayer(cardId);
                else
                    GameMaster.instance.MakeTurn(0, cardId);
                isDraggable = false;
            }
            else
            {
                transform.GetComponent<RectTransform>().pivot = previousPivot;
                transform.localPosition = previousPos;
                transform.localRotation = previousRot;
            }
        }
    }

    public void SetIsDragable(bool state)
    {
        isDraggable = state;
    }

    public void SetBGActive(bool state)
    {
        bg.SetActive(state);
    }
}
