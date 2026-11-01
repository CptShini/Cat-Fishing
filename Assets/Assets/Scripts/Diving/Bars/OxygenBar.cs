using UnityEngine;

namespace Assets.Scripts.Diving.Bars
{
    public class OxygenBar : StatBar
    {
        public static OxygenBar Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }
        
        private void Start() => 
            SetMaxValue(DivingPlayerStats.Instance.oxygenCapacity);

        private void Update()
        {
            if (DivingController.Instance.state == PlayerState.PlayerState.GRAB) return;
            
            CurrentValue -= Time.deltaTime;
        }
    }
}
