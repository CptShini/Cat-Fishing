using TMPro;
using UnityEngine;

namespace Assets.Scripts.Diving.Bars
{
    public class BarText : MonoBehaviour
    {
        [SerializeField] private StatBar bar;
        
        private TextMeshProUGUI _text;
        
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            bar.ValueChanged.AddListener(SetText);
        }

        private void SetText(float value, float max) => 
            _text.text = $"{value:N0}/{max} KG";
    }
}
