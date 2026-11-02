using Assets.Scripts.Fish.Domain;
using UnityEngine;

namespace Assets.Scripts.Fish
{
    public class FishStats : MonoBehaviour
    {
        [SerializeField] private FishSpecies fishSpecies;
        [SerializeField] private FishSpriteSetter spriteSetter;
        private FishDTO _fishDto;

        public FishDTO FishDto => _fishDto;

        void Awake()
        {
            _fishDto = new FishDTO(fishSpecies);
        }
        
        void Start()
        {
            spriteSetter.SetSprite(_fishDto.sprite);
        }
    }
}
