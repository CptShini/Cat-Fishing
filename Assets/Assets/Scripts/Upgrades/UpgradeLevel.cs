using System;
using UnityEngine;

namespace Assets.Scripts.Upgrades
{
    [Serializable]
    public class UpgradeLevel
    {
        public float Stat;
        [Min(0)] public int Price;
    }
}