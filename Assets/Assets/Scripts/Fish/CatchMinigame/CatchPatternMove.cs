using UnityEngine;

namespace Assets.Scripts.Fish.CatchMinigame
{
    [System.Serializable]
    public record CatchPatternMove
    {
        [SerializeField] public FishMove move;
        [SerializeField] public float duration;
        [SerializeField] public int weight;
    }
}