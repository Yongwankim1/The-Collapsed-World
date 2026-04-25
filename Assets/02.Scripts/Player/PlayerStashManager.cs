using UnityEngine;
[DefaultExecutionOrder(-996)]
public class PlayerStashManager : MonoBehaviour
{
    public static PlayerStashManager Instance;

    [SerializeField] int slotMaxIndex;

    public ItemPositiones[] SlotUIs = new ItemPositiones[100];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //SlotUIs = new ItemPositiones[slotMaxIndex];
    }


    public StashData GetData()
    {
        StashData data = new StashData();

        int length = SlotUIs.Length;

        data.stashIDs = new string[length];
        data.amounts = new int[length];
        data.maxAmounts = new int[length];

        for (int i = 0; i < length; i++)
        {
            data.stashIDs[i] = SlotUIs[i].ItemID;
            data.amounts[i] = SlotUIs[i].Amount;
            data.maxAmounts[i] = SlotUIs[i].MaxAmount;
        }

        return data;
    }

    public void LoadData(StashData data)
    {
        if (data == null) return;
        if (data.stashIDs == null || data.amounts == null || data.maxAmounts == null) return;

        int length = Mathf.Min(SlotUIs.Length, data.stashIDs.Length, data.amounts.Length, data.maxAmounts.Length);
        SlotUIs = new ItemPositiones[length];
        for (int i = 0; i < length; i++)
        {
            SlotUIs[i].ItemID = data.stashIDs[i];
            SlotUIs[i].Amount = data.amounts[i];
            SlotUIs[i].MaxAmount = data.maxAmounts[i];
        }
    }
}
