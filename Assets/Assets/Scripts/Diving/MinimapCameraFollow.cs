using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    private void LateUpdate()
    {
        if (!target)
            return;
        
        transform.position = new(target.position.x, target.position.y, -10f);
    }
}