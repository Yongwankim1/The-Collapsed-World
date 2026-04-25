using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float damage = 5f;
    [SerializeField] float attackCoolDown = 1f;
    [SerializeField] HitBox hitBox;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float attackDistance;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip hitSound;
    public float AttackCoolDown => attackCoolDown;

    public void BeginAttack(Transform targetTansform, out HitBox hitBox)
    {
        Vector2 dir = (targetTansform.position - transform.position).normalized;
        Vector2 attackPos = (Vector2)transform.position + dir * attackDistance;

        GameObject go = PoolingManager.Instance.Get(this.hitBox.gameObject);
        if(go == null)
        {
            go = Instantiate(this.hitBox.gameObject, attackPos , Quaternion.identity);
            Debug.Log("Null");
        }

        go.transform.position = attackPos;
        go.name = this.hitBox.name;
        hitBox = go.GetComponent<HitBox>();
        hitBox.Initialize(damage, targetLayer,hitSound);
        go.SetActive(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySfxOneShot(attackSound);
    }


    public void EndAttack(HitBox hitBox)
    {
        if (hitBox == null) return;
        hitBox.ActiveFalse();
    }
}