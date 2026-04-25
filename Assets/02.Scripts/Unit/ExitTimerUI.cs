using System.Collections;
using TMPro;
using UnityEngine;

public class ExitTimerUI : MonoBehaviour
{
    [SerializeField] GameObject timerPanel;
    [SerializeField] TextMeshProUGUI timerText;

    Coroutine exitZoneActive;
    private void Awake()
    {
        if(timerPanel == null) timerPanel = transform.GetChild(0).gameObject;
        if(timerText == null) timerText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        if(timerPanel) timerPanel.SetActive(false);
    }
    IEnumerator Exit(float timer)
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer < 0f) timer = 0f;

            int sec = Mathf.FloorToInt(timer);
            int msec = Mathf.FloorToInt((timer - sec) * 100f);

            timerText.text = $"{sec:00}:{msec:00}";

            yield return null;
        }

        timerText.text = "00:00";

        GameSceneManager.Instance.LoadSceneByName("ResultScene");
        exitZoneActive = null;
    }
    public void OnStartExitTimer(float timer)
    {
        timerPanel.SetActive(true);
        if(exitZoneActive == null) exitZoneActive = StartCoroutine(Exit(timer));
    }

    public void OnStopExitTimer()
    {
        if (exitZoneActive == null) return;
        timerPanel.SetActive(false);

        StopCoroutine(exitZoneActive);
        exitZoneActive = null;
    }
}
