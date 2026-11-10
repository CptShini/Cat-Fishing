using System;
using NavMeshPlus.Components;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{

    public static NavigationManager Instance { get; private set; }
    [SerializeField] private NavMeshSurface surface;

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
        RebuildNavigation();
    }


    private void RebuildNavigation()
    {
        surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
