using UnityEngine;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    [SerializeField] Image myImage;
    [SerializeField] Sprite activeSprite;
    [SerializeField] int stageIndex;
    private void Awake()
    {
        if(myImage == null)
            myImage = GetComponent<Image>();
    }

    private void OnDisable()
    {
        
    }
    private void OnEnable()
    {
        if (StageManager.Instance == null) return;

        if (StageManager.Instance.Stages[stageIndex].IsClear())
        {
            myImage.sprite = activeSprite;
        }
    }
}
