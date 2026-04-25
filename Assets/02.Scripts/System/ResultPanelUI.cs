using System.Collections;
using TMPro;
using UnityEngine;

public class ResultPanelUI : MonoBehaviour
{
    [SerializeField] KillLogSlot killLogSlotPrefab;
    [SerializeField] PickUpItemSlot pickUpItemSlotPrefab;
    [SerializeField] Transform killLogParent;
    [SerializeField] Transform totalItemLogParent;
    [SerializeField] TextMeshProUGUI totalPrice;
    private void Awake()
    {
        if (ResultManager.Instance == null) return;

        Init();
    }

    void Init()
    {
        if (totalPrice == null || killLogSlotPrefab == null || pickUpItemSlotPrefab == null) return;
        if(PlayerInventoryData.Instance != null)
        {
            int price = PlayerInventoryData.Instance.CheckTotalPrice();
            StartCoroutine(CoCountPrice(price));
            for(int i = 0; i < PlayerInventoryData.Instance.BaseBackPack.Length; i++)
            {
                if (string.IsNullOrEmpty(PlayerInventoryData.Instance.BaseBackPack[i].ItemID)) continue;

                PickUpItemSlot pickupSlot = Instantiate(pickUpItemSlotPrefab, totalItemLogParent);
                pickupSlot.Init(PlayerInventoryData.Instance.BaseBackPack[i].ItemID, PlayerInventoryData.Instance.BaseBackPack[i].Amount);
            }
        }

        if (ResultManager.Instance == null) return;
        int count = ResultManager.Instance.KillLogDataCount();
        for(int i = 0; i < count; i++)
        {
            KillLogSlot slot = Instantiate(killLogSlotPrefab,killLogParent);
            KillLogData data = ResultManager.Instance.DeQueueKillLog();
            int min = (int)data.KillTime / 60;
            int sec = (int)data.KillTime % 60;
            string time = $"{min:00} : {sec:00}";
            slot.Init(time, data.KillName, data.KillWeapon);
        }
    }

    IEnumerator CoCountPrice(int targetPrice)
    {
        float duration = 0.7f;
        float time = 0f;

        int startPrice = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            int currentPrice = Mathf.RoundToInt(Mathf.Lerp(startPrice, targetPrice, t));

            totalPrice.text = $"ÃÑ È¹µæ °¡Ä¡\n{currentPrice:N0}";

            yield return null;
        }

        // ¸¶Áö¸· º¸Á¤ (µü ¸ÂÃß±â)
        totalPrice.text = $"ÃÑ È¹µæ °¡Ä¡\n{targetPrice:N0}";
    }
}
