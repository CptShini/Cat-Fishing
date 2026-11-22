using Assets.Scripts.Data.Domain.TankDecor;

namespace Assets.Scripts.Diving.DivingInventory
{
    public class TankDecorCollecible : Collectible
    {
        public readonly TankDecoration tankDecoration;
        
        public TankDecorCollecible(TankDecoration tankDecoration)
            : base(tankDecoration.Type.Name, tankDecoration.Type.Icon, tankDecoration.Type.Weight)
        {
            this.tankDecoration = tankDecoration;
        }

        public override void Drop()
        {
            // SOMETHING HAPPENS WHEN WE DROP A TANK DECORATION OUT OF INVENTORY
        }
    }
}
