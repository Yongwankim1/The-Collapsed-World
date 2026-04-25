using TMPro;
using UnityEngine;

public class KillLogSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeStampText;
    [SerializeField] TextMeshProUGUI killNameText;
    [SerializeField] TextMeshProUGUI killItemText;
    
    public void Init(string time, string name, string item)
    {
        timeStampText.text = time;
        killNameText.text = name;
        killItemText.text = item;
    }
}
