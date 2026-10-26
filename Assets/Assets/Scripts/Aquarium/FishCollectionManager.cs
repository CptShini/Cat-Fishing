using System;
using System.Collections.Generic;
using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    private readonly List<FishSO> collectedFish = new();
    public IReadOnlyList<FishSO> CollectedFish => collectedFish;

    public event Action OnCollectionChanged;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddFish(FishSO fish)
    {
        if (fish == null)
            return;

        collectedFish.Add(fish);
        Debug.Log("Added to collection: " + fish.name);
        OnCollectionChanged?.Invoke();
    }
}