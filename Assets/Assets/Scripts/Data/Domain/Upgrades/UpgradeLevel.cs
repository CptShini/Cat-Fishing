using System;
using UnityEngine;

namespace Assets.Scripts.Data.Domain.Upgrades
{
    [Serializable]
    public class UpgradeLevel
    {
        public float Stat;
        [Min(0)] public int Price;
    }
}