using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class DebugDrawer : MonoBehaviour
    {
        public static DebugDrawer Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
                Destroy(gameObject);
            else
                Instance = this;
        }

        public bool EnableDebug;

        //[Header("--- Fish ---")]
        
        //[Header("Fish Navigator")]
        public bool
            Fish_Nav_DrawDirections,
            Fish_Nav_DrawPathing;
    }
}