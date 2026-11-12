using Assets.Scripts.Diving.Cat;
using Assets.Scripts.PlayerState;
using UnityEngine;

[RequireComponent(typeof(SwimScript))]
public class BodyScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private GameObject _head;

    private bool _flipped;
    private bool _stealth;

    [SerializeField] private float dragSpeedMod = 100f;
    
    private void Start()
    {
        HeadScript headScript = GetComponent<HeadScript>();
        headScript.OnRotating.AddListener(Clamp);
        headScript.OnFlipping.AddListener(Flip);
        
        SwimScript swimScript = GetComponent<SwimScript>();
        swimScript.OnSwim.AddListener(Drag);
        swimScript.OnSwim.AddListener(headScript.RotateHead);
        swimScript.OnStealth.AddListener(Drag);
        
        PounceScript pounceScript = GetComponent<PounceScript>();
        pounceScript.OnPouncing.AddListener(Drag);
        
        
        
        DivingController.Instance.OnStateChanged.AddListener(SetIsStealth);
    }

    private void SetIsStealth(PlayerState state)
    {
        _stealth = state == PlayerState.STEALTH;
    }

    private void Flip(bool flipped)
    {
        _body.flipY = flipped;
        _flipped = flipped;
    }

    private void Drag(Vector2 movement)
    {
        Drag(movement, 1);
    }

    private void Clamp(float clampAngle)
    {
        if (!_stealth)
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

    private void CheckFlip()
    {
        float curRotation = _body.transform.rotation.eulerAngles.z;
        float wrappedRotation = (curRotation + 360) % 360;
        switch (_flipped)
        {
            case false when wrappedRotation is > 90f and <= 268f:
                _flipped = true;
                _body.flipY = true;
                return;
            case true when wrappedRotation is <= 90f or > 272f:
                _flipped = false;
                _body.flipY = false;
                break;
        }
    }

    private void Drag(Vector2 movement, float speed)
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
}