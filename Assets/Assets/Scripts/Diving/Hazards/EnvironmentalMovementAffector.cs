

using Assets.Scripts.Audio;
using Assets.Scripts.Audio.SFX;
using UnityEngine;

namespace Assets.Scripts.Diving.Hazards
{
    public class EnvironmentalMovementAffector : MonoBehaviour
    {
        protected Vector2 _direction;
        [SerializeField] public int power;

        public Vector2 Velocity => _direction.normalized * power;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out IEnvironmentAffectedMovement _)) return;
            foreach (IEnvironmentAffectedMovement movement in other.gameObject.GetComponents<IEnvironmentAffectedMovement>())
            {
                movement.AddAffector(this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IEnvironmentAffectedMovement _)) return;
            foreach (IEnvironmentAffectedMovement movement in collision.gameObject.GetComponents<IEnvironmentAffectedMovement>())
            {
                movement.RemoveAffector(this);
            }
        }

    }
}
