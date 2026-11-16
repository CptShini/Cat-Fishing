using Assets.Scripts.Audio;
using Assets.Scripts.Audio.SFX;
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
            SFXManager.Instance.Play(SFXId.Hurt);
        }
    }
}
