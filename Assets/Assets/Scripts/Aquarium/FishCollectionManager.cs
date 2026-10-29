using System;
using System.Collections.Generic;
using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

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
    }

    public void AddFish(Fish fish)
    {
        if (fish == null)
            return;

        collectedFish.Add(fish);
        Debug.Log($"Added to collection: {fish.fishName}, weight: {fish.weight:F2}");
        OnCollectionChanged?.Invoke();
    }

    public bool RemoveFishItem(Fish fish)
    {
        if (fish == null)
            return false;

        bool removed = collectedFish.Remove(fish);

        if (removed)
            OnCollectionChanged?.Invoke();

        return removed;
    }
    
    public void TransferFishCollectibles(List<FishCollectible> list)
    {
        foreach (FishCollectible collectible in list)
        {
            collectedFish.Add(collectible.fish);
        }

        OnCollectionChanged?.Invoke();
    }
}