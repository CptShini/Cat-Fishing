using System.Collections.Generic;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

[CreateAssetMenu(menuName = "Aquarium/Fish Database")]
public class FishDatabase : ScriptableObject
{
    [SerializeField] private List<FishSpecies> fishTypes = new();

    public FishSpecies GetFishSO(int key)
    {
        return fishTypes.Find(f => f != null && f.Id == key);
    }
}