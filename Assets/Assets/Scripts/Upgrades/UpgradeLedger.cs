using System;
using UnityEngine;

namespace Assets.Scripts.Upgrades
{
    [CreateAssetMenu(fileName = "New Upgrade Ledger", menuName = "ScriptableObjects/Shop/Upgrade Ledger")]
    public class UpgradeLedger : ScriptableObject
    {
        [field: SerializeField, Header("Level Upgrades")]
        public UpgradeLevel[] Speed { get; private set; }
        
        [field: SerializeField]
        public UpgradeLevel[] PounceSpeed { get; private set; }
        
        [field: SerializeField]
        public UpgradeLevel[] ClawStrength { get; private set; }
        
        [field: SerializeField]
        public UpgradeLevel[] OxygenCapacity { get; private set; }
        
        [field: SerializeField]
        public UpgradeLevel[] CarryingCapacity { get; private set; }
        
        public UpgradeLevel[] GetUpgradeLevels(LevelUpgrade upgrade) =>
            upgrade switch
            {
                LevelUpgrade.Speed => Speed,
                LevelUpgrade.PounceSpeed => PounceSpeed,
                LevelUpgrade.ClawStrength => ClawStrength,
                LevelUpgrade.OxygenCapacity => OxygenCapacity,
                LevelUpgrade.CarryingCapacity => CarryingCapacity,
                _ => throw new ArgumentOutOfRangeException(nameof(upgrade), upgrade, null)
            };

        public int GetLevelCount(LevelUpgrade upgrade) =>
            GetUpgradeLevels(upgrade).Length - 1;
        

        [field: SerializeField, Header("One Time Upgrades")]
        public int MinimapPrice { get; private set; }
    }
}