using System.Collections.Generic;
using UnityEngine;
public interface IInventoryAddRemove
{
    public void AddItem(string itemID, int amount);
    public void RemoveItem(string itemID, int amount);

}
public class InventoryRefactoring : MonoBehaviour, IInventoryAddRemove
{
    [SerializeField] protected string backpackID;

    protected Dictionary<string, int> itemIdByCount = new Dictionary<string, int>();
    public Dictionary<string, int> ItemIdByCount => itemIdByCount;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
    protected void Init()
    {

    }
    public void AddItem(string itemID,int amount)
    {
        IncreaseItem(itemID, amount);
    }
    public void RemoveItem(string itemID, int amount)
    {
        DecreaseItem(itemID, amount);
    }

    private int IncreaseItem(string itemID, int amount)
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out var data)) return amount;

        if (!CheckItemCount(amount))
        {
            return amount;
        }

        if (itemIdByCount.ContainsKey(itemID))
        {
            itemIdByCount[itemID] += amount;
            amount = 0;
        }
        else
        {
            itemIdByCount.Add(itemID, amount);
            amount = 0;

        }
        return amount;
    }
    private bool DecreaseItem(string itemID, int amount)
    {
        if (!itemIdByCount.ContainsKey(itemID))
        {
            return false;
        }
        if(itemIdByCount[itemID] - amount >= 0)
        {
            itemIdByCount[itemID] -= amount;
            if (itemIdByCount[itemID] <= 0)
            {
                itemIdByCount.Remove(itemID);
            }
            return true;
        }
        else
        {
            return false;
        }

    }
    private bool CheckItemCount(int amount)
    {
        int maxCount = 0;
        int currentCount = 0;
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BackPackID, out ItemData itemData)) maxCount = 8;
        else
        {
            maxCount = itemData.Value1;
        }
        foreach (var item in itemIdByCount)
        {
            currentCount += item.Value;
        }
        if ((currentCount + amount) <= maxCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
