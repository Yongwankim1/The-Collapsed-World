using System;
using UnityEngine;
[System.Serializable]
public struct Stage
{
    public bool IsWaterRepair;
    public bool IsPowerRepair;
    public bool IsBossDie;
    
    public bool IsClear()
    {
        if (IsWaterRepair && IsPowerRepair) return true;
        return false;
    }
}
[DefaultExecutionOrder(-995)]
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public Stage[] Stages;
    public EnemyLevelScriptable[] enemyLevelScriptables;

    public string CurrentStageName { get; private set; } = "±¤ÁÖ";
    public int CurrentStage { get; private set; } = 0;
    public EnemyLevelScriptable currentEnemyLevel;

    public event Action OnChangeCurrentStage;
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
    public void SetCurrentStageName(string stageName)
    {
        CurrentStageName = stageName;
        OnChangeCurrentStage?.Invoke();
    }
    public void SetCurrentStage(int index)
    {
        if (index >= Stages.Length || index < 0) return;
        CurrentStage = index;
        OnChangeCurrentStage?.Invoke();
    }
    public void SetEnemyLevel(int index)
    {
        currentEnemyLevel = enemyLevelScriptables[index];
    }

    public StageData GetData()
    {
        StageData data = new StageData();
        int len = Stages.Length;
        data.isBossDie = new bool[len];
        data.isPowerRepair = new bool[len];
        data.isWaterRepair = new bool[len];
        for (int i = 0; i < Stages.Length; i++)
        {
            data.isBossDie[i] = Stages[i].IsBossDie;
            data.isPowerRepair[i] = Stages[i].IsPowerRepair;
            data.isWaterRepair[i] =(Stages[i].IsWaterRepair);
        }
        return data;
    }
    public void LoadData(StageData data)
    {
        Stages = new Stage[data.isBossDie.Length];
        for (int i = 0; i < data.isPowerRepair.Length; i++)
        {
            Stages[i].IsBossDie = data.isBossDie[i];
            Stages[i].IsWaterRepair = data.isWaterRepair[i];
            Stages[i].IsPowerRepair = data.isPowerRepair[i];
        }
    }
}
