using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishReportSlot : MonoBehaviour
{
    private Fish fish;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = $"{fish.Species.Name}";
    }

    public void Setup(Fish fish)
    {
        this.fish = fish;
        image.sprite = fish.Species.Icon;
    }
}
