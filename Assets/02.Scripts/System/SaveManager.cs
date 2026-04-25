using Newtonsoft.Json;
using System.IO;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public PlayerData player;
    public InventoryData inventory;
    public EquipmentData equipment;
    public StashData playerStash;
    public StageData stageData;
    public TimeData timeData;
}
[System.Serializable]
public class InventoryData
{
    public string[] itemIDs;
    public int[] amounts;
    public int[] maxAmounts;
}
[System.Serializable]
public class EquipmentData
{
    public string weapon;
    public string head;
    public string body;
    public string pents;
    public string shoes;
    public string backpack;
}
[System.Serializable]
public class PlayerData
{
    public float baseMaxHP;
    public float maxHP;
    public float currentHP;
    public float equipHP;

    public float maxHunger;
    public float hunger;
    public float maxHydration;
    public float hydration;
}
[System.Serializable]
public class StashData
{
    public string[] stashIDs;
    public int[] amounts;
    public int[] maxAmounts;
}
[System.Serializable]
public class StageData
{
    public bool[] isWaterRepair;
    public bool[] isPowerRepair;
    public bool[] isBossDie;
}
[System.Serializable]
public class TimeData
{
    public float playDay;
    public float time;
}
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

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

        savePath = Path.Combine(Application.persistentDataPath, "save.json");

    }
    private void OnEnable()
    {
        Load();
        Save();
    }
    public void Init()
    {
        Debug.Log("새 게임 시작");

        GameData newData = CreateDefaultData();
        ApplyData(newData);
        Save();
    }
    public void Save()
    {
        GameData data = new GameData();

        data.player = PlayerBaseState.Instacne.GetData();
        data.inventory = PlayerInventoryData.Instance.GetData();
        data.equipment = PlayerBaseEquipment.Instance.GetData();
        data.playerStash = PlayerStashManager.Instance.GetData();
        data.stageData = StageManager.Instance.GetData();
        data.timeData = TimeLightingController.Instance.GetData();
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(savePath, json);

        Debug.Log("저장 완료: " + savePath);
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("세이브 파일 없음");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonConvert.DeserializeObject<GameData>(json);

        PlayerBaseState.Instacne.LoadData(data.player);
        PlayerInventoryData.Instance.LoadData(data.inventory);
        PlayerBaseEquipment.Instance.LoadData(data.equipment);
        PlayerStashManager.Instance.LoadData(data.playerStash);
        StageManager.Instance.LoadData(data.stageData);
        TimeLightingController.Instance.LoadData(data.timeData);
        Debug.Log("불러오기 완료");
    }

    GameData CreateDefaultData()
    {
        GameData data = new GameData();

        data.player = new PlayerData
        {
            baseMaxHP = 100f,
            maxHP = 100f,
            currentHP = 100f,
            equipHP = 0f,
            maxHunger = 100f,
            hunger = 100f,
            maxHydration = 100f,
            hydration = 100f
        };

        data.inventory = new InventoryData
        {
            itemIDs = new string[20],
            amounts = new int[20],
            maxAmounts = new int[20]
        };

        data.equipment = new EquipmentData
        {
            weapon = "Weapon01",
            head = "",
            body = "",
            pents = "",
            shoes = "",
            backpack = ""
        };

        data.playerStash = new StashData
        {
            stashIDs = new string[100],
            amounts = new int[100],
            maxAmounts = new int[100],
        };
        data.stageData = new StageData
        {
            isBossDie = new bool[10],
            isPowerRepair = new bool[10],
            isWaterRepair = new bool[10]
        };
        data.timeData = new TimeData
        {
            time = 0f,
            playDay = 0,
        };
        data.playerStash.stashIDs[0] = "Weapon01";
        data.playerStash.stashIDs[1] = "Weapon01";
        data.playerStash.stashIDs[2] = "Weapon01";
        data.playerStash.amounts[0] = 1;
        data.playerStash.amounts[1] = 1;
        data.playerStash.amounts[2] = 1;
        data.playerStash.maxAmounts[0] = 1;
        data.playerStash.maxAmounts[1] = 1;
        data.playerStash.maxAmounts[2] = 1;
        data.playerStash.stashIDs[3] = "BackPack01";
        data.playerStash.stashIDs[4] = "BackPack01";
        data.playerStash.stashIDs[5] = "BackPack01";
        data.playerStash.amounts[3] = 1;
        data.playerStash.amounts[4] = 1;
        data.playerStash.amounts[5] = 1;
        data.playerStash.maxAmounts[3] = 1;
        data.playerStash.maxAmounts[4] = 1;
        data.playerStash.maxAmounts[5] = 1;
        return data;
    }

    void ApplyData(GameData data)
    {
        PlayerBaseState.Instacne.LoadData(data.player);
        PlayerInventoryData.Instance.LoadData(data.inventory);
        PlayerBaseEquipment.Instance.LoadData(data.equipment);
        PlayerStashManager.Instance.LoadData(data.playerStash);
        StageManager.Instance.LoadData(data.stageData);
        TimeLightingController.Instance.LoadData(data.timeData);
    }
}