using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class CrowdPopulator : MonoBehaviour
{

    [SerializeField] private GameObject NPCPrefab;

    [SerializeField] private int npcCount = 3;

    private NavMeshSurface surface;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surface = NavigationManager.Instance.surface;

        for (int i = 0; i < npcCount; i++)
        {
            Instantiate(NPCPrefab, SetStartPosition(10f), Quaternion.identity, this.transform);
        }
    }
    public Vector2 SetStartPosition(float maxRadius)
    {
        Vector2 direction = Random.insideUnitCircle * maxRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(direction, out hit, maxRadius, 1);
		
        Vector3 destination = hit.position;

        return destination;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
