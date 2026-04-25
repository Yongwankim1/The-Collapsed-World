using TMPro;
using UnityEngine;

public class PlayerStashTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI weaponText;
    [SerializeField] TextMeshProUGUI equipmentsText;
    [SerializeField] TextMeshProUGUI hungerText;
    [SerializeField] TextMeshProUGUI hydrationText;
    [SerializeField] PlayerHP playerHP;
    private void Awake()
    {
        if(playerHP == null) playerHP = GetComponent<PlayerHP>();
    }

    private void OnEnable()
    {
        if (!PlayerBaseState.Instacne || !PlayerBaseEquipment.Instance || !playerHP) return;
        PlayerBaseEquipment.Instance.OnChangeEquip += OnChangeWeaponDamageText;
        PlayerBaseEquipment.Instance.OnChangeEquip += OnChangeEquipmentText;
        playerHP.OnChangeHP += OnChangeHPText;

        PlayerBaseState.Instacne.OnChangeState += OnChangeHungerHydrationText;


        OnChangeHPText(playerHP.MaxHP,playerHP.CurrentHP);
        OnChangeEquipmentText();
        OnChangeHungerHydrationText();
        OnChangeWeaponDamageText();
    }
    private void OnDisable()
    {
        if (!PlayerBaseState.Instacne || !PlayerBaseEquipment.Instance || !playerHP) return;
        PlayerBaseEquipment.Instance.OnChangeEquip -= OnChangeWeaponDamageText;
        PlayerBaseEquipment.Instance.OnChangeEquip -= OnChangeEquipmentText;
        playerHP.OnChangeHP -= OnChangeHPText;

        PlayerBaseState.Instacne.OnChangeState -= OnChangeHungerHydrationText;

        OnChangeHPText(playerHP.MaxHP,playerHP.CurrentHP);
        OnChangeEquipmentText();
        OnChangeHungerHydrationText();
        OnChangeWeaponDamageText();
    }

    public void OnChangeHPText(float maxvValue, float currentValue)
    {
        if (!hpText || !playerHP) return;

        hpText.text = $"체력 : {currentValue}/{maxvValue}";
    }

    public void OnChangeWeaponDamageText()
    {
        if (!damageText || !PlayerBaseEquipment.Instance) return;

        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.WeaponID, out var item))
        {
            damageText.text = "공격력 : 0";
            weaponText.text = "무기 : 없음";
            return;
        }
        damageText.text = $"공격력 : {item.Damage}";
        weaponText.text = $"무기 : {item.DisplayName}";
    }

    public void OnChangeEquipmentText()
    {
        if(!equipmentsText || !PlayerBaseEquipment.Instance) return;

        string equipments = "착용 장비\n";
        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.HeadArmorID, out var itemData)) equipments += "머리 : \n";
        else equipments += $"머리 : {itemData.DisplayName}\n";
        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BodyArmorID, out itemData)) equipments += "상의 : \n";
        else equipments += $"상의 : {itemData.DisplayName}\n";
        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.PentsArmorID, out itemData)) equipments += "하의 : \n";
        else equipments += $"하의 : {itemData.DisplayName}\n";
        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.ShoesArmorID, out itemData)) equipments += "신발 : \n";
        else equipments += $"신발 : {itemData.DisplayName}\n";
        if(!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BackPackID, out itemData)) equipments += "가방 : \n";
        else equipments += $"가방 : {itemData.DisplayName}\n";

       equipmentsText.text = equipments;
    }

    public void OnChangeHungerHydrationText()
    {
        if (!hungerText || !hydrationText || !PlayerBaseState.Instacne) return;

        hungerText.text = $"배고픔 : {PlayerBaseState.Instacne.hunger.ToString("0.0")}/{PlayerBaseState.Instacne.MaxHunger}";
        hydrationText.text = $"수분 : {PlayerBaseState.Instacne.Hydration.ToString("0.0")}/{PlayerBaseState.Instacne.MaxHydration}";
    }
}
