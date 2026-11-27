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

        [SerializeField] private Color hoverColor = Color.lightGray;

        private bool _hovering;
        
        public void Init(PlacedTankDecoration placedTankDecoration)
        {
            PlacedTankDecoration = placedTankDecoration;
            image.sprite = placedTankDecoration.TankDecoration.Type.IconInFishTank;

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
                                    
            collider.size = image.sprite.bounds.size;
            collider.offset = image.sprite.bounds.center;
        }

        private void LateUpdate()
        {
            if (_hovering && image.color != hoverColor)
            {
                image.color = hoverColor;
            }
            else if(!_hovering && image.color == hoverColor)
            {
                image.color = Color.white;
            }

            _hovering = false;
        }

        public void Hovering() {
            _hovering = true;
        }
    }
}