using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItemSlot : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI displayNameText;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] TextMeshProUGUI priceText;

    private void Awake()
    {
        if(iconImage == null) iconImage = transform.GetChild(0).GetComponent<Image>();
        if(displayNameText == null) displayNameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if(amountText == null) amountText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        if(priceText == null) priceText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }
    public void Init(string itemID, int amount)
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(itemID, out ItemData itemData)) return;
        iconImage.sprite = itemData.ItemIcon;
        displayNameText.text = $"{itemData.DisplayName}";
        amountText.text = amount.ToString();
        priceText.text = itemData.ItemPrice.ToString();
    }
}
