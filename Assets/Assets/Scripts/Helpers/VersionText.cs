using TMPro;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class VersionText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            #if UNITY_EDITOR
            _text.text = "EDITOR";
            #else
            _text.text = $"v{Application.version}";
            #endif
        }
    }
}
