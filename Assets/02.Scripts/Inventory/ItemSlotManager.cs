using UnityEngine;

public class ItemSlotManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] RandomDropInventory enemyInventory;
    [SerializeField] ItemDetailPanel itemPanel;
    [SerializeField] PlayerStashUI playerStashUI;
    [SerializeField] TradePanelUI tradePanelUI;
    public SlotType DragType;
    public SlotType DropType;

    public SlotData SelectDrag;
    public SlotData SelectDrop;
    public void Initialize(RandomDropInventory enemyInventory)
    {
        this.enemyInventory = enemyInventory;
    }
    private void OnDisable()
    {
        if (itemPanel) itemPanel.gameObject.SetActive(false);
        enemyInventory = null;
    }
    public bool InventoryUseItem(string itemId, int index, int amount) => inventory.UseItem(itemId,index,amount);
    void Initialize()
    {
        DragType = SlotType.None;
        DropType = SlotType.None;

        SelectDrag = default;
        SelectDrop = default;
        itemPanel.gameObject.SetActive(false);
    }
    public void OnDetailPanel(string itemID,int index, SlotType type)
    {
        itemPanel.Initialize(itemID, index,type);
    }
    public void QuickSlotChange(int clickIndex, string itemId, int amount, SlotType type)
    {

        switch (type)
        {
            case SlotType.None: break;
            case SlotType.Enemy:
                if (enemyInventory == null) return;
                if (inventory.RemainingBagCount() > 0)
                {
                    inventory.AddItem(itemId, amount);
                    enemyInventory.ListRemove(clickIndex);
                }
                break;
            case SlotType.Player:

                if (enemyInventory == null && playerStashUI == null)
                {
                    if (!inventory.RemoveItem(itemId, clickIndex, amount)) return;
                    if (!PlayerBaseEquipment.Instance.Equip(itemId, out string backItemID)) return;
                    inventory.AddItem(backItemID, 1);
                    Initialize();
                    return;
                }
                if (!inventory.RemoveItem(itemId, clickIndex, amount))
                {
                    Debug.Log("삭제 못함");
                    Initialize();
                    return;
                }
                if(enemyInventory == null && playerStashUI)
                {
                    playerStashUI.AddItemSlot(itemId, amount);
                }
                else if(enemyInventory && !playerStashUI)
                    enemyInventory.ListAddItem(itemId, amount);
                break;
            case SlotType.Stash:
                if(tradePanelUI != null)
                {
                    playerStashUI.RemoveItemSlot(clickIndex);
                    tradePanelUI.AddSlot(itemId,amount);
                    return;
                }
                if (inventory.RemainingBagCount() > 0)
                {
                    playerStashUI.RemoveItemSlot(clickIndex);
                    inventory.AddItem(itemId, amount);
                }
                break;
            case SlotType.Equip:
                if(inventory.RemainingBagCount() > 0)
                {
                    if (!PlayerBaseEquipment.Instance.UnEquip(clickIndex)) return;

                    inventory.AddItem(itemId, amount);

                }
                break;
            case SlotType.Container:
                if (playerStashUI)
                {
                    playerStashUI.AddItemSlot(itemId, amount);
                    tradePanelUI.RemoveSlot(clickIndex);
                }
                break;
        }
        Initialize();
    }
    public void SelectTradeItem(string itemID)
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out var itemData)) return;

        string text = itemData.DisplayName + "\t" + "아이템 가치 : " + itemData.ItemPrice; 
        tradePanelUI.SetItemDisplayText(text,itemData.ItemID);
    }
    public bool InventorySlotChange()
    {
        if (DragType == SlotType.Player && DropType == SlotType.Player)
        {
            inventory.PositionChange(SelectDrag.SlotIndex, SelectDrop.SlotIndex);
            SelectDrag = default;
            SelectDrop = default;
        }
        else if (DragType == SlotType.Enemy && DropType == SlotType.Player)
        {
            if (inventory.AddItem(SelectDrag.ItemID, SelectDrag.Amount) >= 1)
            {
                Initialize();
                return false;
            }
            Debug.Log("추가됨");
            enemyInventory.ListRemove(SelectDrag.SlotIndex);
        }
        else if (DragType == SlotType.Player && DropType == SlotType.Enemy)
        {
            if (!inventory.RemoveItem(SelectDrag.ItemID, SelectDrag.SlotIndex, SelectDrag.Amount))
            {
                Initialize();
                return false;
            }
            enemyInventory.ListAddItem(SelectDrag.ItemID, SelectDrag.Amount);
            inventory.AddItem(SelectDrop.ItemID, SelectDrop.Amount);
        }
        else if (DragType == SlotType.Player && DropType == SlotType.Stash)
        {
            if (!inventory.RemoveItem(SelectDrag.ItemID, SelectDrag.SlotIndex, SelectDrag.Amount))
            {
                Debug.Log("리턴됨");
                Initialize();
                return false;
            }
            playerStashUI.DrawSlot(SelectDrag.ItemID, SelectDrag.Amount, SelectDrop.SlotIndex);
            inventory.AddItem(SelectDrop.ItemID, SelectDrop.Amount);

        }
        else if (DragType == SlotType.Stash && DropType == SlotType.Player)
        {
            playerStashUI.RemoveItemSlot(SelectDrag.SlotIndex);
            inventory.AddItem(SelectDrag.ItemID, SelectDrag.Amount, SelectDrop.SlotIndex);
            playerStashUI.DrawSlot(SelectDrop.ItemID, SelectDrop.Amount, SelectDrag.SlotIndex);

        }
        else if (DragType == SlotType.Stash && DropType == SlotType.Stash)
        {
            playerStashUI.SlotChange(SelectDrag.SlotIndex, SelectDrop.SlotIndex);
        }
        else if (DragType == SlotType.Stash && DropType == SlotType.Container)
        {
            if (tradePanelUI.IsSlotItemID(SelectDrop.SlotIndex)) return false;
            playerStashUI.RemoveItemSlot(SelectDrag.SlotIndex);
            tradePanelUI.AddSlot(SelectDrag.ItemID,SelectDrag.Amount,SelectDrop.SlotIndex);
            tradePanelUI.ContainerTotalPrice();

        }
        else if (DragType == SlotType.Player && DropType == SlotType.Equip)
        {
            if (!IsVailedCheck(SelectDrag.ItemID, SelectDrop.ItemID)) return false;

            inventory.RemoveItem(SelectDrag.ItemID, SelectDrag.SlotIndex, SelectDrag.Amount);
            if (!PlayerBaseEquipment.Instance.Equip(SelectDrag.ItemID, out string backItemID)) return false;
            inventory.AddItem(backItemID, 1);
        }
        else if (DragType == SlotType.Equip && DropType == SlotType.Player)
        {
            if (!IsVailedCheck(SelectDrag.ItemID, SelectDrop.ItemID)) return false;
            if (!PlayerBaseEquipment.Instance.UnEquip(SelectDrag.SlotIndex)) return false;
            inventory.RemoveItem(SelectDrop.ItemID, SelectDrop.SlotIndex, SelectDrop.Amount);
            PlayerBaseEquipment.Instance.Equip(SelectDrop.ItemID, out _);

            if (!ItemCatalogManager.Instance.TryGetItemData(SelectDrag.ItemID, out var dragData)) return false;

            if (dragData.Type != ItemType.BackPack) inventory.AddItem(SelectDrag.ItemID, 1, SelectDrop.SlotIndex);
            else inventory.AddItem(SelectDrag.ItemID, 1);
        }
        else if (DragType == SlotType.Stash && DropType == SlotType.Equip)
        {
            if (!IsVailedCheck(SelectDrag.ItemID, SelectDrop.ItemID)) return false;
            playerStashUI.RemoveItemSlot(SelectDrag.SlotIndex);
            PlayerBaseEquipment.Instance.Equip(SelectDrag.ItemID, out string backItemID);
            playerStashUI.DrawSlot(backItemID, 1, SelectDrag.SlotIndex);
        }
        else if (DragType == SlotType.Equip && DropType == SlotType.Stash)
        {
            if (!IsVailedCheck(SelectDrag.ItemID, SelectDrop.ItemID)) return false;
            //아이템 확인 먼저 필요
            if (!PlayerBaseEquipment.Instance.UnEquip(SelectDrag.SlotIndex)) return false;
            playerStashUI.RemoveItemSlot(SelectDrop.SlotIndex);
            playerStashUI.DrawSlot(SelectDrag.ItemID, 1, SelectDrop.SlotIndex);
            PlayerBaseEquipment.Instance.Equip(SelectDrop.ItemID, out _);
        }
        Initialize();
        return true;
    }

    private bool IsVailedCheck(string dragID, string dropID)
    {
        ItemType dragType = ItemType.None;
        ItemType dropType = ItemType.None;

        if(ItemCatalogManager.Instance.TryGetItemData(dragID,out var data))
        {
            dragType = data.Type;
        }
        if (ItemCatalogManager.Instance.TryGetItemData(dropID,out data))
        {
            dropType = data.Type;
        }
        if((dragType == dropType) || (dragType == ItemType.Helmet && dropType == ItemType.None))
        {
            return true;
        }
        else if ((dragType == dropType) || (dragType == ItemType.Weapon && dropType == ItemType.None))
        {
            return true;
        }
        else if ((dragType == dropType) || (dragType == ItemType.Body && dropType == ItemType.None))
        {
            return true;
        }
        else if ((dragType == dropType) || (dragType == ItemType.Pents && dropType == ItemType.None))
        {
            return true;
        }
        else if ((dragType == dropType) || (dragType == ItemType.Shoes && dropType == ItemType.None))
        {
            return true;
        }
        else if ((dragType == dropType) || (dragType == ItemType.BackPack && dropType == ItemType.None))
        {
            return true;
        }
        return false;
    }
}
