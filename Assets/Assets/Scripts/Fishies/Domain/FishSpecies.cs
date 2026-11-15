using Assets.Scripts.Fishies.CatchMinigame;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Fishies.Domain
{
    [CreateAssetMenu(fileName = "New Species", menuName = "ScriptableObjects/Fish/Species")]
    public class FishSpecies : ScriptableObject
    {
        // --- Information ---
        
        [field: SerializeField, Header("Information")]
        public int Id { get; private set; }

        [field: SerializeField]
        public string Name { get; private set; } = "Fish";

        [field: SerializeField]
        public string LatinName { get; private set; } = "Fishus Maximus";
        
        [field: SerializeField, Multiline]
        public string Description { get; private set; }
        
        [field: SerializeField, Range(1, 10)]
        public int Rarity { get; private set; }

        [field: SerializeField, NavMeshAreaMask]
        public int Biome { get; private set; }
        
        // --- Visuals ---
        
        [field: SerializeField, Header("Visuals")]
        public Sprite Icon { get; private set; }
        
        // --- Stats ---
        
        [field: SerializeField, Header("Stats")]
        public FishCatchPattern CatchPattern { get; private set; }
        
        [field: SerializeField]
        public Range Weight { get; private set; }
        
        [field: SerializeField, Range(1, 10)]
        public int Strength { get; private set; }
        
        [field: SerializeField, Range(1, 10)]
        public int Speed { get; private set; }
        
        [field: SerializeField, Range(0, 10)]
        public int Reaction { get; private set; }
        
        [field: SerializeField, Range(0, 10)]
        public int Perception { get; private set; }
        
        [field: SerializeField, Range(0, 10)]
        public int Sight { get; private set; }
    }
}