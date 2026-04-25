using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelUI : MonoBehaviour
{
    [System.Serializable]
    public enum SettingPanelType
    {
        Sound = 0,
        Graphic = 1,
    }
    private enum VolumeType
    {
        Master,
        BGM,
        SFX
    }

    [Header("스크롤바")]
    [SerializeField] private Scrollbar masterScrollbar;
    [SerializeField] private Scrollbar bgmScrollbar;
    [SerializeField] private Scrollbar sfxScrollbar;

    [Header("인풋필드")]
    [SerializeField] private TMP_InputField masterInputField;
    [SerializeField] private TMP_InputField bgmInputField;
    [SerializeField] private TMP_InputField sfxInputField;

    [Header("Toggle")]
    [SerializeField] private Toggle isMasterMute;
    [SerializeField] private Toggle isBGMMute;
    [SerializeField] private Toggle isSFXMute;


    [Header("Panels")] //0 Sounds, 1 Grapic
    [SerializeField] GameObject[] settingsPanels;

    private void Start()
    {

    }
    private void OnEnable()
    {
        OnDisplay(SettingPanelType.Sound);
        if (SoundManager.Instance != null) LoadValue();
        RefreshAllUI();
    }

    public void OnDisplay(int index)
    {
        OnDisplay((SettingPanelType)index);
    }
    private void OnDisplay(SettingPanelType value)
    {
        for (int i = 0; i < settingsPanels.Length; i++)
        {
            if (i == (int)value) continue;
            settingsPanels[i].SetActive(false);
        }
        settingsPanels[(int)value].SetActive(true);
    }

    private void LoadValue()
    {
        if (SoundManager.Instance == null) return;

        masterScrollbar.value = SoundManager.Instance.MasterVolume;
        bgmScrollbar.value = SoundManager.Instance.BgmVolume;
        sfxScrollbar.value = SoundManager.Instance.SfxVolume;
    }
    #region 슬라이드
    public void OnChangeSliderMasterVolume() => UpdateFromScrollbar(VolumeType.Master);

    public void OnChangeSliderBGMVolume() => UpdateFromScrollbar(VolumeType.BGM);

    public void OnChangeSliderSFXVolume() => UpdateFromScrollbar(VolumeType.SFX);
    #endregion

    #region 인풋필드
    public void OnChangeIFMasterVolume() => UpdateFromInputField(VolumeType.Master);

    public void OnChangeIFBGMVolume() => UpdateFromInputField(VolumeType.BGM);

    public void OnChangeIFSFXVolume() => UpdateFromInputField(VolumeType.SFX);
    #endregion

    #region 토글
    public void OnChangeMasterToggle() => UpdateFromToggle(VolumeType.Master);
    public void OnChangeBGMToggle() => UpdateFromToggle(VolumeType.BGM);
    public void OnChangeSFXToggle() => UpdateFromToggle(VolumeType.SFX);

    #endregion


    private void UpdateFromToggle(VolumeType type)
    {
        switch (type)
        {
            case VolumeType.Master:
                SoundManager.Instance.MasterVolumeChange(isMasterMute.isOn);
                break;
            case VolumeType.BGM:
                SoundManager.Instance.BGMVolumeChange(isBGMMute.isOn);
                break;
            case VolumeType.SFX:
                SoundManager.Instance.SFXVolumeChange(isSFXMute.isOn);
                break;
        }
    }
    private void UpdateFromScrollbar(VolumeType type)
    {
        Scrollbar scrollbar = GetScrollbar(type);
        TMP_InputField inputField = GetInputField(type);

        if (scrollbar == null || inputField == null)
        {
            return;
        }

        float normalizedValue = Mathf.Clamp01(scrollbar.value);
        int displayValue = Mathf.RoundToInt(normalizedValue * 100f);

        inputField.SetTextWithoutNotify(displayValue.ToString("0"));
        ApplyVolume(type, normalizedValue);
    }

    private void UpdateFromInputField(VolumeType type)
    {
        Scrollbar scrollbar = GetScrollbar(type);
        TMP_InputField inputField = GetInputField(type);

        if (scrollbar == null || inputField == null)
        {
            return;
        }

        if (!float.TryParse(inputField.text, out float value))
        {
            value = Mathf.Round(scrollbar.value * 100f);
        }

        value = Mathf.Clamp(value, 0f, 100f);

        float normalizedValue = value * 0.01f;

        scrollbar.SetValueWithoutNotify(normalizedValue);
        inputField.SetTextWithoutNotify(value.ToString("0"));
        ApplyVolume(type, normalizedValue);
    }

    private void ApplyVolume(VolumeType type, float value)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        switch (type)
        {
            case VolumeType.Master:
                SoundManager.Instance.MasterVolumeChange(value);
                break;

            case VolumeType.BGM:
                SoundManager.Instance.BGMVolumeChange(value);
                break;

            case VolumeType.SFX:
                SoundManager.Instance.SFXVolumeChange(value);
                break;
        }
    }

    private Scrollbar GetScrollbar(VolumeType type)
    {
        switch (type)
        {
            case VolumeType.Master:
                return masterScrollbar;

            case VolumeType.BGM:
                return bgmScrollbar;

            case VolumeType.SFX:
                return sfxScrollbar;
        }

        return null;
    }

    private TMP_InputField GetInputField(VolumeType type)
    {
        switch (type)
        {
            case VolumeType.Master:
                return masterInputField;

            case VolumeType.BGM:
                return bgmInputField;

            case VolumeType.SFX:
                return sfxInputField;
        }

        return null;
    }

    private void RefreshAllUI()
    {
        RefreshUI(VolumeType.Master);
        RefreshUI(VolumeType.BGM);
        RefreshUI(VolumeType.SFX);
    }

    private void RefreshUI(VolumeType type)
    {
        Scrollbar scrollbar = GetScrollbar(type);
        TMP_InputField inputField = GetInputField(type);

        if (scrollbar == null || inputField == null)
        {
            return;
        }

        float normalizedValue = Mathf.Clamp01(scrollbar.value);
        int displayValue = Mathf.RoundToInt(normalizedValue * 100f);

        scrollbar.SetValueWithoutNotify(normalizedValue);
        inputField.SetTextWithoutNotify(displayValue.ToString("0"));
        ApplyVolume(type, normalizedValue);
    }
}