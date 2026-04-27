using UnityEngine;
[CreateAssetMenu(fileName = "HPHeal", menuName = "Item/HealItem")]
public class HPHealScriptable : ItemScriptable
{
    [SerializeField] AudioClip healSound;
    public override void Use(Inventory inventory)
    {
        PlayerHP playerHP = inventory.GetComponent<PlayerHP>();
        if (playerHP == null)
        {
            PlayerBaseState.Instacne.CurrentHP += ItemData.Value1;
            PlayerBaseState.Instacne.CurrentHP = Mathf.Min(PlayerBaseState.Instacne.CurrentHP, PlayerBaseState.Instacne.MaxHP);
            return;
        }
        playerHP.Heal(ItemData.Value1);
        Debug.Log("ÈúÅÛ »ç¿ë");
        if (!healSound || !SoundManager.Instance) return;
        SoundManager.Instance.PlaySfxOneShot(healSound);
    }
}
