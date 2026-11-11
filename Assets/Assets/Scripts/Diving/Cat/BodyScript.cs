using Assets.Scripts.Diving.Cat;
using Assets.Scripts.PlayerState;
using UnityEngine;

public class BodyScript : MonoBehaviour
{

    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _head;

    public bool flipped = false;
    private bool stealth = false;

    [SerializeField] private float dragSpeedMod = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SwimScript>().OnSwim.AddListener(Drag);
        gameObject.GetComponent<SwimScript>().OnStealth.AddListener(Drag);
        gameObject.GetComponent<PounceScript>().OnPouncing.AddListener(Drag);
        gameObject.GetComponent<HeadScript>().OnRotating.AddListener(Clamp);
        gameObject.GetComponent<HeadScript>().OnFlipping.AddListener(Flip);
        DivingController.Instance.OnStateChanged.AddListener(SetIsStealth);
    }

    void SetIsStealth(PlayerState state)
    {
        stealth = state == PlayerState.STEALTH;
    }

    void Flip(bool flipped)
    {
        _body.GetComponent<SpriteRenderer>().flipY = flipped;
        this.flipped = flipped;
    }

    void Drag(Vector2 movement)
    {
        Drag(movement, 1);
    }

    void Clamp(float clampAngle)
    {
        if (!stealth)
        {
            return;
        }
        float curAngle = _body.transform.rotation.eulerAngles.z;

        if(clampAngle < 0f)
        {
            clampAngle += 360;
        }
        Mathf.DeltaAngle(curAngle, clampAngle);
        if(curAngle < 0f)
        {
            curAngle += 360;
        }
        float clampedRotation = Mathf.Clamp(curAngle, clampAngle - 30, clampAngle + 30);

        if(clampedRotation == curAngle)
        {
            return;
        }

        float headDragMod = Mathf.Abs(Mathf.DeltaAngle(curAngle, _head.transform.rotation.eulerAngles.z)) / 360;
        float newAngle = Mathf.MoveTowardsAngle(
            curAngle,
            clampedRotation,
            Time.deltaTime * 2000 * headDragMod
        );

        _body.transform.rotation = Quaternion.Euler(0,0, newAngle);  
        
        CheckFlip();
    }

    void CheckFlip()
    {
        float curRotation = _body.transform.rotation.eulerAngles.z;
        if(!flipped && 90f < (curRotation + 360) % 360 && (curRotation + 360) % 360 <= 268f)
        {
            flipped = true;
            _body.GetComponent<SpriteRenderer>().flipY = true;
            return;
        }
        else if(flipped && (!(90f < (curRotation + 360) % 360) || !((curRotation + 360) % 360 <= 272f)))
        {
            flipped = false;
            _body.GetComponent<SpriteRenderer>().flipY = false;
        }
    }

    void Drag(Vector2 movement, float speed)
    {
        float curRotation = _body.transform.rotation.eulerAngles.z;

        float headDragMod = Mathf.Abs(Mathf.DeltaAngle(curRotation, _head.transform.rotation.eulerAngles.z)) / 360;
        
        float targetRotation = ((Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg) + 360f) % 360f;
        
        float newRotation = Mathf.MoveTowardsAngle(
            curRotation,
            targetRotation,
            Time.deltaTime * movement.magnitude * dragSpeedMod * headDragMod
        );  

        _body.transform.rotation = Quaternion.Euler(0,0, newRotation);
        //CheckFlip();
       
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
