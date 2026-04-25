using UnityEngine;

public class ResultSceneManager : MonoBehaviour
{
    public void OnGOBaseScene()
    {
        if(GameSceneManager.Instance != null)
            GameSceneManager.Instance.LoadSceneByName("BaseScene");
    }
}
