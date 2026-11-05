using Unity.VisualScripting;
using UnityEngine;

public class HeadScript : MonoBehaviour
{

    private Rigidbody2D _pb;
    private HingeJoint2D _hj;
    [SerializeField] private Rigidbody2D _body;
    [SerializeField] private AimScript _aimScript;

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

        if(90f < (newRotation + 360) % 360 && (newRotation + 360) % 360 < 270f)
        {
            SetHingeBounds(-mainUpper, mainLower);
            _pb.transform.localScale = new Vector3 (0.5f, -0.5f, 1);
            _body.transform.localScale = new Vector3 (0.5f, -0.5f, 1);
            return;
        }
        SetHingeBounds(mainLower, mainUpper);
        _pb.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
        _body.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
    }

}
