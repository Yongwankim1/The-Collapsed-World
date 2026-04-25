using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BaseSceneBtnManager : MonoBehaviour
{
    [SerializeField] GameObject baseInventoryPanel;
    [SerializeField] GameObject selectPlayPanel;
    [SerializeField] GameObject tradePanel;
    [SerializeField] GameObject settingPanel;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (!PlayerBaseState.Instacne) return;

        PlayerBaseState.Instacne.IncraseState();
    }
    private void OnDisable()
    {
        if (!PlayerBaseState.Instacne) return;

        PlayerBaseState.Instacne.StopIncraseState();
    }
    public void OnSetCurrentStageName(string stageName)
    {
        if (StageManager.Instance == null) return;

        StageManager.Instance.SetCurrentStageName(stageName);
    }
    public void OnSetCurrentStage(int value)
    {
        if (StageManager.Instance == null) return;

        StageManager.Instance.SetCurrentStage(value);
    }
    public void OnBaseInventory()
    {
        //baseInventoryPanel.SetActive(true);
        EscManager.Instance.PushPanel(baseInventoryPanel);
    }

    public void OnSelectPlayPanel()
    {
        //selectPlayPanel.SetActive(true);
        EscManager.Instance.PushPanel(selectPlayPanel);
    }

    public void OnLoadScene(string sceneName)
    {
        if (SaveManager.Instance != null) SaveManager.Instance.Save();
        if (GameSceneManager.Instance == null) return;
        EscManager.Instance.PopPanel();
        GameSceneManager.Instance.LoadSceneByName(sceneName);
    }
    public void OnGameSceneLoad()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.Save();
        if (GameSceneManager.Instance == null) return;
        EscManager.Instance.PopPanel();
        string stageName = "Stage" + StageManager.Instance.CurrentStage;
        GameSceneManager.Instance.LoadSceneByName(stageName);
    }

    public void OnGameExit()
    {
        if(SaveManager.Instance != null) SaveManager.Instance.Save();
        Application.Quit();
    }

    public void OnTradePanel()
    {
        //tradePanel.SetActive(true);
        EscManager.Instance.PushPanel(tradePanel);
    }
    public void OnSettingPanel()
    {

        if (!settingPanel.activeSelf)
        {
            EscManager.Instance.PushPanel(settingPanel);
        }
        else
        {
            EscManager.Instance.PopPanel();
        }
        //settingPanel.SetActive(!settingPanel.activeSelf);
    }
}
