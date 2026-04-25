using System.Collections.Generic;
using UnityEngine;
public struct KillLogData
{
    public string KillName;
    public string KillWeapon;
    public float KillTime;
}
public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;

    private Queue<KillLogData> killLogDatas = new Queue<KillLogData>();
    [SerializeField] private float timer = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        timer += 1 * Time.deltaTime;
    }
    public void SetTimer()
    {
        timer = 0;
    }
    public int KillLogDataCount()
    {
        return killLogDatas.Count;
    }
    public void EnqueueKillLog(string KillName)
    {
        KillLogData data = new KillLogData();
        if (!ItemCatalogManager.Instance.TryGetItemData(PlayerBaseEquipment.Instance.WeaponID, out ItemData itemData)) return;
        data.KillName = KillName;
        data.KillWeapon = itemData.DisplayName;
        data.KillTime = timer;
        killLogDatas.Enqueue(data);
    }
    public KillLogData DeQueueKillLog()
    {
        return killLogDatas.Dequeue();
    }
}
