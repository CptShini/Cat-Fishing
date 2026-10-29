using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Fish.Perception
{
    public class FishPerceptionHandler : FishPerception
    {
        public readonly UnityEvent<Vector2, bool> onDetectPlayer = new();

        private protected override string TriggerTag() => "Player Presence";
        
        private protected override bool IsOffset() => false;

        private protected override void Detect(Collider2D other) =>
            onDetectPlayer.Invoke(other.transform.position, false);
    }
}