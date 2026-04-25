using System;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-997)]
public class PlayerInventoryData : MonoBehaviour
{
    public static PlayerInventoryData Instance;

    [SerializeField] ItemPositiones[] baseBackPack;

    public ItemPositiones[] BaseBackPack => baseBackPack;

    public event Action OnChangeBackpackData;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetBackPack();
    }

    public void OnDie()
    {
        baseBackPack = new ItemPositiones[8];
    }
    private void OnEnable()
    {
        if(PlayerBaseEquipment.Instance != null)
        {
            PlayerBaseEquipment.Instance.OnChangeBackpack += SetBackPack;
        }
    }

    private void OnDisable()
    {
        if (PlayerBaseEquipment.Instance != null)
        {
            PlayerBaseEquipment.Instance.OnChangeBackpack -= SetBackPack;
        }
    }
    public int CheckChangeItems()
    {
        int count = 0;

        for(int i = 0; i < baseBackPack.Length; i++)
        {
            if (string.IsNullOrEmpty(baseBackPack[i].ItemID)) continue;

            count++;
        }

        return count;
    }
    public int CheckTotalPrice()
    {
        int price = 0;
        for(int i = 0; i < baseBackPack.Length; i++)
        {
            if(string.IsNullOrEmpty(baseBackPack[i].ItemID)) continue;
            if (!ItemCatalogManager.Instance.TryGetItemData(baseBackPack[i].ItemID,out ItemData data)) continue;

            price += data.ItemPrice;
        }
        return price;
    }
    public bool CheckChangeItems(int amount)
    {
        int count = 0;

        for (int i = 0; i < baseBackPack.Length; i++)
        {
            if (string.IsNullOrEmpty(baseBackPack[i].ItemID)) continue;

            count++;
        }

        if(count >= amount)
        {
            return false;
        }
        return true;
    }

    public void SetBackPack()
    {
        int amount = 8;
        if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BackPackID, out var itemdata))
        {
            amount = itemdata.Value1;
        }
        if (CheckChangeItems() > amount) return;

        ItemPositiones[] items = baseBackPack;
        baseBackPack = new ItemPositiones[amount];
        List<ItemPositiones> itemlist = new List<ItemPositiones>();
        for (int i = 0; i < items.Length; i++)
        {
            if (string.IsNullOrEmpty(items[i].ItemID)) continue;

            itemlist.Add(items[i]);
        }
        for(int i = 0; i < itemlist.Count; i++)
        {
            baseBackPack[i] = itemlist[i];
        }
        OnChangeBackpackData?.Invoke();
    }

    public InventoryData GetData()
    {
        InventoryData data = new InventoryData();

        int len = BaseBackPack.Length;
        data.itemIDs = new string[len];
        data.amounts = new int[len];
        data.maxAmounts = new int[len];

        for (int i = 0; i < len; i++)
        {
            data.itemIDs[i] = BaseBackPack[i].ItemID;
            data.amounts[i] = BaseBackPack[i].Amount;
            data.maxAmounts[i] = baseBackPack[i].MaxAmount;
        }

        return data;
    }

    public void LoadData(InventoryData data)
    {
        baseBackPack = new ItemPositiones[data.itemIDs.Length];
        for (int i = 0; i < data.itemIDs.Length; i++)
        {
            BaseBackPack[i].ItemID = data.itemIDs[i];
            BaseBackPack[i].Amount = data.amounts[i];
            baseBackPack[i].MaxAmount = data.maxAmounts[i];
        }
    }
}
