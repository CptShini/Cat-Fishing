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

        decorationCamera.Lens.OrthographicSize = targetSize;

        // center deco camera on the room
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