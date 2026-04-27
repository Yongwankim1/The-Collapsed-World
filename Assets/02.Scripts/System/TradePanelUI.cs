using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradePanelUI : MonoBehaviour
{
    [SerializeField] PlayerStashUI playerStashUI;
    [SerializeField] List<string> buyItems = new List<string>();
    [SerializeField] SlotUI slotPrefab;
    [SerializeField] Transform traderSlotParent;
    [SerializeField] Transform containerParent;
    [SerializeField] List<SlotUI> buySlots = new List<SlotUI>();

    [SerializeField] List<SlotUI> playerSellContainer = new List<SlotUI>();
    [SerializeField] TextMeshProUGUI selectItemDisplayText;
    [SerializeField] TextMeshProUGUI containerPriceText;

    [SerializeField] string selectItemID;

    [SerializeField] AudioClip tradeSuccess;
    [SerializeField] AudioClip tradeFail;
    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        ContainerToStash();
    }

    public void SetItemDisplayText(string text,string itemId)
    {
        if (string.IsNullOrEmpty(text)) return;
        selectItemDisplayText.text = text;
        selectItemID = itemId;
    }

    void Init()
    {
        if(!playerStashUI) playerStashUI = GetComponent<PlayerStashUI>();
        selectItemDisplayText.text = "선택된 아이템";
        containerPriceText.text = "총 아이템 가치 : 0";
        selectItemID = string.Empty;
        ClearPlayerSellContainer();
        if (buyItems.Count <= 0) return;
        for(int i = buySlots.Count - 1; i >= 0; i--)
        {
            Destroy(buySlots[i].gameObject);
        }
        buySlots.Clear();
        for(int i = 0; i < buyItems.Count; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, traderSlotParent);
            slot.Initialize(buyItems[i], 1, SlotType.Trade, i);
            buySlots.Add(slot);
        }
    }
    void ContainerToStash()
    {
        for(int i = 0; i < playerSellContainer.Count;i++)
        {
            if (string.IsNullOrEmpty(playerSellContainer[i].SlotData.ItemID)) continue;

            if (!ItemCatalogManager.Instance.TryGetItemData(playerSellContainer[i].SlotData.ItemID, out var itemData)) continue;
            ItemPositiones item = new ItemPositiones();
            item.ItemID = playerSellContainer[i].SlotData.ItemID;
            item.Amount = playerSellContainer[i].SlotData.Amount;

            if(playerStashUI)
                playerStashUI.AddItemSlot(item.ItemID, item.Amount);
            
        }
        ClearPlayerSellContainer();
    }
    void ClearPlayerSellContainer()
    {
        for(int i = playerSellContainer.Count - 1; i >= 0; i--)
        {
            if (playerSellContainer[i] == null) continue;
            Destroy(playerSellContainer[i].gameObject);
        }
        for(int i = 0; i < playerSellContainer.Count; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, containerParent);
            slot.Initialize(SlotType.Container,i);
            playerSellContainer[i] = slot;
        }
    }

    public bool IsSlotItemID(int index)
    {
        if (string.IsNullOrEmpty(playerSellContainer[index].SlotData.ItemID))
        {
            return false;
        }
        return true;
    }
    public void AddSlot(string itemid, int amount)
    {
        for(int i = 0; i < playerSellContainer.Count;i++)
        {
            if (!string.IsNullOrEmpty(playerSellContainer[i].SlotData.ItemID)) continue;

            playerSellContainer[i].Initialize(itemid, amount, SlotType.Container, i);
            break;
        }
        ContainerTotalPrice();
    }
    public void AddSlot(string itemid,int amount ,int index)
    {
        playerSellContainer[index].Initialize(itemid, amount, SlotType.Container, index);
        ContainerTotalPrice();
    }
    public void RemoveSlot(int index)
    {
        playerSellContainer[index].Initialize(SlotType.Container, index);
        ContainerTotalPrice();
    }
    public void ContainerTotalPrice()
    {
        int totalPrice = 0;
        for(int i = 0; i < playerSellContainer.Count;i++)
        {
            if (string.IsNullOrEmpty(playerSellContainer[i].SlotData.ItemID)) continue;
            if (!ItemCatalogManager.Instance.TryGetItemData(playerSellContainer[i].SlotData.ItemID,out ItemData itemData)) continue;

            totalPrice += itemData.ItemPrice;
        }

        containerPriceText.text = $"총 아이템 가치 : {totalPrice}";
    }
    public void OnBuy()
    {
        if (string.IsNullOrEmpty(selectItemID))
        {
            SoundManager.Instance.PlaySfxOneShot(tradeFail);
            return;
        }
        ItemData itemData;
        int totalPrice = 0;
        for (int i = 0; i < playerSellContainer.Count; i++)
        {
            if (string.IsNullOrEmpty(playerSellContainer[i].SlotData.ItemID)) continue;
            if (!ItemCatalogManager.Instance.TryGetItemData(playerSellContainer[i].SlotData.ItemID, out itemData)) continue;

            totalPrice += itemData.ItemPrice;
        }
        if (!ItemCatalogManager.Instance.TryGetItemData(selectItemID, out itemData)) return;


        if (totalPrice < itemData.ItemPrice)
        {
            SoundManager.Instance.PlaySfxOneShot(tradeFail);
            return;
        }
        playerStashUI.AddItemSlot(selectItemID, 1);
        selectItemID = string.Empty;
        selectItemDisplayText.text = "선택된 아이템";
        ClearPlayerSellContainer();
        ContainerTotalPrice();
        SoundManager.Instance.PlaySfxOneShot(tradeSuccess);
    }
}
