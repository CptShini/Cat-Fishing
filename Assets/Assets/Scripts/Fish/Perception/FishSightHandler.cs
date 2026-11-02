using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Perception
{
    public class FishSightHandler : FishPerception
    {
        public readonly UnityEvent<Vector2> OnSeePlayer = new();

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
    }
}