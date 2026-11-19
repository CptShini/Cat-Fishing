


using UnityEngine;
namespace Assets.Scripts.Data.Domain.TankDecor
{
    [CreateAssetMenu(fileName = "New Tankdecoration Type", menuName = "ScriptableObjects/Decor/FishTank")]
    public class TankDecorationType: ScriptableObject
    {
        // --- Information ---

        [field: SerializeField, Header("Information")]
        public int Id { get; private set; }
        
        [field: SerializeField]
        public string Name;

        // --- Visuals ---

        [field: SerializeField, Header("Visuals")]
        public Sprite Icon;

        // --- Stats ---

        [field: SerializeField, Header("Stats")]
        public float EntertainmentValue;

        [field: SerializeField]
        public float Weight;

    }
}