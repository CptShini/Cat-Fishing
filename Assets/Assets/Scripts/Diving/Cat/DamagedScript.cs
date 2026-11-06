using Assets.Scripts.Diving.Bars;
using UnityEngine;
using UnityEngine.Events;

public class DamagedScript : MonoBehaviour, IDamageable
{
    [SerializeField] public float invincibleDuration;
    [SerializeField] private StatBar oxygenBar;

    private float curInvincibleDuration = 0;

    private Rigidbody2D _rb;

    public readonly UnityEvent OnDamaged = new();


    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Damaged(float amount, Vector2 knockback)
    {
        OnDamaged.Invoke();
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
