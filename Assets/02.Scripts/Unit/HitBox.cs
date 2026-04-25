using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] BoxCollider2D attackCollider;
    private HashSet<IDamageable> hash = new HashSet<IDamageable>();
    [SerializeField] float damage;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] AudioClip hitSound = null;
    private void Awake()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }
    public void Initialize(float damage, LayerMask target, AudioClip clip = null)
    {
        hitSound = clip;
        this.damage = damage;
        gameObject.SetActive(true);
        targetLayer = target;
        hash.Clear();
    }
    private void OnEnable()
    {
        hash.Clear();
    }
    public void OnAttack()
    {
        attackCollider.enabled = true;
    }

    public void OffAttack()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null) return;
        if (hash.Contains(damageable)) return;
        damageable.TakeDamage(damage);
        hash.Add(damageable);
        if(SoundManager.Instance != null && hitSound != null) SoundManager.Instance.PlaySfxOneShot(hitSound);
        Debug.Log(collision.name + "이 " + damage + "만큼 맞음");
    }
    public void ActiveFalse()
    {
        if(PoolingManager.Instance != null)
        {
            PoolingManager.Instance.Return(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
