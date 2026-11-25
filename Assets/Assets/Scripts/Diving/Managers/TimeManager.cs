using UnityEngine;


namespace Assets.Scripts.Diving.Managers
{
    public class TimeManager : MonoBehaviour
    {

        public static TimeManager Instance {get; private set;}

        public float catchingTimeScale = 1; // 0 when catching is happening

        void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }
        
        public void SetCatchingScaledTime(bool pause)
        {
            if (pause)
            {
                catchingTimeScale = 0;
            }
            else
            {
                catchingTimeScale = 1;
            }
        }
    }
}
