using Assets.Scripts.PlayerState;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{

[SerializeField] private Camera mainCamera;
[SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DivingController.Instance.OnStateChanged.AddListener(AimEnabler);
        animator.SetFloat("speed", 1 / 100);
    }

    void AimEnabler(PlayerState state)
    {
        if(state == PlayerState.STEALTH)
        {
            gameObject.SetActive(true);
            animator.Play("Paw");
            animator.SetFloat("speed", 1.0f / DivingPlayerStats.Instance.chargeUpSpeed);
            animator.SetBool("exit", false);
        }
        else
        {
            animator.SetBool("exit", true);
            gameObject.SetActive(false);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 pounceDirection = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(pounceDirection.y, pounceDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
