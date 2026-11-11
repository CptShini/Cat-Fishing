using Assets.Scripts.Fishies.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Aquarium.Computer.Encyclopedia
{
    public class EncyclopediaSlot : MonoBehaviour
    {
        [SerializeField] private Image fishIcon;
        [SerializeField] private TextMeshProUGUI _name;

        private bool _discovered;
        private Fish _fish;
        private FishSpecies _species;

        public void Setup(FishSpecies species, Fish fish, bool discovered)
        {
            if (fish == null) return;

            _discovered = discovered;
            _fish = fish;
            _species = species;
            fishIcon.sprite = fish.Species.Icon;
            fishIcon.enabled = fish.Species.Icon != null;
        
            HandleDiscovered();
        }
    
        public void Setup(FishSpecies species)
        {
            _species = species;
            _fish = null;
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
                _name.text = _fish.Species.Name;
            }
            else
            {
                fishIcon.color = Color.black;
                _name.text = "???";
            }
        }

        public void OnClickSlot()
        {
            if (_fish == null)
                EncyclopediaManager.Instance.SetPageToUnknownFish(_species);
            else
                EncyclopediaManager.Instance.SetPageToFish(_fish);
        }
    }
}
