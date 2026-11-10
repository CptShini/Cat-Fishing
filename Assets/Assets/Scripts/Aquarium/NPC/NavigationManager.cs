using System;
using NavMeshPlus.Components;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    public static NavigationManager Instance { get; private set; }
    [SerializeField] public NavMeshSurface surface;

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
        PlacementController.Instance.OnPlaced.AddListener(RebuildNavigation);
        PlacementController.Instance.OnPickedUp.AddListener(RebuildNavigation);
        RebuildNavigation();
    }


    private void RebuildNavigation()
    {
        surface.BuildNavMesh();
    }
}
