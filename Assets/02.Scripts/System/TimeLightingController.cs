using UnityEngine;

[DefaultExecutionOrder(-994)]
public class TimeLightingController : MonoBehaviour
{
    public static TimeLightingController Instance;
    [SerializeField] private float playDay;
    [SerializeField] private float timeLightingValue;
    public float PlayDay => playDay;
    public float TimeLightingValue => timeLightingValue;

    [SerializeField] float cycleDuration;

    public float CycleDuration => cycleDuration;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (cycleDuration <= 0f) return;

        float delta = Time.deltaTime;

        playDay += delta / cycleDuration;

        timeLightingValue += delta;

        if (timeLightingValue >= cycleDuration)
        {
            timeLightingValue -= cycleDuration;
        }
    }

    public TimeData GetData()
    {
        TimeData data = new TimeData();
        data.time = TimeLightingValue;
        data.playDay = PlayDay;

        return data;
    }

    public void LoadData(TimeData data)
    {
        timeLightingValue = data.time;
        playDay = data.playDay;
    }
}
