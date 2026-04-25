using UnityEngine;

public class BGMSFXContainer : MonoBehaviour
{
    [SerializeField] AudioClip[] soundClips;
    [SerializeField, Range(0, 1f)] float volume;
    public AudioClip[] SoundClips => soundClips;

    private void Start()
    {
        //처음 브금은 무조건 0번
        SoundManager.Instance.PlayBgm(soundClips[0]);
    }


}