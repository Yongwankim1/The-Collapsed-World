using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : CharacterHP
{
    [SerializeField] PlayerSurvivalStat playerSurvival;
    [SerializeField] float equipHP;

    public float BaseMaxHP => baseMaxHP;
    public float CurrentHP => currentHP;

    public event Action<float, float> OnChangeHP;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        if (playerSurvival != null)
        {
            playerSurvival.OnHungerDebuff += TakeDamage;
            playerSurvival.OnHydrationDebuff += TakeDamage;
        }

        if (PlayerBaseEquipment.Instance != null)
        {
            PlayerBaseEquipment.Instance.OnChangeEquip += RefreshEquipHP;
        }
        if (PlayerBaseState.Instacne) PlayerBaseState.Instacne.OnChangeState += Initialize;
        Initialize();
    }

    private void OnDisable()
    {
        if (playerSurvival != null)
        {
            playerSurvival.OnHungerDebuff -= TakeDamage;
            playerSurvival.OnHydrationDebuff -= TakeDamage;
        }

        if (PlayerBaseEquipment.Instance != null)
        {
            PlayerBaseEquipment.Instance.OnChangeEquip -= RefreshEquipHP;
        }

        if (PlayerBaseState.Instacne != null)
        {
            PlayerBaseState.Instacne.BaseMaxHP = baseMaxHP;
            PlayerBaseState.Instacne.CurrentHP = currentHP;
            PlayerBaseState.Instacne.MaxHP = maxHP;
            PlayerBaseState.Instacne.EquipHP = equipHP;
            PlayerBaseState.Instacne.OnChangeState -= Initialize;
        }
    }

    private void Initialize()
    {
        if (playerSurvival == null)
            playerSurvival = GetComponent<PlayerSurvivalStat>();

        if (PlayerBaseState.Instacne != null)
        {
            baseMaxHP = PlayerBaseState.Instacne.BaseMaxHP;
            currentHP = PlayerBaseState.Instacne.CurrentHP;
            maxHP = PlayerBaseState.Instacne.MaxHP;
            equipHP = PlayerBaseState.Instacne.EquipHP;
        }

        RefreshEquipHP();
    }

    private float CalculateEquipHP()
    {
        float value = 0f;

        if (PlayerBaseEquipment.Instance == null) return value;
        if (ItemCatalogManager.Instance == null) return value;

        if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.HeadArmorID, out var headData))
            value += headData.Value1;

        if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.BodyArmorID, out var bodyData))
            value += bodyData.Value1;

        if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.PentsArmorID, out var pentsData))
            value += pentsData.Value1;

        if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.ShoesArmorID, out var shoesData))
            value += shoesData.Value1;

        return value;
    }

    private void RefreshEquipHP()
    {
        equipHP = CalculateEquipHP();
        maxHP = baseMaxHP + equipHP;

        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        currentHP = Mathf.Round(currentHP);
        maxHP = Mathf.Round(maxHP);

        SyncToBaseState();

        OnChangeHP?.Invoke(maxHP, currentHP);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        SyncToBaseState();
        OnChangeHP?.Invoke(maxHP, currentHP);
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        SyncToBaseState();
        OnChangeHP?.Invoke(maxHP, currentHP);
    }

    protected override void Died()
    {
        base.Died();
        if (PlayerInventoryData.Instance != null)
            PlayerInventoryData.Instance.OnDie();
        if(PlayerBaseEquipment.Instance != null)
            PlayerBaseEquipment.Instance.OnDie();
        SceneManager.LoadScene("ResultScene");
    }
    private void SyncToBaseState()
    {
        if (PlayerBaseState.Instacne == null) return;

        PlayerBaseState.Instacne.BaseMaxHP = baseMaxHP;
        PlayerBaseState.Instacne.CurrentHP = currentHP;
        PlayerBaseState.Instacne.MaxHP = maxHP;
        PlayerBaseState.Instacne.EquipHP = equipHP;
    }
}