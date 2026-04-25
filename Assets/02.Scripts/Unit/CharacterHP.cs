using System;
using UnityEngine;

public class CharacterHP : MonoBehaviour, IDamageable
{
    [SerializeField] protected float currentHP;
    [SerializeField] protected float baseMaxHP;
    [SerializeField] protected float maxHP;
    [SerializeField] Animator animator;

    public bool IsDead => currentHP <= 0;
    public event Action OnDied;
    public event Action<float> OnHit;
    public float MaxHP => maxHP;
    public virtual void TakeDamage(float amount)
    {
        if(IsDead || amount <= 0) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        if (IsDead)
        {
            Died();
            return;
        }
        OnHit?.Invoke(amount);
    }

    public virtual void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    protected virtual void Died()
    {
        //TODO:Á×¾úÀ» ¶§
        OnDied?.Invoke();
        

    }


}
