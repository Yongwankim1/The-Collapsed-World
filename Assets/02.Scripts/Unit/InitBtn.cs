using UnityEngine;

public class InitBtn : MonoBehaviour
{
    public void OnInit()
    {
        if(SaveManager.Instance) SaveManager.Instance.Init();
    }
}
