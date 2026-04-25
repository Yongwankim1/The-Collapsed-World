using TMPro;
using UnityEngine;

public class InGameTimerViewUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] Transform hourHandTransform;
    [SerializeField] Transform minuteHandTransform;

    private void Update()
    {
        var time = TimeLightingController.Instance;
        if (time == null) return;

        float cycle = time.CycleDuration;
        float current = time.TimeLightingValue;

        float ratio = current / cycle;

        int hour = Mathf.FloorToInt(ratio * 24f);
        int minute = Mathf.FloorToInt((ratio * 24f - hour) * 60f);

        bool isDay = ratio >= 0.25f && ratio < 0.75f;

        dayText.text = isDay ? "Day" : "Night";
        timeText.text = $"{hour:00} : {minute:00}";

        float minuteAngle = -(minute / 60f) * 360f;
        float hourAngle = -((hour % 12) / 12f) * 360f - (minute / 60f) * 30f;

        minuteHandTransform.localRotation = Quaternion.Euler(0, 0, minuteAngle);
        hourHandTransform.localRotation = Quaternion.Euler(0, 0, hourAngle);
    }
}