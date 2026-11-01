using UnityEngine;

namespace Assets.Scripts.Diving.Bars
{
    public class StaminaBar : StatBar
    {
        public static StaminaBar Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            SetMaxValue(DivingPlayerStats.Instance.maxStamina);
            _minValue = 5; // Idk, ask Andreas
        }

        private void Update()
        {
            if (DivingController.Instance.state != PlayerState.PlayerState.SWIM) return;

            float staminaRegen = DivingPlayerStats.Instance.staminaRegen * Time.deltaTime;
            CurrentValue += staminaRegen;
        }
    }
}
