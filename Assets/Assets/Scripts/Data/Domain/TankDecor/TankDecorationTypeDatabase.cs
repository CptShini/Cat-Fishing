
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Assets.Scripts.Data.Domain.TankDecor
{
    [CreateAssetMenu(fileName = "New Tankdecoration Type Database", menuName = "ScriptableObjects/Decor/FishTank")]
    public class TankDecorationTypeDatabase: ScriptableObject
    {
        [SerializeField] private List<TankDecorationType> decorTypes;
        
        public IReadOnlyCollection<TankDecorationType> Species => decorTypes;

        public TankDecorationType this[int id] =>
            decorTypes
                .ToDictionary(s => s.Id)
                .TryGetValue(id, out TankDecorationType result) 
                ? result 
                : throw new KeyNotFoundException($"Unknown Tank decoration type ID: {id}");

    }
}