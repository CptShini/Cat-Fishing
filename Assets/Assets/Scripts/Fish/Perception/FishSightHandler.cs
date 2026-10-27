using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Perception
{
    public class FishSightHandler : FishPerception
    {
        public readonly UnityEvent<Vector2> OnSeePlayer = new();

        private protected override string TriggerTag() => "Player";

        private protected override bool IsOffset() => true;

        private protected override void Detect(Collider2D other) => 
            OnSeePlayer.Invoke(other.transform.position);
    }
}