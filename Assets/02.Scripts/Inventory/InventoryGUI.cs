using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryGUI : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] PlayerInputReader inputReader;
    [SerializeField] Inventory inventory;
    [SerializeField] Transform slotParent;
    [SerializeField] SlotUI slotUIPrefab;

    [SerializeField] GameObject enemyInventoryUI;
    [SerializeField] Transform enemySlotParent;
    [SerializeField] RandomDropInventory enemyInventory;
    [SerializeField] ItemSlotManager slotManager;

    [SerializeField] TextMeshProUGUI inventoryNameText;
    private SlotUI[] slots;
    private SlotUI[] enemySlots;


    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }
        if(inputReader == null) Debug.LogWarning("인풋 리더 없음");
        if(slotManager == null) slotManager = GetComponent<ItemSlotManager>();
        slots = new SlotUI[0];
        enemySlots = new SlotUI[10];
    }
    private void OnEnable()
    {
        if (enemyInventoryUI != null) enemyInventoryUI.SetActive(false);
        if(inventory != null)
        {
            inventory.OnBackPackChanage += BackPackSlotChanage;
            inventory.OnChangeItem += DrawAllSlot;
        }
        if(slots.Length == 0) BackPackSlotChanage();
        DrawAllSlot();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnBackPackChanage -= BackPackSlotChanage;
            inventory.OnChangeItem -= DrawAllSlot;
        }
        if(enemyInventoryUI != null)
        {
            enemyInventory = null;
            enemyInventoryUI.SetActive(false);
        }
    }
    public void OnEnemyInventory(RandomDropInventory enemyInventory)
    {
        if (enemyInventoryUI == null)
        {
            Debug.LogWarning("적 인벤토리 참조 안됨");
            return;
        }
        inventoryNameText.text = enemyInventory.InventoryName;
        this.enemyInventory = enemyInventory;
        slotManager.Initialize(enemyInventory);
        enemyInventoryUI.SetActive(true);
        EnemyDrawAllSlot();
    }
    Coroutine enemyDrawSlots;
    private void EnemyDrawAllSlot()
    {
        if(enemyDrawSlots == null)
        {
            enemyDrawSlots = StartCoroutine(EnemyDrawAllSlots());
        }
        else
        {
            StopCoroutine(enemyDrawSlots);
            enemyDrawSlots = StartCoroutine(EnemyDrawAllSlots());
        }
    }
    IEnumerator EnemyDrawAllSlots()
    {
        if (enemyInventoryUI == null) yield break;

        int index;
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] == null)
            {
                enemySlots[i] = Instantiate(slotUIPrefab, enemySlotParent);
            }
            enemySlots[i].Initialize(SlotType.Enemy, i);
        }

        for (index = 0; index < enemyInventory.DropList.Count; index++)
        {
            if (enemyInventory.DropList[index].IsCheck == false)
            {
                enemySlots[index].OnCheck();
                while(enemyInventory.DropList[index].CheckTimer >= 0.1f)
                { 
                    yield return null;
                    DropTable temp = enemyInventory.DropList[index];
                    temp.CheckTimer -= Time.deltaTime;
                    enemyInventory.DropList[index] = temp;
                }
                DropTable dropTable = enemyInventory.DropList[index];
                dropTable.IsCheck = true;

                enemyInventory.DropList[index] = dropTable;
                enemySlots[index].IsCheck(enemyInventory.DropList[index].IsCheck);
            }
            enemySlots[index].Initialize(enemyInventory.DropList[index].DropItemID, enemyInventory.DropList[index].Amount, SlotType.Enemy, index);
        }
        enemyDrawSlots = null;
    }

    private void BackPackSlotChanage()
    {
        if (inventory == null) return;
        for(int i = slots.Length -1;  i >= 0; i--)
        {
            Destroy(slots[i].gameObject);
        }
        slots = new SlotUI[inventory.ItemPositiones.Length];
        DrawAllSlot();
    }

    void DrawAllSlot()
    {
        if (inventory == null) return;
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = Instantiate(slotUIPrefab, slotParent);
                slots[i].Initialize(inventory.ItemPositiones[i].ItemID, inventory.ItemPositiones[i].Amount, SlotType.Player,i);
            }
            else
            {
                slots[i].Initialize(inventory.ItemPositiones[i].ItemID, inventory.ItemPositiones[i].Amount, SlotType.Player,i);
            }
        }
    }



}
