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
            _text.text = $"v{Application.version}";
        }
    }
}
