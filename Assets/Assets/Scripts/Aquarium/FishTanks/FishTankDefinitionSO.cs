using UnityEngine;

namespace Assets.Scripts.Aquarium.Domain
{
    [CreateAssetMenu(fileName = "NewFishTankDefinition", menuName = "ScriptableObjects/Aquarium/Fish Tank Definition")]
    public class FishTankDefinitionSO : ScriptableObject
    {
        public string definitionID;
        public string tankName;

        [Header("Tank Settings")]
        [Min(1)]
        public int capacity = 5;
    }
}