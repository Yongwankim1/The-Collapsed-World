using UnityEngine;

public abstract class FacilityBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected string facilityName = string.Empty;
    [SerializeField] protected RequirementMaterials requirementMaterials;
    [SerializeField] protected InteractionUI interactionUI;
    [SerializeField] protected bool isRepair;
    void Awake()
    {
        if (interactionUI == null) interactionUI = FindFirstObjectByType<InteractionUI>(FindObjectsInactive.Include);
    }
    public RequirementMaterials RequirementMaterials => requirementMaterials;
    public bool IsRepair => isRepair;
    private Inventory playerInventory;
    public void Interact(PlayerInteract player)
    {
        playerInventory = player.GetComponent<Inventory>();
        if (playerInventory == null) return;
        interactionUI.Init(this,facilityName,isRepair);
    }
    public abstract void Repair();
    public int GetRequirementCount() => requirementMaterials.ItemID.Length;
    public bool CheckItem()
    {
        for(int i= 0; i < requirementMaterials.ItemID.Length; i++)
        {
            if (!playerInventory.CheckItem(requirementMaterials.ItemID[i], requirementMaterials.Amount[i]))
            {
                return false;
            }
        }
        return true;
    }



}
