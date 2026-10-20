using UnityEngine;

public class PlayerMovementAquarium : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;

    void FixedUpdate() // 50x per frame (not once)
    {
        float horizontal = Input.GetAxis("Horizontal"); // from Input Manager
        float vertical = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
    }
}
