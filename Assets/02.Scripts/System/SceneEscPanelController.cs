using UnityEngine;

public class SceneEscPanelController : MonoBehaviour
{
    [SerializeField] GameObject escPanel;
    [SerializeField] PlayerHP playerHP;
    [SerializeField] GameObject exitCheckPanel;

    [SerializeField] GameObject settingPanel;
    void Start()
    {
        if(playerHP ==null) playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        if (!EscManager.Instance || !escPanel) return;
        EscManager.Instance.SetCurrentEscPanel(escPanel);
    }

    public void OnExitCheckPanel()
    {
        if (!EscManager.Instance || !exitCheckPanel) return;
        EscManager.Instance.PushPanel(exitCheckPanel);
    }
    public void OnClosePanel()
    {
        if (!EscManager.Instance || !escPanel) return;
        EscManager.Instance.PopPanel();
    }
    public void OnExitPlayScene()
    {
        if(playerHP == null) return;
        playerHP.TakeDamage(float.MaxValue);
    }

    public void OnSettingPanel()
    {
        if (!settingPanel || !EscManager.Instance) return;

        EscManager.Instance.PushPanel(settingPanel);
    }
}
