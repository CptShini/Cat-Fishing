using UnityEngine;

public class Hazard : MonoBehaviour
{

    [SerializeField] public int damage;

    private Collider2D _collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damaged();
        }
    }
}
