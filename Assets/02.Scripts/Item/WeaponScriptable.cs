using UnityEngine;


[CreateAssetMenu(fileName = "WeaponItem", menuName = "Item/WeaponItem")]
public class WeaponScriptable : ItemScriptable, IAttack
{
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip hitSound;
    public void Attack(Transform attackTransform, float attackDistance, Vector3 mouseWorldPos, LayerMask targetLayer)
    {
        Vector3 dir = mouseWorldPos - attackTransform.position;
        Vector3 attackPosition = attackTransform.position + dir.normalized * attackDistance;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject go = PoolingManager.Instance.Get(ItemData.WeaponEffect);
        if (go == null)
        {
            go = Instantiate(ItemData.WeaponEffect);
            go.GetComponent<HitBox>().Initialize(ItemData.Damage, targetLayer,hitSound);
            go.name = ItemData.WeaponEffect.name;
        }
        go.GetComponent<HitBox>().Initialize(ItemData.Damage, targetLayer,hitSound);
        go.transform.position = attackPosition;
        go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        go.SetActive(true);
        if(SoundManager.Instance != null) SoundManager.Instance.PlaySfxOneShot(attackSound);
    }
    public override void Use(Inventory inventory)
    {
        if (PlayerBaseEquipment.Instance == null) return;
        Debug.Log(ItemData.ItemID);
        PlayerBaseEquipment.Instance.Equip(ItemData.ItemID, out string backItemID);
        inventory.AddItem(backItemID, 1);
    }
}
