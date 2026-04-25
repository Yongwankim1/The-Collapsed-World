using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemPositiones
{
    public string ItemID;
    public int Amount;
    public int MaxAmount;

    public void SetItemPositiones(string itemID, int amount)
    {
        ItemID = itemID;
        Amount = amount;
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out var data)) return;

        MaxAmount = data.MaxStack;
    }
}
public class Inventory : MonoBehaviour
{
    [SerializeField] protected string backpackID;
    [SerializeField] protected ItemPositiones[] itemPositiones;

    public ItemPositiones[] ItemPositiones => itemPositiones;

    protected Dictionary<string, int> itemIdByCount = new Dictionary<string, int>();
    public Dictionary<string,int> ItemIdByCount => itemIdByCount;

    public event Action OnChangeItem;
    public event Action OnBackPackChanage;
    private void RaiseOnChanageItem() => OnChangeItem?.Invoke();
    private void RaiseOnBackPackChange() => OnBackPackChanage?.Invoke();

    private void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        if(PlayerInventoryData.Instance != null)
        {
            PlayerInventoryData.Instance.OnChangeBackpackData += Init;

        }
    }

    private void OnDisable()
    {
        if (PlayerInventoryData.Instance != null)
        {
            PlayerInventoryData.Instance.OnChangeBackpackData -= Init;
        }
    }
    protected void Init()
    {
        if(PlayerInventoryData.Instance != null)
        {
            itemIdByCount.Clear();
            backpackID = PlayerBaseEquipment.Instance.BackPackID;
            itemPositiones = PlayerInventoryData.Instance.BaseBackPack;
            for(int i =0; i < itemPositiones.Length; i++)
            {
                if (string.IsNullOrEmpty(itemPositiones[i].ItemID)) continue;
                if (itemIdByCount.ContainsKey(itemPositiones[i].ItemID))
                {
                    itemIdByCount[itemPositiones[i].ItemID] += itemPositiones[i].Amount;
                }
                else
                {
                    itemIdByCount.Add(itemPositiones[i].ItemID, itemPositiones[i].Amount);
                }
            }
        }
        else
        {
            itemPositiones = new ItemPositiones[8];
        }
        RaiseOnBackPackChange();
    }
    public void Init(string backpackID)
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(backpackID, out ItemData itemData))
        {
            itemPositiones = new ItemPositiones[8];
            return;
        }
        this.backpackID = backpackID;
        ItemPositiones[] items = itemPositiones;

        itemPositiones = new ItemPositiones[itemData.Value1];

        for(int i = 0; i < items.Length; i++)
        {
            itemPositiones[i] = items[i];
        }
        RaiseOnBackPackChange();
    }
    public int AddItem(string itemId, int amount)
    {
        Debug.Log("1");
        return IncreaseItem(itemId, amount);
    }
    public void AddItem(string itemId, int amount, int index)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        if (!string.IsNullOrEmpty(itemPositiones[index].ItemID)) return;

        itemPositiones[index].SetItemPositiones(itemId, amount);
        if (itemIdByCount.ContainsKey(itemId))
        {
            ItemIdByCount[itemId] += amount;
        }
        else
        {
            ItemIdByCount.Add(itemId, amount);
        }
        RaiseOnChanageItem();
    }
    public bool UseItem(string itemid, int index, int amount)
    {
        return DecreaseItem(itemid,index, amount);
    }
    public void PositionChange(int drag, int drop)
    {
        ItemPositiones dragPos = itemPositiones[drag];
        ItemPositiones dropPos = itemPositiones[drop];

        itemPositiones[drag] = dropPos;
        itemPositiones[drop] = dragPos;


        RaiseOnChanageItem();
        Debug.Log("체인지");
    }
    public int RemainingBagCount()
    {
        int count = 0;
        for(int i =0; i < itemPositiones.Length; i++)
        {
            if (string.IsNullOrEmpty(itemPositiones[i].ItemID))
            {
                count++;
            }
        }
        Debug.Log(count);
        return count;
    }
    protected int IncreaseItem(string itemId, int amount)
    {
        if (amount <= 0) return 0;
        if (!ItemCatalogManager.Instance.TryGetItemData(itemId, out var data)) return amount;

        int restAmount = amount;
        for (int i = 0; i < itemPositiones.Length; i++)
        {
            if (string.IsNullOrEmpty(itemPositiones[i].ItemID)) continue;
            if (itemPositiones[i].Amount >= itemPositiones[i].MaxAmount) continue;

            if (itemPositiones[i].ItemID == itemId && itemPositiones[i].Amount < itemPositiones[i].MaxAmount)
            {
                int canAdd = itemPositiones[i].MaxAmount - itemPositiones[i].Amount;
                int addAmount = Mathf.Min(canAdd, restAmount);

                itemPositiones[i].Amount += addAmount;
                restAmount -= addAmount;

                if (itemIdByCount.ContainsKey(itemId))
                {
                    itemIdByCount[itemId] += addAmount;
                }
                else
                {
                    itemIdByCount.Add(itemId, addAmount);
                }
                if (restAmount <= 0)
                {
                    RaiseOnChanageItem();
                    return restAmount;
                }
            }
        }
        for (int i = 0; i < itemPositiones.Length; i++)
        {
            if (!string.IsNullOrEmpty(itemPositiones[i].ItemID)) continue;
            itemPositiones[i].ItemID = itemId;
            itemPositiones[i].MaxAmount = data.MaxStack;
            int canAdd = itemPositiones[i].MaxAmount + itemPositiones[i].Amount;
            int addAmount = Math.Min(canAdd, restAmount);

            itemPositiones[i].Amount = addAmount;
            restAmount -= addAmount;
            if (itemIdByCount.ContainsKey(itemId))
            {
                itemIdByCount[itemId] += addAmount;
            }
            else
            {
                itemIdByCount.Add(itemId, addAmount);
            }
            if (restAmount <= 0)
            {
                RaiseOnChanageItem();
                return restAmount;
            }
        }


        RaiseOnChanageItem();
        return restAmount;
    }

    protected bool DecreaseItem(string itemId,int index ,int amount)
    {
        if (amount <= 0) return false;
        if (itemPositiones[index].ItemID != itemId) return false;
        if (itemPositiones[index].Amount - amount < 0) return false;

        itemPositiones[index].Amount -= amount;
        if (itemPositiones[index].Amount <= 0)
        {
            itemPositiones[index] = default;
        }

        if (itemIdByCount.ContainsKey(itemId))
        {
            itemIdByCount[itemId] -= amount;
            if (itemIdByCount[itemId] <= 0)
            {
                itemIdByCount.Remove(itemId);
            }
        }
        if (ItemCatalogManager.Instance.TryGetItemClass(itemId, out var itemClass))
        {
            itemClass.Use(this);
        }
        RaiseOnChanageItem();
        return true;
    }
    public void RemoveItem(string itemId, int amount)
    {
        if (amount < 0) return;
        if (!itemIdByCount.ContainsKey(itemId))
        {
            return;
        }

        itemIdByCount[itemId] -= amount;
        if (itemIdByCount[itemId] <= 0)
        {
            itemIdByCount.Remove(itemId);
        }
        int canAmount;
        for(int i = itemPositiones.Length -1; i >= 0; i--)
        {
            if (itemPositiones[i].ItemID != itemId) continue;
            canAmount = itemPositiones[i].Amount;
            itemPositiones[i].Amount -= canAmount;
            if (itemPositiones[i].Amount <= 0)
            {
                itemPositiones[i] = new ItemPositiones();
            }
            amount -= canAmount;
            if(amount <= 0)
            {
                break;
            }
        }


        RaiseOnChanageItem();
        return;
    }
    public bool RemoveItem(string itemId, int index, int amount)
    {
        if (amount <= 0) return false;
        if (itemPositiones[index].ItemID != itemId) return false;
        if (itemPositiones[index].Amount - amount < 0) return false;
        if (!itemIdByCount.ContainsKey(itemId))
        {
            return false;
        }

        itemIdByCount[itemId] -= amount;
        if (itemIdByCount[itemId] <= 0)
        {
            itemIdByCount.Remove(itemId);
        }
        itemPositiones[index].Amount -= amount;
        if (itemPositiones[index].Amount <= 0)
        {
            itemPositiones[index] = default;
        }

        Debug.Log("제거됨");
        RaiseOnChanageItem();
        return true;
    }
    [ContextMenu("PrintItem")]
    private void DebugInventory()
    {
        foreach (var item in ItemIdByCount)
        {
            Debug.Log(item.Key + " : " + item.Value);
        }
    }
    public bool CheckItem(string itemID, int amount)
    {
        if (itemIdByCount.ContainsKey(itemID))
        {
            if (itemIdByCount[itemID] >= amount)
            {
                return true;
            }
        }
        return false;
    }
}
