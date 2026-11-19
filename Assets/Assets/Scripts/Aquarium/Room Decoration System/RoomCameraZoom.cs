using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;
using System.Collections;

public class RoomCameraZoom : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera decorationCamera;

    [Header("Room")]
    [SerializeField] private Tilemap roomTilemap;

    [Header("Zoom")]
    [SerializeField] private float padding = 1.2f;
    [SerializeField] private float smoothTime = 0.35f;

    private Coroutine zoomCoroutine;
    private float zoomVelocity;
    private Vector3 positionVelocity;

    private void Awake()
    {
        playerCamera.Priority = 10;
        decorationCamera.Priority = 0;
    }

    public void EnterDecorationMode()
    {
        UpdateZoom();

        playerCamera.Priority = 0;
        decorationCamera.Priority = 10;
    }

    public void UpdateZoom()
    {
        roomTilemap.CompressBounds();

        // get the bounds of the room tilemap
        BoundsInt cellBounds = roomTilemap.cellBounds;
        Vector3 bottomLeft = roomTilemap.CellToWorld(new Vector3Int(cellBounds.xMin, cellBounds.yMin, cellBounds.zMin));
        Vector3 topRight = roomTilemap.CellToWorld(new Vector3Int(cellBounds.xMax, cellBounds.yMax, cellBounds.zMin));
        float width = Mathf.Abs(topRight.x - bottomLeft.x);
        float height = Mathf.Abs(topRight.y - bottomLeft.y);
        Vector3 center = (bottomLeft + topRight) / 2f;

        Camera mainCamera = Camera.main;

        // zoom
        float verticalSize = height / 2f;
        float horizontalSize = width / (2f * mainCamera.aspect);

        float targetSize = Mathf.Max(verticalSize, horizontalSize) * padding;

        Vector3 targetPosition = new Vector3(center.x, center.y, playerCamera.transform.position.z);

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomVelocity = 0f;
        positionVelocity = Vector3.zero;

        zoomCoroutine = StartCoroutine(SmoothZoom(targetSize, targetPosition));
    }

    private IEnumerator SmoothZoom(float targetSize, Vector3 targetPosition)
    {
        while (true)
        {
            decorationCamera.Lens.OrthographicSize = Mathf.SmoothDamp(
                decorationCamera.Lens.OrthographicSize,
                targetSize,
                ref zoomVelocity,
                smoothTime
            );

            decorationCamera.transform.position = Vector3.SmoothDamp(
                decorationCamera.transform.position,
                targetPosition,
                ref positionVelocity,
                smoothTime
            );

            float sizeDifference = Mathf.Abs(
                decorationCamera.Lens.OrthographicSize - targetSize
            );

            float positionDifference = Vector3.Distance(
                decorationCamera.transform.position,
                targetPosition
            );

            if (sizeDifference < 0.01f && positionDifference < 0.01f)
            {
                decorationCamera.Lens.OrthographicSize = targetSize;
                decorationCamera.transform.position = targetPosition;
                yield break;
            }

            yield return null;
        }
    }

    public void ExitDecorationMode()
    {
        decorationCamera.Priority = 0;
        playerCamera.Priority = 10;
    }
}