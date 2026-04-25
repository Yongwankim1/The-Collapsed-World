public class PowerFacility : FacilityBase
{
    void OnEnable()
    {
        if (StageManager.Instance != null)
        {
            isRepair = StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsPowerRepair;
        }
    }

    void OnDisable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsPowerRepair = isRepair;
        }
    }

    public override void Repair()
    {
        isRepair = true;
        if (StageManager.Instance != null)
        {
            StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsPowerRepair = isRepair;
        }
    }
}
