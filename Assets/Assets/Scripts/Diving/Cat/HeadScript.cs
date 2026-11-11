using UnityEngine;
using UnityEngine.Events;

public class HeadScript : MonoBehaviour
{

    //private Rigidbody2D _pb;
    //private HingeJoint2D _hj;
    //[SerializeField] private Rigidbody2D _body;
    [SerializeField] private AimScript _aimScript;
    [SerializeField] private GameObject _head;
    bool flipped = false;
    public readonly UnityEvent<bool> OnFlipping = new();

    public readonly UnityEvent<float> OnRotating = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SwimScript>().OnSwim.AddListener(RotateHead);
        _aimScript.OnAiming.AddListener(RotateHead);
    }

    public void RotateHead(Vector2 direction, float speed)
    {
        float curRotation = _head.transform.rotation.eulerAngles.z;
        float targetRotation = ((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 360f) % 360f;
        if(direction == Vector2.zero)
        {
            return;
        }

        float newRotation = Mathf.MoveTowardsAngle(
            curRotation,
            targetRotation,
            Time.deltaTime * speed
        );        
        
        _head.transform.rotation = Quaternion.Euler(0,0, newRotation);
        OnRotating.Invoke(_head.transform.rotation.eulerAngles.z);

        if(!flipped && 90f < (curRotation + 360) % 360 && (curRotation + 360) % 360 <= 268f)
        {
            flipped = true;
           
            _head.GetComponent<SpriteRenderer>().flipY = true;
            OnFlipping.Invoke(flipped);
            return;
        }
        else if(flipped && (!(90f < (curRotation + 360) % 360) || !((curRotation + 360) % 360 <= 272f)))
        {
            flipped = false;
            _head.GetComponent<SpriteRenderer>().flipY = false;
            OnFlipping.Invoke(flipped);
        }
    }
}
