using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class VersionCanvas : MonoBehaviour
    {
        private static VersionCanvas _instance;

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
    }
}