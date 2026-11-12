using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class CrowdPopulator : MonoBehaviour
{
    public static CrowdPopulator Instance {get; private set;}
    [SerializeField] private GameObject NPCPrefab;

    [SerializeField] private int npcCount = 0;
    [SerializeField] private float startPosRadius = 10f;

    private NavMeshSurface surface;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surface = NavigationManager.Instance.surface;
        InitializeCrowd();
    }

    private void InitializeCrowd()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Instantiate(NPCPrefab, SetStartPosition(startPosRadius), Quaternion.identity, this.transform);
        }
    }

    public void AddToCrowd(int count)
    {
        npcCount += count;
        for (int i = 0; i < count; i++)
        {
            Instantiate(NPCPrefab, SetStartPosition(startPosRadius), Quaternion.identity, this.transform);
        }
    }

    public Vector2 SetStartPosition(float maxRadius)
    {
        Vector2 direction = Random.insideUnitCircle * maxRadius;
        NavMesh.SamplePosition((Vector2)transform.position + direction, out NavMeshHit hit, maxRadius, 1);
        return hit.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
