using Assets.Scripts.Aquarium.Domain;
using Assets.Scripts.Fishies.Movement;
using Assets.Scripts.Fishies.Visuals;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Aquarium.FishTanks.Decor
{
    public class PlacedTankDecorationObject : MonoBehaviour
    {
        public PlacedTankDecoration PlacedTankDecoration { get; private set; }

        [SerializeField] private SpriteRenderer image;

        private bool _hovering;
        
        public void Init(PlacedTankDecoration placedTankDecoration)
        {
            PlacedTankDecoration = placedTankDecoration;
            image.sprite = placedTankDecoration.TankDecoration.Type.IconInFishTank;
        }

        private void LateUpdate()
        {
            _hovering = false;
        }

        public void Hovering() => _hovering = true;
    }
}