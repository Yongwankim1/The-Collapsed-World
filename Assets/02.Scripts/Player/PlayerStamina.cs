using System;
using System.Collections;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] PlayerInputReader inputReader;
    [SerializeField] PlayerSurvivalStat playerSurvival;
    [Header("Value")]
    [SerializeField] float maxStamina;
    [SerializeField] float currentStamina;
    [SerializeField] float walkDeCreaseValue;
    [SerializeField] float runDeCraseValue;
    [SerializeField] float inCreaseValue;
    [SerializeField] float attactDeCreaseValue;
    [Header("Option")]
    [SerializeField] bool isDebug;

    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;

    public event Action<float,float> OnChangeStamina;

    public bool IsAttack => CurrentStamina >= attactDeCreaseValue && !IsStaminaDepleted;

    public bool IsStaminaDepleted {  get; private set; }

    Coroutine staminaIncrease;
    private void Awake()
    {
        Init();
    }
    void Init()
    {
        if (PlayerBaseState.Instacne != null)
        {
            maxStamina = PlayerBaseState.Instacne.MaxStamina;
            currentStamina = MaxStamina;
        }
        else
        {
            if (maxStamina <= 0) maxStamina = 100f;
            currentStamina = maxStamina;
        }
        ChangeWeapon();

        if (inputReader == null) inputReader = GetComponent<PlayerInputReader>();
        if (playerSurvival == null) playerSurvival = GetComponent<PlayerSurvivalStat>();
    }
    void ChangeWeapon()
    {
        if (PlayerBaseEquipment.Instance != null)
        {
            if (ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.WeaponID, out var itemData))
            {
                attactDeCreaseValue = itemData.Value1;
            }
        }
    }
    private void OnEnable()
    {
        if (PlayerBaseEquipment.Instance == null) return;

        PlayerBaseEquipment.Instance.OnChangeEquip += ChangeWeapon;
    }
    private void OnDisable()
    {
        if (PlayerBaseEquipment.Instance == null) return;

        PlayerBaseEquipment.Instance.OnChangeEquip -= ChangeWeapon;
    }
    private void Update()
    {
        SetWalkStamina(walkDeCreaseValue);

        SetRunStamina(runDeCraseValue);
    }

    private void SetWalkStamina(float walkValue)
    {
        if (IsStaminaDepleted) return;
        InCreaseCurrentStamina();
        //if (inputReader.MoveAction())
        //{
        //    DeCreaseCurrentStamina(walkValue);
        //}
        //else
        //{
        //    InCreaseCurrentStamina();
        //}
    }




    private void SetRunStamina(float runValue)
    {
        if (IsStaminaDepleted) return;
        if (inputReader.RunAction())
        {
            DeCreaseCurrentStamina(runValue);
        }
    }
    public void AttackDecraseValue()
    {
        if (IsStaminaDepleted) return;

        if(playerSurvival.IsHunger && playerSurvival.IsHydration)
        {
            currentStamina -= attactDeCreaseValue * 2.4f;
        }
        else if (playerSurvival.IsHunger || playerSurvival.IsHydration)
        {
            currentStamina -= attactDeCreaseValue * 1.6f;
        }
        else
        {
            currentStamina -= attactDeCreaseValue;
        }

            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        OnChangeStamina?.Invoke(maxStamina, currentStamina);
        if (isDebug)
            Debug.Log($"스테미나 감소량 {attactDeCreaseValue} 남은 스테미나{currentStamina} 최대 스테미나 {maxStamina}");
        if (staminaIncrease == null)
        {
            staminaIncrease = StartCoroutine(IncreaseCurrentStamina(inCreaseValue));
        }
        else
        {
            StopCoroutine(staminaIncrease);
            staminaIncrease = StartCoroutine(IncreaseCurrentStamina(inCreaseValue));
        }

    }
    private void InCreaseCurrentStamina()
    {
        if (staminaIncrease == null)
        {
            staminaIncrease = StartCoroutine(IncreaseCurrentStamina(inCreaseValue));
        }
    }
    private void DeCreaseCurrentStamina(float amount)
    {
        if (staminaIncrease != null)
        {
            StopCoroutine(staminaIncrease);
            staminaIncrease = null;
        }
        if (playerSurvival.IsHunger && playerSurvival.IsHydration)
        {
            currentStamina -= amount * 2.4f * Time.deltaTime;
            if (isDebug)
                Debug.Log("탈수, 배고픔");
        }
        else if(playerSurvival.IsHunger || playerSurvival.IsHydration)
        {
            currentStamina -= amount * 1.6f * Time.deltaTime;
            if (isDebug)
                Debug.Log("탈수 또는 배고픔");
        }
        else
        {
            currentStamina -= amount * Time.deltaTime;
            if (isDebug)
                Debug.Log("디버프 없음");
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        OnChangeStamina?.Invoke(maxStamina, currentStamina);

        if (isDebug)
            Debug.Log($"스테미나 감소량 {amount} 남은 스테미나{currentStamina} 최대 스테미나 {maxStamina}");

        if(currentStamina <= 0)
        {
            IsStaminaDepleted = true;
            if (staminaIncrease == null)
            {
                staminaIncrease = StartCoroutine(RecoverStamina());
            }
            else
            {
                StopCoroutine(staminaIncrease);
                staminaIncrease = StartCoroutine(RecoverStamina());
            }
        }
    }
    IEnumerator RecoverStamina()
    {
        while (true)
        {
            if(playerSurvival.IsHydration && playerSurvival.IsHunger)
            {
                currentStamina += inCreaseValue * 0.4f * Time.deltaTime;
            }
            else if(playerSurvival.IsHydration || playerSurvival.IsHunger)
            {
                currentStamina += inCreaseValue * 0.8f * Time.deltaTime;
            }
            else
            {
                currentStamina += inCreaseValue * Time.deltaTime;
            }
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            OnChangeStamina?.Invoke(maxStamina, currentStamina);
            if (isDebug)
                Debug.Log($"스테미나 증가량 {inCreaseValue} 남은 스테미나{currentStamina} 최대 스테미나 {maxStamina}");
            if(currentStamina >= maxStamina / 4)
            {
                IsStaminaDepleted = false;
                if (staminaIncrease == null)
                {
                    staminaIncrease = StartCoroutine(IncreaseCurrentStamina(inCreaseValue));
                }
                else
                {
                    StopCoroutine(staminaIncrease);
                    staminaIncrease = StartCoroutine(IncreaseCurrentStamina(inCreaseValue));
                }
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator IncreaseCurrentStamina(float amount)
    {
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            if (playerSurvival.IsHydration && playerSurvival.IsHunger)
            {
                currentStamina += amount * 0.4f * Time.deltaTime;
            }
            else if (playerSurvival.IsHydration || playerSurvival.IsHunger)
            {
                currentStamina += amount * 0.8f * Time.deltaTime;
            }
            else
            {
                currentStamina += amount * Time.deltaTime;
            }
            currentStamina = Mathf.Min(currentStamina, maxStamina);

            OnChangeStamina?.Invoke(maxStamina, currentStamina);

            if (isDebug)
                Debug.Log($"스테미나 증가량 {amount} 남은 스테미나{currentStamina} 최대 스테미나 {maxStamina}");
            yield return null;
        }
    }
}
