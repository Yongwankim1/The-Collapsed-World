using UnityEngine;

public class PlayerStashUI : MonoBehaviour
{
    [SerializeField] SlotUI slotUiPrefab;
    [SerializeField] Transform slotParent;
    [SerializeField] int slotMaxIndex;

    [SerializeField] SlotUI[] slotUIs = new SlotUI[100];
    private void Start()
    {
        //Initialize();
    }
    private void OnEnable()
    {
        Initialize();
    }
    void Initialize()
    {
        for (int i = slotUIs.Length - 1; i >= 0; i--)
        {
            if (slotUIs[i] == null) continue;
            Destroy(slotUIs[i].gameObject);
        }
        if (PlayerStashManager.Instance == null)
        {
            Debug.Log('1');
            return;
        }
        slotMaxIndex = PlayerStashManager.Instance.SlotUIs.Length;
        slotUIs = new SlotUI[slotMaxIndex];
        DrawAllSlots();
    }

    void DrawAllSlots()
    {
        for(int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] == null) slotUIs[i] = Instantiate(slotUiPrefab,slotParent);

            if (string.IsNullOrEmpty(PlayerStashManager.Instance.SlotUIs[i].ItemID))
                slotUIs[i].Initialize(SlotType.Stash,i);
            else
                slotUIs[i].Initialize(PlayerStashManager.Instance.SlotUIs[i].ItemID, PlayerStashManager.Instance.SlotUIs[i].Amount, SlotType.Stash, i);
        }
    }
    public void SlotChange(int dragIndex, int dropIndex)
    {
        SlotData slotDragData = slotUIs[dragIndex].SlotData;
        SlotData slotDropData = slotUIs[dropIndex].SlotData;
        slotUIs[dragIndex].Initialize(slotDropData.ItemID,slotDropData.Amount,SlotType.Stash,dragIndex);
        slotUIs[dropIndex].Initialize(slotDragData.ItemID, slotDragData.Amount, SlotType.Stash, dropIndex);

    }
    public void AddItemSlot(string itemID, int amount)
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out var itemData)) return;
        for(int i =0;  i < slotUIs.Length; i++)
        {
            if (slotUIs[i].SlotData.ItemID != itemID) continue;


            int canAmount = itemData.MaxStack - slotUIs[i].SlotData.Amount;

            slotUIs[i].SlotData.Amount += canAmount;
            amount -= canAmount;
            if (amount <= 0) return;

        }
        for(int i = 0; i < slotUIs.Length;i++)
        {
            if (!string.IsNullOrEmpty(slotUIs[i].SlotData.ItemID)) continue;

            int canAmount = itemData.MaxStack - slotUIs[i].SlotData.Amount;
            slotUIs[i].Initialize(itemID, canAmount, SlotType.Stash, i);
            amount -= canAmount;

            if (amount <= 0) return;
        }
    }
    public void RemoveItemSlot(int index)
    {
        slotUIs[index].Initialize(SlotType.Stash,index);
    }
    public void DrawSlot(string itemID, int amount, int index)
    {
        if(string.IsNullOrEmpty(itemID)) return;
        slotUIs[index].Initialize(itemID, amount, SlotType.Stash, index);
    }
    private void SaveData()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] == null) continue;
            PlayerStashManager.Instance.SlotUIs[i].SetItemPositiones(slotUIs[i].SlotData.ItemID, slotUIs[i].SlotData.Amount);

        }
    }
    private void OnDisable()
    {
        if (PlayerStashManager.Instance == null) return;
        SaveData();
    }
}
