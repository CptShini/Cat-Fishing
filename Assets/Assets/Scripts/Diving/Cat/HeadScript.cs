using UnityEngine;
using UnityEngine.Events;

public class HeadScript : MonoBehaviour
{

    private Rigidbody2D _pb;
    private HingeJoint2D _hj;
    [SerializeField] private Rigidbody2D _body;
    [SerializeField] private AimScript _aimScript;
    
    bool flipped = false;
    public readonly UnityEvent<bool> OnFlipping = new();
    private float mainLower;
    private float mainUpper;
    private float curLower;
    private float curUpper;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pb = gameObject.GetComponent<Rigidbody2D>();
        _hj = gameObject.GetComponent<HingeJoint2D>();
        gameObject.GetComponent<SwimScript>().OnSwim.AddListener(RotateHead);
        _aimScript.OnAiming.AddListener(RotateHead);
        mainLower = _hj.limits.min;
        mainUpper = _hj.limits.max;
        curLower = mainLower;
        curUpper = mainUpper;
    }

    private void SetHingeBounds(float min, float max)
    {
        JointAngleLimits2D limits = _hj.limits;
        limits.min = min;
        limits.max = max;
        _hj.limits = limits;
    }

    public void RotateHead(Vector2 direction, float speed)
    {
        float targetRotation = ((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 360f) % 360f;
        if(direction == Vector2.zero)
        {
            return;
        }

        float newRotation = Mathf.MoveTowardsAngle(
            _pb.rotation,
            targetRotation,
            Time.deltaTime * speed
        );

        _pb.rotation += Mathf.DeltaAngle(_pb.rotation, newRotation);

        if(!flipped && 90f < (_pb.rotation + 360) % 360 && (_pb.rotation + 360) % 360 <= 268f)
        {
            flipped = true;
            SetHingeBounds(-mainUpper, mainLower);
            _pb.transform.localScale = new Vector3 (0.5f, -0.5f, 1);
            _body.transform.localScale = new Vector3 (0.5f, -0.5f, 1);
            OnFlipping.Invoke(flipped);
            return;
        }
        else if(flipped && (!(90f < (_pb.rotation + 360) % 360) || !((_pb.rotation + 360) % 360 <= 272f)))
        {
            flipped = false;
            _pb.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
            _body.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
            SetHingeBounds(mainLower, mainUpper);
            OnFlipping.Invoke(flipped);
        }
        
    }

}
