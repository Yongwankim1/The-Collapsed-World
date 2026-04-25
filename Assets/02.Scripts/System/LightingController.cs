using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingController : MonoBehaviour
{
    [SerializeField] Light2D globalLighting;
    [SerializeField] float minIntensity = 0.05f;
    [SerializeField] float maxIntensity = 0.6f;

    private void Update()
    {
        float t = TimeLightingController.Instance.TimeLightingValue
                  / TimeLightingController.Instance.CycleDuration;

        float curve = Mathf.Sin(t * Mathf.PI);

        curve = Mathf.Clamp01(curve);

        curve = Mathf.SmoothStep(0f, 1f, curve);

        globalLighting.intensity = Mathf.Lerp(minIntensity, maxIntensity, curve);
    }
}