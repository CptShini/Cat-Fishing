using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlot : MonoBehaviour
{
    [SerializeField] private Image fishIcon;
    [SerializeField] private TextMeshProUGUI _name;
    bool _discovered = false;
    public Fish fish { get; private set; }
    public FishSpecies species { get; private set; }

    public void Setup(FishSpecies species, Fish fish, bool discovered)
    {
        if (fish == null) return;

        _discovered = discovered;
        this.fish = fish;
        this.species = species;
        fishIcon.sprite = fish.Species.Icon;
        fishIcon.enabled = fish.Species.Icon != null;
        
        HandleDiscovered();
    }
    public void Setup(FishSpecies species)
    {
        this.species = species;
        fish = null;
        _discovered = false;
        fishIcon.sprite = species.Icon;
        fishIcon.enabled = species.Icon != null;
        
        HandleDiscovered();
    }

    private void HandleDiscovered()
    {
        if (_discovered)
        {
            fishIcon.color = Color.white;
            _name.text = fish.Species.Name;
        }
        else
        {
            fishIcon.color = Color.black;
            _name.text = "???";
        }
    }

    public void OnClickSlot()
    {
        if(fish == null)
        {
            EncyclopediaManager.Instance.SetPageToUnknownFish(species);
        }
        else
        {
            EncyclopediaManager.Instance.SetPageToFish(fish);
        }
    }

}
