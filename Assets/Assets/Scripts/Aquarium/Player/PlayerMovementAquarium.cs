using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementAquarium : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            AudioManager.Instance.StopPlaying();
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
        bool isWalking = rb.linearVelocity.magnitude > 0;

        animator.SetBool("isWalking", isWalking);

        if (isWalking)
            AudioManager.Instance.PlayWalking();
        else
            AudioManager.Instance.StopPlaying();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
}