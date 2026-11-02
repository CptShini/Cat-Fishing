using System;
using System.Collections.Generic;
using Assets.Scripts.Fish;
using Assets.Scripts.Fish.Domain;
using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    [SerializeField] private FishDatabase fishDatabase;

    private readonly List<FishDTO> collectedFish = new();
    public IReadOnlyList<FishDTO> CollectedFish => collectedFish;

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

    public void AddFish(FishDTO fishDto)
    {
        if (fishDto == null || collectedFish.Exists(f => f.Id == fishDto.Id))
            return;

        collectedFish.Add(fishDto);
        Debug.Log($"Added to collection: {fishDto.Name}, weight: {fishDto.Weight:F2}");
        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();
    }

    public bool RemoveFishItem(FishDTO fishDto)
    {
        if (fishDto == null)
            return false;

        int index = collectedFish.FindIndex(f => f.Id == fishDto.Id);

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
            collectedFish.Add(collectible.fishDto);
        }

        FishSaveSystem.SaveInventory(collectedFish);
        OnCollectionChanged?.Invoke();
    }
}