using System;
using System.Collections;
using UnityEngine;
[DefaultExecutionOrder(-996)]
public class PlayerBaseState : MonoBehaviour
{
    public static PlayerBaseState Instacne;
    private enum StateType
    {
        HP,
        Hunger,
        Hydration
    }
    [Header("HP, Stamina State")]
    public float BaseMaxHP;
    public float EquipHP;
    public float MaxHP;
    public float CurrentHP;
    public float MaxStamina;
    
    [Header("SurivalState")]
    public float Hydration;
    public float MaxHydration;
    public float hunger;
    public float MaxHunger;

    [Header("IncraseValue")]
    [SerializeField] float hpIncraseAmount;
    [SerializeField] float hungerIncraseAmount;
    [SerializeField] float hydrationIncraseAmount;

    public event Action OnChangeState;

    private void Awake()
    {
        if(Instacne == null)
        {
            Instacne = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public PlayerData GetData()
    {
        return new PlayerData
        {
            baseMaxHP = BaseMaxHP,
            maxHP = MaxHP,
            currentHP = CurrentHP,
            equipHP = EquipHP,
            hunger = hunger,
            hydration = Hydration,
            maxHunger = MaxHunger,
            maxHydration = MaxHydration,
        };
    }

    public void LoadData(PlayerData data)
    {
        BaseMaxHP = data.baseMaxHP;
        MaxHP = data.maxHP;
        CurrentHP = data.currentHP;
        EquipHP = data.equipHP;
        hunger = data.hunger;
        Hydration = data.hydration;
        MaxHunger = data.maxHunger;
        MaxHydration = data.maxHydration;

        OnChangeState?.Invoke();
    }

    public void IncraseState()
    {
        StartCoroutine(InceraseValue(StateType.HP));
        StartCoroutine(InceraseValue(StateType.Hunger));
        StartCoroutine(InceraseValue(StateType.Hydration));
    }
    public void StopIncraseState()
    {
        StopAllCoroutines();
    }
    IEnumerator InceraseValue(StateType type)
    {
        float amount = 0f;

        switch (type)
        {
            case StateType.HP:
                amount = hpIncraseAmount;
                break;
            case StateType.Hunger:
                amount = hungerIncraseAmount;
                break;
            case StateType.Hydration:
                amount = hydrationIncraseAmount;
                break;
        }

        if (amount <= 0f) yield break;

        while (true)
        {
            if (CurrentHP <= 0f) CurrentHP = 10f;
            if (hunger <= 0f) hunger = 10f;
            if (Hydration <= 0f) Hydration = 10f;
            yield return new WaitForSeconds(60f);

            switch (type)
            {
                case StateType.HP:

                    CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
                    break;

                case StateType.Hunger:

                    hunger = Mathf.Min(hunger + amount, MaxHunger);
                    break;

                case StateType.Hydration:

                    Hydration = Mathf.Min(Hydration + amount, MaxHydration);
                    break;
            }

            OnChangeState?.Invoke();
        }
    }
}


