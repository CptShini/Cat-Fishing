using UnityEngine;
using System.Collections.Generic;

public class FishTank : MonoBehaviour, IInteractable
{
    [Header("Fish in this tank")]
    [SerializeField] private List<FishSO> fishes = new List<FishSO>();

    public IReadOnlyList<FishSO> Fishes => fishes;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        FishTankUIController.Instance.Open(this);
    }

    public bool AddFish(FishSO fish)
    {
        fishes.Add(fish);
        Debug.Log(fish.fishName + " added to tank: " + gameObject.name);

        return true;
    }

    public bool RemoveFish(FishSO fish)
    {
        bool removed = fishes.Remove(fish);

        if (removed)
            Debug.Log(fish.fishName + " removed from tank: " + gameObject.name);

        return removed;
    }
}
