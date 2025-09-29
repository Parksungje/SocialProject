using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableObj : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Canvas canva;

    private void Awake()
    {
        canva = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        canva.sortingOrder = 1;
        transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canva.sortingOrder = 0;
    }
}
