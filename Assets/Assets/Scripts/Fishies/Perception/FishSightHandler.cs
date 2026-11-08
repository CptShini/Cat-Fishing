using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fishies.Perception
{
    public class FishSightHandler : FishPerception
    {
        public readonly UnityEvent<Vector2> OnSeePlayer = new();

        [SerializeField] private LayerMask playerLayer;

        private protected override string TriggerTag() => "Player";

        private protected override bool IsOffset() => true;
        
        private protected override float ComputeReactionTime(int reactionStat)
        {
            int x = reactionStat;
            float reactionTimeMs = (42 * x * x - 910 * x + 5000) / 2f;
            return reactionTimeMs / 1000f;
        }

        private protected override float ComputeRadius(int sizeStat)
        {
            int x = sizeStat;
            float radius = (x < 2
                ? Mathf.Sqrt(x / 2f)
                : Mathf.Pow(1.2f, x - 2)) * 2f;
            return radius;
        }

        private protected override void Detect(Collider2D other) => 
            OnSeePlayer.Invoke(other.transform.position);

        private protected override bool PerceptionCheck(Collider2D other)
        {
            Vector2 direction = other.transform.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, playerLayer);
            return hit.transform.CompareTag(_triggerTag);
        }
    }
}