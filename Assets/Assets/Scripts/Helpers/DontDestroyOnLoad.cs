using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() => 
            DontDestroyOnLoad(gameObject);
    }
}
