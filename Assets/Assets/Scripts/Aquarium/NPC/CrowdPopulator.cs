using UnityEngine;
using UnityEngine.AI;

public class CrowdPopulator : MonoBehaviour
{
    public static CrowdPopulator Instance { get; private set; }
    
    [SerializeField] private GameObject NPCPrefab;
    [SerializeField] private float startPosRadius = 10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        Instance = this;
    }

    public void InitializeCrowd(int crowdCount)
    {
        for (int i = 0; i < crowdCount; i++)
        {
            Instantiate(NPCPrefab, SetStartPosition(startPosRadius), Quaternion.identity, this.transform);
        }
    }

    /*public void AddToCrowd(int count)
    {
        npcCount += count;
        for (int i = 0; i < count; i++)
        {
            Instantiate(NPCPrefab, SetStartPosition(startPosRadius), Quaternion.identity, this.transform);
        }
    }*/

    private Vector2 SetStartPosition(float maxRadius)
    {
        Vector2 direction = Random.insideUnitCircle * maxRadius;
        NavMesh.SamplePosition((Vector2)transform.position + direction, out NavMeshHit hit, maxRadius, 1);
        return hit.position;
    }
}
