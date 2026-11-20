using Assets.Scripts.Data.Domain.TankDecor;
using Assets.Scripts.Diving.DivingInventory;
using UnityEngine;


namespace Assets.Scripts.Diving.TankDecor
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
            TankDecorCollecible col = new(tankDecoration);
            if (!DivingInventory.DivingInventory.Instance.CanAdd(col)) // Checks if putting in inventory is successful
            {
                return;
            }
            DivingInventory.DivingInventory.Instance.Add(col);
            Destroy(gameObject);
        }

    }
}