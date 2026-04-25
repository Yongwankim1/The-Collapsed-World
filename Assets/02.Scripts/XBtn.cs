using UnityEngine;
using UnityEngine.UI;

public class XBtn : MonoBehaviour
{
    [SerializeField] GameObject closePanel;
    Button btn;
    private void Awake()
    {
        btn = GetComponent<Button>();
    }
    private void OnEnable()
    {
        btn.onClick.AddListener(() =>
        {
            //closePanel.SetActive(false);
            EscManager.Instance.PopPanel();
        });
    }
    private void OnDisable()
    {
        btn.onClick.RemoveAllListeners();
    }
}
