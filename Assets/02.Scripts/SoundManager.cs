using System;
using UnityEngine;

/// <summary>
/// Singleton Pattern으로 전역 사운드 재생을 담당한다.
/// - BGM: 배경음 재생/정지
/// - SFX: 효과음 1회 재생(PlayOneShot)
/// </summary>
[DisallowMultipleComponent]
public class SoundManager : MonoBehaviour
{
    private const string Master_VOLUME_KEY = "BgmVolume";
    private const string BGM_VOLUME_KEY = "BgmVolume";
    private const string SFX_VOLUME_KEY = "BgmVolume";
    /// <summary>
    /// 다른 스크립트에서 SoundManager.Instance로 접근한다.
    /// 예) SoundManager.Instance.PlaySfxOneShot(clip);
    /// </summary>
    public static SoundManager Instance { get; private set; }

    [Header("AudioSource (Optional)")]
    [Tooltip("비어 있으면 Awake에서 자동 생성한다. BGM 전용 AudioSource.")]
    [SerializeField] private AudioSource bgmSource;
    [Tooltip("비어 있으면 Awake에서 자동 생성한다. SFX 전용 AudioSource.")]
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private float masterVolumeValue = 1f;
    [SerializeField] private float bgmVolumeValue = 1f;
    [SerializeField] private float sfxVolumeValue = 1f;
    [SerializeField] private float currentBGMVolume = 1f;


    [SerializeField] private bool isMasterMute = false;
    [SerializeField] private bool isBgmMute = false;
    [SerializeField] private bool isSfxMute = false;

    public float MasterVolume => masterVolumeValue;
    public float BgmVolume => bgmVolumeValue;
    public float SfxVolume => sfxVolumeValue;

    public bool IsMasterMute => isMasterMute;
    public bool IsBgmMute => isBgmMute;
    public bool IsSfxMute => isSfxMute;

    private void Awake()
    {
        // [Singleton 1단계] 이미 인스턴스가 있고, 그 인스턴스가 나 자신이 아니면 중복 오브젝트다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // [Singleton 2단계] 최초 1개 인스턴스를 전역 참조로 등록한다.
        Instance = this;

        // [Singleton 3단계] 씬이 바뀌어도 파괴되지 않도록 유지한다.
        DontDestroyOnLoad(gameObject);

        LoadVolumeSettingValue();
        EnsureAudioSources();
    }
    private void OnDisable()
    {
        SaveVolumeSettingValue();
    }
    private void LoadVolumeSettingValue()
    {
        masterVolumeValue = PlayerPrefs.GetFloat(Master_VOLUME_KEY);
        bgmVolumeValue = PlayerPrefs.GetFloat (BGM_VOLUME_KEY);
        sfxVolumeValue = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
    }
    private void SaveVolumeSettingValue()
    {
        PlayerPrefs.SetFloat(Master_VOLUME_KEY, masterVolumeValue);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolumeValue);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolumeValue);
        PlayerPrefs.Save();
    }
    public void DeleteOption()
    {
        PlayerPrefs.DeleteKey(Master_VOLUME_KEY);
        PlayerPrefs.DeleteKey(BGM_VOLUME_KEY);
        PlayerPrefs.DeleteKey(SFX_VOLUME_KEY);
        PlayerPrefs.Save();
    }

    #region 볼륨 세팅
    public void MasterVolumeChange(float value)
    {
        masterVolumeValue = Mathf.Clamp01(value);
        OnChangeBgmValue();
    }
    public void BGMVolumeChange(float value)
    {
        bgmVolumeValue = Mathf.Clamp01(value);
        OnChangeBgmValue();
    }
    public void SFXVolumeChange(float value)
    {
        sfxVolumeValue = Mathf.Clamp01(value);
    }
    public void MasterVolumeChange(bool value)
    {
        isMasterMute = value;
        OnChangeBgmValue();
    }
    public void BGMVolumeChange(bool value)
    {
        isBgmMute = value;
        OnChangeBgmValue();
    }
    public void SFXVolumeChange(bool value)
    {
        isSfxMute = value;
    }
    #endregion
    /// <summary>
    /// BGM을 재생한다. 기존 BGM이 있으면 clip을 교체해 다시 재생한다.
    /// </summary>
    public void PlayBgm(AudioClip bgmClip,float volume = 1f ,bool loop = true)
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("[SoundManager] PlayBgm 실패: bgmClip이 null입니다.");
            return;
        }

        if (bgmSource == null)
        {
            Debug.LogWarning("[SoundManager] PlayBgm 실패: bgmSource가 없습니다.");
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.loop = loop;
        currentBGMVolume = volume;
        bgmSource.volume = Mathf.Clamp01(masterVolumeValue * volume * bgmVolumeValue);
        if (isMasterMute || isBgmMute) bgmVolumeValue = 0;
        bgmSource.Play();
    }

    public void OnChangeBgmValue()
    {
        float value =  Mathf.Clamp01(currentBGMVolume * masterVolumeValue * bgmVolumeValue);
        if(isMasterMute || isBgmMute) value = 0;
        bgmSource.volume = value;
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 정지한다.
    /// </summary>
    public void StopBgm()
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("[SoundManager] StopBgm 실패: bgmSource가 없습니다.");
            return;
        }

        bgmSource.Stop();
    }

    /// <summary>
    /// 효과음을 1회 재생한다. (AudioSource.PlayOneShot)
    /// </summary>
    public void PlaySfxOneShot(AudioClip sfxClip, float volumeScale = 1f)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("[SoundManager] PlaySfxOneShot 실패: sfxClip이 null입니다.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] PlaySfxOneShot 실패: sfxSource가 없습니다.");
            return;
        }
        volumeScale = Mathf.Clamp01(masterVolumeValue * volumeScale * sfxVolumeValue);
        if (isMasterMute || isSfxMute) return;
        sfxSource.PlayOneShot(sfxClip, volumeScale);
    }
    public void PlaySfx(AudioClip sfxClip, float volumeScale = 1f)
    {
        if (sfxClip == null)
        {
            Debug.LogWarning("[SoundManager] PlaySfxOneShot 실패: sfxClip이 null입니다.");
            return;
        }
        if (sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] PlaySfxOneShot 실패: sfxSource가 없습니다.");
            return;
        }
        volumeScale = Mathf.Clamp01(masterVolumeValue * volumeScale * sfxVolumeValue);
        if (isMasterMute || isSfxMute) return;
        sfxSource.clip = sfxClip;
        sfxSource.loop = false;
        sfxSource.Play();
    }
    public void StopSfx()
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] PlaySfxOneShot 실패: sfxSource가 없습니다.");
            return;
        }
        sfxSource.Stop();
        sfxSource.clip = null;
    }
    /// <summary>
    /// 인스펙터에서 AudioSource를 연결하지 않았을 때 자동 생성한다.
    /// "필수 컴포넌트 누락"으로 막히지 않게 하는 방어 코드다.
    /// </summary>
    private void EnsureAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // 역할 분리를 위해 기본값을 명시적으로 지정한다.
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }
}

