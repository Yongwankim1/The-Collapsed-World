using UnityEngine;

public class ToolTipBtn : MonoBehaviour
{
    [SerializeField] GameObject tooltipPanel;

    public void OnTooltipPanel()
    {
        if (EscManager.Instance == null) return;

        EscManager.Instance.PushPanel(tooltipPanel);
    }
}
