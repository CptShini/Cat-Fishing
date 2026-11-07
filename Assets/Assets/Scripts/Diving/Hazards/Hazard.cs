using UnityEngine;

public class Hazard : MonoBehaviour
{

    [SerializeField] public int damage;
    [SerializeField] public int power;

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damaged(damage, -other.GetContact(0).normal * power);
            AudioManager.Instance.PlayHurt();
        }
    }
}
