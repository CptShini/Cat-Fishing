using System;
using System.Collections.Generic;
using Assets.Scripts.Fish;
using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    [SerializeField] private FishDatabase fishDatabase;

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

        collectedFish.AddRange(FishSaveSystem.LoadInventory(fishDatabase));
    }

    public void AddFish(Fish fish)
    {
        if (fish == null || collectedFish.Exists(f => f.fishId == fish.fishId))
            return;

        collectedFish.Add(fish);
        Debug.Log($"Added to collection: {fish.fishName}, weight: {fish.weight:F2}");
        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();
    }

    public bool RemoveFishItem(Fish fish)
    {
        if (fish == null)
            return false;

        int index = collectedFish.FindIndex(f => f.fishId == fish.fishId);

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