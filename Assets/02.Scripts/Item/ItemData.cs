using UnityEngine;
public enum ItemType
{
    None,
    //무기
    Weapon,

    //가방
    BackPack,

    //방어구
    Helmet,
    Body,
    Pents,
    Shoes,

    //총알
    Bullet,

    //회복타입
    HPHeal,
    Bandage,
    Splint,

    //주사기 타입
    Vaccine,
    Healing,

    //재료들
    Material,
    Food,
}
[System.Serializable]
public struct ItemData
{
    [SerializeField] ItemType type;
    [SerializeField] WorldItem worldItemPrefab;
    [SerializeField] string itemID;
    [SerializeField] string displayName;
    [SerializeField] string description;
    [SerializeField] int maxStack;
    [SerializeField] Sprite itemIcon;
    [SerializeField] int value1;
    [SerializeField] int value2;
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] float attackCoolDown;
    [SerializeField] GameObject weaponEffect;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] AttackType attackType;
    [SerializeField] int itemPrice;

    public readonly WorldItem WorldItem => worldItemPrefab;
    public readonly ItemType Type => type;
    public readonly string ItemID => itemID;
    public readonly string DisplayName => displayName;
    public readonly string Description => description;
    public readonly int MaxStack => maxStack;
    public readonly Sprite ItemIcon => itemIcon;
    public readonly int Value1 => value1;
    public readonly int Value2 => value2;
    public readonly float Damage => damage;
    public readonly float Speed => speed;
    public readonly float AttackCoolDown => attackCoolDown;
    public readonly GameObject WeaponEffect => weaponEffect;
    public readonly GameObject WeaponPrefab => weaponPrefab;
    public readonly AttackType AttackType => attackType;
    public readonly int ItemPrice => itemPrice;
}
