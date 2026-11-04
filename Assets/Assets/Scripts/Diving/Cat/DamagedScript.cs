using Assets.Scripts.Diving;
using Unity.VisualScripting;
using UnityEngine;

public class DamagedScript : MonoBehaviour, IDamageable
{
    [SerializeField] public float invincibleDuration;
    [SerializeField] private StatBar oxygenBar;

    private float curInvincibleDuration = 0;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Damaged(float amount, Vector2 knockback)
    {
        if(curInvincibleDuration < invincibleDuration)
        {
            return;
        }

        oxygenBar.CurrentValue -= amount;
        curInvincibleDuration = 0;

        if(knockback == Vector2.zero)
        {
            return;
        }

        _rb.linearVelocity = new Vector2(knockback.x, knockback.y);
    }

    void Update()
    {
        curInvincibleDuration += Time.deltaTime;
    }
}
