using TMPro;
using UnityEngine;

public class PlayerStateUI : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] TextMeshProUGUI[] stateTexts; // 0 HP, 1 AttackValue, 2 EquipWeapon, 3 Armor, 4 SurvivalStat
    [SerializeField] PlayerHP playerHP;
    [SerializeField] PlayerStamina playerStamina;
    [SerializeField] PlayerSurvivalStat playerSurvival;

    private void Awake()
    {
        if(playerHP == null) playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        if(playerStamina == null) playerStamina = playerHP.GetComponent<PlayerStamina>();
        if(playerSurvival == null) playerSurvival = playerHP.GetComponent<PlayerSurvivalStat>();
        if (stateTexts == null) stateTexts = GetComponentsInChildren<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        if (!playerHP || !playerStamina || !playerSurvival) return;
        playerHP.OnChangeHP += UpdateHP;
        PlayerBaseEquipment.Instance.OnChangeEquip += UpdateEquipText;
        playerSurvival.OnHungerDisplayChanged += UpdateSurivalStat;
        playerSurvival.OnHydrationDisplayChanged += UpdateSurivalStat;
        UpdateHP(playerHP.MaxHP,playerHP.CurrentHP);
        UpdateEquipText();
        UpdateSurivalStat();
    }

    private void OnDisable()
    {
        if (!playerHP || !playerStamina || !playerSurvival) return;
        playerHP.OnChangeHP -= UpdateHP;
        PlayerBaseEquipment.Instance.OnChangeEquip -= UpdateEquipText;
        playerSurvival.OnHungerDisplayChanged -= UpdateSurivalStat;
        playerSurvival.OnHydrationDisplayChanged -= UpdateSurivalStat;
        UpdateHP(playerHP.MaxHP, playerHP.CurrentHP);

    }
    private void UpdateSurivalStat()
    {
        stateTexts[4].text = $"배고픔 : {playerSurvival.Hunger.ToString("0.0")}\t수분 : {playerSurvival.Hydration.ToString("0.0")}";
    }
    private void UpdateHP(float maxValue, float currentValue)
    {
        stateTexts[0].text = $"체력 : {currentValue} / {maxValue}";
    }

    private void UpdateEquipText()
    {
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.WeaponID, out var itemData))
        {
            stateTexts[1].text = $"공격력 :";
            stateTexts[2].text = $"무기 :";
        }
        else
        {
            stateTexts[1].text = $"공격력 : {itemData.Damage}";
            stateTexts[2].text = $"무기 : {itemData.DisplayName}";
        }
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.HeadArmorID, out itemData))
        {
            stateTexts[3].text = "착용장비\n" + "머리 :\n";
        }
        else stateTexts[3].text = "착용장비\n" + $"머리 : {itemData.DisplayName}\n";
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BodyArmorID, out itemData))
        {
            stateTexts[3].text += $"상의 :\n";
        }
        else stateTexts[3].text += $"상의 : {itemData.DisplayName}\n";
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.PentsArmorID, out itemData))
        {
            stateTexts[3].text += $"하의 :\n";
        }
        else stateTexts[3].text += $"하의 : {itemData.DisplayName}\n";
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.ShoesArmorID, out itemData))
        {
            stateTexts[3].text += $"신발 :\n";
        }
        else stateTexts[3].text += $"신발 : {itemData.DisplayName}\n";
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BackPackID, out itemData))
        {
            stateTexts[3].text += $"가방 :\n";
        }
        else stateTexts[3].text += $"가방 : {itemData.DisplayName}\n";
    }
}
