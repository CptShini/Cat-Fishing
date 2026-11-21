using UnityEngine;

namespace Assets.Scripts.Aquarium.Domain
{
    [CreateAssetMenu(fileName = "New Fish Tank Definition", menuName = "ScriptableObjects/Aquarium/Fish Tank Definition")]
    public class FishTankDefinitionSO : ScriptableObject
    {
        public string definitionID;
        public string tankName;

        [Header("Upgrade Settings")]
        [Tooltip("Capacity for each level: Level 1, Level 2, Level 3.")]
        [Min(1)] public int[] capacities = { 4, 7, 10 };

        [Tooltip("Price to upgrade from Level 1 to 2, then Level 2 to 3.")]
        [Min(0)] public int[] upgradePrices = { 100, 250 };


        public int MaxLevel => capacities.Length;

        public int GetCapacity(int level)
        {
            level = Mathf.Clamp(level, 1, capacities.Length);
            return capacities[level - 1];
        }

        public int GetUpgradePrice(int currentLevel)
        {
            if (currentLevel >= MaxLevel)
                return -1;

            if (currentLevel - 1 < 0 || currentLevel - 1 >= upgradePrices.Length)
                return -1;

            return upgradePrices[currentLevel - 1];
        }
    }
}