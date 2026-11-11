using NavMeshPlus.Components;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    public static NavigationManager Instance { get; private set; }
    [SerializeField] public NavMeshSurface surface;

    bool buildReady = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        PlacementController.Instance.OnPlaced.AddListener(ReadyBuild);
        PlacementController.Instance.OnPickedUp.AddListener(ReadyBuild);
        Invoke(nameof(RebuildNavigation), 0.1f);
    }

    private void ReadyBuild()
    {
        buildReady = true;
    }

    private void RebuildNavigation()
    {
        surface.BuildNavMesh();
        buildReady = false;
    }

    void LateUpdate()
    {
        if (!buildReady)
        {
            return;
        }
        RebuildNavigation();
    }
}
