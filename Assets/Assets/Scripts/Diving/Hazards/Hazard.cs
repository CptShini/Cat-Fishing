using Assets.Scripts.Audio;
using UnityEngine;

namespace Assets.Scripts.Diving.Hazards
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField] public int damage;
        [SerializeField] public int power;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable damageable)) return;
        
            damageable.Damaged(damage, -other.GetContact(0).normal * power);
            AudioManager.Instance.Play(AudioId.Hurt);
        }
    }
}
