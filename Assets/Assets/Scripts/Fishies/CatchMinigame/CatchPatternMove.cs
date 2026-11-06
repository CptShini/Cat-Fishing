using UnityEngine;

namespace Assets.Scripts.Fishies.CatchMinigame
{
    [System.Serializable]
    public record CatchPatternMove
    {
        [SerializeField] public FishMove move;
        [SerializeField] public float duration;
        [SerializeField] public int weight;
    }
}