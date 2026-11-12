using UnityEngine;
using UnityEngine.Events;

public class HeadScript : MonoBehaviour
{
    [SerializeField] private AimScript aimScript;
    [SerializeField] private SpriteRenderer head;
    
    private bool flipped;
    
    public readonly UnityEvent<bool> OnFlipping = new();
    public readonly UnityEvent<float> OnRotating = new();
    
    private void Start() => aimScript.OnAiming.AddListener(RotateHead);

    public void RotateHead(Vector2 direction, float speed)
    {
        float curRotation = head.transform.rotation.eulerAngles.z;
        float targetRotation = ((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 360f) % 360f;
        if (direction == Vector2.zero)
        {
            return;
        }

        float newRotation = Mathf.MoveTowardsAngle(
            curRotation,
            targetRotation,
            Time.deltaTime * speed
        );        
        
        head.transform.rotation = Quaternion.Euler(0,0, newRotation);
        OnRotating.Invoke(head.transform.rotation.eulerAngles.z);

        float wrappedRotation = (curRotation + 360) % 360;
        switch (flipped)
        {
            case false when wrappedRotation is > 90f and <= 268f:
                flipped = true;
                head.flipY = true;
                OnFlipping.Invoke(flipped);
                return;
            case true when wrappedRotation is <= 90f or > 272f:
                flipped = false;
                head.flipY = false;
                OnFlipping.Invoke(flipped);
                return;
        }
    }
}
