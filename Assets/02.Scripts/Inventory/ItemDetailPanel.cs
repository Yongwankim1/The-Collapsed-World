using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerStashUI stashUI;
    //TODO::스테쉬 추가해서 Use기능 사용하기
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] Button closeBtn;
    [SerializeField] Button useBtn;

    [SerializeField] TextMeshProUGUI useBtnText;
    [SerializeField] SlotType currentSlotType;
    private string itemID;
    public int Index {  get; private set; }
    private void Awake()
    {
        if(inventory == null) inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if(itemNameText == null) itemNameText = GetComponent<TextMeshProUGUI>();
        if(stashUI == null) stashUI = GetComponent<PlayerStashUI>();
    }
    private void Initialize()
    {
        itemID = string.Empty;
        itemNameText.text = string.Empty;
        itemDescriptionText.text = string.Empty;
    }
    public void Initialize(string itemID, int index, SlotType slotType)
    {
        Initialize();
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out var itemData))
        {
            return;
        }
        this.itemID = itemID;
        Index = index;
        EscManager.Instance.PushPanel(gameObject);
        //gameObject.SetActive(true);
        iconImage.sprite = itemData.ItemIcon;
        itemNameText.text = "이름\n" + itemData.DisplayName;
        string description = "설명\n" + itemData.Description +"\n";
        switch (itemData.Type)
        {
            case ItemType.Weapon: description += $"공격력 : {itemData.Damage}\n스테미나 소모량: {itemData.Value1}\n공격 쿨타임 : {itemData.AttackCoolDown}"; break;
            case ItemType.Helmet:
            case ItemType.Body:
            case ItemType.Pents:
            case ItemType.Shoes: description += $"추가 체력 : {itemData.Value1}"; break;
            case ItemType.HPHeal: description += $"회복량 : {itemData.Value1}"; break;
            case ItemType.BackPack: description += $"가방 크기 : {itemData.Value1}"; break;
            case ItemType.Food: description += $"수분 : {itemData.Value2}\n허기 : {itemData.Value1}"; break;
            default: break;
        }
        itemDescriptionText.text = description;
        currentSlotType = slotType;
        useBtn.gameObject.SetActive(true);
        switch (currentSlotType)
        {
            case SlotType.Player: useBtnText.text = "Use"; break;
            case SlotType.Equip: useBtnText.text = "UnEquip"; break;
            default: useBtn.gameObject.SetActive(false); break;
        }
    }


    private void OnEnable()
    {
        closeBtn.onClick.AddListener(() =>
        {
            EscManager.Instance.PopPanel();
        });
        useBtn.onClick.AddListener(() =>
        {
            if (!ItemCatalogManager.Instance.TryGetItemClass(itemID, out var itemClass)) return;
            if (inventory == null) return;

            if (currentSlotType == SlotType.Player)
            {
                inventory.RemoveItem(itemID, Index, 1);
                itemClass.Use(inventory);
            }
            else if (currentSlotType == SlotType.Stash)
            {
                //TODO::스테쉬에서 디테일 패널 유즈 사용
                //stashUI.RemoveItemSlot(Index);
                //itemClass.Use(inventory);
            }
            else if (currentSlotType == SlotType.Equip)
            {
                //TODO:: 장비해체
                if (inventory.RemainingBagCount() <= 0) return;

                if(inventory.AddItem(itemID, 1) > 0) return;
                if(!PlayerBaseEquipment.Instance.UnEquip(Index)) return;
            }
            currentSlotType = SlotType.None;
            gameObject.SetActive(false);
        });
    }
    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        useBtn.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
