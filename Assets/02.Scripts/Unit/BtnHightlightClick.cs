using UnityEngine;
using UnityEngine.EventSystems;

public class BtnHightlightClick : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] AudioClip btnHightlight;

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance == null && btnHightlight) return;
        SoundManager.Instance.PlaySfxOneShot(btnHightlight,0.7f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SoundManager.Instance == null && btnHightlight) return;
        //SoundManager.Instance.StopSfx();
    }
}
