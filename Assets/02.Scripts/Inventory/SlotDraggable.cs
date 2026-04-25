using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Transform canvas;
    [SerializeField] Transform previousParent;
    [SerializeField] RectTransform rect;
    [SerializeField] CanvasGroup cg;

    [SerializeField] SlotUI mySlot;
    
    [SerializeField] ItemSlotManager changer;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if(changer == null)
            changer = GetComponentInParent<ItemSlotManager>();
        if (cg == null)
            cg = GetComponent<CanvasGroup>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>().transform;
        if (rect == null)
            rect = GetComponent<RectTransform>();
        if (mySlot == null)
            mySlot = GetComponentInParent<SlotUI>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (changer == null) return;
        if (mySlot.SlotType == SlotType.Trade || mySlot.SlotType == SlotType.Container) return;
        changer.SelectDrag = mySlot.SlotData;
        changer.DragType = mySlot.SlotType;

        previousParent = transform.parent;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        cg.alpha = 0.6f;
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mySlot.SlotType == SlotType.Trade || mySlot.SlotType == SlotType.Container) return;
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (mySlot.SlotType == SlotType.Trade || mySlot.SlotType == SlotType.Container) return;
        transform.SetParent(previousParent);
        rect.position = previousParent.GetComponent<RectTransform>().position;
        cg.alpha = 1.0f;
        cg.blocksRaycasts = true;
        if (changer == null) return;
        changer.InventorySlotChange();
    }
}
