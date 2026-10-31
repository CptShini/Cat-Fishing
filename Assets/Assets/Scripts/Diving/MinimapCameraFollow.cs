using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = new Vector3(target.position.x, target.position.y, -10f);
    }
}