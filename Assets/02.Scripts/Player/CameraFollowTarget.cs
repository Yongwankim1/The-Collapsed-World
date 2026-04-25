using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float followDistance;
    [SerializeField] float projectionSize;

    [Header("Map Bounds")]
    [SerializeField] Vector2 minBounds;
    [SerializeField] Vector2 maxBounds;

    public bool IsFollow { get; private set; } = true;
    public void SetIsFollow(bool value) => IsFollow = value;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        FollowCamera();
    }

    void FollowCamera()
    {
        if (!IsFollow) return;

        mainCamera.orthographicSize = projectionSize;

        Vector3 position = transform.position;

        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float clampedX = Mathf.Clamp(position.x, minBounds.x + cameraWidth, maxBounds.x - cameraWidth);
        float clampedY = Mathf.Clamp(position.y, minBounds.y + cameraHeight, maxBounds.y - cameraHeight);

        position.x = clampedX;
        position.y = clampedY;
        position.z = -1 * followDistance;

        mainCamera.transform.position = position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) * 0.5f,
            (minBounds.y + maxBounds.y) * 0.5f,
            0f
        );

        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}