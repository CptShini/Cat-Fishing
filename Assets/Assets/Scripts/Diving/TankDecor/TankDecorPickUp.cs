using Assets.Scripts.Data.Domain.TankDecor;
using UnityEngine;

namespace Assets.Scripts.Diving.Spawning
{
    public class TankDecorPickUp : MonoBehaviour, IInteractable
    {

        public TankDecoration tankDecoration;
        public void Init(TankDecoration tankDecoration)
        {
            this.tankDecoration = tankDecoration;
            GetComponent<SpriteRenderer>().sprite = tankDecoration.Type.Icon;
        }

        public bool CanInteract() => true;

        public void Interact()
        {
            throw new System.NotImplementedException();
        }

    }
}