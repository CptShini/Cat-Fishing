using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleReportSlot : MonoBehaviour
{
    private string _name;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = $"{_name}";
    }

    public void Setup(string name, Sprite icon)
    {
        this._name = name;
        image.sprite = icon;
    }
}
