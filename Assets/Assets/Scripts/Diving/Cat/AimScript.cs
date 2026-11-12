using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
    [SerializeField] private AimPawScript aimPaw;
    
    private Camera _mainCamera;

    public readonly UnityEvent<Vector2, float> OnAiming = new();

    private void Start()
    {
        DivingController.Instance.OnStateChanged.AddListener(AimEnabler);
        gameObject.SetActive(false);
        aimPaw.AimPawEnabler(false);
        _mainCamera = Camera.main;
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
        Vector2 mousePosScreen = Mouse.current.position.ReadValue();
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(mousePosScreen);
        Vector2 pounceDirection = mousePos - (Vector2)transform.position;
        
        float angle = Mathf.Atan2(pounceDirection.y, pounceDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, angle));
        OnAiming.Invoke(pounceDirection, 1000);
    }
}