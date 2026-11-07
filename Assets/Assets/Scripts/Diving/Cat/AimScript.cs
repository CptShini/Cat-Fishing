using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AimPawScript aimPaw;
    [SerializeField] private HeadScript headScript;

    private Vector3 originalScale;

    public readonly UnityEvent<Vector2, float> OnAiming = new();

    void Awake()
    {
        originalScale = gameObject.transform.localScale;
    }

    private void Start()
    {
        DivingController.Instance.OnStateChanged.AddListener(AimEnabler);
        //headScript.OnFlipping.AddListener(CounterFlip);
        gameObject.SetActive(false);
        aimPaw.AimPawEnabler(false);
        
    }
    private void CounterFlip(bool flipped)
    {
        if (flipped)
        {
            //gameObject.transform.localScale = originalScale;
            gameObject.transform.localScale = new Vector3 (originalScale.x, -originalScale.y, originalScale.z);
        }
        else
        {
            gameObject.transform.localScale = originalScale;
            //gameObject.transform.localScale = new Vector3 (originalScale.x, -originalScale.y, originalScale.z);

        }
    }

    private void AimEnabler(PlayerState state)
    {
        if (state == PlayerState.STEALTH)
        {
            gameObject.SetActive(true);
            aimPaw.AimPawEnabler(true);
        }
        else
        {
            gameObject.SetActive(false);
            aimPaw.AimPawEnabler(false);
        } 
    }
    
    private void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 pounceDirection = mousePos - (Vector2)transform.position;
        
        float angle = Mathf.Atan2(pounceDirection.y, pounceDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, angle));
        OnAiming.Invoke(pounceDirection, 1000);
    }
}