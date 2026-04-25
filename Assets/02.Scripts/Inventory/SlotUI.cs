using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct SlotData
{
    public string ItemID;
    public int Amount;
    public int SlotIndex;
}
public enum SlotType
{
    None,
    Enemy,
    Player,
    Stash,
    Equip,
    Trade,
    Container
}
public class SlotUI : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] string isCheckParameterName = "IsCheck";
    private int isCheckHash;

    [SerializeField] protected Image iconImage;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected SlotType slotType;

    public SlotType SlotType => slotType;
    public SlotData SlotData;
    private void Awake()
    {
        if(animator == null)  animator = GetComponent<Animator>();
        if(iconImage == null) iconImage = GetComponentInChildren<Image>();
        isCheckHash = Animator.StringToHash(isCheckParameterName);
    }
    public void Initialize(SlotType type, int index)
    {
        slotType = type;
        SlotData = new SlotData();
        SlotData.SlotIndex = index;
        iconImage.gameObject.SetActive(false);
    }
    public void Initialize(string itemID, int amount, SlotType type, int index)
    {
        Initialize(type,index);
        SlotData.SlotIndex = index;
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out ItemData itemdata))
        {
            return;
        }
        SlotData.ItemID = itemID;
        SlotData.Amount = amount;
        if(SlotData.Amount <= 1)
        {
            amountText.text = string.Empty;
        }
        else
        {
            amountText.text = amount.ToString();
        }
        iconImage.sprite = itemdata.ItemIcon;
        iconImage.gameObject.SetActive(true);
    }

    public void Initialize(SlotData slotData)
    {
        SlotData = slotData;
    }
    public void OnCheck()
    {
        iconImage.gameObject.SetActive(true);
        amountText.text = "?";
        animator.SetTrigger("OnCheck");
    }
    public void IsCheck(bool value)
    {
        animator.SetBool(isCheckHash, value);
    }
}
