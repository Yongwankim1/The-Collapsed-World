using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-998)]
public class ItemCatalogManager : MonoBehaviour
{
    public static ItemCatalogManager Instance;

    [Header("Weapon")]
    [SerializeField] private List<ItemScriptable> registeredWeaponData = new List<ItemScriptable>();
    [Header("BackPack")]
    [SerializeField] private List<ItemScriptable> registeredBackPackData = new List<ItemScriptable>();

    [Header("Armor")]
    [SerializeField] private List<ItemScriptable> registeredHelmetData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredBodyData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredPentsData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredShoesData = new List<ItemScriptable>();

    [Header("Bullet")]
    [SerializeField] private List<ItemScriptable> registeredBulletData = new List<ItemScriptable>();
    [Header("HealType")]
    [SerializeField] private List<ItemScriptable> registeredHPHealData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredBandageData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredSplintData = new List<ItemScriptable>();
    [Header("Syringe")]
    [SerializeField] private List<ItemScriptable> registeredVaccineData = new List<ItemScriptable>();
    [SerializeField] private List<ItemScriptable> registeredHealingData = new List<ItemScriptable>();
    [Header("Material")]
    [SerializeField] private List<ItemScriptable> registeredMaterialData = new List<ItemScriptable>();
    [Header("Food")]
    [SerializeField] private List<ItemScriptable> registeredFoodData = new List<ItemScriptable>();



    private Dictionary<string,ItemData> itemIdByItemData = new Dictionary<string,ItemData>();
    private Dictionary<string,ItemScriptable> itemIdByItemClass = new Dictionary<string,ItemScriptable>();

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
        RegisterAddData(registeredWeaponData);
        RegisterAddData(registeredBackPackData);
        RegisterAddData(registeredHelmetData);
        RegisterAddData(registeredBodyData);
        RegisterAddData(registeredPentsData);
        RegisterAddData(registeredShoesData);
        RegisterAddData(registeredBulletData);
        RegisterAddData(registeredHPHealData);
        RegisterAddData(registeredBandageData);
        RegisterAddData(registeredSplintData);
        RegisterAddData(registeredVaccineData);
        RegisterAddData(registeredHealingData);
        RegisterAddData(registeredMaterialData);
        RegisterAddData(registeredFoodData);
    }
    private void Start()
    {

    }
    private void RegisterAddData(List<ItemScriptable> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            itemIdByItemData.Add(list[i].ItemData.ItemID, list[i].ItemData);
            itemIdByItemClass.Add(list[i].ItemData.ItemID, list[i]);
        }
    }
    public bool IsRegisteredItem(string itemID)
    {
        if(itemIdByItemData.ContainsKey(itemID))
        {
            return true;
        }
        return false;
    }
    public bool TryGetItemClass(string itemID, out ItemScriptable itemClass)
    {
        itemClass = default;
        if (string.IsNullOrEmpty(itemID)) return false;

        if (itemIdByItemClass.ContainsKey(itemID))
        {
            itemClass = itemIdByItemClass[itemID];
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetItemData(string itemID, out ItemData itemData)
    {
        itemData = default;
        if (string.IsNullOrEmpty(itemID)) return false;
        if (itemIdByItemData.ContainsKey(itemID))
        {
            itemData = itemIdByItemData[itemID];
            return true;
        }
        else
        {
            return false;
        }
    }
}
