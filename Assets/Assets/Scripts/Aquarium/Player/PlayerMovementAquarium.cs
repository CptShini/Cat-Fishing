using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovementAquarium : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private CapsuleCollider2D playerCollider;
    public Tilemap groundTilemap;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        if(PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        Vector2 movement = moveInput.normalized * moveSpeed;
        Vector2 targetPosition = rb.position + movement * Time.fixedDeltaTime;

        if (CanMoveTo(targetPosition))
        {
            rb.MovePosition(targetPosition);
        }

        animator.SetBool("isWalking", moveInput != Vector2.zero);
    }

    bool CanMoveTo(Vector2 position)
    {
        Bounds bounds = playerCollider.bounds;

        Vector2 offset = position - rb.position;

        Vector2 min = (Vector2)bounds.min + offset;
        Vector2 max = (Vector2)bounds.max + offset;

        Vector3Int minCell = groundTilemap.WorldToCell(min);
        Vector3Int maxCell = groundTilemap.WorldToCell(max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                if (!groundTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
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
