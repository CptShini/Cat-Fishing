using System;
using System.Collections.Generic;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    [SerializeField] private FishSpeciesDatabase fishSpeciesDatabase;

    private readonly List<Fish> collectedFish = new();
    public IReadOnlyList<Fish> CollectedFish => collectedFish;

    public event Action OnCollectionChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        collectedFish.AddRange(FishSaveSystem.LoadInventory(fishSpeciesDatabase));
    }

    public void AddFish(Fish fish)
    {
        if (fish == null || collectedFish.Exists(f => f.Id == fish.Id))
            return;

        collectedFish.Add(fish);
        Debug.Log($"Added to collection: {fish.Species.Name}, weight: {fish.Weight:F2}");
        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();
    }

    public bool RemoveFishItem(Fish fish)
    {
        if (fish == null)
            return false;

        int index = collectedFish.FindIndex(f => f.Id == fish.Id);

        if (index < 0)
            return false;

        collectedFish.RemoveAt(index);
        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();

        return true;
    }

    public void ReloadInventoryAfterReset()
    {
        collectedFish.Clear();
        OnCollectionChanged?.Invoke();
    }
    
    public void TransferFishCollectibles(List<FishCollectible> list)
    {
        foreach (FishCollectible collectible in list)
        {
            collectedFish.Add(collectible.fish);
        }

        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();
    }
}