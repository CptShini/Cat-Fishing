using Assets.Scripts.Diving.Bars;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class DamagedScript : MonoBehaviour, IDamageable
{
    [SerializeField] public float invincibleDuration;
    [SerializeField] private StatBar oxygenBar;

    private float curInvincibleDuration;

    private Rigidbody2D _rb;

    public readonly UnityEvent OnDamaged = new();
    
    private void Start() => _rb = GetComponent<Rigidbody2D>();

    public void Damaged(float amount, Vector2 knockback)
    {
        OnDamaged.Invoke();
        if (curInvincibleDuration < invincibleDuration)
            return;

        oxygenBar.CurrentValue -= amount;
        curInvincibleDuration = 0;

        if (knockback == Vector2.zero)
            return;

        _rb.linearVelocity = new(knockback.x, knockback.y);
    }

    private void Update()
    {
        curInvincibleDuration += Time.deltaTime;
    }
}
