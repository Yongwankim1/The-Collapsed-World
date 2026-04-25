using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Item/FoodItem")]
public class FoodScriptable : ItemScriptable
{
    public override void Use(Inventory inventory)
    {
        PlayerSurvivalStat playerSurvival = inventory.GetComponent<PlayerSurvivalStat>();
        if (ItemData.Type != ItemType.Food) return;
        if (playerSurvival == null) return;

        playerSurvival.IncraseHunger(ItemData.Value1);
        playerSurvival.IncraseHyDration(ItemData.Value2);
    }
}
