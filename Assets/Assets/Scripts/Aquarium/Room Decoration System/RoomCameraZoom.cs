using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;

public class RoomCameraZoom : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera decorationCamera;

    [Header("Room")]
    [SerializeField] private Tilemap roomTilemap;

    [Header("Zoom")]
    [SerializeField] private float padding = 1.2f;

    private void Awake()
    {
        playerCamera.Priority = 10;
        decorationCamera.Priority = 0;
    }

    public void EnterDecorationMode()
    {
        Bounds bounds = roomTilemap.localBounds;

        Vector3 center = roomTilemap.transform.TransformPoint(bounds.center);

        Vector3 min = roomTilemap.transform.TransformPoint(bounds.min);
        Vector3 max = roomTilemap.transform.TransformPoint(bounds.max);

        float width = Mathf.Abs(max.x - min.x);
        float height = Mathf.Abs(max.y - min.y);

        Camera mainCamera = Camera.main;

        float verticalSize = height / 2f;
        float horizontalSize = width / (2f * mainCamera.aspect);

        float targetSize = Mathf.Max(verticalSize, horizontalSize) * padding;

        decorationCamera.Lens.OrthographicSize = targetSize;

        decorationCamera.transform.position = new Vector3(center.x, center.y, playerCamera.transform.position.z);

        playerCamera.Priority = 0;
        decorationCamera.Priority = 10;
    }

    public void ExitDecorationMode()
    {
        decorationCamera.Priority = 0;
        playerCamera.Priority = 10;
    }
}