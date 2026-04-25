using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] PlayerInputReader inputReader;
    [SerializeField] FacilityBase facilityBase;
    [SerializeField] RequimentSlotUI slotPrefab;
    [SerializeField] Transform slotParent;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] private RequimentSlotUI[] slots = new RequimentSlotUI[4];
    [SerializeField] Inventory playerInventory;

    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button repairBtn;

    public void Init(FacilityBase facilityBase, string title,bool isRepair)
    {
        this.facilityBase = facilityBase;
        titleText.text = title;

        for (int i = 0; i < slots.Length; i ++)
        {
            if (slots[i] == null)
            {
                slots[i] = Instantiate(slotPrefab, slotParent);
                slots[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < facilityBase.GetRequirementCount(); i ++)
        {
            slots[i].Init(facilityBase.RequirementMaterials.ItemID[i], facilityBase.RequirementMaterials.Amount[i]);
        }
        repairBtn.interactable = !isRepair;
        descriptionText.text = isRepair ? "이 시설은 수리되어 있습니다" : "시설을 수리 하시겠습니까?";
        gameObject.SetActive(true);

    }

    public void OnRepair()
    {
        if(facilityBase == null) return;
        if (!facilityBase.CheckItem()) return;
        if (facilityBase.IsRepair) return;
        for (int i = 0; i < facilityBase.RequirementMaterials.ItemID.Length; i++)
        {
            playerInventory.RemoveItem(facilityBase.RequirementMaterials.ItemID[i], facilityBase.RequirementMaterials.Amount[i]);
        }
        facilityBase.Repair();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        facilityBase = null;
        foreach(var item in slots)
        {
            if(item == null) continue;
            item.gameObject.SetActive(false);
        }
    }
}
