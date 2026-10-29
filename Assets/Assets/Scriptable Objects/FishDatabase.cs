using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Aquarium/Fish Database")]
public class FishDatabase : ScriptableObject
{
    [SerializeField] private List<FishSO> fishTypes = new();

    public FishSO GetFishSO(string key)
    {
        return fishTypes.Find(f => f != null && f.fishKey == key);
    }
}