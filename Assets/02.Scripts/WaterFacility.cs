using UnityEngine;

public class WaterFacility : FacilityBase
{
    [SerializeField] GameObject activeObject;
    private void Awake()
    {
        if(activeObject == null)
            activeObject = gameObject.GetComponentInChildren<GameObject>();
        activeObject.SetActive(false);
    }
    private void CheckRepair()
    {
        activeObject.SetActive(isRepair);
    }
    void OnEnable()
    {
        if (StageManager.Instance != null)
        {
            isRepair = StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsWaterRepair;
        }
        CheckRepair();
    }

    void OnDisable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsWaterRepair = isRepair;
        }
        CheckRepair();
    }

    public override void Repair()
    {
        isRepair = true;
        if (StageManager.Instance != null)
        {
            StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsWaterRepair = isRepair;
        }
        CheckRepair();
    }
}
