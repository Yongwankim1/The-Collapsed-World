using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlotClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    Image image;
    ItemSlotManager slotManager;
    SlotUI slotUI;
    [SerializeField] AudioClip btnHighlight;
    void Awake()
    {
        slotUI = GetComponent<SlotUI>();
        image = GetComponent<Image>();
        slotManager = GetComponentInParent<ItemSlotManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (slotManager == null) return;

        slotManager.SelectDrop = slotUI.SlotData;
        slotManager.DropType = slotUI.SlotType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(slotUI.SlotData.ItemID)) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Keyboard.current.shiftKey.IsPressed())
            {
                Debug.Log("쉬프트 클릭됨");
                slotManager.QuickSlotChange(slotUI.SlotData.SlotIndex, slotUI.SlotData.ItemID, slotUI.SlotData.Amount, slotUI.SlotType);
                Debug.Log($"{slotUI.SlotData.SlotIndex} {slotUI.SlotData.ItemID} {slotUI.SlotData.Amount} {slotUI.SlotType}");
            }
            else
            {
                if(slotUI.SlotType == SlotType.Trade)
                {
                    slotManager.SelectTradeItem(slotUI.SlotData.ItemID);
                    return;
                }
                //TODO:: 상세보기 패널 활성화
                slotManager.OnDetailPanel(slotUI.SlotData.ItemID, slotUI.SlotData.SlotIndex, slotUI.SlotType);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(slotUI.SlotType == SlotType.Container)
            {
                slotManager.QuickSlotChange(slotUI.SlotData.SlotIndex, slotUI.SlotData.ItemID, slotUI.SlotData.Amount, slotUI.SlotType);
            }
            if (slotUI.SlotType != SlotType.Player) return;
            //TODO:: 인벤토리에서 아이템 사용
            if (slotManager.InventoryUseItem(slotUI.SlotData.ItemID, slotUI.SlotData.SlotIndex, 1))
            {
                Debug.Log("아이템 사용됨");
            }
            else
            {
                Debug.Log("사용 불가");
            }
        }

    }
    void OnDisable()
    {
        image.color = Color.white;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.yellow;
        if(SoundManager.Instance !=null && btnHighlight && !string.IsNullOrEmpty(slotUI.SlotData.ItemID))
        {
            SoundManager.Instance.PlaySfxOneShot(btnHighlight,0.7f);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
        //if (SoundManager.Instance != null && btnHighlight && !string.IsNullOrEmpty(slotUI.SlotData.ItemID))
        //{
        //    SoundManager.Instance.StopSfx();
        //}
    }
}
