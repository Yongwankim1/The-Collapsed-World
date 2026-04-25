using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class PlayerSurvivalStat : MonoBehaviour
{
    [Header("PlayerState")]
    [SerializeField] float currentHunger;
    [SerializeField] float maxHunger;
    [SerializeField] float currentHydration;
    [SerializeField] float maxHydration;

    [Header("Value")]
    [SerializeField] float hungerDecraseValue;
    [SerializeField] float hydrationDecraseValue;
    [SerializeField] float hungerDebuffTimer;
    [SerializeField] float hydrationDebuffTimer;
    [SerializeField] float hungerDebuffValue;
    [SerializeField] float hydrationDebuffValue;

    [Header("Option")]
    [SerializeField] bool isDebug;
    public float Hunger => currentHunger;
    public float Hydration => currentHydration;

    public bool IsHunger => currentHunger <= 0;
    public bool IsHydration => currentHydration <= 0;

    public event Action OnHungerDisplayChanged;
    public event Action OnHydrationDisplayChanged;
    public event Action<float> OnHungerDebuff;
    public event Action<float> OnHydrationDebuff;

    Coroutine hungerDebuff;
    Coroutine hydrationDebuff;

    float prevHungerDisplay;
    float prevHydrationDisplay;
    private void Awake()
    {
        if(PlayerBaseState.Instacne == null)
        {
            currentHunger = maxHunger;
            currentHydration = maxHydration;
        }
        else
        {
            maxHunger = PlayerBaseState.Instacne.MaxHunger;
            maxHydration = PlayerBaseState.Instacne.MaxHydration;
            currentHunger = PlayerBaseState.Instacne.hunger;
            currentHydration = PlayerBaseState.Instacne.Hydration;
        }
        if (ResultManager.Instance != null)
            ResultManager.Instance.SetTimer();
    }
    private void OnEnable()
    {
        if (PlayerBaseState.Instacne == null) return;
        maxHunger = PlayerBaseState.Instacne.MaxHunger;
        maxHydration = PlayerBaseState.Instacne.MaxHydration;
        currentHunger = PlayerBaseState.Instacne.hunger;
        currentHydration = PlayerBaseState.Instacne.Hydration;
    }

    private void OnDisable()
    {
        if (PlayerBaseState.Instacne == null) return;
        PlayerBaseState.Instacne.MaxHunger = maxHunger;
        PlayerBaseState.Instacne.MaxHydration = maxHydration;
        PlayerBaseState.Instacne.hunger = currentHunger;
        PlayerBaseState.Instacne.Hydration = currentHydration;
    }
    private void Update()
    {
        if(StageManager.Instance != null && !StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsWaterRepair)
            DecraseHydration(hydrationDecraseValue);
        UpdateHydrationEvent();
        if (StageManager.Instance != null && !StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsPowerRepair)
            DecraseHunger(hungerDecraseValue);
        UpdateHungerEvent();
        

        HungerDebuffCheck();
        HydrationDebuffCheck();
    }


    void UpdateHungerEvent()
    {
        float currentDisplay = Mathf.Round(currentHunger * 10f) / 10f;

        if (currentDisplay != prevHungerDisplay)
        {
            prevHungerDisplay = currentDisplay;
            OnHungerDisplayChanged?.Invoke();
        }
    }
    void UpdateHydrationEvent()
    {
        float currentDisplay = Mathf.Round(currentHunger * 10f) / 10f;

        if (currentDisplay != prevHydrationDisplay)
        {
            prevHydrationDisplay = currentDisplay;
            OnHydrationDisplayChanged?.Invoke();
        }
    }
    public void IncraseHunger(float amount)
    {
        currentHunger += amount;
        
        currentHunger = Mathf.Clamp(currentHunger,0,maxHunger);

    }
    public void IncraseHyDration(float amount)
    {
        currentHydration += amount;

        currentHydration = Mathf.Clamp(currentHydration,0,maxHydration);
    }

    private float DecraseHunger(float amount)
    {
        if (currentHunger <= 0) return 0;
        currentHunger -= amount * Time.deltaTime;
        currentHunger = Mathf.Max(currentHunger, 0);
        if (isDebug)
            Debug.Log($"배고픔이 {amount}만큼 감소 되었습니다");
        return currentHunger;
    }
    private float DecraseHydration(float amount)
    {
        if (currentHydration <= 0) return 0;
        currentHydration -= amount * Time.deltaTime;
        currentHydration = Mathf.Max(currentHydration, 0);
        if (isDebug)
            Debug.Log($"목마름이 {amount}만큼 감소 되었습니다");
        return currentHydration;
    }
    private void HungerDebuffCheck()
    {
        if (!IsHunger)
        {
            if (hungerDebuff != null)
            {
                StopCoroutine(hungerDebuff);
                hungerDebuff = null;
            }
            return;
        }

        if (hungerDebuff == null)
        {
            hungerDebuff = StartCoroutine(OnHungerTickDebuff());
        }

    }
    private void HydrationDebuffCheck()
    {
        if (!IsHydration)
        {
            if (hydrationDebuff != null)
            {
                StopCoroutine(hydrationDebuff);
                hydrationDebuff = null;
            }
            return;
        }

        if (hydrationDebuff == null)
        {
            hydrationDebuff = StartCoroutine(OnHydrationTickDebuff());
        }
    }
    IEnumerator OnHungerTickDebuff()
    {
        float timer = 0;
        while (timer <= hungerDebuffTimer)
        {
            OnHungerDebuff?.Invoke(hungerDebuffValue);
            yield return new WaitForSeconds(3f);
            timer += 3f;
        }
        hungerDebuff = null;
    }
    IEnumerator OnHydrationTickDebuff()
    {
        float timer = 0;
        while (timer <= hydrationDebuffTimer)
        {
            OnHydrationDebuff?.Invoke(hydrationDebuffValue);
            yield return new WaitForSeconds(3f);
            timer += 3f;
        }
        hydrationDebuff = null;
    }
}
