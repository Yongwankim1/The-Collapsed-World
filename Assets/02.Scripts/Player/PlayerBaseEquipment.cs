using System;
using UnityEngine;
[DefaultExecutionOrder(-999)]
public class PlayerBaseEquipment : MonoBehaviour
{
    public static PlayerBaseEquipment Instance;
    public string WeaponID;
    public string HeadArmorID;
    public string BodyArmorID;
    public string ShoesArmorID;
    public string PentsArmorID;
    public string BackPackID;
    public event Action OnChangeEquip;
    public event Action OnChangeBackpack;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public EquipmentData GetData()
    {
        return new EquipmentData
        {
            head = HeadArmorID,
            body = BodyArmorID,
            pents = PentsArmorID,
            shoes = ShoesArmorID,
            backpack = BackPackID,
            weapon = WeaponID
        };
    }

    public void LoadData(EquipmentData data)
    {
        HeadArmorID = data.head;
        BodyArmorID = data.body;
        PentsArmorID = data.pents;
        ShoesArmorID = data.shoes;
        BackPackID = data.backpack;
        WeaponID = data.weapon;
        OnChangeBackpack?.Invoke();
        OnChangeEquip?.Invoke();
    }
    public void OnDie()
    {
        WeaponID = string.Empty;
        HeadArmorID = string.Empty;
        BodyArmorID = string.Empty;
        ShoesArmorID = string.Empty;
        PentsArmorID = string.Empty;
        BackPackID = string.Empty;
        OnChangeBackpack?.Invoke();
        OnChangeEquip?.Invoke();
    }
    public bool UnEquip(int index)
    {
        if (index == 0) HeadArmorID = string.Empty;
        else if (index == 1) BodyArmorID = string.Empty;
        else if (index == 2) PentsArmorID = string.Empty;
        else if (index == 3) ShoesArmorID = string.Empty;
        else if (index == 4) WeaponID = string.Empty;
        else if (index == 5)
        {
            if (!PlayerInventoryData.Instance.CheckChangeItems(8)) return false;

            BackPackID = string.Empty;
            OnChangeBackpack?.Invoke();
        }
        OnChangeEquip?.Invoke();
        return true;
    }
    public bool Equip(string equipID, out string backItemID)
    {
        backItemID = string.Empty;
        
        ItemCatalogManager.Instance.TryGetItemData(equipID, out var data);

        if(data.Type == ItemType.BackPack)
        {
            if (!PlayerInventoryData.Instance.CheckChangeItems(data.Value1)) return false;
        }
        switch (data.Type)
        {
            case ItemType.Weapon: backItemID = WeaponID; WeaponID = equipID; break;
            case ItemType.Helmet: backItemID = HeadArmorID; HeadArmorID = equipID; break;
            case ItemType.Body: backItemID = BodyArmorID; BodyArmorID = equipID; break;
            case ItemType.Pents: backItemID = PentsArmorID; PentsArmorID = equipID; break;
            case ItemType.Shoes: backItemID = ShoesArmorID; ShoesArmorID = equipID;break;
            case ItemType.BackPack: backItemID = BackPackID; BackPackID = equipID; OnChangeBackpack?.Invoke(); break;
        }
        OnChangeEquip?.Invoke();
        return true;
    }
}
