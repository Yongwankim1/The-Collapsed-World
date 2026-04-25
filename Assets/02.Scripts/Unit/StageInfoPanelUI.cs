using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanelUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageNameText;
    [SerializeField] Toggle powerToggle;
    [SerializeField] Toggle waterToggle;
    [SerializeField] Toggle bossToggle;
    [SerializeField] Button startBtn;
    private string stageName;
    public void Start()
    {
        if(startBtn == null) startBtn = GetComponentInChildren<Button>();
    }
    private void OnEnable()
    {
        if (StageManager.Instance == null) return;

        StageManager.Instance.OnChangeCurrentStage += SetInfoPanel;
    }

    private void OnDisable()
    {
        if (StageManager.Instance == null) return;

        StageManager.Instance.OnChangeCurrentStage -= SetInfoPanel;
    }
    public void SetInfoPanel(string stageName)
    {
        this.stageName = stageName;
    }
    private void SetInfoPanel()
    {
        if(StageManager.Instance == null) return;
        if(!powerToggle || !waterToggle || !bossToggle || !stageNameText)
        powerToggle.isOn = StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsPowerRepair;
        waterToggle.isOn = StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsWaterRepair;
        bossToggle.isOn = StageManager.Instance.Stages[StageManager.Instance.CurrentStage].IsBossDie;
        stageNameText.text = StageManager.Instance.CurrentStageName;
        string sceneName = "Stage" + StageManager.Instance.CurrentStage;
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            //Debug.LogWarning($"[GameSceneManager] LoadSceneByName 실패: Build Settings에 없는 씬입니다. sceneName={sceneName}");
            startBtn.interactable = false;
            return;
        }
        else
        {
            startBtn.interactable = true;
        }
    }
}
