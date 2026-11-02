using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Perception
{
    public class FishPerceptionHandler : FishPerception
    {
        public readonly UnityEvent<Vector2> OnDetectPlayer = new();

        private protected override string TriggerTag() => "Player Presence";
        
        private protected override bool IsOffset() => false;

        private protected override float RadiusFromSizeStat(int sizeStat)
        {
            int x = sizeStat;
            return x < 2
                ? Mathf.Sqrt(x / 2f)
                : Mathf.Pow(1.2f, x - 2);
        }

        private protected override void Detect(Collider2D other) =>
            OnDetectPlayer.Invoke(other.transform.position);
    }
}