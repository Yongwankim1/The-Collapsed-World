using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    float currentDamage;
    bool isActive;

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    public void Open(float damage)
    {
        currentDamage = damage;
        isActive = true;
        hitTargets.Clear();
    }

    public void Close()
    {
        isActive = false;
        hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        IDamageable damageable = other.GetComponentInChildren<IDamageable>();
        if (damageable == null) return;
        if (hitTargets.Contains(damageable)) return;

        hitTargets.Add(damageable);
        damageable.TakeDamage(currentDamage);
    }
}