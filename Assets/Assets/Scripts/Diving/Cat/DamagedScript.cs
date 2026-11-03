using Assets.Scripts.Diving;
using UnityEngine;

public class DamagedScript : MonoBehaviour, IDamageable
{
    [SerializeField] public float invincibleDuration;
    [SerializeField] private StatBar oxygenBar;

    private float curInvincibleDuration;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Damaged(float amount, Vector2 normal)
    {
        if(curInvincibleDuration < invincibleDuration)
        {
            return;
        }

        oxygenBar.CurrentValue -= amount;
        curInvincibleDuration = 0;
        Debug.Log($"NORMAL: {normal.x}, {normal.y}");
        _rb.linearVelocity = new Vector3(-normal.x * amount, -normal.y * amount, 0);

    }

    void Update()
    {
        curInvincibleDuration += Time.deltaTime;
    }
}
