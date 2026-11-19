using UnityEngine;


namespace Assets.Scripts.Diving.Managers
{
    public class TimeManager : MonoBehaviour
    {

        public static TimeManager Instance {get; private set;}

        public float timeScale = 1.0f;

        void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pause">true if game scaled time should be paused, false otherwise</param>
        public void SetPausedScaledTime(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
