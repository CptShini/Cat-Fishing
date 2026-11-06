using System.Threading.Tasks;
using Assets.Scripts.Diving.Bars;
using TMPro;
using UnityEditor.SettingsManagement;
using UnityEngine;

public class BarText : MonoBehaviour
{
    [SerializeField] private StatBar bar;
    private TextMeshProUGUI text;
    private int _maxValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bar.ValueChanged.AddListener(SetText);
        _maxValue = (int)bar.MaxVal;
        text = GetComponent<TextMeshProUGUI>();
        SetText(bar.CurrentValue);
    }

    void SetText(float value)
    {
        text.text = $"{(int) value}/{_maxValue} KG";
    }

}
