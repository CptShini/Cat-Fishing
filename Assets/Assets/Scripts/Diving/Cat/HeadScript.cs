using UnityEngine;

public class HeadScript : MonoBehaviour
{

    private Rigidbody2D _pb;
    private HingeJoint2D _hj;

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

        Debug.Log($"{_hj.limits.max}, {_hj.limits.min}");

    }

    public void RotateHead(Vector2 direction)
    {
        if(direction == Vector2.zero)
        {
            return;
        }
        _pb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if(_pb.rotation < -90)
        {
            _pb.rotation = _pb.rotation + 360;
        }

        if(direction.x < 0)
        {
            SetHingeBounds(-mainUpper, mainLower);
            _pb.transform.localScale = new Vector3 (0.5f, -0.5f, 1);
            return;
        }
        
        SetHingeBounds(mainLower, mainUpper);
        _pb.transform.localScale = new Vector3 (0.5f, 0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
