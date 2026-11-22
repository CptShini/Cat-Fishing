using Assets.Scripts.Helpers;
using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.Events;

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
        if (!transform.TryGetMouseWorldPosition(_mainCamera, out Vector3 mouseWorldPos))
            return;
        
        Vector2 pounceDirection = (Vector2)mouseWorldPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(pounceDirection.y, pounceDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        OnAiming.Invoke(pounceDirection, 1000);
    }
}